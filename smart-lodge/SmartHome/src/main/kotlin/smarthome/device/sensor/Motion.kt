/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.device.sensor

import ch.abbts.buildingclient.building
import javafx.scene.paint.Color.*
import smarthome.device.Device
import smarthome.device.Position

class Motion(
    position: Position) : Device("Bewegung", position) {
    private var value = false

    fun update() {
        value = building.isMotion()
        view.setColor(LIGHTBLUE)
        if (value) {
            view.setText("detektiert")
        }
        else {
            view.setText("keine")
        }
    }

    fun isActive() = value

    override fun toString() = "$name: $value"
}