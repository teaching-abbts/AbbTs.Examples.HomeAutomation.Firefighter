/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.gui

import javafx.scene.Group
import javafx.scene.paint.Color.BLACK
import javafx.scene.paint.Color.WHITE
import javafx.scene.shape.Line
import javafx.scene.shape.Rectangle
import javafx.scene.text.Font
import javafx.scene.text.Text
import smarthome.config
import smarthome.device.Device

object home {
    private val devices = mutableListOf<Device>()

    fun add(device: Device) {
        devices.add(device)
    }

    fun getAll() = devices

    fun getView(): Group {
        val group = Group()
        val width = 240.0 * config.DEVICE_ZOOM
        val height = 160.0 * config.DEVICE_ZOOM

        val exteriorWalls = Rectangle(0.0, 0.0, width, height)
        exteriorWalls.fill = WHITE
        exteriorWalls.stroke = BLACK

        val wallVertical = Line(130 * config.DEVICE_ZOOM, 0.0,
                                130 * config.DEVICE_ZOOM, 160 * config.DEVICE_ZOOM)

        val wallHorizontal = Line(0.0, 70 * config.DEVICE_ZOOM,
                                  130 * config.DEVICE_ZOOM, 70 * config.DEVICE_ZOOM)

        val sleepTxt = createText(50, 35, "Schlafen")
        val workTxt = createText(50, 115, "Arbeiten")
        val liveTxt = createText(170, 80, "Wohnen")
        val entranceTxt = createText(170, 155, "Eingang")

        group.children.addAll(exteriorWalls, wallVertical, wallHorizontal,
                              sleepTxt, workTxt, liveTxt, entranceTxt)

        for (device in devices) {
            group.children.add(device.getView())
        }
        return group
    }

    private fun createText(x: Int, y: Int, text: String): Text {
        val txt = Text(x * config.DEVICE_ZOOM, y * config.DEVICE_ZOOM, text)
        txt.setFont(Font(6 * config.DEVICE_ZOOM))
        return txt
    }
}