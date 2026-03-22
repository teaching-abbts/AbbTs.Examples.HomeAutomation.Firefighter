/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.device.sensor

import ch.abbts.buildingclient.building
import smarthome.device.Position

class Temperature(
    position: Position) : Analog("Temperatur", position, "C") {

    fun update() {
        value = building.getTemperature()
        repaint()
    }
}