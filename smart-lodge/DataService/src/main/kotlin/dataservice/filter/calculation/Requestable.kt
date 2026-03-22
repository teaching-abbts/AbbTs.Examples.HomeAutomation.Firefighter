/*
  Projekt:      SmartQuartier DataService
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Format fromTimeStamp und toTimeStamp YYYY-MM-DD hh:mm:ss
*/

package dataservice.filter.calculation

import dataservice.filter.calculation.statistic.Statistic
import dataservice.filter.persistence.HistoryRequestable

interface Requestable : HistoryRequestable {
    suspend fun requestStatistic(fromTimeStamp: String, toTimeStamp: String, buildingID: String): Statistic
    suspend fun requestForecast(fromTimeStamp: String, toTimeStamp: String, buildingID: String): Data
}