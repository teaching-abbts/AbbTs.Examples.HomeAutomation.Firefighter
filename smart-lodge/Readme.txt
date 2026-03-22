Version 0.5
===========
- "get smarthome state" liefert alle relevanten Daten (Gateway, Controls, Setpoints) mit einer Abfrage
- SmartHome mit Metrics für den TelecomServer ergänzt
- SmartHome Gas- und Feuer-Alarmschwelle konfigurierbar


Version 0.4.4
=============
 - WebSockets zwischen SmartHome und ServiceProvider
 - REST-Schnittstelle zwischen WebPlattform/FireFighter und DataService 
   (gleichbleibend wie bei Version 0.3)


Allgemein
=========
 - Compiliert mit JDK 11
 - Für das Ausführen ist Java mit integriertem FX erforderlich.
   Zum Beispiel: Azul Zulu OpenJDK (JDK FX) Version 21
 - Detaillierte Informationen siehe Ordner Dokumentation


SmartHome
---------
 - Starten mit SmartHomeStarterDebug.bat oder SmartHomeStarter.bat
 - Defaultmässig wird das SmartHome simuliert
 - Um Events auszulösen, muss die Alarm-Checkbox selektiert sein
 - DataService wird defaultmässig lokal gesucht
 - Konfiguration mit SmartHome.conf


DataService
-----------
 - Starten mit SmartQuartierDataServiceStarter.bat
 - Bevor sich ein SmartHome mit dem DataService verbinden kann, muss der DataService laufen
 - Wenn SmartHomes nicht mehr erreichbar sind, entfernt der DataService diese aus seiner Liste
 - History-Abfragen:      http://localhost:11001/smart-quartier/data-service/history
 - Statistik-Abfragen:    http://localhost:11001/smart-quartier/data-service/statistic
 - Bei HTTP 500er Fehler: vor der ersten Abfrage mindestens eine Messung und ein Event auslösen damit der DataService die 2 csv-Files anlegt
 - Konfiguration mit DataService.conf
 - Defaultmässig wird in csv-Files gespeichert
 - Sobald Messungen und Ereignisse vorkommen, werden die Files SmartQuartierEvents.csv und SmartQuartierEvents.csv angelegt
 - Das File DataService.log wird automatisch angelegt
 - Falls Daten in einer Datenbank gespeichert werden, muss folgende MySQL-DB eingerichtet werden:

    CREATE DATABASE SmartHome;

    CREATE TABLE Measurements (
                  id int(11) NOT NULL AUTO_INCREMENT,
                  time_stamp datetime NOT NULL,
                  building_id varchar(64) DEFAULT NULL,
                  brightness int(11) DEFAULT NULL,
                  temperature int(11) DEFAULT NULL,
                  humidity int(11) DEFAULT NULL,
                  gas int(11) DEFAULT NULL,
                  PRIMARY KEY (id)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

    CREATE TABLE Events (
                  id int NOT NULL AUTO_INCREMENT,
                  time_stamp datetime NOT NULL,
                  building_id varchar(64) NOT NULL,
                  type varchar(16) NOT NULL,
                  data varchar(128) DEFAULT NULL,
                  PRIMARY KEY (id)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


    Zusätzlich folgender Datenbank-User einrichten mit den Rechten Insert, Update und Select:
     > DATABASE_USER = application
     > DATABASE_PASSWORD = geheim