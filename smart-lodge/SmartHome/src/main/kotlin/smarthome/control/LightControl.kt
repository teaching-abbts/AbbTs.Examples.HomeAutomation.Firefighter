/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Objektorientierte Gebaeudesteuerung mit realer Hardware.
*/

package smarthome.control

import javafx.application.Platform.runLater
import javafx.scene.control.Label
import javafx.scene.control.Spinner
import javafx.scene.layout.HBox
import smarthome.communication.gateway.gateway
import smarthome.device.Position
import smarthome.device.actor.Color.GREEN
import smarthome.device.actor.Led
import smarthome.device.sensor.Brightness
import smarthome.device.userinterface.display
import smarthome.device.userinterface.displayFormatter

class LightControl : Control("Licht") {
    private val brightness = Brightness(Position("Wohnen", 210.0, 120.0))
    private val led = Led(GREEN, Position("Wohnen", 133.0, 30.0))
    private val SET_POINT = 300
    private val setPointBox = HBox()
    private val setPointSpn = Spinner<Int>(0, 1000, SET_POINT)
    private val unitLbl = Label(brightness.unit)

    init {
        activateCheckBox()
        checkBox.text = "Sollwert"
        with(setPointSpn) {
            setEditable(true)
            setPrefSize(75.0, 25.0)
        }
        with(setPointBox) {
            spacing = 10.0
            children.addAll(setPointSpn, unitLbl)
        }
        add(setPointBox)

        gateway.add(brightness)
        gateway.add(this)
    }

    fun getSetPoint() = setPointSpn.value

    fun setSetPoint(newValue: Int) {
        runLater {
            setPointSpn.valueFactory.value = newValue
        }
    }

    override suspend fun work() {
        brightness.update()

        if (checkBox.isSelected()) {
            if (brightness.value < setPointSpn.value) {
                led.on()
            }
            else {
                led.off()
            }
        }

        display.print(displayFormatter.format(brightness, 3), 0, 0)
    }
}