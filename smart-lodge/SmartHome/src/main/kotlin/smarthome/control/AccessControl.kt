/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Objektorientierte Gebaeudesteuerung mit realer Hardware.
*/

package smarthome.control

import smarthome.communication.gateway.deviceandcontrol.acquisition.Event
import smarthome.communication.gateway.gateway
import smarthome.config
import smarthome.config.RFID_CARD_USER
import smarthome.config.RFID_KEY_USER
import smarthome.device.Position
import smarthome.device.actor.Color.WHITE
import smarthome.device.actor.Led
import smarthome.device.actor.Servo
import smarthome.device.sensor.Distance
import smarthome.device.userinterface.Rfid
import smarthome.device.userinterface.display
import smarthome.gui.monitoring.logger
import smarthome.utilities
import java.lang.Thread.sleep

class AccessControl : Control("Zutritt") {
    private val distance = Distance(Position("Eingang", 60.0, 165.0))
    private val rfid = Rfid(Position("Eingang", 90.0, 165.0))
    private val led = Led(WHITE, Position("Eingang", 133.0, 165.0))
    private val door = Servo(Position("Eingang", 200.0, 143.0))

    private val OUT_Of_RANGE = 60
    private val IN_RANGE = 30
    private val NEAR = 10

    private val DURATION_S = 3

    init {
        checkBox.text = "Kontrollieren"

        gateway.add(door)
    }

    override suspend fun work() {
        distance.update()
        if (checkBox.isSelected()) {
            detectUser()
        }
    }

    private suspend fun detectUser() {
        when {
            distance.value > OUT_Of_RANGE -> led.off()
            distance.value > IN_RANGE -> led.on()
            distance.value > NEAR -> show("RFID Bitte!")
            else -> identifyUser()
        }
    }

    private suspend fun identifyUser() {
        rfid.update()
        when {
            rfid.getValue() == RFID_CARD_USER -> {
                show("Card Willkommen!")
                door.open(DURATION_S)
                logger.log("Zutritt für ${rfid.getValue()}")
                gateway.push(Event(utilities.now(), config.BUILDING_ID, "RFID", "${rfid.getValue()} Card"))
            }

            rfid.getValue() == RFID_KEY_USER -> {
                show("Key Willkommen!")
                door.open(DURATION_S)
                logger.log("Zutritt für ${rfid.getValue()}")
                gateway.push(Event(utilities.now(), config.BUILDING_ID, "RFID", "${rfid.getValue()} Key"))
            }

            else -> {
                show("Unbekannter User")
                logger.log("Kein Zutritt für ${rfid.getValue()}")
                gateway.push(Event(utilities.now(), config.BUILDING_ID, "RFID", "${rfid.getValue()} Unknown"))
            }
        }
    }

    private fun show(message: String) {
        display.clear()
        display.print(message, 0)
        sleep(1000)
        display.clear()
    }
}