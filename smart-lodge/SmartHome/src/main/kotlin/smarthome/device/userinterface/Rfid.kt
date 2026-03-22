/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.device.userinterface

import ch.abbts.buildingclient.building
import javafx.scene.paint.Color.LIGHTBLUE
import smarthome.device.Device
import smarthome.device.Position
import smarthome.gui.monitoring.logger

class Rfid(
    position: Position) : Device("RFID", position) {
    private var value = "none"

    fun update() {
        value = building.getRfid()
        view.setColor(LIGHTBLUE)
        if (value == "none") {
            view.setText("keine")
        }
        else {
            view.setText(value.substring(0, 7) + "...")
            logger.log("${position.room} $name $value")
        }
    }

    fun getValue() = value

    override fun toString() = "$name: $value"
}