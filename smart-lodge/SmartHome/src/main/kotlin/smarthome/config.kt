/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome

import java.io.File

object config {
    val VERSION = "0.5"
    val DEVICE_ZOOM = 3.0
    val DEVICE_SIZE = 27.0 * DEVICE_ZOOM

    var HARDWARE_ON = "false"
    var COM_PORT = "None"
    var BUILDING_ID = "no Building ID"
    var GAS_ALARM_VALUE = 100
    var FIRE_ALARM_VALUE = 800
    var RFID_CARD_USER = "000-000-000-000-001"
    var RFID_KEY_USER = "000-000-000-000-002"
    var TELECOM_SERVER_IP = "no Server"
    var X_COORDINATE = 0
    var Y_COORDINATE = 0
    var OWNER = "no Owner"
    val SERVICE_PROVIDERS = mutableListOf<String>()

    fun read(fileName: String) {
        println("Config: read $fileName")
        val file = File(fileName)
        val lines = file.readLines()
        for (line in lines) {
            when {
                line.startsWith("//")                -> continue
                line.trim().length == 0              -> continue
                line.startsWith("HARDWARE_ON")       -> HARDWARE_ON = extractParameter(line)
                line.startsWith("COM_PORT")          -> COM_PORT = extractParameter(line)
                line.startsWith("BUILDING_ID")       -> BUILDING_ID = extractParameter(line)
                line.startsWith("GAS_ALARM_VALUE")   -> GAS_ALARM_VALUE = extractParameter(line).toInt()
                line.startsWith("FIRE_ALARM_VALUE")  -> FIRE_ALARM_VALUE = extractParameter(line).toInt()
                line.startsWith("RFID_CARD_USER")    -> RFID_CARD_USER = extractParameter(line)
                line.startsWith("RFID_KEY_USER")     -> RFID_KEY_USER = extractParameter(line)
                line.startsWith("TELECOM_SERVER_IP") -> TELECOM_SERVER_IP = extractParameter(line)
                line.startsWith("X_COORDINATE")      -> X_COORDINATE = extractParameter(line).toInt()
                line.startsWith("Y_COORDINATE")      -> Y_COORDINATE = extractParameter(line).toInt()
                line.startsWith("OWNER")             -> OWNER = extractParameter(line)
                line.startsWith("SERVICE_PROVIDER")  -> SERVICE_PROVIDERS += extractParameter(line)
                else -> {
                    println("Config: unknown Parameter: $line")
                    System.exit(1)
                }
            }
            println("Config: $line")
        }
    }

    private fun extractParameter(line: String) = line.trim().split("=")[1].trim()
}