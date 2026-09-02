import type { HouseCardProps } from "@/components/house/house-card.vue";
import {
  HubConnectionBuilder,
  LogLevel,
  type HubConnection,
} from "@microsoft/signalr";
import { defineStore } from "pinia";
import { readonly, ref } from "vue";

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
  const houses = ref<House[]>([]);
  const readonlyHouses = readonly(houses);
  const hubConnection = ref<HubConnection | null>(null);

  const applyHousesChanged = (payload: House[]) => {
    houses.value = payload;
  };

  const applyHouseUpdated = (payload: House) => {
    const index = houses.value.findIndex(
      (house) => house.buildingId === payload.buildingId,
    );

    if (index === -1) {
      houses.value.push(payload);
      return;
    }

    houses.value.splice(index, 1, payload);
  };

  function connect() {
    const connection = new HubConnectionBuilder()
      .withUrl("/hubs/houses")
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();

    connection.on("housesChanged", applyHousesChanged);
    connection.on("houseUpdated", applyHouseUpdated);
    connection.onreconnected(() => connection.invoke("SubscribeAll"));

    hubConnection.value = connection;

    connection
      .start()
      .then(() => connection.invoke("SubscribeAll"))
      .catch((error: unknown) =>
        console.error("Failed to connect to house hub", error),
      );
  }

  // The store is a singleton whose setup body runs once, so connecting here
  // (rather than via a method every page must remember to call) is enough.
  connect();

  // Every setting change is a command sent to the server; the UI only updates once the
  // authoritative state comes back via the "houseUpdated"/"housesChanged" hub events.
  function toggleLight(buildingId: string, isLightOn: boolean) {
    return hubConnection.value?.invoke("ToggleLight", buildingId, isLightOn);
  }

  function toggleDoorLock(buildingId: string, isDoorLocked: boolean) {
    return hubConnection.value?.invoke(
      "ToggleDoorLock",
      buildingId,
      isDoorLocked,
    );
  }

  function toggleHeating(buildingId: string, isHeatingOn: boolean) {
    return hubConnection.value?.invoke(
      "ToggleHeating",
      buildingId,
      isHeatingOn,
    );
  }

  function toggleAlarm(buildingId: string, isAlarmOn: boolean) {
    return hubConnection.value?.invoke("ToggleAlarm", buildingId, isAlarmOn);
  }

  function setLightIntensity(buildingId: string, lightIntensity: number) {
    return hubConnection.value?.invoke(
      "SetLightIntensity",
      buildingId,
      lightIntensity,
    );
  }

  function setHeatingIntensity(buildingId: string, heatingIntensity: number) {
    return hubConnection.value?.invoke(
      "SetHeatingIntensity",
      buildingId,
      heatingIntensity,
    );
  }

  function setDisplayValue(buildingId: string, displayValue: string) {
    return hubConnection.value?.invoke(
      "SetDisplayValue",
      buildingId,
      displayValue,
    );
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
