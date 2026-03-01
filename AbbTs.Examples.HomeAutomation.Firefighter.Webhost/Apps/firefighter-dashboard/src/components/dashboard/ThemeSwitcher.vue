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
import { useI18n } from "vue-i18n";
import { useTheme } from "vuetify";

const { t } = useI18n({ useScope: "global" });
const theme = useTheme();

const isDarkTheme = computed({
  get: () => theme.current.value.dark,
  set: (v) => (theme.global.name.value = v ? "dark" : "light"),
});
</script>
