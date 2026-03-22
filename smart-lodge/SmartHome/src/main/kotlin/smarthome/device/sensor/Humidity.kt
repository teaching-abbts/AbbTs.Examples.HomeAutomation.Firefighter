/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.device.sensor

import ch.abbts.buildingclient.building
import smarthome.device.Position

class Humidity(
    position: Position) : Analog("Feuchtigkeit", position, "%") {

    fun update() {
        value = building.getHumidity()
        repaint()
    }
}