<template>
  <v-navigation-drawer
    class="sidebar-fill"
    color="surface-variant"
    location="right"
    permanent
    width="300"
  >
    <div class="px-4 py-4 text-h5 font-weight-bold">
      {{ t("dashboard.sections.actions") }}
    </div>

    <div class="px-3 pb-3">
      <v-switch
        v-model="showOnlyOpenActions"
        class="mb-3"
        color="primary"
        density="compact"
        hide-details
        :label="t('dashboard.actions.onlyOpen')"
      />

      <v-card
        v-for="action in filteredActions"
        :key="action.id"
        class="mb-4 action-card"
        rounded="lg"
        :style="{ backgroundColor: action.color }"
        variant="flat"
      >
        <v-card-item :style="{ color: action.textColor }">
          <v-card-title class="text-h6" :style="{ color: action.textColor }">{{
            t(action.titleKey)
          }}</v-card-title>
          <v-card-subtitle :style="{ color: action.textColor, opacity: 1 }">{{
            t("dashboard.houseName", { id: action.houseNumber })
          }}</v-card-subtitle>
          <div class="text-body-2 mt-1" :style="{ color: action.textColor }">
            <strong>{{ t("dashboard.actions.createdAt") }}:</strong>
            {{ formatActionTimestamp(action.openedAt) }}
          </div>
          <div class="text-body-2" :style="{ color: action.textColor }">
            <strong>{{ t("dashboard.actions.resolvedAt") }}:</strong>
            {{
              action.closedAt
                ? formatActionTimestamp(action.closedAt)
                : t("dashboard.actions.notResolved")
            }}
          </div>
          <v-chip
            v-if="!showOnlyOpenActions"
            class="mt-2"
            :color="action.state === 'done' ? 'success' : 'secondary'"
            size="small"
            variant="flat"
          >
            {{
              action.state === "done"
                ? t("dashboard.actions.done")
                : t("dashboard.actions.open")
            }}
          </v-chip>
        </v-card-item>

        <v-card-actions>
          <v-btn
            block
            rounded="lg"
            :style="{ backgroundColor: '#b0b4b8', color: '#ffffff' }"
            variant="elevated"
            @click="emit('toggle-action', action.actionKey)"
            >{{
              showOnlyOpenActions
                ? t("dashboard.actions.markDone")
                : action.state === "done"
                  ? t("dashboard.actions.reopen")
                  : t("dashboard.actions.markDone")
            }}</v-btn
          >
        </v-card-actions>
      </v-card>
    </div>
  </v-navigation-drawer>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import { storeToRefs } from "pinia";
import { useI18n } from "vue-i18n";

import type { ActionItem } from "./types";
import { useAppStore } from "@/stores/app";

const props = defineProps<{
  actions: ActionItem[];
}>();

const emit = defineEmits<{
  "toggle-action": [actionKey: string];
}>();

const { t } = useI18n();
const appStore = useAppStore();
const { onlyOpenAlarms } = storeToRefs(appStore);

const showOnlyOpenActions = computed({
  get: () => onlyOpenAlarms.value,
  set: (value: boolean) => appStore.setOnlyOpenAlarms(value),
});

const filteredActions = computed(() => {
  return props.actions;
});

const formatActionTimestamp = (timestamp: number) => {
  return new Intl.DateTimeFormat("de-DE", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  }).format(new Date(timestamp));
};
</script>

<style scoped>
.sidebar-fill {
  height: 100%;
}

.action-card {
  border-top-right-radius: 28px !important;
}
</style>
