<template>
  <v-list-item
    :base-color="props.baseColor"
    :subtitle="props.subtitle"
    :title="props.title"
    link
  >
    <template #prepend>
      <v-icon>{{ props.icon }}</v-icon>
    </template>
  </v-list-item>
</template>

<script setup lang="ts">
interface EventListItemProps {
  title: string;
  subtitle?: string;
  baseColor: string;
  icon: string;
}
const props = defineProps<EventListItemProps>();
</script>
