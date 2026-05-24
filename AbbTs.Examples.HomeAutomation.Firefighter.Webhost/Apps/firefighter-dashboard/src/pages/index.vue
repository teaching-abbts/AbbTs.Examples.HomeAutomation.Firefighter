<template>
  <div class="dashboard-page mt-10">
    <EventsSidebar :events="sidebarEvents" />
    <div class="dashboard-content">
      <v-row class="mb-4" justify="space-between" align="center">
        <v-col cols="12" md="8">
          <div class="text-overline text-primary">
            {{ t("dashboard.badge") }}
          </div>
          <h1 class="text-h4 font-weight-bold mb-2">
            {{ t("dashboard.title") }}
          </h1>
          <p class="text-body-1 text-medium-emphasis mb-0">
            {{ t("dashboard.subtitle") }}
          </p>
        </v-col>
      </v-row>

      <v-alert
        v-if="smartHomesError"
        class="mb-4"
        type="warning"
        variant="tonal"
      >
        {{ smartHomesError }}
      </v-alert>
      <v-tabs v-model="landscapeView" color="primary">
        <v-tab v-for="view in views" :key="view" :value="view">{{
          view
        }}</v-tab>
      </v-tabs>
      <v-tabs-window v-model="landscapeView">
        <v-tabs-window-item value="3d">
          <v-row v-if="smartHomes.length > 0" class="mb-2">
            <v-col cols="12">
              <SmartHomesLandscape
                :homes="smartHomes"
                :title="t('smartHomes.landscapeTitle')"
                :house-colors="houseAlertColors"
                @select="openSmartHomeDetails"
              />
            </v-col>
          </v-row>
        </v-tabs-window-item>
        <v-tabs-window-item value="2d">
          <v-row v-if="smartHomes.length > 0" class="ma-2">
            <v-col
              v-for="smartHome in smartHomes"
              :key="smartHome.id"
              cols="12"
              md="6"
              xl="4"
            >
              <SmartHomeCard
                :summary="smartHome"
                @select="openSmartHomeDetails(smartHome.id)"
              />
            </v-col>
          </v-row>
        </v-tabs-window-item>
      </v-tabs-window>
    </div>

    <v-dialog v-model="showFireDisplayDialog" max-width="640">
      <v-card>
        <v-card-title>{{
          t("dashboard.actions.fireDialogTitle")
        }}</v-card-title>
        <v-card-text>
          <p class="mb-3 text-body-2 text-medium-emphasis">
            {{ t("dashboard.actions.fireDialogDescription") }}
          </p>
          <v-textarea
            v-model="fireDisplayMessage"
            auto-grow
            :label="t('dashboard.actions.fireDialogMessageLabel')"
            rows="3"
            variant="outlined"
          />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn variant="text" @click="cancelFireDisplayDialog">
            {{ t("dashboard.actions.fireDialogCancel") }}
          </v-btn>
          <v-btn
            color="error"
            variant="elevated"
            @click="confirmFireDisplayDialog"
          >
            {{ t("dashboard.actions.fireDialogSend") }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-snackbar v-model="showQuickActionToast" :color="quickActionToastColor">
      {{ quickActionToastMessage }}
    </v-snackbar>

    <ActionsSidebar
      :actions="actions"
      @toggle-action="toggleAction"
      @quick-action="handleQuickAction"
    />
  </div>
</template>

<script lang="ts" setup>
import {
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from "@microsoft/signalr";
import { Client } from "@/api/AbbTs.Examples.HomeAutomation.Firefighter.Webhost";
import ActionsSidebar from "@/components/dashboard/ActionsSidebar.vue";
import EventsSidebar from "@/components/dashboard/EventsSidebar.vue";
import SmartHomeCard from "@/components/smart-homes/SmartHomeCard.vue";
import SmartHomesLandscape from "@/components/smart-homes/SmartHomesLandscape.vue";
import { computed, onMounted, onUnmounted, ref } from "vue";
import { useRouter } from "vue-router";
import { useHouseDetailsStore } from "@/stores/houseDetails";
import { storeToRefs } from "pinia";
import { useAppStore } from "@/stores/app";
import type { SmartHomeCommand, SmartHomeSummary } from "@/types/smartHomes";
import { useI18n } from "vue-i18n";

const { t } = useI18n({ useScope: "global" });
const router = useRouter();
const houseDetailsStore = useHouseDetailsStore();
const appStore = useAppStore();
const sidebarEvents = computed(() => houseDetailsStore.sidebarEvents);
const actions = computed(() => houseDetailsStore.actions);
const { normalizedNeighborFireDistanceThreshold } = storeToRefs(appStore);

const views = ["2d", "3d"];

const landscapeView = ref(views[0]);

const COLOR_PRIORITY: Record<string, number> = {
  "#C62828": 4, // endangeredLives
  "#F44336": 3, // fire
  "#FFEB3B": 2, // gas
  "#FF9800": 1, // observe
};

const houseAlertColors = computed(() => {
  const result: Record<number, string> = {};
  const bestPriority: Record<number, number> = {};
  for (const action of actions.value) {
    if (action.state !== "open") continue;
    const prio = COLOR_PRIORITY[action.color] ?? 0;
    if (prio > (bestPriority[action.houseNumber] ?? 0)) {
      bestPriority[action.houseNumber] = prio;
      result[action.houseNumber] = action.color;
    }
  }
  return result;
});

const smartHomes = ref<SmartHomeSummary[]>([]);
const smartHomesError = ref("");
const showFireDisplayDialog = ref(false);
const fireDisplayMessage = ref("Feueralarm\nFeuerwehr unterwegs!");
const pendingFireActionHouseNumber = ref<number | null>(null);
const quickActionToastMessage = ref("");
const quickActionToastColor = ref<"success" | "warning" | "error">("success");
const showQuickActionToast = ref(false);
const apiClient = new Client();

const smartHomesConnection = new HubConnectionBuilder()
  .withUrl("/hubs/smart-homes")
  .withAutomaticReconnect()
  .configureLogging(LogLevel.Warning)
  .build();

const mapSmartHomes = (input: SmartHomeSummary[]): SmartHomeSummary[] => {
  return input.map((item) => ({
    id: item.id ?? "",
    owner: item.owner ?? "",
    xCoordinate: item.xCoordinate ?? 0,
    yCoordinate: item.yCoordinate ?? 0,
    isConnected: item.isConnected ?? false,
    connectedAtUtc: item.connectedAtUtc ?? null,
    lastSeenUtc: item.lastSeenUtc ?? null,
    recentMessageCount: item.recentMessageCount ?? 0,
  }));
};

const loadSmartHomes = async () => {
  try {
    const response = await apiClient.getSmartHomes();
    smartHomes.value = mapSmartHomes((response ?? []) as SmartHomeSummary[]);
    houseDetailsStore.setHouseCoordinates(smartHomes.value);
    smartHomesError.value = "";
  } catch {
    smartHomesError.value = t("smartHomes.loadError");
  }
};

onMounted(async () => {
  await houseDetailsStore.fetchHistory();
  await houseDetailsStore.startLiveUpdates();

  await loadSmartHomes();

  smartHomesConnection.on(
    "smartHomesChanged",
    (payload: SmartHomeSummary[]) => {
      smartHomes.value = mapSmartHomes(payload);
      houseDetailsStore.setHouseCoordinates(smartHomes.value);
    },
  );

  try {
    await smartHomesConnection.start();
  } catch {
    smartHomesError.value ||= t("smartHomes.liveUnavailable");
  }
});

onUnmounted(async () => {
  await houseDetailsStore.stopLiveUpdates();
  smartHomesConnection.off("smartHomesChanged");
  await smartHomesConnection.stop();
});

const openSmartHomeDetails = (smartHomeId: string) => {
  router.push(`/smart-homes/${encodeURIComponent(smartHomeId)}`);
};

const toggleAction = (actionKey: string) => {
  houseDetailsStore.toggleActionState(actionKey);
};

const parseHouseNumberFromSmartHomeId = (smartHomeId: string) => {
  const match = smartHomeId.match(/\d+/);
  return match ? Number.parseInt(match[0], 10) : null;
};

const showQuickActionFeedback = (
  message: string,
  color: "success" | "warning" | "error",
) => {
  quickActionToastMessage.value = message;
  quickActionToastColor.value = color;
  showQuickActionToast.value = true;
};

const getSmartHomeIdByHouseNumber = (houseNumber: number) => {
  const match = smartHomes.value.find((home) => {
    return parseHouseNumberFromSmartHomeId(home.id) === houseNumber;
  });

  return match?.id ?? null;
};

const getNeighborSmartHomeIds = (sourceHouseNumber: number) => {
  const sourceHome = smartHomes.value.find((home) => {
    return parseHouseNumberFromSmartHomeId(home.id) === sourceHouseNumber;
  });

  if (!sourceHome) {
    return [];
  }

  const threshold = normalizedNeighborFireDistanceThreshold.value;

  return smartHomes.value
    .filter((candidate) => {
      if (candidate.id === sourceHome.id) {
        return false;
      }

      const dx = (candidate.xCoordinate ?? 0) - (sourceHome.xCoordinate ?? 0);
      const dy = (candidate.yCoordinate ?? 0) - (sourceHome.yCoordinate ?? 0);
      const distance = Math.hypot(dx, dy);

      return distance <= threshold;
    })
    .map((home) => home.id);
};

const sendCommandToSmartHome = async (
  smartHomeId: string,
  payload: SmartHomeCommand,
) => {
  if (smartHomesConnection.state !== HubConnectionState.Connected) {
    throw new Error(t("dashboard.actions.errors.hubDisconnected"));
  }

  await smartHomesConnection.invoke("SendCommand", smartHomeId, payload);
};

const sendDoorOpenForGas = async (houseNumber: number) => {
  const smartHomeId = getSmartHomeIdByHouseNumber(houseNumber);
  if (!smartHomeId) {
    showQuickActionFeedback(
      t("dashboard.actions.errors.houseNotFound"),
      "error",
    );
    return;
  }

  await sendCommandToSmartHome(smartHomeId, {
    device: "Door",
    command: "open",
    value: "",
  });

  showQuickActionFeedback(
    t("dashboard.actions.feedback.ventilationSent"),
    "success",
  );
};

const sendHeatingOffForFire = async (houseNumber: number) => {
  const smartHomeId = getSmartHomeIdByHouseNumber(houseNumber);
  if (!smartHomeId) {
    showQuickActionFeedback(
      t("dashboard.actions.errors.houseNotFound"),
      "error",
    );
    return;
  }

  await sendCommandToSmartHome(smartHomeId, {
    device: "HeatingControl",
    command: "off",
    value: "",
  });

  showQuickActionFeedback(
    t("dashboard.actions.feedback.heatingOffSent"),
    "success",
  );
};

const openFireDisplayDialog = (houseNumber: number) => {
  pendingFireActionHouseNumber.value = houseNumber;
  fireDisplayMessage.value = "Feueralarm\nFeuerwehr unterwegs!";
  showFireDisplayDialog.value = true;
};

const cancelFireDisplayDialog = () => {
  showFireDisplayDialog.value = false;
  pendingFireActionHouseNumber.value = null;
};

const confirmFireDisplayDialog = async () => {
  const sourceHouseNumber = pendingFireActionHouseNumber.value;
  if (!sourceHouseNumber) {
    cancelFireDisplayDialog();
    return;
  }

  const neighborIds = getNeighborSmartHomeIds(sourceHouseNumber);
  if (neighborIds.length === 0) {
    showQuickActionFeedback(
      t("dashboard.actions.errors.noNeighborsFound"),
      "warning",
    );
    cancelFireDisplayDialog();
    return;
  }

  const message = fireDisplayMessage.value.trim();
  if (!message) {
    showQuickActionFeedback(
      t("dashboard.actions.errors.emptyMessage"),
      "warning",
    );
    return;
  }

  const displayValue = message
    .split(/\r?\n/)
    .map((line) => line.trim())
    .filter((line) => line.length > 0)
    .join(";");

  if (!displayValue) {
    showQuickActionFeedback(
      t("dashboard.actions.errors.emptyMessage"),
      "warning",
    );
    return;
  }

  let successCount = 0;
  for (const neighborId of neighborIds) {
    try {
      await sendCommandToSmartHome(neighborId, {
        device: "Display",
        command: "set",
        value: displayValue,
      });
      successCount += 1;
    } catch {
      // Continue with remaining neighbors.
    }
  }

  if (successCount === 0) {
    showQuickActionFeedback(
      t("dashboard.actions.errors.notifyFailed"),
      "error",
    );
  } else {
    showQuickActionFeedback(
      t("dashboard.actions.feedback.neighborsNotified", {
        count: successCount,
      }),
      successCount === neighborIds.length ? "success" : "warning",
    );
  }

  cancelFireDisplayDialog();
};

const handleQuickAction = async (
  actionKey: string,
  quickAction: "gas-ventilate" | "fire-stop-heating" | "fire-neighbor-display",
) => {
  const targetAction = actions.value.find(
    (action) => action.actionKey === actionKey,
  );
  if (!targetAction) {
    showQuickActionFeedback(
      t("dashboard.actions.errors.actionNotFound"),
      "error",
    );
    return;
  }

  try {
    if (quickAction === "gas-ventilate") {
      await sendDoorOpenForGas(targetAction.houseNumber);
      return;
    }

    if (quickAction === "fire-stop-heating") {
      await sendHeatingOffForFire(targetAction.houseNumber);
      return;
    }

    openFireDisplayDialog(targetAction.houseNumber);
  } catch (error) {
    const message =
      error instanceof Error
        ? error.message
        : t("dashboard.actions.errors.commandFailed");
    showQuickActionFeedback(message, "error");
  }
};
</script>

<style scoped>
.dashboard-page {
  min-height: calc(100vh - 64px);
}

.dashboard-content {
  padding: 2rem 1.5rem;
  margin-left: 260px;
  margin-right: 300px;
  width: calc(100vw - 560px);
  min-height: calc(100vh - 64px);
}

@media (max-width: 1280px) {
  .dashboard-content {
    width: auto;
    padding: 1rem;
    margin-left: 0;
    margin-right: 0;
  }
}
</style>
