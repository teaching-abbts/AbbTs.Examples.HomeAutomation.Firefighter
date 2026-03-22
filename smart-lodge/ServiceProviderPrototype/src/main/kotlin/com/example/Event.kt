/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: SmartHome mit Gateway erweitert.
*/

package com.example

data class Event(
    val timeStamp: String,  // Format: YYYY-MM-DD hh:mm:ss
    val buildingID: String,
    val type: String,
    val data: String
)