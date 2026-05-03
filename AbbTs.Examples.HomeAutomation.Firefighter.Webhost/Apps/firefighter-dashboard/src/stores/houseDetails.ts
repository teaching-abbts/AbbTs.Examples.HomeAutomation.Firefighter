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
import type { SmartHomeSummary } from "@/types/smartHomes";

type AlertType = AppEventType;
type ActionAlertType = "fire" | "gas" | "observe";

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

const ACTION_TITLE_BY_TYPE: Record<ActionAlertType, string> = {
  fire: "dashboard.actions.extinguishFire",
  gas: "dashboard.actions.openDoors",
  observe: "dashboard.actions.observe",
};

const ACTION_COLOR_BY_TYPE: Record<ActionAlertType, string> = {
  fire: "#f07f2f",
  gas: "#facc15",
  observe: "#6f42c1",
};

const ACTION_TEXT_COLOR_BY_TYPE: Record<ActionAlertType, string> = {
  fire: "#ffffff",
  gas: "#111111",
  observe: "#ffffff",
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
  alertType: ActionAlertType;
  titleKey: string;
  events: Array<{ event?: SmartQuartierEvent; timestamp: number }>;
};

type HouseCoordinate = {
  x: number;
  y: number;
};

const getDistance = (left: HouseCoordinate, right: HouseCoordinate) => {
  const dx = left.x - right.x;
  const dy = left.y - right.y;
  return Math.hypot(dx, dy);
};

const addEventTriggeredGroup = ({
  groupedByEquivalent,
  event,
  alertType,
  houseNumber,
}: {
  groupedByEquivalent: Map<string, EquivalentGroup>;
  event: SmartQuartierEvent;
  alertType: Exclude<AlertType, "motion">;
  houseNumber: number;
}) => {
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
    return;
  }

  group.events.push({ event, timestamp });
};

const getLatestNeighborFireTimestamp = ({
  houseNumber,
  houseCoordinates,
  latestFireTimestampByHouse,
  distanceThreshold,
}: {
  houseNumber: number;
  houseCoordinates: Record<number, HouseCoordinate>;
  latestFireTimestampByHouse: Map<number, number>;
  distanceThreshold: number;
}) => {
  const ownCoordinate = houseCoordinates[houseNumber];
  if (!ownCoordinate) {
    return 0;
  }

  let latestNeighborFireTimestamp = 0;

  for (const [fireHouseNumber, fireTimestamp] of latestFireTimestampByHouse) {
    if (fireHouseNumber === houseNumber) {
      continue;
    }

    const fireCoordinate = houseCoordinates[fireHouseNumber];
    if (!fireCoordinate) {
      continue;
    }

    const distance = getDistance(ownCoordinate, fireCoordinate);
    if (
      distance <= distanceThreshold &&
      fireTimestamp > latestNeighborFireTimestamp
    ) {
      latestNeighborFireTimestamp = fireTimestamp;
    }
  }

  return latestNeighborFireTimestamp;
};

const addNeighborObserveGroup = ({
  groupedByEquivalent,
  houseNumber,
  timestamp,
}: {
  groupedByEquivalent: Map<string, EquivalentGroup>;
  houseNumber: number;
  timestamp: number;
}) => {
  const titleKey = ACTION_TITLE_BY_TYPE.observe;
  const equivalentKey = `${houseNumber}:observe:${titleKey}`;
  const group = groupedByEquivalent.get(equivalentKey);

  if (!group) {
    groupedByEquivalent.set(equivalentKey, {
      houseNumber,
      alertType: "observe",
      titleKey,
      events: [{ timestamp }],
    });
    return;
  }

  group.events.push({ timestamp });
};

