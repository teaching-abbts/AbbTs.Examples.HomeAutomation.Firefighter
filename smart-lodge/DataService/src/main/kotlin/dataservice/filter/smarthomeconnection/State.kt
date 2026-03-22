/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: SmartHome mit Gateway erweitert.
*/

package dataservice.filter.smarthomeconnection

data class State(
    val device: String,
    val state: String,
    val value: Int
)
