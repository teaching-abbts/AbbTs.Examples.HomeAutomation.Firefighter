/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: SmartHome mit Gateway erweitert.
*/

package smarthome.communication.gateway.deviceandcontrol.remotecontrol

import kotlinx.coroutines.delay
import kotlinx.coroutines.sync.withLock
import smarthome.communication.gateway.gateway
import smarthome.control.*
import smarthome.device.actor.Servo
import smarthome.device.userinterface.display

class RemoteControl {
    private var lightControl: LightControl? = null
    private var heatingControl: HeatingControl? = null
    private var alarmControl: AlarmControl? = null
    private var door: Servo? = null

    fun add(element: Any) {
        when (element) {
            is LightControl   -> lightControl = element
            is HeatingControl -> heatingControl = element
            is AlarmControl   -> alarmControl = element
            is Servo          -> door = element
            else              -> println("Gateway add: Unbekanntes Element: $element")
        }
    }

    fun getState() = State(gateway.getState(),
                           toState(lightControl), lightControl!!.getSetPoint(),
                           toState(heatingControl), heatingControl!!.getSetPoint(),
                           toState(alarmControl))

    suspend fun execute(command: Command) = when (command.device) {
        "LightControl"   -> setLightControl(command)
        "HeatingControl" -> setHeatingControl(command)
        "AlarmControl"   -> setAlarmControl(command)
        "Door"           -> setDoor(command)
        "Display"        -> setDisplay(command)
        else             -> {
            println("Gateway execute: unknown device: $command")
            "Gateway execute: unknown device: $command"
        }
    }

    private fun setLightControl(command: Command) = when (command.command) {
        "on" -> {
            lightControl?.activateCheckBox()
            "command executed"
        }
        "off" -> {
            lightControl?.deactivateCheckBox()
            "command executed"
        }
        "setpoint" -> {
            var result = "command executed"
            try {
                val newValue = command.value.toInt()
                if ((newValue >= 0) and (newValue <= 1023)) {
                    lightControl?.setSetPoint(newValue)
                }
                else {
                    result = "Gateway execute: value not in range: $command"
                }
            }
            catch (e: Exception) {
                result = "Gateway execute: wrong value: $command"
            }
            result
        }
        else -> "Gateway execute: unknown command: $command"
    }

    private fun setHeatingControl(command: Command) = when (command.command) {
        "on" -> {
            heatingControl?.activateCheckBox()
            "command executed"
        }
        "off" -> {
            heatingControl?.deactivateCheckBox()
            "command executed"
        }
        "setpoint" -> {
            var result = "command executed"
            try {
                val newValue = command.value.toInt()
                if ((newValue >= 0) and (newValue <= 30)) {
                    heatingControl?.setSetPoint(newValue)
                }
                else {
                    result = "Gateway execute: value not in range: $command"
                }
            }
            catch (e: Exception) {
                result = "Gateway execute: wrong value: $command"
            }
            result
        }
        else -> "Gateway execute: unknown command: $command"
    }

    private fun setAlarmControl(command: Command) = when (command.command) {
        "on" -> {
            alarmControl?.activateCheckBox()
            "command executed"
        }
        "off" -> {
            alarmControl?.deactivateCheckBox()
            "command executed"
        }
        else -> "Gateway execute: unknown command: $command"
    }

    private fun setDoor(command: Command) = when (command.command) {
        "open" -> {
            door?.open()
            "command executed"
        }
        "close" -> {
            door?.close()
            "command executed"
        }
        else -> "Gateway execute: unknown command: $command"
    }

    private suspend fun setDisplay(command: Command) = if (command.value.length <= 33) {
            command.command +=  ":"                // ServiceProvider-Name
            val lines = command.value.split(";")

            display.clear()
            display.print(command.command, 0)
            delay(1000)
            display.clear()
            for (lineNr in 0..< lines.size) {
                display.print(lines[lineNr], lineNr)
            }
            delay(2000)
            display.clear()
            "command executed"
        }
        else {
            "Gateway execute: more then 16 Characters: $command"
        }

    private fun toState(control: Control?) = if (control!!.isActive()) {
            "on"
        }
        else {
            "off"
        }
}