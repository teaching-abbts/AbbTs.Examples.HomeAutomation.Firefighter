/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.device.actor

import ch.abbts.buildingclient.building
import smarthome.device.Position

class Buzzer(
    position: Position) : Digital("Summer", position) {

    override fun on() {
        if (isOff()) {
            state = true
            building.setBuzzer(state)
            repaintOn()
        }
    }

    override fun off() {
        if (isOn()) {
            state = false
            building.setBuzzer(state)
            repaintOff()
        }
    }

    override fun toString() = "$name: $state"
}