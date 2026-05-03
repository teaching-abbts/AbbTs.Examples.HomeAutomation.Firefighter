<template>
  <v-navigation-drawer
    class="sidebar-fill"
    :style="{ backgroundColor: '#8fb1d5' }"
    permanent
    width="260"
  >
    <div class="px-4 py-4 text-h5 font-weight-bold">
      {{ t("dashboard.sections.events") }}
    </div>

    <div class="px-3 pb-2">
      <v-select
        :items="eventTypeOptions"
        :label="t('dashboard.filters.eventTypes')"
        chips
        clearable
        closable-chips
        density="compact"
        hide-details
        item-title="title"
        item-value="value"
        multiple
        v-model="selectedEventTypes"
        variant="outlined"
      />

      <v-text-field
        :label="t('dashboard.filters.lastEventsLimit')"
        class="mt-3"
        density="compact"
        hide-details
        min="1"
        prepend-inner-icon="mdi-counter"
        type="number"
        v-model.number="eventsLimit"
        variant="outlined"
      />
    </div>

    <v-list class="px-2">
      <v-list-item
        :key="event.id"
        class="mb-3 px-0"
        lines="three"
        v-for="event in events"
      >
        <v-card
          :style="{ backgroundColor: event.color }"
          class="w-100"
          rounded="xl"
          variant="flat"
        >
          <v-card-item>
            <v-card-title
              class="d-flex align-center ga-2 pb-1"
              :style="{ color: event.textColor }"
            >
              <v-icon :icon="event.icon" />
              {{ t(event.titleKey) }}
            </v-card-title>
            <v-card-subtitle :style="{ color: event.textColor, opacity: 1 }">{{
              t("dashboard.houseName", { id: event.houseNumber })
            }}</v-card-subtitle>
            <div
              class="text-body-2 mt-1 font-italic"
              :style="{ color: event.textColor }"
            >
              {{ event.time }}
            </div>
          </v-card-item>
        </v-card>
      </v-list-item>
    </v-list>
  </v-navigation-drawer>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import { storeToRefs } from "pinia";
import { useI18n } from "vue-i18n";

import type { EventItem } from "./types";
import { useAppStore, type AppEventType } from "@/stores/app";

defineProps<{
  events: EventItem[];
}>();

const { t } = useI18n();
const appStore = useAppStore();
const { normalizedEventTypeFilter, normalizedLastEventsLimit } =
  storeToRefs(appStore);

const eventTypeOptions = computed(() => [
  { title: t("dashboard.events.fire"), value: "fire" },
  { title: t("dashboard.events.gas"), value: "gas" },
  { title: t("dashboard.events.motion"), value: "motion" },
]);

const selectedEventTypes = computed({
  get: () => normalizedEventTypeFilter.value,
  set: (value: AppEventType[]) => appStore.setEventTypes(value ?? []),
});

const eventsLimit = computed({
  get: () => normalizedLastEventsLimit.value,
  set: (value: number) => appStore.setLastEventsLimit(value),
});
</script>

<style scoped>
.sidebar-fill {
  height: 100%;
}
</style>
