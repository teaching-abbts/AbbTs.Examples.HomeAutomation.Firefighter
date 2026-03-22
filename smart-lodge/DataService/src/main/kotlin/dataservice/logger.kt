/*
  Projekt:      SmartQuartier DataService
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Logger schreibt ServiceProvider-Abfragen in ein Log-File.
*/

package dataservice

import java.io.File
import java.time.LocalDateTime
import java.time.format.DateTimeFormatter

object logger {
    private val file = File("DataService.log")

    fun log(message: String) {
        val text = "${now()}\t$message"
        file.appendText(text + "\n")
    }

    private fun now() = toDateTimeString(LocalDateTime.now())

    private fun toDateTimeString(timeStamp: LocalDateTime): String {
        val formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss")
        return formatter.format(timeStamp)
    }
}