/*
  Projekt:      SmartQuartier DataService
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Format fromTimeStamp und toTimeStamp YYYY-MM-DD hh:mm:ss
*/

package dataservice.filter.persistence

import dataservice.filter.calculation.Data

interface HistoryRequestable {
    suspend fun requestHistory(fromTimeStamp: String, toTimeStamp: String, buildingID: String): Data
}