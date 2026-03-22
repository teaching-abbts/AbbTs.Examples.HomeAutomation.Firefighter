/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: SmartHome mit Gateway erweitert.
*/

package dataservice.filter.smarthomeconnection

data class Registration(
    val buildingID: String,
    val xCoordinate: Int,   // Nullpunkt: Smarthome vorne links Blickrichtung Whiteboard
    val yCoordinate: Int,
    val owner: String
)
