/*
  Projekt:      SmartQuartier DataService
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Liest Livedaten und Ereignisse von Smarthomes und speichert diese langlebig.
                ServiceProvider können Histroy-, Statistic- und Forcast-Daten abfragen.

                Es wird ein Pipes and Filters Architekturmuster eingesetzt.
*/

package dataservice

import dataservice.filter.*
import dataservice.filter.calculation.Calculation
import dataservice.filter.persistence.FilePersistence
import dataservice.filter.persistence.HistoryRequestable
import dataservice.filter.smarthomeconnection.SmartHomeConnection
import dataservice.pipe.Processable
import java.net.InetAddress
import kotlin.system.exitProcess

suspend fun main() {
    showInfo()
    readConfiguration()
    buildAndStart()
}

private fun showInfo() {
    println()
    println("#########################################")
    println("  SmartQuartier DataService Version ${config.VERSION}")
    println("#########################################")
    println()
    val address = InetAddress.getLocalHost()
    println("DataService Host: ${address.hostName}")
    println("DataService IP:   ${address.hostAddress}")
}

private fun readConfiguration() {
    config.read("DataService.conf")
}

private suspend fun buildAndStart() {
    val persistence = choosePersistence()
    val serviceProviderConnection = ServiceProviderConnection(Calculation(persistence as HistoryRequestable))
    val smartHomeConnection = SmartHomeConnection(Plausibility(persistence as Processable))

    serviceProviderConnection.start()
    smartHomeConnection.start()
}

private fun choosePersistence() = when (config.PERSISTENCE) {
    "File"     -> FilePersistence()
    "Database" -> {
        println("Database not implemented yet")
        exitProcess(-1)
    }
    else       -> {
        println("No persistence configured")
        exitProcess(-1)
    }
}
