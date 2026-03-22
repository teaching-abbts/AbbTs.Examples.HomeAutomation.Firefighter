/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.device

import javafx.application.Platform
import javafx.application.Platform.runLater
import javafx.scene.Group
import javafx.scene.paint.Color
import javafx.scene.paint.Color.GRAY
import javafx.scene.paint.Color.LIGHTGRAY
import javafx.scene.shape.Rectangle
import javafx.scene.text.Font
import javafx.scene.text.Text
import smarthome.config.DEVICE_SIZE
import smarthome.config.DEVICE_ZOOM

class View(
    name: String,
    position: Position) {
    val group: Group
    private val rectangle: Rectangle
    private val nameTxt: Text
    private val textTxt: Text

    init {
        rectangle = Rectangle(position.x, position.y, DEVICE_SIZE, DEVICE_SIZE / 2)
        with(rectangle) {
            fill = LIGHTGRAY
            arcWidth = 20.0
            arcHeight = 20.0
            stroke = GRAY
        }

        val font = Font(4.0 * DEVICE_ZOOM)
        nameTxt = Text((position.x + 3 * DEVICE_ZOOM), (position.y + 5 * DEVICE_ZOOM), name)
        nameTxt.font = font

        textTxt = Text((position.x + 3 * DEVICE_ZOOM), (position.y + 10 * DEVICE_ZOOM), "-")
        textTxt.font = font

        group = Group()
        group.children.addAll(rectangle, nameTxt, textTxt)
    }

    fun setColor(color: Color) {
        runLater() {
            rectangle.fill = color
        }
    }

    fun setText(text: String) {
        runLater() {
            textTxt.text = text
        }
    }
}