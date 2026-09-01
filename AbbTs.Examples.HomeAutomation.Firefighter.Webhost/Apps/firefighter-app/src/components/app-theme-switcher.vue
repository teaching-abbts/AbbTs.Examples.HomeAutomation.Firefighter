<template>
  <div class="mr-2">
    <v-switch
      v-model="appStore.isDarkTheme"
      true-icon="mdi-moon-waning-crescent"
      false-icon="mdi-white-balance-sunny"
      density="comfortable"
      hide-details
    />
  </div>
</template>

<script lang="ts" setup>
import { useAppStore } from "@/stores/app";

const appStore = useAppStore();
</script>
