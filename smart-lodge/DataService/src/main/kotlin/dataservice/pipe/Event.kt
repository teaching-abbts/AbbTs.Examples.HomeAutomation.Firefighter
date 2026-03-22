/*
  Projekt:      SmartQuartier DataService
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali
*/

package dataservice.pipe

data class Event(
    val timeStamp: String,  // Format: YYYY-MM-DD hh:mm:ss
    val buildingID: String,
    val type: String,
    val data: String
)
