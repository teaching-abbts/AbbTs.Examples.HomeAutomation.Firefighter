<template>
  <v-navigation-drawer v-model="modelValue" :location="props.location">
    <v-list-item :title="t('app.actions.title')" />
    <v-divider />
    <v-list-item
      title="Brandlöschung"
      subtitle="house-1"
      base-color="error"
      selected
      active
    >
      <v-checkbox hide-details density="compact" label="Heizung ausschalten" />
      <v-checkbox hide-details density="compact" label="Brand gelöscht" />
    </v-list-item>
    <v-list-item title="Lüften!" subtitle="house-1" active color="warning">
      <v-checkbox hide-details density="compact" label="Türe öffnen!" />
    </v-list-item>
    <v-list-item title="Beobachten" subtitle="house-1" active color="info">
      <v-checkbox hide-details density="compact" label="Situation beobachten" />
    </v-list-item>
  </v-navigation-drawer>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";
const { t } = useI18n();
export interface AppDrawerActionsModels {
  modelValue: boolean | null;
}
export interface AppDrawerActionsProps {
  location: "left" | "right";
}

const modelValue = defineModel<AppDrawerActionsModels["modelValue"]>();
const props = defineProps<AppDrawerActionsProps>();
</script>
