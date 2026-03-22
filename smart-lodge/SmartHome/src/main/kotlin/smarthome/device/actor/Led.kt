/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.device.actor

import ch.abbts.buildingclient.building
import smarthome.device.Position
import java.lang.Thread.sleep

class Led(
    private val color: Color,
    position: Position) : Digital("Licht", position) {

    override fun on() {
        if (isOff()) {
            state = true
            building.setLed(color.ordinal, state)
            repaintOn()
        }
    }

    override fun off() {
        if (isOn()) {
            state = false
            building.setLed(color.ordinal, state)
            repaintOff()
        }
    }

    fun flash(amount: Int, timeMs: Int) {
        for (count in 1..amount) {
            pulse(timeMs)
            sleep(timeMs.toLong())
        }
    }

    override fun toString() = "$name $color: $state"
}