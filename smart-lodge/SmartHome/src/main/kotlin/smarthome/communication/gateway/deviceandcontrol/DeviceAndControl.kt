/*
  Projekt:        SmartHome
  Firma:          ABB Technikerschule
  Autor:          Marco Bontognali

  Beschreibung:   SmartHome mit Gateway erweitert.

  Design-Pattern: Facade
*/

package smarthome.communication.gateway.deviceandcontrol

import smarthome.communication.gateway.deviceandcontrol.acquisition.Acquisition
import smarthome.communication.gateway.deviceandcontrol.remotecontrol.RemoteControl
import smarthome.device.sensor.Analog

class DeviceAndControl() {
    val acquisition = Acquisition()
    val remoteControl = RemoteControl()

    fun add(element: Any) {
        if (element is Analog) {
            acquisition.add(element)
        }
        else {
            remoteControl.add(element)
        }
    }
}
