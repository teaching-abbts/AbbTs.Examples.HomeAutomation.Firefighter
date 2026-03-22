/*
  Projekt:        SmartHome
  Firma:          ABB Technikerschule
  Autor:          Marco Bontognali

  Beschreibung:   SmartHome mit Gateway erweitert.

  Design-Pattern: State
*/

package smarthome.communication.gateway.gatewaystate

import smarthome.communication.gateway.serviceprovider.ServiceProviderConnectionPool
import smarthome.communication.gateway.deviceandcontrol.DeviceAndControl
import smarthome.communication.gateway.deviceandcontrol.acquisition.Event
import smarthome.communication.gateway.deviceandcontrol.remotecontrol.Command
import smarthome.gui.monitoring.logger

open class ReadOnly(
    protected val deviceAndControl: DeviceAndControl,
    private val serviceProviderConnectionPool: ServiceProviderConnectionPool): GatewayState {

    private val READ_ONLY_TEXT = "SmartHome is read only"

    override fun pullState(): Any {
        logger.log("Gateway: pull State")
        return deviceAndControl.remoteControl.getState()
    }

    override fun pullMeasurement(): Any {
        logger.log("Gateway: pull Measurement")
        return deviceAndControl.acquisition.getMeasurement()
    }

    override fun push(event: Event) {
        logger.log("Gateway: push Event ${event.type}")
        serviceProviderConnectionPool.send(event)
    }

    override suspend fun push(command: Command) = READ_ONLY_TEXT
}