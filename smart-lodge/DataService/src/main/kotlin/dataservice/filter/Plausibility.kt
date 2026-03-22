/*
  Projekt:      SmartQuartier DataService
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Plausibility überprüft eingehende Werte auf Gültigkeit und protokolliert falsche Daten.

  Todo:         Wenn die gleiche RFID an verschiedenen SmartHomes gleichzeitig erscheint testen.
*/

package dataservice.filter

import dataservice.pipe.*

class Plausibility(
    private val nextFilter: Processable) : Processable {

    private val MIN_MEASUREMENT = Measurement("none", "none", 0, 0, 20, 0)
    private val MAX_MEASUREMENT = Measurement("none", "none", 1023, 50, 90, 1023)
    private val EVENT_TYPES = setOf("Gas", "Fire", "Motion", "Sound", "RFID")

    override suspend fun process(measurement: Measurement) {
        if ( ((measurement.brightness  >= MIN_MEASUREMENT.brightness)  and (measurement.brightness  <= MAX_MEASUREMENT.brightness))  and
             ((measurement.temperature >= MIN_MEASUREMENT.temperature) and (measurement.temperature <= MAX_MEASUREMENT.temperature)) and
             ((measurement.humidity    >= MIN_MEASUREMENT.humidity)    and (measurement.humidity    <= MAX_MEASUREMENT.humidity))    and
             ((measurement.gas         >= MIN_MEASUREMENT.gas)         and (measurement.gas         <= MAX_MEASUREMENT.gas)) ) {
            println("Plausibility:        Measurement-Data ok")
        }
        else {
            println("Plausibility:        Measurement-Data not ok: $measurement")
        }
        nextFilter.process(measurement)
    }

    override suspend fun process(event: Event) {
        if ( (event.timeStamp.length == 19) and
             event.buildingID.isNotBlank()  and
             EVENT_TYPES.contains(event.type) ) {
            println("Plausibility:        Event-Data ok")
        }
        else {
            println("Plausibility:        Event-Data not ok: $event")
        }
        nextFilter.process(event)
    }
}