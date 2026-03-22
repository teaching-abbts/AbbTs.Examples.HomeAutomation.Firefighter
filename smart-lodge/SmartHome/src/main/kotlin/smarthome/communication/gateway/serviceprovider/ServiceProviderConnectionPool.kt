/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: SmartHome mit Gateway erweitert.
*/

package smarthome.communication.gateway.serviceprovider

import kotlinx.coroutines.GlobalScope
import kotlinx.coroutines.launch
import smarthome.communication.gateway.deviceandcontrol.acquisition.Event
import smarthome.config
import smarthome.gui.monitoring.logger

class ServiceProviderConnectionPool {
    private val registration = Registration(config.BUILDING_ID, config.X_COORDINATE, config.Y_COORDINATE, config.OWNER)
    private val connections = mutableListOf<ServiceProviderConnection>()

    fun start() {
        for (service in config.SERVICE_PROVIDERS) {
            val serviceProvider = create(service)
            val serviceProviderConnection = ServiceProviderConnection(serviceProvider, registration)
            serviceProviderConnection.start()
            connections += serviceProviderConnection
            logger.log("Gateway: registriere bei ${serviceProvider.name}")
        }
    }

    fun send(event: Event) {
        GlobalScope.launch {
            for (connection in connections) {
                connection.send(event)
            }
        }
    }

    fun close() {
        GlobalScope.launch {
            for (connection in connections) {
                connection.close()
            }
        }
    }

    private fun create(service: String): ServiceProvider {
        val parameter = service.split(";")
        val name = parameter[0].trim()
        val ipAndPort = parameter[1].split(":")
        val ip = ipAndPort[0].trim()
        val portNr = ipAndPort[1].toInt()
        return ServiceProvider(name, ip, portNr)
    }
}