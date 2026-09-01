import type { HouseCardProps } from "@/components/house/house-card.vue";
import { defineStore } from "pinia";
import { computed, readonly, ref } from "vue";

interface HouseSensors {
  isLightOn: boolean;
  isDoorLocked: boolean;
  isHeatingOn: boolean;
  isAlarmOn: boolean;
  lightIntensity: number;
  heatingIntensity: number;
  displayValue: string;
}

interface House extends HouseCardProps {
  sensors: HouseSensors;
}

export const useHouseStore = defineStore("house", () => {
  const houses = ref<House[]>([
    {
      buildingId: "house-1",
      owner: "Max",
      coordinates: {
        x: 0,
        y: 0,
      },
      temperature: "22°C",
      brightness: "80%",
      humidity: "48%",
      gasLevel: "5%",
      state: {
        dangerKind: "None",
        endangerment: "Normal",
      },
      sensors: {
        isLightOn: true,
        isDoorLocked: true,
        isHeatingOn: false,
        isAlarmOn: false,
        lightIntensity: 80,
        heatingIntensity: 22,
        displayValue: "Normal",
      },
    },
    {
      buildingId: "house-2",
      owner: "Moritz",
      coordinates: {
        x: 1,
        y: 0,
      },
      temperature: "21°C",
      brightness: "75%",
      humidity: "50%",
      gasLevel: "4%",
      state: {
        dangerKind: "None",
        endangerment: "Normal",
      },
      sensors: {
        isLightOn: false,
        isDoorLocked: true,
        isHeatingOn: true,
        isAlarmOn: false,
        lightIntensity: 75,
        heatingIntensity: 21,
        displayValue: "Normal",
      },
    },
    {
      buildingId: "house-3",
      owner: "Lehrer Lämpel",
      coordinates: {
        x: 0,
        y: 1,
      },
      temperature: "23°C",
      brightness: "90%",
      humidity: "45%",
      gasLevel: "6%",
      state: {
        dangerKind: "None",
        endangerment: "Normal",
      },
      sensors: {
        isLightOn: true,
        isDoorLocked: false,
        isHeatingOn: true,
        isAlarmOn: false,
        lightIntensity: 90,
        heatingIntensity: 23,
        displayValue: "Normal",
      },
    },
    {
      buildingId: "house-4",
      owner: "Onkel Fritz",
      coordinates: {
        x: 1,
        y: 1,
      },
      temperature: "22°C",
      brightness: "85%",
      humidity: "47%",
      gasLevel: "5%",
      state: {
        dangerKind: "None",
        endangerment: "Normal",
      },
      sensors: {
        isLightOn: false,
        isDoorLocked: false,
        isHeatingOn: false,
        isAlarmOn: true,
        lightIntensity: 85,
        heatingIntensity: 22,
        displayValue: "Normal",
      },
    },
    {
      buildingId: "house-5",
      owner: "Meister Bäcker",
      coordinates: {
        x: 2,
        y: 0,
      },
      temperature: "20°C",
      brightness: "70%",
      humidity: "52%",
      gasLevel: "3%",
      state: {
        dangerKind: "None",
        endangerment: "Normal",
      },
      sensors: {
        isLightOn: true,
        isDoorLocked: true,
        isHeatingOn: false,
        isAlarmOn: false,
        lightIntensity: 70,
        heatingIntensity: 20,
        displayValue: "Normal",
      },
    },
  ]);

  const readonlyHouses = computed(() => readonly(houses.value));

  function setSetting<T extends keyof HouseSensors>(
    buildingId: string,
    key: T,
    value: HouseSensors[T],
  ) {
    const house = houses.value.find((h) => h.buildingId === buildingId);
    if (house) {
      house.sensors[key] = value;
    }
  }

  function toggleBooleanSetting(
    buildingId: string,
    key: Exclude<
      keyof HouseSensors,
      "lightIntensity" | "heatingIntensity" | "displayValue"
    >,
    value: boolean,
  ) {
    setSetting(buildingId, key, value);
  }

  function setLightIntensity(buildingId: string, lightIntensity: number) {
    const cleanedLightIntensity = Math.max(0, Math.min(1023, lightIntensity));
    setSetting(buildingId, "lightIntensity", cleanedLightIntensity);
  }

  function setHeatingIntensity(buildingId: string, heatingIntensity: number) {
    const cleanedHeatingIntensity = Math.max(0, Math.min(30, heatingIntensity));
    setSetting(buildingId, "heatingIntensity", cleanedHeatingIntensity);
  }

  function setDisplayValue(buildingId: string, displayValue: string) {
    const cleanedDisplayValue = displayValue.trim().slice(0, 33);
    setSetting(buildingId, "displayValue", cleanedDisplayValue);
  }

  function toggleLight(buildingId: string, isLightOn: boolean) {
    toggleBooleanSetting(buildingId, "isLightOn", isLightOn);
  }

  function toggleDoorLock(buildingId: string, isDoorLocked: boolean) {
    toggleBooleanSetting(buildingId, "isDoorLocked", isDoorLocked);
  }

  function toggleHeating(buildingId: string, isHeatingOn: boolean) {
    toggleBooleanSetting(buildingId, "isHeatingOn", isHeatingOn);
  }

  function toggleAlarm(buildingId: string, isAlarmOn: boolean) {
    toggleBooleanSetting(buildingId, "isAlarmOn", isAlarmOn);
  }

  return {
    readonlyHouses,
    setDisplayValue,
    setHeatingIntensity,
    setLightIntensity,
    toggleAlarm,
    toggleDoorLock,
    toggleHeating,
    toggleLight,
  };
});
