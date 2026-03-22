/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.device.userinterface

import smarthome.device.sensor.Analog

object displayFormatter {
    private val MAX_CHAR_PER_LINE = 16

    fun format(device: Analog, digits: Int): String {
        val shortName = device.name.substring(0, 3)
        val value = insertSpaces(device.value.toString(), digits)
        val shortUnit = device.unit.substring(0, 1)
        return "$shortName:$value$shortUnit"
    }

    private fun insertSpaces(text: String, amount: Int): String {
        val textWithSpaces = StringBuilder(text)
        while (textWithSpaces.length < amount) {
            textWithSpaces.insert(0, " ")
        }
        return textWithSpaces.toString()
    }

    fun appendSpaces(text: String): String {
        var textWithSpaces = text
        textWithSpaces += " ".repeat(MAX_CHAR_PER_LINE - text.length)
        return textWithSpaces
    }
}