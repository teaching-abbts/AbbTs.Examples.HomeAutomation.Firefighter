/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Objektorientierte Gebaeudesteuerung mit realer Hardware.
*/

package smarthome.device

import smarthome.gui.home

abstract class Device(
    val name: String,
    protected val position: Position) {
    protected val view = View(name, position)

    init {
        home.add(this)
    }

    fun getView() = view.group
}