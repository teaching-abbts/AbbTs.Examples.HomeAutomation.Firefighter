/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.device.sensor

import ch.abbts.buildingclient.building
import smarthome.device.Position

class Distance(
    position: Position) : Analog("Distanz", position, "cm") {

    fun update() {
        value = building.getDistance()
        repaint()
    }
}