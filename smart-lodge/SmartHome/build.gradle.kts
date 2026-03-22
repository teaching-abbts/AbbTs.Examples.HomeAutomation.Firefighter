val ktor_version: String by project
val kotlin_version: String by project
val logback_version: String by project

plugins {
    kotlin("jvm") version "2.0.21"
    id("io.ktor.plugin") version "2.3.12"
    id("org.openjfx.javafxplugin") version "0.0.9"
}

ktor {
    fatJar {
        archiveFileName.set("SmartHome.jar")
    }
}

javafx {
    modules("javafx.controls", "javafx.fxml")
}

group = "smarthome"
version = "0.0.1"

application {
    mainClass.set("smarthome.SmartHomeKt")

    val isDevelopment: Boolean = project.ext.has("development")
    applicationDefaultJvmArgs = listOf("-Dio.ktor.development=$isDevelopment")
}

repositories {
    mavenCentral()
}

dependencies {
    implementation("io.ktor:ktor-serialization-gson:$ktor_version")
    implementation("ch.qos.logback:logback-classic:$logback_version")

    implementation("io.ktor:ktor-client-core:$ktor_version")
    implementation("io.ktor:ktor-client-websockets:2.3.12")
    implementation("io.ktor:ktor-client-cio:2.3.12")

//    implementation("io.ktor:ktor-client-content-negotiation:$ktor_version")

    implementation("org.jetbrains.kotlinx:kotlinx-coroutines-javafx")

    implementation(files("$projectDir/lib/BuildingClient.jar"))
    implementation(files("$projectDir/lib/jSerialComm-2.9.2.jar"))
}

