/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Hilfsklasse für den TelecomClient.
*/

package smarthome.communication.telecom

import java.io.BufferedReader
import java.io.InputStreamReader
import java.io.PrintWriter
import java.net.Socket

class Connection(
    private val name: String,
    private val socket: Socket) {
    private val output = PrintWriter(socket.getOutputStream())
    private val input = BufferedReader(InputStreamReader(socket.getInputStream()))

    fun send(message: String) {
        output.println(message)
        output.flush()
        println("$name: gesendet:  $message")
    }

    fun receive(): String {
        val message = input.readLine()
        println("$name: empfangen: $message")
        return message
    }

    fun close() {
        input.close()
        output.close()
        socket.close()
    }
}