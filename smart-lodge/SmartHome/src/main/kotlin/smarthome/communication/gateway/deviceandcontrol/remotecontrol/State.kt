/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: SmartHome mit Gateway erweitert.
*/

package smarthome.communication.gateway.deviceandcontrol.remotecontrol

data class State(
    val gateway: String,      // readonly, readandwrite
    val lightControl: String,
    val lightControlSetPoint: Int,
    val heatingControl: String,
    val heatingControlSetPoint: Int,
    val alarmControl: String
)
