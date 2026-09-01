<template>
  <v-separator />
  <v-navigation-drawer v-model="modelValue" :location="props.location">
    <v-list-item :title="t('app.actions.title')" />
    <v-divider />
    <v-list-item link title="List Item 1"></v-list-item>
    <v-list-item link title="List Item 2"></v-list-item>
    <v-list-item link title="List Item 3"></v-list-item>
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
