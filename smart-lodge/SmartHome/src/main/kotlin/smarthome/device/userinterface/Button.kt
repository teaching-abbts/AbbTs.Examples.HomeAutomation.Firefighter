/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Objektorientierte Gebaeudesteuerung mit realer Hardware.
*/

package smarthome.device.userinterface

import ch.abbts.buildingclient.building
import java.lang.Thread.sleep

class Button(
    private var name: String,
    private val nr: Int) {

    override fun toString(): String {
        return "$name $nr"
    }

    fun waitForPressed() {
        while (isPressed().not()) {
            sleep(100)
        }
    }

    private fun isPressed(): Boolean {
        return building.isButtonPressed(nr)
    }
}