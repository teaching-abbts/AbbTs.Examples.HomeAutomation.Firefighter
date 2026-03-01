<template>
  <v-dialog
    v-model="dialogModel"
    max-width="920"
    @click:outside="store.close()"
  >
    <v-card>
      <v-card-title class="text-h4 text-center py-4">
        {{ dialogTitle }}
      </v-card-title>

      <v-divider />

      <v-card-text>
        <v-progress-linear
          v-if="loading"
          color="primary"
          indeterminate
          rounded
        />

        <v-alert v-else-if="error" class="mb-3" type="error" variant="tonal">
          {{ t("dashboard.houseDetails.error") }}
        </v-alert>

        <v-table v-else class="history-table" density="comfortable">
          <thead>
            <tr>
              <th scope="col">{{ t("dashboard.houseDetails.timestamp") }}</th>
              <th scope="col">{{ t("dashboard.houseDetails.type") }}</th>
              <th scope="col">{{ t("dashboard.houseDetails.details") }}</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="(event, index) in sortedEvents"
              :key="rowKey(event, index)"
            >
              <td>{{ formatTimestamp(event.timeStamp) }}</td>
              <td>{{ formatType(event.type) }}</td>
              <td>{{ event.data || "..." }}</td>
            </tr>
            <tr v-if="sortedEvents.length === 0">
              <td class="text-center" colspan="3">
                {{ t("dashboard.houseDetails.noData") }}
              </td>
            </tr>
          </tbody>
        </v-table>
      </v-card-text>

      <v-card-actions class="justify-end px-6 pb-4">
        <v-btn variant="text" @click="store.close()">
          {{ t("dashboard.houseDetails.close") }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import { storeToRefs } from "pinia";
import { useI18n } from "vue-i18n";

import { useHouseDetailsStore } from "@/stores/houseDetails";
import type { SmartQuartierEvent } from "@/api/AbbTs.Examples.HomeAutomation.Firefighter.Webhost";

const { locale, t, te } = useI18n({ useScope: "global" });
const store = useHouseDetailsStore();
const { error, isOpen, loading, selectedHouseEvents, selectedHouseNumber } =
  storeToRefs(store);

const dialogModel = computed({
  get: () => isOpen.value,
  set: (value: boolean) => {
    if (!value) {
      store.close();
    }
  },
});

const dialogTitle = computed(() => {
  if (!selectedHouseNumber.value) {
    return t("dashboard.houseDetails.title");
  }

  return t("dashboard.houseName", { id: selectedHouseNumber.value });
});

const sortedEvents = computed(() => {
  return [...selectedHouseEvents.value].sort((left, right) => {
    const leftMs = left.timeStamp ? left.timeStamp.getTime() : 0;
    const rightMs = right.timeStamp ? right.timeStamp.getTime() : 0;
    return rightMs - leftMs;
  });
});

const rowKey = (event: SmartQuartierEvent, index: number) => {
  const timeStamp = event.timeStamp?.toISOString() ?? "na";
  return `${timeStamp}-${event.type ?? "unknown"}-${index}`;
};

const formatTimestamp = (timeStamp: Date | undefined) => {
  if (!timeStamp) {
    return "...";
  }

  return new Intl.DateTimeFormat(locale.value, {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  }).format(timeStamp);
};

const formatType = (type: string | undefined) => {
  if (!type) {
    return "...";
  }

  const key = `dashboard.events.${type.trim().toLowerCase()}`;
  return te(key) ? t(key) : type;
};
</script>

<style scoped>
.history-table :deep(th),
.history-table :deep(td) {
  border: thin solid rgba(var(--v-theme-on-surface), 0.5);
}
</style>
