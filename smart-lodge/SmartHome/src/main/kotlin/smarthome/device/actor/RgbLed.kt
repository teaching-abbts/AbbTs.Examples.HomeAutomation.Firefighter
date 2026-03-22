/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.actor

import ch.abbts.buildingclient.building
import javafx.scene.paint.Color.LIGHTGREEN
import smarthome.device.Device
import smarthome.device.Position

class RgbLed(
    position: Position) : Device("RGB-LED", position) {
    private var redState = false
    private var greenState = false
    private var blueState = false

    fun redOn() {
        redState = true
        update()
    }

    fun redOff() {
        redState = false
        update()
    }

    fun greenOn() {
        greenState = true
        update()
    }

    fun greenOff() {
        greenState = false
        update()
    }

    fun blueOn() {
        blueState = true
        update()
    }

    fun blueOff() {
        blueState = false
        update()
    }

    fun allOff() {
        redState = false
        greenState = false
        blueState = false
        update()
    }

    fun rotate() {
        val tempState = redState
        redState = greenState
        greenState = blueState
        blueState = tempState
        update()
    }

    private fun update() {
        building.setLed(redState, greenState, blueState)
        view.setColor(LIGHTGREEN)
        view.setText(toString())
    }

    override fun toString(): String {
        val text = StringBuilder()
        if (redState) {
            text.append("Rot ")
        }
        if (greenState) {
            text.append("Gruen ")
        }
        if (blueState) {
            text.append("Blau")
        }
        return text.toString()
    }
}