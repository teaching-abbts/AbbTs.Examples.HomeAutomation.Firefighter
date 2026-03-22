<template>
  <v-main class="h-full">
    <EventsSidebar :events="sidebarEvents" />
    <HousesGrid :houses="houses" @select-house="openHouseDetails" />
    <HouseDetailsDialog />
    <ActionsSidebar
      :actions="actions"
      :observed-houses="observedHouses"
      @toggle-action="toggleAction"
      @toggle-observed-house="toggleObservedHouse"
    />
  </v-main>
</template>

<script lang="ts" setup>
import ActionsSidebar from "@/components/dashboard/ActionsSidebar.vue";
import EventsSidebar from "@/components/dashboard/EventsSidebar.vue";
import HouseDetailsDialog from "@/components/dashboard/HouseDetailsDialog.vue";
import HousesGrid from "@/components/dashboard/HousesGrid.vue";
import type { HouseItem } from "@/components/dashboard/types";
import { computed, onMounted, onUnmounted, ref } from "vue";
import { storeToRefs } from "pinia";
import { useHouseDetailsStore } from "@/stores/houseDetails";

const houseDetailsStore = useHouseDetailsStore();
const {
  availableHouseNumbers,
  latestEventTypeByHouse,
  observedHouses,
  sidebarEvents,
  actions,
} = storeToRefs(houseDetailsStore);

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

onMounted(async () => {
  await houseDetailsStore.fetchHistory();
  await houseDetailsStore.startLiveUpdates();
});

onUnmounted(async () => {
  await houseDetailsStore.stopLiveUpdates();
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
