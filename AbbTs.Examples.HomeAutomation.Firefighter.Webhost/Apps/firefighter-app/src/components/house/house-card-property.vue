<template>
  <v-tooltip interactive location="top">
    <template #activator="activatorProps">
      <v-list-item
        :prepend-icon="props.icon"
        density="compact"
        v-bind="activatorProps.props"
      >
        <v-list-item-title>{{ props.title }}</v-list-item-title>
      </v-list-item>
    </template>
    <div>
      <router-link
        class="text-primary font-weight-medium"
        :to="props.tooltipTo"
      >
        {{ t("house.property.learn-more") }}
      </router-link>
      {{ t("house.property.about", { property: props.tooltipText }) }}
    </div>
  </v-tooltip>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

export interface HouseCardPropertyProps {
  title: string;
  icon: string;
  tooltipTo: string;
  tooltipText: string;
}

const props = defineProps<HouseCardPropertyProps>();
const { t } = useI18n({ useScope: "global" });
</script>
