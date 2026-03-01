import { defineStore } from "pinia";
import { computed, ref } from "vue";

import {
  Client,
  type SmartQuartierEvent,
  type SmartQuartierMeasurement,
} from "@/api/AbbTs.Examples.HomeAutomation.Firefighter.Webhost";

const normalizeBuildingId = (buildingId: string | undefined) => {
  if (!buildingId) {
    return "";
  }

  return buildingId.trim().toLowerCase();
};

const parseHouseNumberFromBuildingId = (buildingId: string | undefined) => {
  if (!buildingId) {
    return null;
  }

  const match = /\d+/.exec(buildingId);
  return match ? Number(match[0]) : null;
};

export const useHouseDetailsStore = defineStore("house-details", () => {
  const isOpen = ref(false);
  const selectedHouseNumber = ref<number | null>(null);
  const loading = ref(false);
  const loaded = ref(false);
  const error = ref<string | null>(null);
  const events = ref<SmartQuartierEvent[]>([]);
  const measurements = ref<SmartQuartierMeasurement[]>([]);

  const selectedHouseEvents = computed(() => {
    if (!selectedHouseNumber.value) {
      return [] as SmartQuartierEvent[];
    }

    const houseNumberAsText = String(selectedHouseNumber.value);

    return events.value.filter((event) => {
      const parsedNumber = parseHouseNumberFromBuildingId(event.buildingID);
      if (parsedNumber !== null) {
        return parsedNumber === selectedHouseNumber.value;
      }

      return normalizeBuildingId(event.buildingID) === houseNumberAsText;
    });
  });

  const availableHouseNumbers = computed(() => {
    const houseNumbers = new Set<number>();

    for (const event of events.value) {
      const houseNumber = parseHouseNumberFromBuildingId(event.buildingID);
      if (houseNumber !== null) {
        houseNumbers.add(houseNumber);
      }
    }

    for (const measurement of measurements.value) {
      const houseNumber = parseHouseNumberFromBuildingId(
        measurement.buildingID,
      );
      if (houseNumber !== null) {
        houseNumbers.add(houseNumber);
      }
    }

    return [...houseNumbers].sort((left, right) => left - right);
  });

  const latestEventTypeByHouse = computed(() => {
    const latestByHouse = new Map<
      number,
      { type?: string; timestamp: number }
    >();

    for (const event of events.value) {
      const houseNumber = parseHouseNumberFromBuildingId(event.buildingID);
      if (houseNumber === null) {
        continue;
      }

      const timestamp = event.timeStamp ? event.timeStamp.getTime() : 0;
      const current = latestByHouse.get(houseNumber);
      if (!current || timestamp > current.timestamp) {
        latestByHouse.set(houseNumber, { type: event.type, timestamp });
      }
    }

    const result: Record<number, string | undefined> = {};
    for (const [houseNumber, value] of latestByHouse.entries()) {
      result[houseNumber] = value.type?.trim().toLowerCase();
    }

    return result;
  });

  const open = (houseNumber: number) => {
    selectedHouseNumber.value = houseNumber;
    isOpen.value = true;
    void fetchHistoryIfNeeded();
  };

  const close = () => {
    isOpen.value = false;
  };

  const fetchHistoryIfNeeded = async (force = false) => {
    if (!force && (loading.value || loaded.value)) {
      return;
    }

    loading.value = true;
    error.value = null;

    try {
      const client = new Client();
      const response = await client.getSmartQuartierHistory();
      events.value = response?.events ?? [];
      measurements.value = response?.measurements ?? [];
      loaded.value = true;
    } catch (fetchError) {
      error.value =
        fetchError instanceof Error ? fetchError.message : "Unknown error";
    } finally {
      loading.value = false;
    }
  };

  return {
    isOpen,
    selectedHouseNumber,
    loading,
    loaded,
    error,
    events,
    measurements,
    selectedHouseEvents,
    availableHouseNumbers,
    latestEventTypeByHouse,
    open,
    close,
    fetchHistoryIfNeeded,
  };
});
