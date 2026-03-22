/*
  Projekt:      SmartHome
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: Ermittelt Betriebssystem und Speicherverbrauch.
*/

package smarthome.communication.telecom

object metrics {
    private val runtime = Runtime.getRuntime()
    val OPERATING_SYSTEM_NAME = System.getProperty("os.name")
    val OPERATING_SYSTEM_ARCHITECTURE = System.getProperty("os.arch")

    fun getAvailableProcessors() = runtime.availableProcessors()

    fun getMemoryByte() = runtime.totalMemory() - runtime.freeMemory()

    override fun toString() = "$OPERATING_SYSTEM_NAME;$OPERATING_SYSTEM_ARCHITECTURE;${getAvailableProcessors()};${getMemoryByte()}"
}