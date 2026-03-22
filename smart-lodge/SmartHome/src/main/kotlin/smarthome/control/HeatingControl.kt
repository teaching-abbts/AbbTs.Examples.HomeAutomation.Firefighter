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
import smarthome.device.actor.Relay
import smarthome.device.sensor.Humidity
import smarthome.device.sensor.Temperature
import smarthome.device.userinterface.display
import smarthome.device.userinterface.displayFormatter

class HeatingControl : Control("Heizung"){
    private val temperature = Temperature(Position("Wohnen", 180.0, 5.0))
    private val humidity = Humidity(Position("Wohnen", 180.0, 20.0))
    private val relay = Relay(Position("Wohnen", 210.0, 5.0))
    private val SET_POINT = 20
    private val setPointBox = HBox()
    private val setPointSpn = Spinner<Int>(0, 1000, SET_POINT)
    private val unitLbl = Label(temperature.unit)

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

        gateway.add(temperature)
        gateway.add(humidity)
        gateway.add(this)
    }

    fun getSetPoint() = setPointSpn.value

    fun setSetPoint(newValue: Int) {
        runLater {
            setPointSpn.valueFactory.value = newValue
        }
    }

    override suspend fun work() {
        updateSensors()

        if (checkBox.isSelected()) {
            regulate()
        }
        updateDisplay()
    }

    private fun updateSensors() {
        temperature.update()
        humidity.update()
    }

    private fun regulate() {
        if (temperature.value < setPointSpn.value) {
            relay.on()
        }
        else {
            relay.off()
        }
    }

    private fun updateDisplay() {
        display.print(displayFormatter.format(temperature, 2), 0, 9)
        display.print(displayFormatter.format(humidity, 2), 1, 9)
    }
}