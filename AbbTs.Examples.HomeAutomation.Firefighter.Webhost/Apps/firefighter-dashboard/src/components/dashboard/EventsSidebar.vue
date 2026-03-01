<template>
  <v-navigation-drawer color="blue-lighten-4" permanent width="260">
    <div class="px-4 py-4 text-h5 font-weight-bold">
      {{ t("dashboard.sections.events") }}
    </div>

    <v-list class="px-2">
      <v-list-item
        v-for="event in events"
        :key="event.id"
        class="mb-3 px-0"
        lines="three"
      >
        <v-card :color="event.color" class="w-100" rounded="xl" variant="flat">
          <v-card-item>
            <v-card-title
              :class="['d-flex align-center ga-2 pb-1', event.textColor]"
            >
              <v-icon :icon="event.icon" />
              {{ t(event.titleKey) }}
            </v-card-title>
            <v-card-subtitle :class="event.textColor">{{
              t("dashboard.houseName", { id: event.houseNumber })
            }}</v-card-subtitle>
            <div :class="['text-body-2 mt-1 font-italic', event.textColor]">
              {{ event.time }}
            </div>
          </v-card-item>
        </v-card>
      </v-list-item>
    </v-list>
  </v-navigation-drawer>
</template>

<script lang="ts" setup>
import { useI18n } from "vue-i18n";

import type { EventItem } from "./types";

defineProps<{
  events: EventItem[];
}>();

const { t } = useI18n();
</script>
