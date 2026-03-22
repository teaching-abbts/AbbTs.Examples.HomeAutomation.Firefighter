/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: SmartHome mit Gateway erweitert.
*/

package smarthome.communication.gateway.serviceprovider

data class ServiceProvider(
    val name: String,
    val ip: String,
    val portNr: Int
)
