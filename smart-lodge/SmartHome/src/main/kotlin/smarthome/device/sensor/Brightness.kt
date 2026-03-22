/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.device.sensor

import ch.abbts.buildingclient.building
import smarthome.device.Position

class Brightness(
    position: Position) : Analog("Helligkeit", position, "Lux") {

    fun update() {
        value = building.getBrightness()
        repaint()
    }
}