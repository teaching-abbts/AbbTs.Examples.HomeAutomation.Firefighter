<template>
  <div class="dashboard-page mt-10">
    <EventsSidebar :events="sidebarEvents" />
    <div class="dashboard-content">
      <v-alert
        v-if="smartHomesError"
        class="mb-4"
        type="warning"
        variant="tonal"
      >
        {{ smartHomesError }}
      </v-alert>

      <v-row v-if="smartHomes.length > 0" class="mb-2">
        <v-col cols="12">
          <SmartHomesLandscape
            :homes="smartHomes"
            :title="t('smartHomes.landscapeTitle')"
            @select="openSmartHomeDetails"
          />
        </v-col>
      </v-row>

      <v-row v-if="smartHomes.length > 0">
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
    </div>
    <HouseDetailsDialog />
    <ActionsSidebar
      :actions="actions"
      :observed-houses="observedHouses"
      @toggle-action="toggleAction"
      @toggle-observed-house="toggleObservedHouse"
    />
  </div>
</template>

<script lang="ts" setup>
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { Client } from "@/api/AbbTs.Examples.HomeAutomation.Firefighter.Webhost";
import ActionsSidebar from "@/components/dashboard/ActionsSidebar.vue";
import EventsSidebar from "@/components/dashboard/EventsSidebar.vue";
import HouseDetailsDialog from "@/components/dashboard/HouseDetailsDialog.vue";
import SmartHomeCard from "@/components/smart-homes/SmartHomeCard.vue";
import SmartHomesLandscape from "@/components/smart-homes/SmartHomesLandscape.vue";
import { onMounted, onUnmounted, ref } from "vue";
import { storeToRefs } from "pinia";
import { useHouseDetailsStore } from "@/stores/houseDetails";
import type { SmartHomeSummary } from "@/types/smartHomes";
import { useI18n } from "vue-i18n";

const { t } = useI18n({ useScope: "global" });
const houseDetailsStore = useHouseDetailsStore();
const { observedHouses, sidebarEvents, actions } =
  storeToRefs(houseDetailsStore);

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
    smartHomesError.value = "";
  } catch {
    smartHomesError.value = t("smartHomes.loadError");
  }
};

const extractHouseNumber = (smartHomeId: string): number | null => {
  const match = smartHomeId.match(/\d+/);
  if (!match) {
    return null;
  }

  return Number.parseInt(match[0], 10);
};

onMounted(async () => {
  await houseDetailsStore.fetchHistory();
  await houseDetailsStore.startLiveUpdates();

  await loadSmartHomes();

  smartHomesConnection.on(
    "smartHomesChanged",
    (payload: SmartHomeSummary[]) => {
      smartHomes.value = mapSmartHomes(payload);
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
  const houseNumber = extractHouseNumber(smartHomeId);
  if (houseNumber === null || Number.isNaN(houseNumber)) {
    return;
  }

  houseDetailsStore.open(houseNumber);
};

const toggleAction = (actionKey: string) => {
  houseDetailsStore.toggleActionState(actionKey);
};

const toggleObservedHouse = (payload: {
  houseNumber: number;
  observed: boolean;
}) => {
  houseDetailsStore.setHouseObserved(payload.houseNumber, payload.observed);
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
