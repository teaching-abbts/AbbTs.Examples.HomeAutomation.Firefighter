/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: SmartHome mit Gateway erweitert.
*/

package smarthome.communication.gateway.deviceandcontrol.acquisition

import smarthome.*
import smarthome.device.sensor.*

class Acquisition {
    private var brightness: Brightness? = null
    private var temperature: Temperature? = null
    private var humidity: Humidity? = null
    private var gas: Gas? = null

    fun add(sensor: Analog) {
        when (sensor) {
            is Brightness  -> brightness = sensor
            is Temperature -> temperature = sensor
            is Humidity    -> humidity = sensor
            is Gas         -> gas = sensor
            else           -> println("Gateway add: unknown Sensor: $sensor")
        }
    }

    fun getMeasurement() = Measurement(
        utilities.now(),
        config.BUILDING_ID,
        brightness!!.value,
        temperature!!.value,
        humidity!!.value,
        gas!!.value
    )
}