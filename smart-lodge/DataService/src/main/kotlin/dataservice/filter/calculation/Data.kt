/*
  Projekt:      SmartQuartier DataService
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali
*/

package dataservice.filter.calculation

import dataservice.pipe.Event
import dataservice.pipe.Measurement

data class Data(
    val measurements: List<Measurement>,
    val events: List<Event>
)
