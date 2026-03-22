import {
  HubConnectionBuilder,
  LogLevel,
  type HubConnection,
} from "@microsoft/signalr";
import { defineStore, storeToRefs } from "pinia";
import { computed, ref } from "vue";

import {
  Client,
  SmartQuartierEvent as SmartQuartierEventModel,
  type SmartQuartierEvent,
  type SmartQuartierHistoryResponse,
  SmartQuartierMeasurement as SmartQuartierMeasurementModel,
  type SmartQuartierMeasurement,
} from "@/api/AbbTs.Examples.HomeAutomation.Firefighter.Webhost";
import type {
  ActionItem,
  EventItem,
  ObservedHouseItem,
} from "@/components/dashboard/types";
import { useAppStore, type AppEventType } from "@/stores/app";

type AlertType = AppEventType;

const EVENT_ICON_BY_TYPE: Record<AlertType, string> = {
  fire: "mdi-fire",
  gas: "mdi-alert",
  motion: "mdi-motion-sensor",
};

const EVENT_COLOR_BY_TYPE: Record<AlertType, string> = {
  fire: "#ed8936",
  gas: "#facc15",
  motion: "#8fd3ff",
};

const EVENT_TEXT_COLOR_BY_TYPE: Record<AlertType, string> = {
  fire: "#111111",
  gas: "#111111",
  motion: "#0b3b5a",
};

const ACTION_TITLE_BY_TYPE: Record<AlertType, string> = {
  fire: "dashboard.actions.extinguishFire",
  gas: "dashboard.actions.openDoors",
  motion: "dashboard.actions.observe",
};

const ACTION_COLOR_BY_TYPE: Record<AlertType, string> = {
  fire: "#f07f2f",
  gas: "#facc15",
  motion: "#6f42c1",
};

const ACTION_TEXT_COLOR_BY_TYPE: Record<AlertType, string> = {
  fire: "#ffffff",
  gas: "#111111",
  motion: "#ffffff",
};

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

const parseAlertType = (type: string | undefined): AlertType | null => {
  const normalized = type?.trim().toLowerCase();

  if (!normalized) {
    return null;
  }

  if (normalized === "motions" || normalized.includes("motion")) {
    return "motion";
  }

  if (normalized.includes("gas")) {
    return "gas";
  }

  if (normalized.includes("fire")) {
    return "fire";
  }

  if (
    normalized === "fire" ||
    normalized === "gas" ||
    normalized === "motion"
  ) {
    return normalized;
  }

  return null;
};

const formatEventTimestamp = (timeStamp: Date | undefined) => {
  if (!timeStamp) {
    return "...";
  }

  return new Intl.DateTimeFormat("de-DE", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  }).format(timeStamp);
};

const getActionOpenedAt = (actionKey: string) => {
  const separatorIndex = actionKey.lastIndexOf(":");
  if (separatorIndex < 0) {
    return 0;
  }

  const timestamp = Number(actionKey.slice(separatorIndex + 1));
  return Number.isFinite(timestamp) ? timestamp : 0;
};

type EquivalentGroup = {
  houseNumber: number;
  alertType: AlertType;
  titleKey: string;
  events: Array<{ event: SmartQuartierEvent; timestamp: number }>;
};

const getActionKeysForState = (
  states: Record<string, "open" | "done">,
  prefix: string,
  state: "open" | "done",
) => {
  return Object.entries(states)
    .filter(
      ([key, currentState]) => key.startsWith(prefix) && currentState === state,
    )
    .map(([key]) => key)
    .sort((left, right) => getActionOpenedAt(right) - getActionOpenedAt(left));
};

const getLastClosedAt = (
  doneKeys: string[],
  closedAtMap: Record<string, number | null>,
) => {
  return doneKeys.reduce<number | null>((current, key) => {
    const closedAt = closedAtMap[key] ?? getActionOpenedAt(key);
    if (current === null || closedAt > current) {
      return closedAt;
    }

    return current;
  }, null);
};

