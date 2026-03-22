/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Registriert sich beim TelecomServer.
                Gibt auf Anfrage des TelecomServer Status, Betriebssystem und Speicherverbrauch bekannt.
*/

package smarthome.communication.telecom

import javafx.concurrent.Task
import smarthome.config.BUILDING_ID
import smarthome.config.TELECOM_SERVER_IP
import smarthome.gui.monitoring.logger
import java.net.InetAddress
import java.net.ServerSocket
import java.net.Socket

class TelecomClient : Task<Int>() {
    private val TELECOM_SERVER_PORT = 9010
    private val TELECOM_CLIENT_PORT = 9020

    fun register() {
        val address = InetAddress.getLocalHost()
        println("TelecomClient: SmartHome Host: ${address.hostName}")
        println("TelecomClient: SmartHome IP:   ${address.hostAddress}")

        try {
            logger.log("Registrieren beim TelecomServer")
            val connection = Connection("TelecomClient", Socket(TELECOM_SERVER_IP, TELECOM_SERVER_PORT))
            connection.send("register $BUILDING_ID")
            connection.receive()
            connection.close()

            Thread(this).start()
        }
        catch (e: Exception) {
            logger.log("TelecomServer nicht erreichbar")
            println("TelecomClient: $e")
        }
    }

    override fun call(): Int {
        val serverSocket = ServerSocket(TELECOM_CLIENT_PORT)

        while (!isCancelled) {
            println("TelecomClient: warten auf Port $TELECOM_CLIENT_PORT")
            val socket = serverSocket.accept()
            println("TelecomClient: Verbindung hergestellt: $socket")

            val connection = Connection("TelecomClient", socket)

            if (connection.receive() == "get state") {
                logger.log("TelecomServer Statusrequest empfangen")
                connection.send("online;$metrics")
            }
            connection.close()
        }
        return 0
    }
}