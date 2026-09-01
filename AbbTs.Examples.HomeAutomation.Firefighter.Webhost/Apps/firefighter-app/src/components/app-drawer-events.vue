<template>
  <v-navigation-drawer v-model="modelValue" :location="props.location">
    <v-list-item :title="t('app.events.title')" />
    <v-divider />
    <event-list-item
      title="Gefährdung"
      subtitle="house-2"
      base-color="info"
      icon="mdi-information"
    />
    <event-list-item
      title="Gasalarm"
      subtitle="house-1"
      base-color="warning"
      icon="mdi-gas-cylinder"
    />
    <event-list-item
      title="Feueralarm"
      subtitle="house-1"
      base-color="error"
      icon="mdi-fire-alert"
    />
  </v-navigation-drawer>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";
import EventListItem from "@/components/events/event-list-item.vue";

const { t } = useI18n();
export interface AppDrawerEventsModels {
  modelValue: boolean | null;
}
export interface AppDrawerEventsProps {
  location: "left" | "right";
}

const modelValue = defineModel<AppDrawerEventsModels["modelValue"]>();
const props = defineProps<AppDrawerEventsProps>();
</script>
