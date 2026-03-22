/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.device.actor

import ch.abbts.buildingclient.building
import javafx.scene.paint.Color.*
import smarthome.device.Device
import smarthome.device.Position
import smarthome.gui.monitoring.logger
import java.lang.Thread.sleep

class Servo(
    position: Position) : Device("Tuere", position) {
    private var angle = 0

    fun open(durationS: Int) {
        open()
        sleep((durationS * 1000).toLong())
        close()
    }

    fun open() {
        setAngle(90)
        view.setColor(LIGHTGREEN)
        view.setText("offen")
        logger.log("${position.room} $name offen")
    }

    fun close() {
        setAngle(0)
        view.setColor(LIGHTGRAY)
        view.setText("zu")
        logger.log("${position.room} $name zu")
    }

    fun getAngle() = angle

    fun setAngle(degree: Int) {
        building.setDoor(degree)
    }

    override fun toString() = "$name: $angle Grad"
}