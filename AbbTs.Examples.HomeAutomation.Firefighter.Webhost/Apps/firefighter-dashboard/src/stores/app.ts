import { useLocalStorage } from "@vueuse/core";
import { computed } from "vue";
import { defineStore } from "pinia";

export type AppTheme = "light" | "dark";
export type AppLocale = "de" | "en" | "jp";
export type AppEventType = "fire" | "gas" | "motion";

const STORAGE_KEYS = {
  locale: "ff.settings.locale",
  theme: "ff.settings.theme",
  observedHouses: "ff.settings.observedHouses",
  onlyOpenAlarms: "ff.settings.onlyOpenAlarms",
  eventTypes: "ff.settings.eventTypes",
  lastEventsLimit: "ff.settings.lastEventsLimit",
} as const;

const ALL_EVENT_TYPES: AppEventType[] = ["fire", "gas", "motion"];

const normalizeLimit = (value: number) => {
  if (!Number.isFinite(value)) {
    return 100;
  }

  return Math.min(500, Math.max(1, Math.trunc(value)));
};

export const useAppStore = defineStore("app", () => {
  const locale = useLocalStorage<AppLocale>(STORAGE_KEYS.locale, "de");
  const theme = useLocalStorage<AppTheme>(STORAGE_KEYS.theme, "light");
  const observedHouses = useLocalStorage<Record<string, boolean>>(
    STORAGE_KEYS.observedHouses,
    {},
  );
  const onlyOpenAlarms = useLocalStorage<boolean>(
    STORAGE_KEYS.onlyOpenAlarms,
    false,
  );
  const eventTypeFilter = useLocalStorage<AppEventType[]>(
    STORAGE_KEYS.eventTypes,
    [...ALL_EVENT_TYPES],
  );
  const lastEventsLimit = useLocalStorage<number>(
    STORAGE_KEYS.lastEventsLimit,
    100,
  );

  const normalizedEventTypeFilter = computed<AppEventType[]>(() => {
    const allowed = new Set(ALL_EVENT_TYPES);
    const selected = (eventTypeFilter.value ?? []).filter(
      (item): item is AppEventType => allowed.has(item as AppEventType),
    );

    return selected;
  });

  const effectiveEventTypeFilter = computed<AppEventType[]>(() => {
    return normalizedEventTypeFilter.value.length > 0
      ? normalizedEventTypeFilter.value
      : [...ALL_EVENT_TYPES];
  });

  const normalizedLastEventsLimit = computed(() => {
    return normalizeLimit(lastEventsLimit.value);
  });

  const setLocale = (nextLocale: AppLocale) => {
    locale.value = nextLocale;
  };

  const setTheme = (nextTheme: AppTheme) => {
    theme.value = nextTheme;
  };

  const setHouseObserved = (houseNumber: number, observed: boolean) => {
    observedHouses.value[String(houseNumber)] = observed;
  };

  const setOnlyOpenAlarms = (value: boolean) => {
    onlyOpenAlarms.value = value;
  };

  const toggleEventType = (eventType: AppEventType, enabled: boolean) => {
    const selected = new Set(normalizedEventTypeFilter.value);

    if (enabled) {
      selected.add(eventType);
    } else {
      selected.delete(eventType);
    }

    eventTypeFilter.value = [...selected];
  };

  const setEventTypes = (eventTypes: AppEventType[]) => {
    const allowed = new Set(ALL_EVENT_TYPES);
    const selected = eventTypes.filter((item) => allowed.has(item));

    eventTypeFilter.value = selected;
  };

  const setLastEventsLimit = (value: number) => {
    lastEventsLimit.value = normalizeLimit(value);
  };

  return {
    locale,
    theme,
    observedHouses,
    onlyOpenAlarms,
    eventTypeFilter,
    lastEventsLimit,
    normalizedEventTypeFilter,
    effectiveEventTypeFilter,
    normalizedLastEventsLimit,
    setLocale,
    setTheme,
    setHouseObserved,
    setOnlyOpenAlarms,
    toggleEventType,
    setEventTypes,
    setLastEventsLimit,
  };
});
