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
      <v-switch
        v-model="showOnlyOpenActions"
        class="mb-3"
        color="primary"
        density="compact"
        hide-details
        :label="t('dashboard.actions.onlyOpen')"
      />

      <v-card
        v-for="action in filteredActions"
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
          <v-chip
            v-if="!showOnlyOpenActions"
            class="mt-2"
            :color="action.state === 'done' ? 'success' : 'secondary'"
            size="small"
            variant="flat"
          >
            {{
              action.state === "done"
                ? t("dashboard.actions.done")
                : t("dashboard.actions.open")
            }}
          </v-chip>
        </v-card-item>

        <v-card-actions>
          <v-btn
            block
            rounded="lg"
            :style="{ backgroundColor: '#b0b4b8', color: '#ffffff' }"
            variant="elevated"
            @click="emit('toggle-action', action.actionKey)"
            >{{
              showOnlyOpenActions
                ? t("dashboard.actions.markDone")
                : action.state === "done"
                  ? t("dashboard.actions.reopen")
                  : t("dashboard.actions.markDone")
            }}</v-btn
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
              <v-checkbox-btn
                :model-value="house.active"
                color="success"
                hide-details
                @update:model-value="
                  emit('toggle-observed-house', {
                    houseNumber: house.number,
                    observed: Boolean($event),
                  })
                "
              />
            </template>
          </v-list-item>
        </v-list>
      </v-card>
    </div>
  </v-navigation-drawer>
</template>

<script lang="ts" setup>
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import type { ActionItem, ObservedHouseItem } from "./types";

const props = defineProps<{
  actions: ActionItem[];
  observedHouses: ObservedHouseItem[];
}>();

const emit = defineEmits<{
  "toggle-action": [actionKey: string];
  "toggle-observed-house": [
    payload: { houseNumber: number; observed: boolean },
  ];
}>();

const { t } = useI18n();
const showOnlyOpenActions = ref(false);

const filteredActions = computed(() => {
  if (!showOnlyOpenActions.value) {
    return props.actions;
  }

  return props.actions.filter((action) => action.state === "open");
});
</script>

<style scoped>
.action-card {
  border-top-right-radius: 28px !important;
}
</style>
