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

          <div
            v-if="hasQuickActions(action)"
            class="mt-4 d-flex flex-column ga-2"
          >
            <div
              class="text-caption font-weight-bold"
              :style="{ color: action.textColor }"
            >
              {{ t("dashboard.actions.quickActionsLabel") }}
            </div>

            <v-btn
              v-if="action.alertType === 'gas'"
              class="quick-action-btn"
              block
              rounded="lg"
              variant="elevated"
              :style="quickActionStyle('gas-ventilate')"
              :disabled="!authStore.isOperator"
              :title="
                !authStore.isOperator
                  ? t('auth.operatorOnlyTooltip')
                  : undefined
              "
              @click="emit('quick-action', action.actionKey, 'gas-ventilate')"
            >
              {{ t("dashboard.actions.quickVentilate") }}
            </v-btn>

            <v-btn
              v-if="action.alertType === 'fire'"
              class="quick-action-btn"
              block
              rounded="lg"
              variant="elevated"
              :style="quickActionStyle('fire-stop-heating')"
              :disabled="!authStore.isOperator"
              :title="
                !authStore.isOperator
                  ? t('auth.operatorOnlyTooltip')
                  : undefined
              "
              @click="
                emit('quick-action', action.actionKey, 'fire-stop-heating')
              "
            >
              {{ t("dashboard.actions.quickStopHeating") }}
            </v-btn>

            <v-btn
              v-if="action.alertType === 'fire'"
              class="quick-action-btn"
              block
              rounded="lg"
              variant="elevated"
              :style="quickActionStyle('fire-neighbor-display')"
              :disabled="!authStore.isOperator"
              :title="
                !authStore.isOperator
                  ? t('auth.operatorOnlyTooltip')
                  : undefined
              "
              @click="
                emit('quick-action', action.actionKey, 'fire-neighbor-display')
              "
            >
              {{ t("dashboard.actions.quickNotification") }}
            </v-btn>
          </div>
        </v-card-item>

        <v-card-actions>
          <v-btn
            block
            rounded="lg"
            :style="{ backgroundColor: '#b0b4b8', color: '#ffffff' }"
            variant="elevated"
            :disabled="!authStore.isOperator"
            :title="
              !authStore.isOperator ? t('auth.operatorOnlyTooltip') : undefined
            "
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
import { useAuthStore } from "@/stores/auth";

const props = defineProps<{
  actions: ActionItem[];
}>();

const emit = defineEmits<{
  "toggle-action": [actionKey: string];
  "quick-action": [
    actionKey: string,
    quickAction:
      | "gas-ventilate"
      | "fire-stop-heating"
      | "fire-neighbor-display",
  ];
}>();

const { t } = useI18n();
const appStore = useAppStore();
const authStore = useAuthStore();
const { onlyOpenAlarms } = storeToRefs(appStore);

const showOnlyOpenActions = computed({
  get: () => onlyOpenAlarms.value,
  set: (value: boolean) => appStore.setOnlyOpenAlarms(value),
});

const filteredActions = computed(() => {
  return props.actions;
});

const hasQuickActions = (action: ActionItem) => {
  return action.alertType === "gas" || action.alertType === "fire";
};

const quickActionStyle = (
  quickAction: "gas-ventilate" | "fire-stop-heating" | "fire-neighbor-display",
) => {
  switch (quickAction) {
    case "gas-ventilate":
      return {
        backgroundColor: "#455a64",
        color: "#ffffff",
      };
    case "fire-stop-heating":
      return {
        backgroundColor: "#6d4c41",
        color: "#ffffff",
      };
    case "fire-neighbor-display":
      return {
        backgroundColor: "#37474f",
        color: "#ffffff",
      };
  }
};

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

.quick-action-btn {
  font-weight: 700;
}

.quick-action-btn :deep(.v-btn__content) {
  color: inherit !important;
}
</style>
