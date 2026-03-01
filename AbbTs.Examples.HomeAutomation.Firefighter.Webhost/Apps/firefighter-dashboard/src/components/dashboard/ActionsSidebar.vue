<template>
  <v-navigation-drawer location="right" permanent width="300">
    <div class="px-4 py-4 text-h5 font-weight-bold">
      {{ t("dashboard.sections.actions") }}
    </div>

    <div class="px-3 pb-3">
      <v-card
        v-for="action in actions"
        :key="action.id"
        :color="action.color"
        class="mb-4"
        rounded="lg"
        variant="flat"
      >
        <v-card-item>
          <v-card-title class="text-h6">{{ t(action.titleKey) }}</v-card-title>
          <v-card-subtitle>{{
            t("dashboard.houseName", { id: action.houseNumber })
          }}</v-card-subtitle>
        </v-card-item>

        <v-card-actions>
          <v-btn block color="surface" variant="elevated">{{
            t("dashboard.actions.execute")
          }}</v-btn>
        </v-card-actions>
      </v-card>

      <v-card rounded="lg" variant="tonal">
        <v-card-title>{{ t("dashboard.actions.observe") }}</v-card-title>
        <v-list class="pt-0" density="comfortable">
          <v-list-item
            v-for="house in observedHouses"
            :key="house.id"
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
