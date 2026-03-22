/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Graphical User Interface um das SmartHome zu visualisieren.
*/

package smarthome.device.sensor

import ch.abbts.buildingclient.building
import javafx.scene.paint.Color.LIGHTBLUE
import smarthome.device.Position

class Sound(
    position: Position) : Analog("Geräusche", position, "") {
    private var stillness = 0
    private var noise = false

    init {
        stillness = building.getSound() - 1
    }

    fun getStillness() = stillness

    fun update() {
        value = building.getSound()
        view.setColor(LIGHTBLUE);

        noise = value < stillness
        if (noise) {
            view.setText("detektiert")
        }
        else {
            view.setText("keine")
        }
    }

    fun isActive() = noise

    override fun toString() = "$name: $value"
}