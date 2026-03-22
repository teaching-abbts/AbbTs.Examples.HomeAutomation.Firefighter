/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.gui

import javafx.stage.FileChooser
import javafx.stage.FileChooser.ExtensionFilter
import javafx.stage.Stage
import java.io.File

class FileDialog(
    private val stage: Stage) {
    private val chooser = FileChooser()

    init {
        chooser.setInitialDirectory(
            File(".")
        )
        chooser.extensionFilters.add(
            ExtensionFilter("Ereignisdateien", "*.log")
        )
    }

    fun showOpenDialog(title: String) = chooser.showOpenDialog(stage)

    fun showSaveDialog(title: String) = chooser.showSaveDialog(stage)
}