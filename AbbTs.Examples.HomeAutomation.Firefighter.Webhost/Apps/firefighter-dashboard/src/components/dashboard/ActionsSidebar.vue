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

        <v-card-actions class="flex-wrap">
          <v-btn
            v-for="quick in getQuickActionsFor(action)"
            :key="quick.id"
            block
            class="mb-2 mx-0"
            :prepend-icon="quick.icon"
            rounded="lg"
            size="small"
            :style="{ backgroundColor: '#ffffff', color: '#212121' }"
            variant="elevated"
            @click="onQuickAction(quick.id, action)"
            >{{ t(quick.labelKey) }}</v-btn
          >
          <v-btn
            block
            class="mx-0"
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

    <v-dialog v-model="fireAlarmDialogOpen" max-width="520">
      <v-card rounded="lg">
        <v-card-title>{{ t("dashboard.fireAlarm.title") }}</v-card-title>
        <v-card-text>
          <p class="text-body-2 mb-3">
            {{
              t("dashboard.fireAlarm.description", {
                id: fireAlarmHouseNumber ?? "?",
              })
            }}
          </p>
          <v-textarea
            v-model="fireAlarmMessage"
            auto-grow
            :label="t('dashboard.fireAlarm.messageLabel')"
            rows="3"
            variant="outlined"
          />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn variant="text" @click="closeFireAlarmDialog">{{
            t("dashboard.fireAlarm.cancel")
          }}</v-btn>
          <v-btn color="error" variant="elevated" @click="confirmFireAlarm">{{
            t("dashboard.fireAlarm.send")
          }}</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-navigation-drawer>
</template>

<script lang="ts" setup>
import { computed, ref } from "vue";
import { storeToRefs } from "pinia";
import { useI18n } from "vue-i18n";

import type { ActionItem } from "./types";
import { useAppStore } from "@/stores/app";

export type QuickActionId =
  | "open-doors"
  | "heating-off"
  | "fire-alarm-broadcast";

export type QuickActionPayload = {
  id: QuickActionId;
  action: ActionItem;
  message?: string;
};

type QuickActionDescriptor = {
  id: QuickActionId;
  labelKey: string;
  icon: string;
};

const QUICK_ACTIONS_BY_COLOR: Record<string, QuickActionDescriptor[]> = {
  "#FFEB3B": [
    {
      id: "open-doors",
      labelKey: "dashboard.actions.quickOpenDoors",
      icon: "mdi-door-open",
    },
  ],
  "#F44336": [
    {
      id: "heating-off",
      labelKey: "dashboard.actions.quickHeatingOff",
      icon: "mdi-radiator-off",
    },
    {
      id: "fire-alarm-broadcast",
      labelKey: "dashboard.actions.quickFireAlarm",
      icon: "mdi-bullhorn",
    },
  ],
};

const props = defineProps<{
  actions: ActionItem[];
}>();

const emit = defineEmits<{
  "toggle-action": [actionKey: string];
  "quick-action": [payload: QuickActionPayload];
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

const getQuickActionsFor = (action: ActionItem): QuickActionDescriptor[] => {
  if (action.state !== "open") {
    return [];
  }

  return QUICK_ACTIONS_BY_COLOR[action.color] ?? [];
};

const fireAlarmDialogOpen = ref(false);
const fireAlarmMessage = ref("Feueralarm\nFeuerwehr unterwegs!");
const fireAlarmHouseNumber = ref<number | null>(null);
const pendingFireAlarmAction = ref<ActionItem | null>(null);

const openFireAlarmDialog = (action: ActionItem) => {
  pendingFireAlarmAction.value = action;
  fireAlarmHouseNumber.value = action.houseNumber;
  fireAlarmMessage.value = "Feueralarm\nFeuerwehr unterwegs!";
  fireAlarmDialogOpen.value = true;
};

const closeFireAlarmDialog = () => {
  fireAlarmDialogOpen.value = false;
  pendingFireAlarmAction.value = null;
};

const confirmFireAlarm = () => {
  const action = pendingFireAlarmAction.value;
  if (!action) {
    closeFireAlarmDialog();
    return;
  }

  emit("quick-action", {
    id: "fire-alarm-broadcast",
    action,
    message: fireAlarmMessage.value,
  });

  closeFireAlarmDialog();
};

const onQuickAction = (id: QuickActionId, action: ActionItem) => {
  if (id === "fire-alarm-broadcast") {
    openFireAlarmDialog(action);
    return;
  }

  emit("quick-action", { id, action });
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
