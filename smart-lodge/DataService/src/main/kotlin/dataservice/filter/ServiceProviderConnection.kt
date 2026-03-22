/*
  Projekt:      SmartQuartier DataService
  Firma:        ABB Technikerschule
  Autor:        Marco Bontognali

  Beschreibung: ServiceProviderConnection bietet eine REST-API für ServiceProvider an.

  Tests:        val data = nextFilterHelper?.requestHistory("2024-04-08 14:07:17","2024-04-12 14:33:18", "TestBuilding")
                val data = nextFilterHelper?.requestHistory("2024-04-08 14:07:17","2024-04-12 14:33:18", "x")
                val data = nextFilterHelper?.requestHistory("2024-04-08 14:07:17","2024-04-12 14:33:18", "")

  Todo:         get("smart-quartier/data-service/forecast") implementieren
                eventuell nextFilterHelper anders implementieren
*/

package dataservice.filter

import dataservice.config
import io.ktor.serialization.gson.*
import io.ktor.server.application.*
import io.ktor.server.engine.*
import io.ktor.server.netty.*
import io.ktor.server.plugins.contentnegotiation.*
import io.ktor.server.response.*
import io.ktor.server.routing.*

import dataservice.filter.calculation.Requestable
import dataservice.logger

private var nextFilterHelper: Requestable? = null

class ServiceProviderConnection(
    private val nextFilter: Requestable) {

    init {
        nextFilterHelper = nextFilter
    }

    fun start() {
        embeddedServer(Netty, port = config.REQUEST_PORT, host = "0.0.0.0", module = Application::module2)
            .start(wait = false)
        println("ServiceProviderConnection: started")
        logger.log("SmartQuartier DataService started")
    }
}

fun Application.module2() {
    install(ContentNegotiation) { gson() }

    routing {
        // http://127.0.0.1:11001/smart-quartier/data-service/history
        // http://127.0.0.1:11001/smart-quartier/data-service/history?fromTimeStamp=2024-04-17T18:37:00&toTimeStamp=2024-04-17T18:37:29:18&buildingID=Haus-1
        get("smart-quartier/data-service/history") {
            var fromTimeStamp = call.parameters["from-time-stamp"] ?: "1000-01-01 00:00:00"
            var toTimeStamp = call.parameters["to-time-stamp"] ?: "9999-12-31 23:59:59"
            val buildingID = call.parameters["building-id"] ?: ""

            fromTimeStamp = removeT(fromTimeStamp)
            toTimeStamp = removeT(toTimeStamp)
            println("\t=============================================================================================")
            println("\tServiceProviderConnection: History request $fromTimeStamp to $toTimeStamp $buildingID")
            logger.log("History request\t$fromTimeStamp\tto\t$toTimeStamp\t$buildingID")

            val data = nextFilterHelper?.requestHistory(fromTimeStamp, toTimeStamp, buildingID)
            if (data != null) {
                call.respond(data)
                println("\tServiceProviderConnection: respond History")
            }
        }

        // http://127.0.0.1:11001/smart-quartier/data-service/statistic
        // http://127.0.0.1:11001/smart-quartier/data-service/statistic?fromTimeStamp=2024-04-17T18:37:00&toTimeStamp=2024-04-17T18:37:29:18&buildingID=Haus-1
        get("smart-quartier/data-service/statistic") {
            var fromTimeStamp = call.parameters["from-time-stamp"] ?: "1000-01-01 00:00:00"
            var toTimeStamp = call.parameters["to-time-stamp"] ?: "9999-12-31 23:59:59"
            val buildingID = call.parameters["building-id"] ?: ""

            fromTimeStamp = removeT(fromTimeStamp)
            toTimeStamp = removeT(toTimeStamp)

            println("\t=============================================================================================")
            println("\tServiceProviderConnection: Statistic request $fromTimeStamp to $toTimeStamp $buildingID")
            logger.log("Statistic request\t$fromTimeStamp\tto\t$toTimeStamp\t$buildingID")

            val data = nextFilterHelper?.requestStatistic(fromTimeStamp, toTimeStamp, buildingID)
            if (data != null) {
                call.respond(data)
                println("\tServiceProviderConnection: respond Statistic")
            }
        }

        get("smart-quartier/data-service/forecast") {
//            var fromTimeStamp = call.parameters["from-time-stamp"] ?: "1000-01-01 00:00:00"
//            var toTimeStamp = call.parameters["to-time-stamp"] ?: "9999-12-31 23:59:59"
//            val buildingID = call.parameters["building-id"] ?: ""
//
//            fromTimeStamp = removeT(fromTimeStamp)
//            toTimeStamp = removeT(toTimeStamp)
//
//            println("\t=============================================================================================")
//            println("\tServiceProviderConnection: Forecast request $fromTimeStamp to $toTimeStamp $buildingID")
//            logger.log("Forecast request\t$fromTimeStamp\tto\t$toTimeStamp\t$buildingID")
//
//            val data = nextFilterHelper?.requestStatistic(fromTimeStamp, toTimeStamp, buildingID)
//            if (data != null) {
                call.respond("Not implemented yet.")
//                println("\tServiceProviderConnection: respond Forecast")
//            }
        }
    }
}

fun removeT(timeStamp:String): String {
    return timeStamp.replace("T", " ")  // YYYY-MM-DDThh:mm:ss -> YYYY-MM-DD hh:mm:ss   (T entfernen)
}