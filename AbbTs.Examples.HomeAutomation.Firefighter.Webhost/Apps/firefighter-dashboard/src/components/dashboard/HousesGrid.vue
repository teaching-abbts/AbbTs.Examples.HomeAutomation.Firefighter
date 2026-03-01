<template>
  <v-container class="py-8" fluid>
    <v-row>
      <v-col v-for="house in houses" :key="house.id" cols="12" md="4" sm="6">
        <v-card
          class="house-card"
          rounded="lg"
          :style="{ backgroundColor: house.color }"
          variant="flat"
        >
          <v-card-item :style="{ color: house.textColor }">
            <div class="d-flex align-center justify-space-between">
              <v-icon icon="mdi-home-city-outline" size="44" />
              <v-icon :icon="house.statusIcon" size="28" />
            </div>
            <v-card-title class="pt-4 text-h4">{{
              t("dashboard.houseName", { id: house.number })
            }}</v-card-title>
            <v-card-subtitle :style="{ color: house.textColor, opacity: 1 }">{{
              t(house.statusKey)
            }}</v-card-subtitle>
          </v-card-item>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts" setup>
import { useI18n } from "vue-i18n";

import type { HouseItem } from "./types";

defineProps<{
  houses: HouseItem[];
}>();

const { t } = useI18n();
</script>

<style scoped>
.house-card {
  min-height: 180px;
  border: 2px solid rgba(var(--v-theme-on-surface), 0.35);
}
</style>
