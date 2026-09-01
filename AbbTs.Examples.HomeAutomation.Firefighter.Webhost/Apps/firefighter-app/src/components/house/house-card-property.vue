<template>
  <!-- <v-list-item :prepend-icon="props.icon" density="compact">
    <v-list-item-title>{{ props.title }}</v-list-item-title>
  </v-list-item> -->
  <v-tooltip interactive location="bottom">
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
        Learn more
      </router-link>
      about {{ props.tooltipText }}.
    </div>
  </v-tooltip>
</template>

<script lang="ts" setup>
export interface HouseCardPropertyProps {
  title: string;
  icon: string;
  tooltipTo: string;
  tooltipText: string;
}

const props = defineProps<HouseCardPropertyProps>();
</script>