const updateLatestFireTimestamp = ({
  latestFireTimestampByHouse,
  alertType,
  houseNumber,
  event,
}: {
  latestFireTimestampByHouse: Map<number, number>;
  alertType: AlertType | null;
  houseNumber: number | null;
  event: SmartQuartierEvent;
}) => {
  if (alertType !== "fire" || houseNumber === null) {
    return;
  }

  const timestamp = event.timeStamp ? event.timeStamp.getTime() : 0;
  const current = latestFireTimestampByHouse.get(houseNumber) ?? 0;
  if (timestamp > current) {
    latestFireTimestampByHouse.set(houseNumber, timestamp);
  }
};

const toActionAlertType = (
  alertType: AlertType | null,
  selectedTypeSet: Set<AlertType>,
): Exclude<AlertType, "motion"> | null => {
  if (!alertType || alertType === "motion" || !selectedTypeSet.has(alertType)) {
    return null;
  }

  return alertType;
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
  houseCoordinates: Record<number, HouseCoordinate>,
  distanceThreshold: number,
) => {
  const groupedByEquivalent = new Map<string, EquivalentGroup>();
  const latestFireTimestampByHouse = new Map<number, number>();
  const candidateObserveHouses = Object.keys(houseCoordinates)
    .map(Number)
    .filter((value) => Number.isFinite(value));

  for (const event of sourceEvents) {
    const alertType = parseAlertType(event.type);
    const houseNumber = parseHouseNumberFromBuildingId(event.buildingID);

    updateLatestFireTimestamp({
      latestFireTimestampByHouse,
      alertType,
      houseNumber,
      event,
    });

    if (!alertType || houseNumber === null || !observedSet.has(houseNumber)) {
      continue;
    }

    const actionAlertType = toActionAlertType(alertType, selectedTypeSet);
    if (!actionAlertType) {
      continue;
    }

    addEventTriggeredGroup({
      groupedByEquivalent,
      event,
      alertType: actionAlertType,
      houseNumber,
    });
  }

  for (const houseNumber of candidateObserveHouses) {
    const latestNeighborFireTimestamp = getLatestNeighborFireTimestamp({
      houseNumber,
      houseCoordinates,
      latestFireTimestampByHouse,
      distanceThreshold,
    });

    if (latestNeighborFireTimestamp === 0) {
      continue;
    }

    addNeighborObserveGroup({
      groupedByEquivalent,
      houseNumber,
      timestamp: latestNeighborFireTimestamp,
    });
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
    normalizedNeighborFireDistanceThreshold,
    onlyOpenAlarms,
  } = storeToRefs(appStore);

  const isOpen = ref(false);
  const selectedHouseNumber = ref<number | null>(null);
  const loading = ref(false);
  const error = ref<string | null>(null);
  const events = ref<SmartQuartierEvent[]>([]);
  const measurements = ref<SmartQuartierMeasurement[]>([]);
  const houseCoordinates = ref<Record<number, HouseCoordinate>>({});
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

  const observedHouseNumbers = computed(() => {
    return [...availableHouseNumbers.value];
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

        if (
          alertType === null ||
          alertType === "motion" ||
          houseNumber === null
        ) {
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
      events.value,
      observedSet,
      selectedTypeSet.value,
      houseCoordinates.value,
      normalizedNeighborFireDistanceThreshold.value,
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
        timestamp: selectedEvent?.timeStamp
          ? selectedEvent.timeStamp.getTime()
          : openedAt,
      });
    }

    generatedActions.sort((left, right) => right.timestamp - left.timestamp);
    const sorted = generatedActions.map(({ timestamp, ...action }) => action);

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

  const setHouseCoordinates = (smartHomes: SmartHomeSummary[]) => {
    const nextCoordinates: Record<number, HouseCoordinate> = {};

    for (const smartHome of smartHomes) {
      const houseNumber = parseHouseNumberFromBuildingId(smartHome.id);
      if (houseNumber === null) {
        continue;
      }

      nextCoordinates[houseNumber] = {
        x: smartHome.xCoordinate ?? 0,
        y: smartHome.yCoordinate ?? 0,
      };
    }

    houseCoordinates.value = nextCoordinates;
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
    setHouseCoordinates,
    fetchHistory,
    startLiveUpdates,
    stopLiveUpdates,
  };
});
