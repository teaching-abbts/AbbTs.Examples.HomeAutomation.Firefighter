/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.device.sensor

import ch.abbts.buildingclient.building
import smarthome.device.Position

class Fire(
    position: Position) : Analog("Feuer", position, "") {

    fun update() {
        value = building.getFire()
        repaint()
    }
}