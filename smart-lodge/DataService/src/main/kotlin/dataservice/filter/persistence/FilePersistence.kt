/*
  Projekt:      SmartQuartier DataService
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Persistence speichert Daten langlebig. Als Variante File realisiert.
*/

package dataservice.filter.persistence

import dataservice.filter.calculation.Data
import dataservice.pipe.*
import kotlinx.coroutines.sync.Mutex
import kotlinx.coroutines.sync.withLock
import java.io.File

class FilePersistence : Processable, HistoryRequestable {
    private val measurementFile = File("SmartQuartierMeasurements.csv")
    private val eventFile = File("SmartQuartierEvents.csv")
    private val mutex = Mutex()

    init {
        println("Persistence: Files $measurementFile and $eventFile ready")
    }

    override suspend fun process(measurement: Measurement) {
        val line = "${measurement.timeStamp};${measurement.buildingID};${measurement.brightness};${measurement.temperature};${measurement.humidity};${measurement.gas}\n"
        mutex.withLock {
            measurementFile.appendText(line)
            println("Persistence:         Measurement stored")
        }
    }

    override suspend fun process(event: Event) {
        val line = "${event.timeStamp};${event.buildingID};${event.type};${event.data}\n"
        mutex.withLock {
            eventFile.appendText(line)
            println("Persistence:         Event stored")
        }
    }

    override suspend fun requestHistory(fromTimeStamp: String, toTimeStamp: String, buildingID: String): Data {
        mutex.withLock {
            println("\tPersistence:               load")
            return Data(
                requestMeasurement(fromTimeStamp, toTimeStamp, buildingID),
                requestEvent(fromTimeStamp, toTimeStamp, buildingID)
            )
        }
    }

    private fun requestMeasurement(fromTimeStamp: String, toTimeStamp: String, buildingID: String): List<Measurement> {
        val measurements = mutableListOf<Measurement>()

        val lines = measurementFile.readLines()
        for (line in lines) {
            val parameter = line.split(";")
            val measurement = Measurement(
                parameter[0],
                parameter[1],
                parameter[2].toInt(),
                parameter[3].toInt(),
                parameter[4].toInt(),
                parameter[5].toInt()
            )
            when {
                (measurement.timeStamp < fromTimeStamp) -> continue
                (measurement.timeStamp > toTimeStamp) -> break
                (buildingID == "") -> measurements += measurement
                (measurement.buildingID == buildingID) -> measurements += measurement
                else -> continue
            }
        }
        return measurements
    }

    private fun requestEvent(fromTimeStamp: String, toTimeStamp: String, buildingID: String): List<Event> {
        val events = mutableListOf<Event>()

        val lines = eventFile.readLines()
        for (line in lines) {
            val parameter = line.split(";")
            val event = Event(
                parameter[0],
                parameter[1],
                parameter[2],
                parameter[3]
            )
            when {
                (event.timeStamp < fromTimeStamp) -> continue
                (event.timeStamp > toTimeStamp) -> break
                (buildingID == "") -> events += event
                (event.buildingID == buildingID) -> events += event
                else -> continue
            }
        }
        return events
    }
}