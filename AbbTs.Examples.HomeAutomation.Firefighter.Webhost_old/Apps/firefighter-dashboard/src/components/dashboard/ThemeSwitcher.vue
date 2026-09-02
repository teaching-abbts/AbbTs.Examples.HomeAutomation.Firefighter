<template>
  <div class="mr-2">
    <v-switch
      v-model="isDarkTheme"
      true-icon="mdi-moon-waning-crescent"
      false-icon="mdi-white-balance-sunny"
      density="comfortable"
      hide-details
    />
  </div>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import { useTheme } from "vuetify";

import { useAppStore } from "@/stores/app";

const theme = useTheme();
const appStore = useAppStore();

const isDarkTheme = computed({
  get: () => theme.current.value.dark,
  set: (v) => {
    const nextTheme = v ? "dark" : "light";
    theme.global.name.value = nextTheme;
    appStore.setTheme(nextTheme);
  },
});
</script>
