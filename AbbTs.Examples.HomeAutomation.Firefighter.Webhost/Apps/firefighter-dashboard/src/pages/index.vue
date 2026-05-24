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
    <ActionsSidebar
      :actions="actions"
      @toggle-action="toggleAction"
      @quick-action="handleQuickAction"
    />
  </div>
</template>

<script lang="ts" setup>
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { Client } from "@/api/AbbTs.Examples.HomeAutomation.Firefighter.Webhost";
import ActionsSidebar from "@/components/dashboard/ActionsSidebar.vue";
import type { QuickActionPayload } from "@/components/dashboard/ActionsSidebar.vue";
import EventsSidebar from "@/components/dashboard/EventsSidebar.vue";
import SmartHomeCard from "@/components/smart-homes/SmartHomeCard.vue";
import SmartHomesLandscape from "@/components/smart-homes/SmartHomesLandscape.vue";
import { computed, onMounted, onUnmounted, ref } from "vue";
import { useRouter } from "vue-router";
import { useAppStore } from "@/stores/app";
import { useHouseDetailsStore } from "@/stores/houseDetails";
import type { SmartHomeCommand, SmartHomeSummary } from "@/types/smartHomes";
import { useI18n } from "vue-i18n";

const { t } = useI18n({ useScope: "global" });
const router = useRouter();
const appStore = useAppStore();
const houseDetailsStore = useHouseDetailsStore();
const sidebarEvents = computed(() => houseDetailsStore.sidebarEvents);
const actions = computed(() => houseDetailsStore.actions);

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

const parseHouseNumberFromId = (id: string | undefined): number | null => {
  if (!id) {
    return null;
  }

  const match = /\d+/.exec(id);
  return match ? Number(match[0]) : null;
};

const findSmartHomeByHouseNumber = (houseNumber: number) => {
  return smartHomes.value.find(
    (home) => parseHouseNumberFromId(home.id) === houseNumber,
  );
};

const ensureSmartHomesConnection = async (): Promise<boolean> => {
  if (smartHomesConnection.state === "Connected") {
    return true;
  }

  try {
    await smartHomesConnection.start();
    return true;
  } catch {
    smartHomesError.value ||= t("smartHomes.liveUnavailable");
    return false;
  }
};

const sendSmartHomeCommand = async (
  smartHomeId: string,
  command: SmartHomeCommand,
) => {
  const ready = await ensureSmartHomesConnection();
  if (!ready) {
    return;
  }

  try {
    await smartHomesConnection.invoke("SendCommand", smartHomeId, command);
  } catch {
    smartHomesError.value = t("smartHomes.commandError");
  }
};

const getSmartHomesWithinFireDistance = (originHouseNumber: number) => {
  const origin = findSmartHomeByHouseNumber(originHouseNumber);
  if (!origin) {
    return [] as SmartHomeSummary[];
  }

  const threshold = appStore.normalizedNeighborFireDistanceThreshold;
  const originX = origin.xCoordinate ?? 0;
  const originY = origin.yCoordinate ?? 0;

  return smartHomes.value.filter((home) => {
    const dx = (home.xCoordinate ?? 0) - originX;
    const dy = (home.yCoordinate ?? 0) - originY;
    return Math.hypot(dx, dy) <= threshold;
  });
};

const handleQuickAction = async (payload: QuickActionPayload) => {
  const { id, action, message } = payload;

  if (id === "open-doors") {
    const target = findSmartHomeByHouseNumber(action.houseNumber);
    if (!target) {
      return;
    }

    await sendSmartHomeCommand(target.id, {
      device: "Door",
      command: "open",
      value: "",
    });
    return;
  }

  if (id === "heating-off") {
    const target = findSmartHomeByHouseNumber(action.houseNumber);
    if (!target) {
      return;
    }

    await sendSmartHomeCommand(target.id, {
      device: "HeatingControl",
      command: "off",
      value: "",
    });
    return;
  }

  if (id === "fire-alarm-broadcast") {
    const targets = getSmartHomesWithinFireDistance(action.houseNumber);
    const displayValue = (message ?? "Feueralarm\nFeuerwehr unterwegs!")
      .split(/\r?\n/)
      .map((line) => line.trim())
      .filter((line) => line.length > 0)
      .join(";");

    await Promise.all(
      targets.map((target) =>
        sendSmartHomeCommand(target.id, {
          device: "Display",
          command: "set",
          value: displayValue,
        }),
      ),
    );
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
