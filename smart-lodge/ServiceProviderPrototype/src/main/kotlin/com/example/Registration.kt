package com.example

data class Registration(
    val buildingID: String,
    val xCoordinate: Int,   // Nullpunkt: Smarthome vorne links Blickrichtung Whiteboard
    val yCoordinate: Int,
    val owner: String,
)
