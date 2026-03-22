/*
  Projekt:      SmartQuartier DataService
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali
*/

package dataservice.filter.calculation.statistic

data class ValueTyp(
    val min: ValueTimeSpace,
    val max: ValueTimeSpace,
    val average: Double
)
