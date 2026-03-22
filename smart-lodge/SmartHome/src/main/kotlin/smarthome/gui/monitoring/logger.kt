/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.gui.monitoring

import javafx.application.Platform
import javafx.application.Platform.runLater
import javafx.geometry.Insets
import javafx.scene.control.Label
import javafx.scene.control.TextArea
import javafx.scene.layout.VBox
import java.time.LocalTime
import java.time.format.DateTimeFormatter

object logger {
    private var textArea = TextArea()

    fun getView(): VBox {
        textArea.prefColumnCount = 25
        textArea.prefRowCount = 100

        log("Logger bereit")

        val box = VBox().apply {
            spacing = 10.0
            padding = Insets(10.0)
            children.add(Label("Ereignisse"))
            children.add(textArea)
        }
        return box
    }

    fun log(message: String) {
        val formatter = DateTimeFormatter.ofPattern("HH:mm:ss")
        val time = formatter.format(LocalTime.now())
        runLater {
            textArea.appendText("$time $message\n")
        }
    }

    fun getText() = textArea.text

    fun setText(text: String) {
        runLater {
            textArea.text = text
        }
    }
}