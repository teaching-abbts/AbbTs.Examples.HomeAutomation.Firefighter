<template>
  <v-layout class="dashboard-layout">
    <v-app-bar class="px-6" color="surface" elevation="0">
      <v-spacer />
      <LanguageSwitcher class="mr-4" />
      <ThemeSwitcher />
    </v-app-bar>
    <EventsSidebar :events="events" />
    <v-main class="bg-surface">
      <HousesGrid :houses="houses" @select-house="openHouseDetails" />
      <HouseDetailsDialog />
    </v-main>
    <ActionsSidebar :actions="actions" :observed-houses="observedHouses" />
  </v-layout>
</template>

<script lang="ts" setup>
import { computed, onMounted } from "vue";
import { storeToRefs } from "pinia";

import ActionsSidebar from "@/components/dashboard/ActionsSidebar.vue";
import EventsSidebar from "@/components/dashboard/EventsSidebar.vue";
import HouseDetailsDialog from "@/components/dashboard/HouseDetailsDialog.vue";
import HousesGrid from "@/components/dashboard/HousesGrid.vue";
import LanguageSwitcher from "@/components/dashboard/LanguageSwitcher.vue";
import ThemeSwitcher from "@/components/dashboard/ThemeSwitcher.vue";
import { useHouseDetailsStore } from "@/stores/houseDetails";

import type {
  ActionItem,
  EventItem,
  HouseItem,
  ObservedHouseItem,
} from "@/components/dashboard/types";

const events: EventItem[] = [
  {
    id: 1,
    titleKey: "dashboard.events.fire",
    houseNumber: 1,
    time: "17.11.2033 14:34",
    icon: "mdi-fire",
    color: "#ed8936",
    textColor: "#111111",
  },
  {
    id: 2,
    titleKey: "dashboard.events.gas",
    houseNumber: 5,
    time: "17.11.2033 14:31",
    icon: "mdi-alert",
    color: "#facc15",
    textColor: "#111111",
  },
];

const actions: ActionItem[] = [
  {
    id: 1,
    titleKey: "dashboard.actions.extinguishFire",
    houseNumber: 1,
    color: "#f07f2f",
    textColor: "#ffffff",
  },
  {
    id: 2,
    titleKey: "dashboard.actions.openDoors",
    houseNumber: 5,
    color: "#facc15",
    textColor: "#111111",
  },
];

const observedHouses: ObservedHouseItem[] = [
  { id: 2, number: 2, active: false },
  { id: 4, number: 4, active: false },
  { id: 6, number: 6, active: false },
  { id: 3, number: 3, active: true },
];

const houseDetailsStore = useHouseDetailsStore();
const { availableHouseNumbers, latestEventTypeByHouse } =
  storeToRefs(houseDetailsStore);

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
  await houseDetailsStore.fetchHistoryIfNeeded();
});

const openHouseDetails = (houseNumber: number) => {
  houseDetailsStore.open(houseNumber);
};
</script>

<style scoped>
.dashboard-layout {
  min-height: 100vh;
}
</style>
