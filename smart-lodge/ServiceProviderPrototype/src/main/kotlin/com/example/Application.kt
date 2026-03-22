/*
  Projekt:      SmartQuartier
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Prototyp um SmartHome fernzusteuern via WebSocket
                V0.5: State erweitert damit weniger Kommunikation nötig ist
*/

package com.example

import io.ktor.serialization.gson.*
import io.ktor.server.application.*
import io.ktor.server.engine.*
import io.ktor.server.netty.*
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

var smartHome: DefaultWebSocketServerSession? = null
val mutex = Mutex()

suspend fun main() {
    println("ServiceProvider Prototype Version 0.5")

    coroutineScope {
        launch {
            println("Server started...")
            embeddedServer(Netty, port = 8080, host = "0.0.0.0", module = Application::module)
                .start(wait = true)
        }

        launch {
            println("Waiting for SmartHome...")

            while (smartHome == null) {
                delay (200)
            }
            println("SmartHome is connected")
            println("Type your input commands: [s]tate, [o]pen, [c]lose, [d]isplay, etc...")

            while (true) {
                val command = readLine()

                mutex.withLock {
                    when (command) {
                        "state", "s" -> {
                            smartHome?.send("get state")
                        }
                        "open", "o" -> {
                            smartHome?.send("send command")
                            val command = Command("Door", "open", "")
                            smartHome?.sendSerialized(command)
                        }
                        "close", "c" -> {
                            smartHome?.send("send command")
                            val command = Command("Door", "close", "")
                            smartHome?.sendSerialized(command)
                        }
                        "display", "d" -> {
                            smartHome?.send("send command")
                            val command = Command("Display", "", "Hello;SmartHome")
                            smartHome?.sendSerialized(command)
                        }
                        "lightControl on", "lon" -> {
                            smartHome?.send("send command")
                            val command = Command("LightControl", "on", "")
                            smartHome?.sendSerialized(command)
                        }
                        "lightControl off", "loff" -> {
                            smartHome?.send("send command")
                            val command = Command("LightControl", "off", "")
                            smartHome?.sendSerialized(command)
                        }
                        "lightControl setpoint", "lsp" -> {
                            smartHome?.send("send command")
                            val command = Command("LightControl", "setpoint", "333")
                            smartHome?.sendSerialized(command)
                        }
                        "heatingControl on", "hon" -> {
                            smartHome?.send("send command")
                            val command = Command("HeatingControl", "on", "")
                            smartHome?.sendSerialized(command)
                        }
                        "heatingControl off", "hoff" -> {
                            smartHome?.send("send command")
                            val command = Command("HeatingControl", "off", "")
                            smartHome?.sendSerialized(command)
                        }
                        "heatingControl setpoint", "hsp" -> {
                            smartHome?.send("send command")
                            val command = Command("HeatingControl", "setpoint", "22")
                            smartHome?.sendSerialized(command)
                        }
                        "alarmControl on", "aon" -> {
                            smartHome?.send("send command")
                            val command = Command("AlarmControl", "on", "")
                            smartHome?.sendSerialized(command)
                        }
                        "alarmControl off", "aoff" -> {
                            smartHome?.send("send command")
                            val command = Command("AlarmControl", "off", "")
                            smartHome?.sendSerialized(command)
                        }
                        "measurement", "m" -> {
                            smartHome?.send("get measurement")
                        }
                        else -> println("Unknown Command!")
                    }
                }
            }
        }
    }
}

fun Application.module() {
    install(WebSockets) {
        //pingPeriod = Duration.ofSeconds(15)
        contentConverter = GsonWebsocketContentConverter()
        timeout = Duration.ofSeconds(15)
        maxFrameSize = Long.MAX_VALUE
        masking = false
    }

    routing {
        webSocket("/smart-home/data") {
            println("onConnect")
            mutex.withLock {
                smartHome = this
            }

            try {
                for (frame in incoming) {
                    mutex.withLock {
                        if (frame is Frame.Text) {
                            val receivedText = frame.readText()
//                            print("Received: $receivedText")
//                            print(" | from ${call.request.host()} port: ${call.request.port()}")
//                            println(" | ws $this")

                            when (receivedText) {
                                "send registration" -> {
                                    val registration = receiveDeserialized<Registration>()
                                    println("Received: $registration")
                                }

                                "send event" -> {
                                    val event = receiveDeserialized<Event>()
                                    println("Received: $event")
                                }

                                "send state" -> {
                                    val state = receiveDeserialized<State>()
                                    println("Received: $state")
                                }

                                "send close" -> {
                                }

                                else -> {
                                    println("Received: $receivedText")
                                }
                            }
                            delay(100)
                        }
                    }
                }
            }
            catch (e: ClosedReceiveChannelException) {
                mutex.withLock {
                    println("onClose ${closeReason.await()}")
                }
            }
            catch (e: Throwable) {
                mutex.withLock {
                    println("onError ${closeReason.await()}")
                    e.printStackTrace()
                }
            }
        }
    }
}