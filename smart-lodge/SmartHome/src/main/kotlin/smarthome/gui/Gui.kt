/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.gui

import ch.abbts.buildingclient.building
import javafx.application.Application
import javafx.scene.Scene
import javafx.scene.control.*
import javafx.scene.control.Alert.AlertType.INFORMATION
import javafx.scene.layout.BorderPane
import javafx.stage.Stage
import smarthome.communication.gateway.gateway
import smarthome.communication.telecom.TelecomClient
import smarthome.config
import smarthome.config.BUILDING_ID
import smarthome.config.COM_PORT
import smarthome.config.HARDWARE_ON
import smarthome.config.VERSION
import smarthome.control.supervisor
import smarthome.gui.monitoring.logger
import smarthome.gui.monitoring.plotter
import kotlin.system.exitProcess

class Gui : Application() {
    private var fileDlg: FileDialog? = null
    private val telecomClient = TelecomClient()

    override fun start(stage: Stage) {
        logger.log("SmartHome Version $VERSION")
        if (HARDWARE_ON == "true") {
            building.hardwareOn(COM_PORT)
            logger.log("Betrieb mit Hardware")
        }
        else {
            logger.log("Betrieb simuliert")
        }

        if (config.TELECOM_SERVER_IP != "no Server") {
            telecomClient.register()
        }

        if (config.SERVICE_PROVIDERS.isNotEmpty()) {
            gateway.start()
        }

        fileDlg = FileDialog(stage)

        with(stage) {
            scene = Scene(createMainWindow(), 1200.0, 760.0)
            title = "SmartHome $BUILDING_ID"
            setResizable(false)
            setOnCloseRequest { exit() }
            show()
        }
        supervisor.work()
    }

    private fun createMainWindow() = BorderPane().apply {
        top = createMenuBar()
        left = supervisor.getView()
        right = logger.getView()
        center = home.getView()
        bottom = plotter.getView()
    }

    private fun createMenuBar() = bar(
        menu("Datei",
            item("Öffnen...",          { open() }),
            item("Speichern unter...", { saveAs() }),
            separator(),
            item("Beenden",            { exit() })
        ),
        menu(
            "?",
            item("Hilfe anzeigen",     { showHelpDialog() }),
            item("Über",               { showAboutDialog() })
        )
    )

    private fun open() {
        val file = fileDlg?.showOpenDialog("Öffne Log-File")
        if (file != null) {
            logger.setText(file.readText())
            showDialog("Öffnen", "Ereignisse geladen.", INFORMATION)
        }
    }

    private fun saveAs() {
        val file = fileDlg?.showSaveDialog("Speichere Log-File")
        if (file != null) {
            file.writeText(logger.getText())
            showDialog("Speichern", "Ereignisse gespeichert.", INFORMATION)
        }
    }

    private fun showHelpDialog() {
        var text = "Einzelne Steuerungen können aktiviert oder deaktiviert werden.\n"
        text +=    "Log Files können gespeichert und geöffnet werden."
        showDialog("Hilfe", text, INFORMATION)
    }

    private fun showAboutDialog() {
        var text = "Modul-Übergreifendes Projekt der ABB-Technikerschule.\n"
        text +=    "Autor Marco Bontognali"
        showDialog("Über", text, INFORMATION)
    }

    private fun exit() {
        gateway.stop()
        supervisor.stop()
        building.close()
        exitProcess(0)
    }

    private fun showDialog(title: String, message: String, alertType: Alert.AlertType) {
        with(Alert(alertType)) {
            this.title = title
            headerText = null
            contentText = message
            showAndWait()
        }
    }

    // Hilfsmethoden damit die Menues einfacher codiert werden koennen
    private fun bar(vararg elements: Menu) = MenuBar().apply { getMenus().addAll(elements) }
    private fun menu(text: String, vararg elements: MenuItem) = Menu(text).apply { getItems().addAll(elements) }
    private fun item(text: String, method: () -> Unit) = MenuItem(text).apply { setOnAction { method() } }
    private fun separator() = SeparatorMenuItem()
}