/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Objektorientierte Gebaeudesteuerung mit realer Hardware.
*/

package smarthome.control

import javafx.geometry.Insets
import javafx.scene.control.Label
import javafx.scene.layout.VBox
import kotlinx.coroutines.*
import kotlinx.coroutines.sync.Mutex
import kotlinx.coroutines.sync.withLock
import smarthome.gui.monitoring.logger

object supervisor {
    private val controls = mutableListOf<Control>()
    private var run = true
    private var CYCLE_TIME_SECONDS = 1

    val mutex = Mutex()

    init {
        with(controls) {
            add(LightControl())
            add(HeatingControl())
            add(AlarmControl())
            add(AccessControl())
            add(GatewayControl())
        }
    }

    fun getView(): VBox {
        val box = VBox().apply {
            spacing = 10.0
            padding = Insets(10.0)
            children.add(Label("Steuerung"))
        }
        for (control in controls) {
            box.children.add(control.getView())
        }
        return box
    }

    fun work() {
        GlobalScope.launch {
            logger.log("Supervisor gestartet")
            while (run) {
                mutex.withLock {
                    for (control in controls) {
                        control.work()
                    }
                }
                delay(CYCLE_TIME_SECONDS.toLong() * 1000)
            }
        }
    }

    fun stop() {
        run = false
        logger.log("Supervisor gestoppt")
    }
}