const resolveActionKey = ({
  equivalentKey,
  latestTimestamp,
  existingOpenKey,
  latestDoneKey,
  lastClosedAt,
}: {
  equivalentKey: string;
  latestTimestamp: number;
  existingOpenKey: string | undefined;
  latestDoneKey: string | undefined;
  lastClosedAt: number | null;
}) => {
  if (existingOpenKey) {
    return existingOpenKey;
  }

  if (lastClosedAt === null || latestTimestamp > lastClosedAt) {
    return `${equivalentKey}:${latestTimestamp}`;
  }

  return latestDoneKey ?? null;
};

const buildEquivalentGroups = (
  sourceEvents: SmartQuartierEvent[],
  observedSet: Set<number>,
  selectedTypeSet: Set<AlertType>,
) => {
  const groupedByEquivalent = new Map<string, EquivalentGroup>();

  for (const event of sourceEvents) {
    const alertType = parseAlertType(event.type);
    const houseNumber = parseHouseNumberFromBuildingId(event.buildingID);

    if (
      !alertType ||
      !selectedTypeSet.has(alertType) ||
      houseNumber === null ||
      !observedSet.has(houseNumber)
    ) {
      continue;
    }

    const titleKey = ACTION_TITLE_BY_TYPE[alertType];
    const equivalentKey = `${houseNumber}:${alertType}:${titleKey}`;
    const timestamp = event.timeStamp ? event.timeStamp.getTime() : 0;

    const group = groupedByEquivalent.get(equivalentKey);
    if (!group) {
      groupedByEquivalent.set(equivalentKey, {
        houseNumber,
        alertType,
        titleKey,
        events: [{ event, timestamp }],
      });
      continue;
    }

    group.events.push({ event, timestamp });
  }

  return groupedByEquivalent;
};

const asDate = (value: Date | string | undefined) => {
  if (!value) {
    return undefined;
  }

  if (value instanceof Date) {
    return value;
  }

  const parsed = new Date(value);
  return Number.isNaN(parsed.getTime()) ? undefined : parsed;
};

const toHistoryResponse = (
  payload: SmartQuartierHistoryResponse | null | undefined,
) => {
  const events = (payload?.events ?? []).map(
    (event) =>
      new SmartQuartierEventModel({
        ...event,
        timeStamp: asDate(event.timeStamp),
      }),
  );

  const measurements = (payload?.measurements ?? []).map(
    (measurement) =>
      new SmartQuartierMeasurementModel({
        ...measurement,
        timeStamp: asDate(measurement.timeStamp),
      }),
  );

  return { events, measurements };
};

