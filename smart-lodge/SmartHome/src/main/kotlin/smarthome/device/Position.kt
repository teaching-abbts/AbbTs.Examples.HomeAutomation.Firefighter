/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.device

import smarthome.config.DEVICE_ZOOM

class Position(
    var room: String,
    var x: Double,
    var y: Double) {

    init {
        x *= DEVICE_ZOOM
        y *= DEVICE_ZOOM
    }
}
