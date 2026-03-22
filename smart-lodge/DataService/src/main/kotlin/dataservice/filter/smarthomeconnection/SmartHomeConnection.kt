/*
  Projekt:      SmartQuartier DataService
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: - Verwaltet Verbindungen zu mehreren SmartHomes
                - Liest zyklisch Messwerte
                - Wird bei Ereignissen informiert
                - Sendet Daten an das nächste Filter

  Todo:         eventuell nextFilterHelper anders implementieren

*/

package dataservice.filter.smarthomeconnection

import dataservice.config
import dataservice.pipe.Event
import dataservice.pipe.Measurement
import dataservice.pipe.Processable
import io.ktor.serialization.gson.*
import io.ktor.server.application.*
import io.ktor.server.engine.*
import io.ktor.server.netty.*
import io.ktor.server.plugins.*
import io.ktor.server.request.*
import io.ktor.server.routing.*
import io.ktor.server.websocket.*
import io.ktor.websocket.*
import kotlinx.coroutines.channels.ClosedReceiveChannelException
import kotlinx.coroutines.coroutineScope
import kotlinx.coroutines.delay
import kotlinx.coroutines.launch
import kotlinx.coroutines.sync.Mutex
import kotlinx.coroutines.sync.withLock
import java.time.Duration

private var nextFilterHelper: Processable? = null
private var quartier = mutableListOf<DefaultWebSocketServerSession>()
private val notRespondingSmartHomes = mutableSetOf<DefaultWebSocketServerSession>()
private val mutex = Mutex()

class SmartHomeConnection(
    private val nextFilter: Processable) {

    init {
        nextFilterHelper = nextFilter
    }

    suspend fun start() {
        coroutineScope {
            launch {
                try {
                    println("SmartHomeConnection: started")
                    embeddedServer(Netty, port = config.REGISTER_PORT, host = "0.0.0.0", module = Application::module)
                    .start(wait = true)
                }
                catch (ex: Exception) {
                    println("SmartHomeConnection: could not start Server")
                }
            }

            launch {
                println("SmartHomeConnection: Measurement polling started")
                while (true) {
                    mutex.withLock {
                        pollQuartier()
                        removeNotRespondingSmartHomes()
                    }
                    wait()
                }
            }
        }
    }

    private suspend fun pollQuartier() {
        println("---------------------------------------------------------------------------------------------")
        for (smartHome in quartier) {
            try {
                println("SmartHomeConnection: <-- get measurement")
                smartHome.send("get measurement")
            }
            catch (e: Exception) {
                println("SmartHomeConnection: SmartHome is not responding")
                notRespondingSmartHomes += smartHome
            }
            delay(100)
        }
    }

    private fun removeNotRespondingSmartHomes() {
        if (notRespondingSmartHomes.isNotEmpty()) {
            quartier -= notRespondingSmartHomes
            notRespondingSmartHomes.clear()
            println("SmartHomeConnection: not responding SmartHomes removed")
        }
    }

    private suspend fun wait() {
        delay(config.POLL_CYCLE_SECONDS.toLong() * 1000)
    }
}

fun Application.module() {
    install(WebSockets) {
        //pingInterval = Duration.ofSeconds(15)
        contentConverter = GsonWebsocketContentConverter()
        timeout = Duration.ofSeconds(15)
        maxFrameSize = Long.MAX_VALUE
        masking = false
    }

    routing {
        webSocket("/smart-home/data") {
            var smartHome: Registration? = null
            mutex.withLock {
                //println("SmartHomeConnection: on connect")
                quartier += this
                //println("SmartHomeConnection: new Quartier $quartier")
            }

            try {
                for (frame in incoming) {
                    mutex.withLock {
                        if (frame is Frame.Text) {
                            val receivedText = frame.readText()
                            show(receivedText)
                            handle(receivedText)
                            delay(100)
                        }
                    }
                }
            }
            catch (e: ClosedReceiveChannelException) {
                mutex.withLock {
                    println("SmartHomeConnection: close")
                }
            }
            catch (e: Throwable) {
                mutex.withLock {
                    println("SmartHomeConnection: exit with open session")
                    println(e)
                }
            }
        }
    }
}

private fun DefaultWebSocketServerSession.show(receivedText: String) {
    print("SmartHomeConnection: --> $receivedText")
    println(" | from ${call.request.origin.remoteAddress} port: ${call.request.port()}")
//    println(" | ws $this")
}

private suspend fun DefaultWebSocketServerSession.handle(receivedText: String) {
    when (receivedText) {
        "send registration" -> {
            val registration = receiveDeserialized<Registration>()
            println("SmartHomeConnection: --> $registration")
        }
        "send event" -> {
            val event = receiveDeserialized<Event>()
            println("SmartHomeConnection: --> $event")
            nextFilterHelper?.process(event)
        }
        "send measurement" -> {
            val measurement = receiveDeserialized<Measurement>()
            println("SmartHomeConnection: --> $measurement")
            nextFilterHelper?.process(measurement)
        }
        "send state" -> {
            val state = receiveDeserialized<State>()
            println("SmartHomeConnection: --> $state")
        }
        "send close" -> {
            //println("SmartHomeConnection: old Quartier $quartier")
            quartier -= this
            //println("SmartHomeConnection: new Quartier $quartier")
        }
        "SmartHome is offline" -> {
        }
        else -> {
            println("SmartHomeConnection: unexpected text: $receivedText")
        }
    }
}
