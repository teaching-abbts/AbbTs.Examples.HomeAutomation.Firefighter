/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: SmartHome mit Gateway erweitert.
*/

package com.example

data class State(
    val gateway: String,      // readonly, readandwrite
    val lightControl: String,
    val lightControlSetPoint: Int,
    val heatingControl: String,
    val heatingControlSetPoint: Int,
    val alarmControl: String
)
