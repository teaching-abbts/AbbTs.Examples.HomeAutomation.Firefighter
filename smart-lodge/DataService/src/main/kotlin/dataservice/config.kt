/*
  Projekt:      SmartQuartier DataService
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: List Konfiguration ein und stellt sie der App zur Verfügung.
*/

package dataservice

import java.io.File
import java.lang.System.exit

object config {
    val VERSION = "0.5"
    var POLL_CYCLE_SECONDS = 30
    var REGISTER_PORT = -1
    var REQUEST_PORT = -1
    var PERSISTENCE = "None"
    var DATABASE_HOST = "None"
    var DATABASE_PORT = "None"
    var DATABASE_OPTIONS = "None"
    var DATABASE_USER = "None"
    var DATABASE_PASSWORD = "None"

    fun read(path: String) {
        println("Config: read $path")
        val file = File(path)
        val lines = file.readLines()
        for (line in lines) {
            when {
                line.startsWith("//")                -> continue
                line.trim().length == 0              -> continue
                line.startsWith("POLL_CYCLE")        -> POLL_CYCLE_SECONDS = extractParameter(line).toInt()
                line.startsWith("REGISTER_PORT")     -> REGISTER_PORT = extractParameter(line).toInt()
                line.startsWith("REQUEST_PORT")      -> REQUEST_PORT = extractParameter(line).toInt()
                line.startsWith("PERSISTENCE")       -> PERSISTENCE = extractParameter(line)
                else -> {
                    println("Config: unknown Parameter: $line")
                    exit(1)
                }
            }
            println("Config: $line")
        }
    }

    private fun extractParameter(line: String) = line.substringAfter(" = ").trim()
}