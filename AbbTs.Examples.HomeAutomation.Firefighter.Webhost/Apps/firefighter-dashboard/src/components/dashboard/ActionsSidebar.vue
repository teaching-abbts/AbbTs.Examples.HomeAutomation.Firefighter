<template>
  <v-navigation-drawer
    :style="{ backgroundColor: '#8fb1d5' }"
    location="right"
    permanent
    width="300"
  >
    <div class="px-4 py-4 text-h5 font-weight-bold">
      {{ t("dashboard.sections.actions") }}
    </div>

    <div class="px-3 pb-3">
      <v-card
        v-for="action in actions"
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
        </v-card-item>

        <v-card-actions>
          <v-btn
            block
            rounded="lg"
            :style="{ backgroundColor: '#b0b4b8', color: '#ffffff' }"
            variant="elevated"
            >{{ t("dashboard.actions.execute") }}</v-btn
          >
        </v-card-actions>
      </v-card>

      <v-card
        rounded="lg"
        :style="{ backgroundColor: '#5aa1dc' }"
        variant="flat"
      >
        <v-card-title :style="{ color: '#ffffff' }">{{
          t("dashboard.actions.observe")
        }}</v-card-title>
        <v-list class="pt-0" density="comfortable">
          <v-list-item
            v-for="house in observedHouses"
            :key="house.id"
            class="mb-2 rounded-lg"
            :style="{ backgroundColor: '#b0b4b8', color: '#ffffff' }"
            :title="t('dashboard.houseName', { id: house.number })"
          >
            <template #append>
              <v-icon
                v-if="house.active"
                color="success"
                icon="mdi-check-bold"
              />
            </template>
          </v-list-item>
        </v-list>
      </v-card>
    </div>
  </v-navigation-drawer>
</template>

<script lang="ts" setup>
import { useI18n } from "vue-i18n";

import type { ActionItem, ObservedHouseItem } from "./types";

defineProps<{
  actions: ActionItem[];
  observedHouses: ObservedHouseItem[];
}>();

const { t } = useI18n();
</script>

<style scoped>
.action-card {
  border-top-right-radius: 28px !important;
}
</style>
