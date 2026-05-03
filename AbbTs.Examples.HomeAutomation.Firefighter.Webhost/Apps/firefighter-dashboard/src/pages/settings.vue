<template>
  <v-container class="py-8 mt-8" fluid>
    <v-row class="mb-4" justify="space-between" align="center">
      <v-col cols="12" md="8">
        <div class="text-overline text-primary">
          {{ t("settings.badge") }}
        </div>
        <h1 class="text-h4 font-weight-bold mb-2">
          {{ t("settings.title") }}
        </h1>
        <p class="text-body-1 text-medium-emphasis mb-0">
          {{ t("settings.subtitle") }}
        </p>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" lg="8" xl="6">
        <v-card rounded="xl">
          <v-card-title>
            {{ t("settings.threshold.title") }}
          </v-card-title>
          <v-card-text>
            <p class="text-body-2 text-medium-emphasis mb-4">
              {{ t("settings.threshold.description") }}
            </p>

            <v-text-field
              v-model.number="neighborThreshold"
              :label="t('settings.threshold.label')"
              class="mb-3"
              density="comfortable"
              min="1"
              type="number"
              variant="outlined"
            />

            <v-slider
              v-model="neighborThreshold"
              :label="t('settings.threshold.sliderLabel')"
              :max="100"
              :min="1"
              color="primary"
              step="1"
              thumb-label="always"
            />

            <div class="text-caption text-medium-emphasis mt-2">
              {{
                t("settings.threshold.current", {
                  value: normalizedNeighborFireDistanceThreshold,
                })
              }}
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import { storeToRefs } from "pinia";
import { useI18n } from "vue-i18n";

import { useAppStore } from "@/stores/app";

const { t } = useI18n({ useScope: "global" });
const appStore = useAppStore();
const { normalizedNeighborFireDistanceThreshold } = storeToRefs(appStore);

const neighborThreshold = computed({
  get: () => normalizedNeighborFireDistanceThreshold.value,
  set: (value: number) => appStore.setNeighborFireDistanceThreshold(value),
});
</script>
