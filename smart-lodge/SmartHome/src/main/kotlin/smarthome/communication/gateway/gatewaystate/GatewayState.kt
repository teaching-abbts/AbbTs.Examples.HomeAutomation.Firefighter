/*
  Projekt:        SmartHome
  Firma:          ABB Technikerschule
  Autor:          Marco Bontognali

  Beschreibung:   SmartHome mit Gateway erweitert.

  Design-Pattern: State
*/

package smarthome.communication.gateway.gatewaystate

import smarthome.communication.gateway.deviceandcontrol.acquisition.Event
import smarthome.communication.gateway.deviceandcontrol.remotecontrol.Command

interface GatewayState {
    fun pullState(): Any        // Rückgabe: wenn OK -> State, wenn Error -> String mit Fehlermeldung
    fun pullMeasurement(): Any                 // Rückgabe: wenn OK -> Measurement, wenn Error -> String mit Fehlermeldung
    fun push(event: Event)
    suspend fun push(command: Command): String // Rückgabe: "command executed" oder Fehlermeldung
}

