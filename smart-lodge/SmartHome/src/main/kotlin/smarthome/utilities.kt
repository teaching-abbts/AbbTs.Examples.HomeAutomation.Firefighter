package smarthome

import java.time.LocalDateTime
import java.time.format.DateTimeFormatter

object utilities {
    fun now() = toDateTimeString(LocalDateTime.now())

    fun toDateTimeString(timeStamp: LocalDateTime): String {
        val formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss")
        return formatter.format(timeStamp)
    }
}