/*
  Projekt:      SmartQuartier DataService
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali
*/

package dataservice.filter.calculation.statistic

data class Measurements(
    val brightness: ValueTyp,
    val temperature: ValueTyp,
    val humidity: ValueTyp,
    val gas: ValueTyp
)
