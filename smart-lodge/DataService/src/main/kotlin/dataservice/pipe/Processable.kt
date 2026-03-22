/*
  Projekt:      SmartQuartier DataService
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali
*/

package dataservice.pipe

interface Processable {
    suspend fun process(measurement: Measurement)
    suspend fun process(event: Event)
}