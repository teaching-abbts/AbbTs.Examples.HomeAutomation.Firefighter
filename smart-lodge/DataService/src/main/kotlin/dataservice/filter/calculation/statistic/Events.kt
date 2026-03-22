/*
  Projekt:      SmartQuartier DataService
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali
*/

package dataservice.filter.calculation.statistic

data class Events(
     var gas: Int,
     var fire: Int,
     var motion: Int,
     var sound: Int,
     var rfid: Int
)