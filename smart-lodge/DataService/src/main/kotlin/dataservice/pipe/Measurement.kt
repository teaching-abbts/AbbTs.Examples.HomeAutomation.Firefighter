/*
  Projekt:      SmartQuartier DataService
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali
*/

package dataservice.pipe

data class Measurement(
    val timeStamp: String,  // Format: YYYY-MM-DD hh:mm:ss
    val buildingID: String,
    val brightness: Int,
    val temperature: Int,
    val humidity: Int,
    val gas: Int
)