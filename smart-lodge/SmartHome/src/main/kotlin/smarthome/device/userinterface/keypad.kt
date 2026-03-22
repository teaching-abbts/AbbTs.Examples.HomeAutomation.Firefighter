/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Objektorientierte Gebaeudesteuerung mit realer Hardware.
*/

package smarthome.device.userinterface

import ch.abbts.buildingclient.building
import java.lang.Thread.sleep

object keypad {
    private val ENTER_KEY = "#"
    private val BACKSPACE_KEY = "*"

    fun readInt() = readln().toInt()

    fun readln(): String {
        val input = StringBuilder()
        var key = ""
        while (key != ENTER_KEY) {
            if (key != BACKSPACE_KEY) {
                display.print(key)
                input.append(key)
            }
            else {
                display.backspace()
                input.deleteCharAt(input.length - 1)
            }
            key = waitForKey()
        }
        return input.toString()
    }

    fun waitForKey(): String {
        var key = getKey()
        while (key == "none") {
            sleep(100)
            key = getKey()
        }
        return key
    }

    private fun getKey() = building.getKey()
}