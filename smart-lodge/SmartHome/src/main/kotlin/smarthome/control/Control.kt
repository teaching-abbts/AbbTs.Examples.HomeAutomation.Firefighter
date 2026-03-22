/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Objektorientierte Gebaeudesteuerung mit realer Hardware.
*/

package smarthome.control

import javafx.scene.Node
import javafx.scene.control.CheckBox
import javafx.scene.control.TitledPane
import javafx.scene.layout.VBox

abstract class Control(
    private val name: String) {
    private val view = TitledPane()
    protected val box = VBox()
    protected var checkBox = CheckBox()

    init {
        view.text = name
        view.content = box
        box.spacing = 10.0
        add(checkBox)
    }

    abstract suspend fun work()

    fun getView() = view

    protected fun add(node: Node) {
        box.children.add(node)
    }

    fun activateCheckBox() {
        checkBox.selectedProperty().value = true
    }

    fun deactivateCheckBox() {
        checkBox.selectedProperty().value = false
    }

    fun isActive() = checkBox.selectedProperty().value
}