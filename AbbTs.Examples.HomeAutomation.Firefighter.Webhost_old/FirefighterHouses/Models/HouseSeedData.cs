using System.Collections.Generic;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Models;

// Single source of truth for the demo houses; replaces the array that used to be hardcoded in the frontend store.
public static class HouseSeedData
{
  public static IReadOnlyList<HouseSnapshot> Houses { get; } =
    [
      new HouseSnapshot(
        "house-1",
        "Max",
        new HouseCoordinates(0, 0),
        "22°C",
        "80%",
        "48%",
        "5%",
        new HouseState("None", "Normal"),
        new HouseSensors(true, true, false, false, 80, 22, "Normal")
      ),
      new HouseSnapshot(
        "house-2",
        "Moritz",
        new HouseCoordinates(1, 0),
        "21°C",
        "75%",
        "50%",
        "4%",
        new HouseState("None", "Normal"),
        new HouseSensors(false, true, true, false, 75, 21, "Normal")
      ),
      new HouseSnapshot(
        "house-3",
        "Lehrer Lämpel",
        new HouseCoordinates(0, 1),
        "23°C",
        "90%",
        "45%",
        "6%",
        new HouseState("None", "Normal"),
        new HouseSensors(true, false, true, false, 90, 23, "Normal")
      ),
      new HouseSnapshot(
        "house-4",
        "Onkel Fritz",
        new HouseCoordinates(1, 1),
        "22°C",
        "85%",
        "47%",
        "5%",
        new HouseState("None", "Normal"),
        new HouseSensors(false, false, false, true, 85, 22, "Normal")
      ),
      new HouseSnapshot(
        "house-5",
        "Meister Bäcker",
        new HouseCoordinates(2, 0),
        "20°C",
        "70%",
        "52%",
        "3%",
        new HouseState("None", "Normal"),
        new HouseSensors(true, true, false, false, 70, 20, "Normal")
      ),
    ];
}
