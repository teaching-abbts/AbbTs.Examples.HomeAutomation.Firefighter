/*
  Projekt:        SmartHome
  Firma:          ABB Technikerschule
  Autor:          Marco Bontognali

  Beschreibung:   SmartHome mit Gateway erweitert.

  Design-Pattern: Singleton und State
*/

package smarthome.communication.gateway

import smarthome.communication.gateway.deviceandcontrol.DeviceAndControl
import smarthome.communication.gateway.deviceandcontrol.acquisition.Event
import smarthome.communication.gateway.deviceandcontrol.remotecontrol.Command
import smarthome.communication.gateway.gatewaystate.*
import smarthome.communication.gateway.serviceprovider.ServiceProviderConnectionPool
import smarthome.gui.monitoring.logger

object gateway {
    private var state: GatewayState = Offline()
    private val deviceAndControl = DeviceAndControl()
    private val serviceProviderConnectionPool = ServiceProviderConnectionPool()

    fun add(element: Any) {
        deviceAndControl.add(element)
    }

    fun start() {
        serviceProviderConnectionPool.start()
        logger.log("Gateway: gestartet")
    }

    fun stop() {
        serviceProviderConnectionPool.close()
        logger.log("Gateway: gestoppt")
    }

    fun getState() = state.javaClass.simpleName.lowercase()

    fun setOffline() {
        state = Offline()
        logger.log("Gateway: Offline")
    }

    fun setReadOnly() {
        state = ReadOnly(deviceAndControl, serviceProviderConnectionPool)
        logger.log("Gateway: nur Lesen")
    }

    fun setReadAndWrite() {
        state = ReadAndWrite(deviceAndControl, serviceProviderConnectionPool)
        logger.log("Gateway: Lesen und Schreiben")
    }

    fun pullState() = state.pullState()
    fun pullMeasurement() = state.pullMeasurement()
    fun push(event: Event) = state.push(event)
    suspend fun push(command: Command) = state.push(command)
}