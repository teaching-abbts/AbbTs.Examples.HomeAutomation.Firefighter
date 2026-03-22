/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Objektorientierte Gebaeudesteuerung mit realer Hardware.
*/

package smarthome.device.actor

import javafx.scene.paint.Color
import smarthome.device.Device
import smarthome.device.Position
import smarthome.gui.monitoring.logger

abstract class Digital(
    name: String,
    position: Position) : Device(name, position) {
    protected var state = false

    fun isOn() = state
    fun isOff() = state.not()
    abstract fun on()
    abstract fun off()

    fun pulse(timeMs: Int) {
        on()
        Thread.sleep(timeMs.toLong())
        off()
    }

    fun repaintOn() {
        view.setColor(Color.LIGHTGREEN);
        view.setText("ein")
        logger.log("${position.room} $name ein")
    }

    fun repaintOff() {
        view.setColor(Color.LIGHTGRAY);
        view.setText("aus")
        logger.log("${position.room} $name aus")
    }
}
