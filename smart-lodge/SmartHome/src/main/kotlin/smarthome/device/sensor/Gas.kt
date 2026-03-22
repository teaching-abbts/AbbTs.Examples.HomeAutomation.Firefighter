/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.device.sensor

import ch.abbts.buildingclient.building
import smarthome.device.Position

class Gas(
    position: Position) : Analog("Gas", position, "ppm") {

    fun update() {
        value = building.getGas()
        repaint()
    }
}