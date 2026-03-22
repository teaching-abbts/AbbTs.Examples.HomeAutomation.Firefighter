/*
  Projekt:        SmartHome
  Firma:          ABB Technikerschule
  Autor:          Marco Bontognali

  Beschreibung:   SmartHome mit Gateway erweitert.

  Design-Pattern: State
*/

package smarthome.communication.gateway.gatewaystate

import kotlinx.coroutines.sync.withLock
import smarthome.communication.gateway.serviceprovider.ServiceProviderConnectionPool
import smarthome.communication.gateway.deviceandcontrol.DeviceAndControl
import smarthome.communication.gateway.deviceandcontrol.remotecontrol.Command
import smarthome.control.supervisor
import smarthome.gui.monitoring.logger

class ReadAndWrite(
    deviceAndControl: DeviceAndControl,
    serviceProviderConnectionPool: ServiceProviderConnectionPool
) : ReadOnly(deviceAndControl, serviceProviderConnectionPool), GatewayState {

    override suspend fun push(command: Command): String {
        var result: String
        supervisor.mutex.withLock {
            logger.log("Gateway: push ${command.device} ${command.command}")
            result = deviceAndControl.remoteControl.execute(command)
        }
        return result
    }
}