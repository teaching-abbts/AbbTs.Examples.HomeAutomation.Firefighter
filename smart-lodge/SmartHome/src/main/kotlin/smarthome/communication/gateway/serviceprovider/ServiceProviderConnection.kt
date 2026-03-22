/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: SmartHome mit Gateway erweitert.
*/

package smarthome.communication.gateway.serviceprovider

import io.ktor.client.*
import io.ktor.client.engine.cio.*
import io.ktor.client.plugins.websocket.*
import io.ktor.http.*
import io.ktor.serialization.gson.*
import io.ktor.websocket.*
import kotlinx.coroutines.*
import kotlinx.coroutines.sync.Mutex
import kotlinx.coroutines.sync.withLock
import smarthome.communication.gateway.deviceandcontrol.remotecontrol.Command
import smarthome.communication.gateway.gateway
import smarthome.gui.monitoring.logger

class ServiceProviderConnection(
    private val serviceProvider: ServiceProvider,
    private val registration: Registration) {
    private val client = HttpClient(CIO) {
        install(WebSockets) {
            //pingInterval = 20_000
            contentConverter = GsonWebsocketContentConverter()
        }
    }
    private var session: DefaultClientWebSocketSession? = null

    private val sendMutex = Mutex()

    fun start() {
        GlobalScope.launch {
            try {
                println("ServiceProviderConnection: connect with $serviceProvider")
                session = client.webSocketSession(
                    method = HttpMethod.Get,
                    host = serviceProvider.ip,
                    port = serviceProvider.portNr,
                    path = "/smart-home/data"
                ) {
                }
                send(registration)
                receive()
            }
            catch (ex: Exception) {
                println("ServiceProviderConnection: $serviceProvider not reachable")
                logger.log("Gateway: ${serviceProvider.name} nicht erreichbar")
            }
        }
    }

    suspend fun send(message: Any) {
        try {
            if (message is String) {
                session?.send(message)
            }
            else {
                sendMutex.withLock {
                    val className = message.javaClass.simpleName.lowercase()
                    session?.send("send $className")
                    println("ServiceProviderConnection: --> send $className")
                    session?.sendSerialized(message)
                }
            }
        }
        catch (ex: Exception) {
            println("ServiceProviderConnection: $serviceProvider not reachable")
            logger.log("Gateway: ${serviceProvider.name} nicht erreichbar")
        }
        println("ServiceProviderConnection: --> $message")
    }

    private fun receive() {
        GlobalScope.launch {
            while (true) {
                try {
                    var message = session?.incoming?.receive() as? Frame.Text
                    var receivedText = message?.readText()
                    println("ServiceProviderConnection: <-- $receivedText")
                    handle(receivedText)
                    delay(100)
                }
                catch (ex: Exception) {
                    println("ServiceProviderConnection: $serviceProvider closed session")
                    logger.log("Gateway: ${serviceProvider.name} beendet Verbindung")
                    break
                }
            }
        }
    }

    private suspend fun handle(receivedText: String?) {
        when (receivedText) {
            "get state" -> {
                send(gateway.pullState())
            }
            "get measurement" -> {
                send(gateway.pullMeasurement())
            }
            "send command" -> {
                val command = session?.receiveDeserialized<Command>()
                if (command?.device == "Display") {
                    command.command = serviceProvider.name
                }
                println("ServiceProviderConnection: <-- $command")
                send(gateway.push(command!!))
            }
            else -> {
                println("ServiceProviderConnection: unexpected text: $receivedText")
            }
        }
    }

    suspend fun close() {
        send("send close")
        delay(200)
        println("ServiceProviderConnection: closing...")
        client.close()
        println("ServiceProviderConnection: ...closed")
    }
}

