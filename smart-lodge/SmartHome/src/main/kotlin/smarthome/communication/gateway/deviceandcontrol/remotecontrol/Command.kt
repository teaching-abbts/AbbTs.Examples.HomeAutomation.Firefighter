/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: SmartHome mit Gateway erweitert.
*/

package smarthome.communication.gateway.deviceandcontrol.remotecontrol

data class Command(
    val device: String,
    var command: String,
    var value: String
)
