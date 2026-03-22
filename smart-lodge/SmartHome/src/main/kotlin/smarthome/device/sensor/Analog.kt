/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Objektorientierte Gebaeudesteuerung mit realer Hardware.
*/

package smarthome.device.sensor

import javafx.scene.paint.Color
import smarthome.device.Device
import smarthome.device.Position
import smarthome.gui.monitoring.plotter

abstract class Analog(
    name: String,
    position: Position,
    val unit: String) : Device(name, position) {
    var value = 0

    fun repaint() {
        view.setColor(Color.LIGHTBLUE);
        view.setText(value.toString() + " " + unit)
        plotter.plot(name, value)
    }

    override fun toString() = "$name: $value $unit"
}