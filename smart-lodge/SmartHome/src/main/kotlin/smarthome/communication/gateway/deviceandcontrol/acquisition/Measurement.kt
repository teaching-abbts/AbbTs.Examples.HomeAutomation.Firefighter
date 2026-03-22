/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: SmartHome mit Gateway erweitert.
*/

package smarthome.communication.gateway.deviceandcontrol.acquisition

data class Measurement(
    val timeStamp: String,  // Format: YYYY-MM-DD hh:mm:ss
    val buildingID: String,
    val brightness: Int,
    val temperature: Int,
    val humidity: Int,
    val gas: Int
)