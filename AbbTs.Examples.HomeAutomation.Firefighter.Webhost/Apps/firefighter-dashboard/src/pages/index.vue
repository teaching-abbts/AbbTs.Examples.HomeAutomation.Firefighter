<template>
  <v-layout class="dashboard-layout">
    <EventsSidebar :events="events" />

    <v-main class="bg-surface">
      <div class="d-flex justify-end align-center ga-3 px-6 pt-4">
        <ThemeSwitcher />
        <LanguageSwitcher />
      </div>
      <HousesGrid :houses="houses" />
    </v-main>

    <ActionsSidebar :actions="actions" :observed-houses="observedHouses" />
  </v-layout>
</template>

<script lang="ts" setup>
import ActionsSidebar from "@/components/dashboard/ActionsSidebar.vue";
import EventsSidebar from "@/components/dashboard/EventsSidebar.vue";
import HousesGrid from "@/components/dashboard/HousesGrid.vue";
import LanguageSwitcher from "@/components/dashboard/LanguageSwitcher.vue";
import ThemeSwitcher from "@/components/dashboard/ThemeSwitcher.vue";

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

const houses: HouseItem[] = [
  {
    id: 1,
    number: 1,
    statusKey: "dashboard.houseStatus.fireDetected",
    statusIcon: "mdi-fire",
    color: "#ed8936",
    textColor: "#111111",
  },
  {
    id: 2,
    number: 2,
    statusKey: "dashboard.houseStatus.monitoring",
    statusIcon: "mdi-eye-outline",
    color: "#4169b3",
    textColor: "#ffffff",
  },
  {
    id: 3,
    number: 3,
    statusKey: "dashboard.houseStatus.safe",
    statusIcon: "mdi-check-circle-outline",
    color: "#6cab44",
    textColor: "#ffffff",
  },
  {
    id: 4,
    number: 4,
    statusKey: "dashboard.houseStatus.monitoring",
    statusIcon: "mdi-eye-outline",
    color: "#4169b3",
    textColor: "#ffffff",
  },
  {
    id: 5,
    number: 5,
    statusKey: "dashboard.houseStatus.gasAlert",
    statusIcon: "mdi-alert",
    color: "#facc15",
    textColor: "#111111",
  },
  {
    id: 6,
    number: 6,
    statusKey: "dashboard.houseStatus.monitoring",
    statusIcon: "mdi-eye-outline",
    color: "#4169b3",
    textColor: "#ffffff",
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
</script>

<style scoped>
.dashboard-layout {
  min-height: 100vh;
}
</style>
