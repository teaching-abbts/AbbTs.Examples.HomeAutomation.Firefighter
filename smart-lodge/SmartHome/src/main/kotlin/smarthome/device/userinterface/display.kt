/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Objektorientierte Gebaeudesteuerung mit realer Hardware.
*/

package smarthome.device.userinterface

import ch.abbts.buildingclient.building
import kotlinx.coroutines.sync.Mutex

object display {
    private var lastLine = ""
    private var firstUse = true
    private var cursor = 0

    init {
        clear()
    }

    fun clear() = building.clearDisplay()

    fun println(obj: Any) {
        val text = obj.toString()
        val myText = displayFormatter.appendSpaces(text)    // Weil Display alte Zeichen nicht loescht

        if (firstUse) {
            print(myText, 0)
            firstUse = false
        }
        else {
            print(lastLine, 0)
            print(myText, 1)
            lastLine = myText
        }
    }

    fun print(obj: Any) {
        val text = obj.toString()
        print(text, 0, cursor)
        cursor += text.length
    }

    fun print(obj: Any, lineNr: Int, position: Int = 0) {
        building.setDisplayCursor(lineNr, position)
        building.setDisplay(obj.toString())
    }

    fun backspace() {
        cursor--
        print(" ", 0, cursor)
    }
}