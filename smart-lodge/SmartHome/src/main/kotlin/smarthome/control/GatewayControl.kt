/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.control

import javafx.scene.control.CheckBox
import smarthome.communication.gateway.gateway

class GatewayControl : Control("Gateway") {
    private val checkBox2 = CheckBox()

    init {
        checkBox.text = "Lesen"
        checkBox2.text = "Schreiben"
        add(checkBox2)

        checkBox.setOnAction { setState() }
        checkBox2.setOnAction { setState() }

        activateCheckBox()
        checkBox2.selectedProperty().value = true
        setState()
    }

    override suspend fun work() {
    }

    fun setState() {
        when {
            checkBox.isSelected().not() and checkBox2.isSelected().not() -> {
                gateway.setOffline()
            }
            checkBox.isSelected() and checkBox2.isSelected().not() -> {
                checkBox.disableProperty().value = false
                gateway.setReadOnly()
            }
            else -> {
                gateway.setReadAndWrite()
                activateCheckBox()
                checkBox.disableProperty().value = true
            }
        }
    }
}