export const useHouseDetailsStore = defineStore("house-details", () => {
  const appStore = useAppStore();
  const {
    effectiveEventTypeFilter,
    normalizedLastEventsLimit,
    observedHouses,
    onlyOpenAlarms,
  } = storeToRefs(appStore);

  const isOpen = ref(false);
  const selectedHouseNumber = ref<number | null>(null);
  const loading = ref(false);
  const error = ref<string | null>(null);
  const events = ref<SmartQuartierEvent[]>([]);
  const measurements = ref<SmartQuartierMeasurement[]>([]);
  const actionStates = ref<Record<string, "open" | "done">>({});
  const actionClosedAt = ref<Record<string, number | null>>({});
  const liveConnection = ref<HubConnection | null>(null);

  const selectedTypeSet = computed(
    () => new Set(effectiveEventTypeFilter.value),
  );

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

  const defaultObservedHouseNumbers = computed(() => {
    const numbers = new Set<number>();

    for (const measurement of measurements.value) {
      const houseNumber = parseHouseNumberFromBuildingId(
        measurement.buildingID,
      );
      if (houseNumber !== null) {
        numbers.add(houseNumber);
      }
    }

    if (numbers.size === 0) {
      for (const houseNumber of availableHouseNumbers.value) {
        numbers.add(houseNumber);
      }
    }

    return [...numbers].sort((left, right) => left - right);
  });

  const observedHouseNumbers = computed(() => {
    const defaults = new Set(defaultObservedHouseNumbers.value);

    return availableHouseNumbers.value.filter((houseNumber) => {
      const explicit = observedHouses.value[String(houseNumber)];
      return explicit ?? defaults.has(houseNumber);
    });
  });

  const observedHousesList = computed<ObservedHouseItem[]>(() => {
    const observedSet = new Set(observedHouseNumbers.value);

    return availableHouseNumbers.value.map((houseNumber) => ({
      id: houseNumber,
      number: houseNumber,
      active: observedSet.has(houseNumber),
    }));
  });

  const filteredAndLimitedEvents = computed(() => {
    const observedSet = new Set(observedHouseNumbers.value);

    return events.value
      .filter((event) => {
        const alertType = parseAlertType(event.type);
        const houseNumber = parseHouseNumberFromBuildingId(event.buildingID);

        if (alertType === null || houseNumber === null) {
          return false;
        }

        return (
          observedSet.has(houseNumber) && selectedTypeSet.value.has(alertType)
        );
      })
      .sort((left, right) => {
        const leftMs = left.timeStamp ? left.timeStamp.getTime() : 0;
        const rightMs = right.timeStamp ? right.timeStamp.getTime() : 0;
        return rightMs - leftMs;
      })
      .slice(0, normalizedLastEventsLimit.value);
  });

  const selectedHouseEvents = computed(() => {
    if (!selectedHouseNumber.value) {
      return [] as SmartQuartierEvent[];
    }

    const houseNumberAsText = String(selectedHouseNumber.value);

    return filteredAndLimitedEvents.value.filter((event) => {
      const parsedNumber = parseHouseNumberFromBuildingId(event.buildingID);
      if (parsedNumber !== null) {
        return parsedNumber === selectedHouseNumber.value;
      }

      return normalizeBuildingId(event.buildingID) === houseNumberAsText;
    });
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

  const sidebarEvents = computed<EventItem[]>(() => {
    return filteredAndLimitedEvents.value
      .map((event, index) => {
        const alertType = parseAlertType(event.type);
        const houseNumber = parseHouseNumberFromBuildingId(event.buildingID);

        if (!alertType || houseNumber === null) {
          return null;
        }

        return {
          id: index + 1,
          titleKey: `dashboard.events.${alertType}`,
          houseNumber,
          time: formatEventTimestamp(event.timeStamp),
          icon: EVENT_ICON_BY_TYPE[alertType],
          color: EVENT_COLOR_BY_TYPE[alertType],
          textColor: EVENT_TEXT_COLOR_BY_TYPE[alertType],
          timestamp: event.timeStamp ? event.timeStamp.getTime() : 0,
        };
      })
      .filter((event): event is EventItem & { timestamp: number } =>
        Boolean(event),
      )
      .map(({ timestamp, ...event }) => event);
  });

  const actions = computed<ActionItem[]>(() => {
    const observedSet = new Set(observedHouseNumbers.value);
    const groupedByEquivalent = buildEquivalentGroups(
      filteredAndLimitedEvents.value,
      observedSet,
      selectedTypeSet.value,
    );

    const generatedActions: Array<ActionItem & { timestamp: number }> = [];

    for (const [equivalentKey, grouped] of groupedByEquivalent.entries()) {
      const groupedEvents = [...grouped.events].sort(
        (left, right) => right.timestamp - left.timestamp,
      );

      const latest = groupedEvents[0];
      if (!latest) {
        continue;
      }

      const prefix = `${equivalentKey}:`;
      const openKeys = getActionKeysForState(
        actionStates.value,
        prefix,
        "open",
      );
      const doneKeys = getActionKeysForState(
        actionStates.value,
        prefix,
        "done",
      );

      const existingOpenKey = openKeys[0];
      const latestDoneKey = doneKeys[0];

      const lastClosedAt = getLastClosedAt(doneKeys, actionClosedAt.value);
      const selectedActionKey = resolveActionKey({
        equivalentKey,
        latestTimestamp: latest.timestamp,
        existingOpenKey,
        latestDoneKey,
        lastClosedAt,
      });

      if (!selectedActionKey) {
        continue;
      }

      const openedAt = getActionOpenedAt(selectedActionKey);
      const selectedEvent =
        groupedEvents.find((item) => item.timestamp === openedAt)?.event ??
        latest.event;

      const color = ACTION_COLOR_BY_TYPE[grouped.alertType];
      const textColor = ACTION_TEXT_COLOR_BY_TYPE[grouped.alertType];
      const state = actionStates.value[selectedActionKey] ?? "open";
      const closedAt =
        state === "done"
          ? (actionClosedAt.value[selectedActionKey] ?? openedAt)
          : null;

      generatedActions.push({
        id: selectedActionKey,
        actionKey: selectedActionKey,
        titleKey: grouped.titleKey,
        houseNumber: grouped.houseNumber,
        color,
        textColor,
        state,
        openedAt,
        closedAt,
        timestamp: selectedEvent.timeStamp
          ? selectedEvent.timeStamp.getTime()
          : openedAt,
      });
    }

    const sorted = generatedActions
      .sort((left, right) => right.timestamp - left.timestamp)
      .map(({ timestamp, ...action }) => action);

    if (onlyOpenAlarms.value) {
      return sorted.filter((action) => action.state === "open");
    }

    return sorted;
  });

  const applyHistoryResponse = (
    payload: SmartQuartierHistoryResponse | null | undefined,
  ) => {
    const next = toHistoryResponse(payload);
    events.value = next.events;
    measurements.value = next.measurements;
  };

  const open = (houseNumber: number) => {
    selectedHouseNumber.value = houseNumber;
    isOpen.value = true;
  };

  const close = () => {
    isOpen.value = false;
  };

  const toggleActionState = (actionKey: string) => {
    const currentState = actionStates.value[actionKey] ?? "open";

    if (currentState === "done") {
      actionStates.value[actionKey] = "open";
      actionClosedAt.value[actionKey] = null;
      return;
    }

    actionStates.value[actionKey] = "done";
    actionClosedAt.value[actionKey] = Date.now();
  };

  const setHouseObserved = (houseNumber: number, observed: boolean) => {
    appStore.setHouseObserved(houseNumber, observed);
  };

  const fetchHistory = async () => {
    if (loading.value) {
      return;
    }

    loading.value = true;
    error.value = null;

    try {
      const client = new Client();
      const response = await client.getSmartQuartierHistory();
      applyHistoryResponse(response);
    } catch (fetchError) {
      error.value =
        fetchError instanceof Error ? fetchError.message : "Unknown error";
    } finally {
      loading.value = false;
    }
  };

  const startLiveUpdates = async () => {
    if (liveConnection.value) {
      return;
    }

    const hubConnection = new HubConnectionBuilder()
      .withUrl("/hubs/smart-homes")
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();

    hubConnection.on(
      "historyUpdated",
      (payload: SmartQuartierHistoryResponse) => {
        applyHistoryResponse(payload);
      },
    );

    hubConnection.onreconnecting((reconnectError) => {
      error.value =
        reconnectError instanceof Error
          ? reconnectError.message
          : "Live updates reconnecting";
    });

    hubConnection.onreconnected(async () => {
      try {
        await hubConnection.invoke("SubscribeDashboardHistory");
        error.value = null;
      } catch (subscribeError) {
        error.value =
          subscribeError instanceof Error
            ? subscribeError.message
            : "Live updates unavailable";
        await fetchHistory();
      }
    });

    try {
      await hubConnection.start();
      await hubConnection.invoke("SubscribeDashboardHistory");
      liveConnection.value = hubConnection;
    } catch (liveError) {
      error.value =
        liveError instanceof Error
          ? liveError.message
          : "Live updates unavailable";
      await fetchHistory();
      await hubConnection.stop();
    }
  };

  const stopLiveUpdates = async () => {
    if (!liveConnection.value) {
      return;
    }

    liveConnection.value.off("historyUpdated");

    try {
      await liveConnection.value.invoke("UnsubscribeDashboardHistory");
    } catch {
      // Ignore unsubscribe failures during teardown.
    }

    await liveConnection.value.stop();
    liveConnection.value = null;
  };

  return {
    isOpen,
    selectedHouseNumber,
    loading,
    error,
    events,
    measurements,
    selectedHouseEvents,
    availableHouseNumbers,
    latestEventTypeByHouse,
    observedHouses: observedHousesList,
    sidebarEvents,
    actions,
    open,
    close,
    toggleActionState,
    setHouseObserved,
    fetchHistory,
    startLiveUpdates,
    stopLiveUpdates,
  };
});
