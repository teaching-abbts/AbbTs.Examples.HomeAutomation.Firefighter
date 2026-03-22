/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Objektorientierte Gebaeudesteuerung mit realer Hardware.
*/

package smarthome.control

import smarthome.actor.RgbLed
import smarthome.communication.gateway.deviceandcontrol.acquisition.Event
import smarthome.communication.gateway.gateway
import smarthome.config
import smarthome.config.FIRE_ALARM_VALUE
import smarthome.config.GAS_ALARM_VALUE
import smarthome.device.Position
import smarthome.device.actor.Buzzer
import smarthome.device.sensor.Fire
import smarthome.device.sensor.Gas
import smarthome.device.sensor.Motion
import smarthome.device.sensor.Sound
import smarthome.device.userinterface.display
import smarthome.device.userinterface.displayFormatter
import smarthome.gui.monitoring.logger
import smarthome.utilities

class AlarmControl : Control("Alarm") {
    private val gas = Gas(Position("Wohnen", 210.0, 30.0))
    private val fire = Fire(Position("Wohnen", 210.0, 90.0))
    private val buzzer = Buzzer(Position("Wohnen", 150.0, 5.0))
    private val GAS_ALARM_DURATION_MS = 200

    private val motion = Motion(Position("Schlafen", 3.0, 40.0))
    private val sound = Sound(Position("Wohnen", 210.0, 60.0))
    private val rgbLed = RgbLed(Position("Schlafen", 3.0, 10.0))

    private val MAX_MUTE_EVENT_COUNT = 60
    private var gasEventCount = 0
    private var fireEventCount = 0
    private var motionEventCount = 0
    private var soundEventCount = 0

    init {
        checkBox.text = "Überwachen"
        rgbLed.greenOn()
        logger.log("Soundsensor Stille-Referenz: ${sound.getStillness()}")

        gateway.add(gas)
        gateway.add(this)
    }

    override suspend fun work() {
        updateSensors()
        if (checkBox.isSelected()) {
            detectGasAndFire()
            detectMotionAndSound()
        }
        updateDisplay()
    }

    private fun updateSensors() {
        gas.update()
        fire.update()
        motion.update()
        sound.update()
    }

    private fun detectGasAndFire() {
        when {
            fire.value > FIRE_ALARM_VALUE -> {
                if (fireEventCount < 4) {
                    buzzer.on()
                    if (fireEventCount == 0) {
                        logger.log("Feueralarm!")
                        gateway.push(Event(utilities.now(), config.BUILDING_ID, "Fire", ""))
                    }
                }
                fireEventCount = incrementAndLimitCount(fireEventCount)
            }
            gas.value > GAS_ALARM_VALUE -> {
                if (gasEventCount < 4) {
                    buzzer.pulse(GAS_ALARM_DURATION_MS)
                    if (gasEventCount == 0) {
                        logger.log("Gasalarm!")
                        gateway.push(Event(utilities.now(), config.BUILDING_ID, "Gas", "${gas.value} ${gas.unit}"))
                    }
                }
                gasEventCount = incrementAndLimitCount(gasEventCount)
            }
            else -> {
                buzzer.off()
                fireEventCount = 0
                gasEventCount = 0
            }
        }
    }

    private fun detectMotionAndSound() {
        when {
            motion.isActive() and sound.isActive() -> {
                rgbLed.greenOff()
                rgbLed.redOn()
                logger.log("Bewegungsalarm!")
                rgbLed.blueOn()
                logger.log("Geraeuschalarm!")
            }
            motion.isActive() -> {
                if (motionEventCount < 4) {
                    rgbLed.greenOff()
                    rgbLed.blueOff()
                    rgbLed.redOn()
                    if (motionEventCount == 0) {
                        logger.log("Bewegungsalarm!")
                        gateway.push(Event(utilities.now(), config.BUILDING_ID, "Motion", ""))
                    }
                }
                motionEventCount = incrementAndLimitCount(motionEventCount)
            }
            sound.isActive() -> {
                if (soundEventCount < 4) {
                    rgbLed.greenOff()
                    rgbLed.redOff()
                    rgbLed.blueOn()
                    if (soundEventCount == 0) {
//                        logger.log("Geraeuschalarm!")
//                        gateway.push(Event(utilities.now(), config.BUILDING_ID, "Sound", ""))
                    }
                }
                soundEventCount = incrementAndLimitCount(soundEventCount)
            }
            else -> {
                rgbLed.redOff()
                rgbLed.blueOff()
                rgbLed.greenOn()
                motionEventCount = 0
                soundEventCount = 0
            }
        }
    }

    private fun updateDisplay() {
        display.print(displayFormatter.format(gas, 3), 1, 0)
    }

    private fun incrementAndLimitCount(count: Int): Int {
        var value = count
        if (count == MAX_MUTE_EVENT_COUNT) {
            value = 0
        }
        else {
            value++
        }
        return value
    }
}