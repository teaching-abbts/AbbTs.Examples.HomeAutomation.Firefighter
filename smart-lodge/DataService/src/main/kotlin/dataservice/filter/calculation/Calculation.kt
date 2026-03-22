/*
  Projekt:      SmartQuartier DataService
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Calculation erstellt Histories, Statistiken oder Vorhersagen.

  Todo:         requestForecast() implementieren
*/

package dataservice.filter.calculation

import dataservice.filter.calculation.statistic.*
import dataservice.filter.persistence.HistoryRequestable

class Calculation(
    private val nextFilter: HistoryRequestable) : Requestable {

    override suspend fun requestHistory(fromTimeStamp: String, toTimeStamp: String, buildingID: String): Data {
        return nextFilter.requestHistory(fromTimeStamp, toTimeStamp, buildingID)
    }

    override suspend fun requestStatistic(fromTimeStamp: String, toTimeStamp: String, buildingID: String): Statistic {
        println("\tCalculation:               calculate Statistic")
        val history = requestHistory(fromTimeStamp, toTimeStamp, buildingID)
        return Statistic(
            calculateMeasurements(history),
            calculateEvents(history)
        )
    }

    override suspend fun requestForecast(fromTimeStamp: String, toTimeStamp: String, buildingID: String): Data {
        println("\tCalculation:               calculate Forecast")
        return requestHistory("", "", "") // todo
    }

    private fun calculateMeasurements(history: Data): Measurements {
        val minBrightness = ValueTimeSpace("none", "none", Int.MAX_VALUE)
        val maxBrightness = ValueTimeSpace("none", "none", Int.MIN_VALUE)
        var averageBrightness = 0.0

        val minTemperature = ValueTimeSpace("none", "none", Int.MAX_VALUE)
        val maxTemperature = ValueTimeSpace("none", "none", Int.MIN_VALUE)
        var averageTemperature = 0.0

        val minHumidity = ValueTimeSpace("none", "none", Int.MAX_VALUE)
        val maxHumidity = ValueTimeSpace("none", "none", Int.MIN_VALUE)
        var averageHumidity = 0.0

        val minGas = ValueTimeSpace("none", "none", Int.MAX_VALUE)
        val maxGas = ValueTimeSpace("none", "none", Int.MIN_VALUE)
        var averageGas = 0.0

        for (historyMeasurement in history.measurements) {
            if (historyMeasurement.brightness < minBrightness.value) {
                minBrightness.timeStamp = historyMeasurement.timeStamp
                minBrightness.buildingID = historyMeasurement.buildingID
                minBrightness.value = historyMeasurement.brightness
            }
            if (historyMeasurement.brightness > maxBrightness.value) {
                maxBrightness.timeStamp = historyMeasurement.timeStamp
                maxBrightness.buildingID = historyMeasurement.buildingID
                maxBrightness.value = historyMeasurement.brightness
            }
            averageBrightness += historyMeasurement.brightness

            if (historyMeasurement.temperature < minTemperature.value) {
                minTemperature.timeStamp = historyMeasurement.timeStamp
                minTemperature.buildingID = historyMeasurement.buildingID
                minTemperature.value = historyMeasurement.temperature
            }
            if (historyMeasurement.temperature > maxTemperature.value) {
                maxTemperature.timeStamp = historyMeasurement.timeStamp
                maxTemperature.buildingID = historyMeasurement.buildingID
                maxTemperature.value = historyMeasurement.temperature
            }
            averageTemperature += historyMeasurement.temperature

            if (historyMeasurement.humidity < minHumidity.value) {
                minHumidity.timeStamp = historyMeasurement.timeStamp
                minHumidity.buildingID = historyMeasurement.buildingID
                minHumidity.value = historyMeasurement.humidity
            }
            if (historyMeasurement.humidity > maxHumidity.value) {
                maxHumidity.timeStamp = historyMeasurement.timeStamp
                maxHumidity.buildingID = historyMeasurement.buildingID
                maxHumidity.value = historyMeasurement.humidity
            }
            averageHumidity += historyMeasurement.humidity

            if (historyMeasurement.gas < minGas.value) {
                minGas.timeStamp = historyMeasurement.timeStamp
                minGas.buildingID = historyMeasurement.buildingID
                minGas.value = historyMeasurement.gas
            }
            if (historyMeasurement.gas > maxGas.value) {
                maxGas.timeStamp = historyMeasurement.timeStamp
                maxGas.buildingID = historyMeasurement.buildingID
                maxGas.value = historyMeasurement.gas
            }
            averageGas += historyMeasurement.gas
        }

        if (history.measurements.size > 0) {
            averageBrightness /= history.measurements.size
            averageTemperature /= history.measurements.size
            averageHumidity /= history.measurements.size
            averageGas /= history.measurements.size
        }

        val brightness = ValueTyp(minBrightness, maxBrightness, averageBrightness)
        val temperature = ValueTyp(minTemperature, maxTemperature, averageTemperature)
        val humidity = ValueTyp(minHumidity, maxHumidity, averageHumidity)
        val gas = ValueTyp(minGas, maxGas, averageGas)

        return Measurements(brightness, temperature, humidity, gas)
    }

    private fun calculateEvents(history: Data): Events {
        val events = Events(0, 0, 0, 0, 0)

        for (historyEvent in history.events) {
            when (historyEvent.type) {
                "Gas"    -> events.gas++
                "Fire"   -> events.fire++
                "Motion" -> events.motion++
                "Sound"  -> events.sound++
                "RFID"   -> events.rfid++
                else -> println("Calculation: unknown Event type: ${historyEvent.type}")
            }
        }
        return events
    }
}