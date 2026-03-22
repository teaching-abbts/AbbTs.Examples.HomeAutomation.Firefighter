/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: SmartHome mit Gateway erweitert.
*/

package com.example

data class Command(
    val device: String,
    var command: String,
    var value: String
)
