/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome

import javafx.application.Application
import smarthome.gui.Gui

fun main() {
    config.read("SmartHome.conf")
    Application.launch(Gui::class.java)
}

