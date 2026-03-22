/*
  Projekt:      SmartQuartier DataService
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali
*/

package dataservice.filter.calculation.statistic

data class ValueTimeSpace(
    var timeStamp: String,  // Format: YYYY-MM-DD hh:mm:ss
    var buildingID: String,
    var value: Int
)
