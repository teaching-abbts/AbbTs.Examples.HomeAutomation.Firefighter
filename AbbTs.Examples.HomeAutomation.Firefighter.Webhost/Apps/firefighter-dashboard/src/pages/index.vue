<template>
  <v-layout class="dashboard-layout">
    <v-app-bar class="px-6" color="surface" elevation="0">
      <v-btn
        icon="mdi-dock-left"
        variant="text"
        :title="
          showEventsSidebar ? 'Hide events sidebar' : 'Show events sidebar'
        "
        @click="showEventsSidebar = !showEventsSidebar"
      />
      <v-app-bar-title class="d-flex align-center ga-2 text-h6">
        <span>{{ t("dashboard.title") }}</span>
        <v-progress-circular v-if="loading" color="primary" indeterminate />
        <span v-if="loading" class="text-caption">{{
          t("dashboard.loading")
        }}</span>
      </v-app-bar-title>
      <LanguageSwitcher class="mr-4" />
      <ThemeSwitcher />
      <v-btn
        class="ml-2"
        icon="mdi-dock-right"
        variant="text"
        :title="
          showActionsSidebar ? 'Hide actions sidebar' : 'Show actions sidebar'
        "
        @click="showActionsSidebar = !showActionsSidebar"
      />
    </v-app-bar>
    <EventsSidebar v-if="showEventsSidebar" :events="sidebarEvents" />
    <v-main class="bg-surface">
      <HousesGrid :houses="houses" @select-house="openHouseDetails" />
      <HouseDetailsDialog />
    </v-main>
    <ActionsSidebar
      v-if="showActionsSidebar"
      :actions="actions"
      :observed-houses="observedHouses"
      @toggle-action="toggleAction"
      @toggle-observed-house="toggleObservedHouse"
    />
  </v-layout>
</template>

<script lang="ts" setup>
import { computed, onMounted, onUnmounted, ref } from "vue";
import { storeToRefs } from "pinia";
import { useI18n } from "vue-i18n";

import ActionsSidebar from "@/components/dashboard/ActionsSidebar.vue";
import EventsSidebar from "@/components/dashboard/EventsSidebar.vue";
import HouseDetailsDialog from "@/components/dashboard/HouseDetailsDialog.vue";
import HousesGrid from "@/components/dashboard/HousesGrid.vue";
import LanguageSwitcher from "@/components/dashboard/LanguageSwitcher.vue";
import ThemeSwitcher from "@/components/dashboard/ThemeSwitcher.vue";
import { useHouseDetailsStore } from "@/stores/houseDetails";

import type { HouseItem } from "@/components/dashboard/types";

const houseDetailsStore = useHouseDetailsStore();
const { t } = useI18n({ useScope: "global" });
const {
  availableHouseNumbers,
  latestEventTypeByHouse,
  observedHouses,
  sidebarEvents,
  actions,
  loading,
} = storeToRefs(houseDetailsStore);
const showEventsSidebar = ref(true);
const showActionsSidebar = ref(true);

const houses = computed<HouseItem[]>(() => {
  return availableHouseNumbers.value.map((houseNumber) => {
    const latestType = latestEventTypeByHouse.value[houseNumber];

    if (latestType === "fire") {
      return {
        id: houseNumber,
        number: houseNumber,
        statusKey: "dashboard.houseStatus.fireDetected",
        statusIcon: "mdi-fire",
        color: "#ed8936",
        textColor: "#111111",
      };
    }

    if (latestType === "gas") {
      return {
        id: houseNumber,
        number: houseNumber,
        statusKey: "dashboard.houseStatus.gasAlert",
        statusIcon: "mdi-alert",
        color: "#facc15",
        textColor: "#111111",
      };
    }

    return {
      id: houseNumber,
      number: houseNumber,
      statusKey: "dashboard.houseStatus.monitoring",
      statusIcon: "mdi-eye-outline",
      color: "#4169b3",
      textColor: "#ffffff",
    };
  });
});

let intervalId = null as number | null;

onMounted(async () => {
  await houseDetailsStore.fetchHistory();

  intervalId = setInterval(
    async () => await houseDetailsStore.fetchHistory(),
    5000,
  );
});

onUnmounted(() => {
  if (intervalId) {
    clearInterval(intervalId);
  }
});

const openHouseDetails = (houseNumber: number) => {
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
.dashboard-layout {
  min-height: 100vh;
}
</style>
