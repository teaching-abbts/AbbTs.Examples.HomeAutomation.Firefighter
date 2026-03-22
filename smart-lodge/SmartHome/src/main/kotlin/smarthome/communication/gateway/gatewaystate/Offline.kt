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

class Offline: GatewayState {
    private val OFFLINE_TEXT = "SmartHome is offline"

    override fun pullState() = OFFLINE_TEXT
    override fun pullMeasurement() = OFFLINE_TEXT
    override fun push(event: Event) {}
    override suspend fun push(command: Command) = OFFLINE_TEXT
}