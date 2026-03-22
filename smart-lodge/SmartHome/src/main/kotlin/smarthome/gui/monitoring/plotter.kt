/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.gui.monitoring

import javafx.application.Platform
import javafx.application.Platform.runLater
import javafx.geometry.Side
import javafx.scene.chart.LineChart
import javafx.scene.chart.NumberAxis
import javafx.scene.chart.XYChart
import javafx.scene.chart.XYChart.Series

object plotter {
    private val xAxis = NumberAxis()
    private val yAxis = NumberAxis()
    private var chart = LineChart(xAxis, yAxis)
    private val series: MutableList<Series<Number, Number>> = mutableListOf()
    private var counter = 0.0
    private val MAX_SHOWN_VALUES = 30
    private val CORRECTION_FACTOR = 10

    fun getView(): LineChart<Number, Number> {
        xAxis.label = "Zeit [s]"
        xAxis.isForceZeroInRange = false
        yAxis.label = "Wert"

        with(chart) {
            maxHeight = 200.0
            legendSide = Side.LEFT
            createSymbols = false
            animated = false
        }
        createAllSeries()
        return chart
    }

    fun plot(name: String, value: Int) {
        when (name) {
            "Helligkeit"   -> updateSerie(series[0], value / CORRECTION_FACTOR)
            "Temperatur"   -> updateSerie(series[1], value)
            "Feuchtigkeit" -> updateSerie(series[2], value)
            "Gas"          -> updateSerie(series[3], value / CORRECTION_FACTOR)
            "Distanz"      -> updateSerie(series[4], value / CORRECTION_FACTOR)
            "Feuer"        -> doNothing()
            else           -> println("WARNUNG Plotter: unbekannter Sensor: $name")
        }
    }

    private fun createAllSeries() {
        createSerie("Helligkeit / $CORRECTION_FACTOR")
        createSerie("Temperatur")
        createSerie("Feuchtigkeit")
        createSerie("Gas / $CORRECTION_FACTOR")
        createSerie("Distanz / $CORRECTION_FACTOR")
    }

    private fun createSerie(name: String) {
        val serie = Series<Number, Number>()
        serie.name = name
        chart.data.add(serie)
        series += serie
    }

    private fun updateSerie(serie: Series<Number, Number>, value: Int) {
        runLater {
            val data = XYChart.Data<Number, Number>(counter.toInt(), value)
            serie.data.add(data)
            if (serie.data.size > MAX_SHOWN_VALUES) {
                serie.data.removeAt(0)
            }
            counter += 1.0 / series.size // Passt für 1 Sekunden Zyklus des Supervisors
        }
    }

    private fun doNothing() {
    }
}