/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.device.actor

import ch.abbts.buildingclient.building
import smarthome.device.Position

class Relay(
    position: Position) : Digital("Heizung", position) {

    override fun on() {
        if (isOff()) {
            state = true
            building.setRelay(state)
            repaintOn()
        }
    }

    override fun off() {
        if (isOn()) {
            state = false
            building.setRelay(state)
            repaintOff()
        }
    }

    fun toggle() {
        if (isOn()) {
            off()
        }
        else {
            on()
        }
    }

    override fun toString() = "$name: $state"
}