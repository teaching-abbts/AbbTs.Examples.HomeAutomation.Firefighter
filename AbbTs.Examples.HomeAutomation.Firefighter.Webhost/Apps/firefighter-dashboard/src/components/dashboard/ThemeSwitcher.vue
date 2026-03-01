<template>
  <v-select
    v-model="selectedTheme"
    :items="themeOptions"
    class="theme-switcher"
    density="compact"
    hide-details
    item-title="title"
    item-value="value"
    :label="t('dashboard.theme.label')"
    prepend-inner-icon="mdi-theme-light-dark"
    variant="outlined"
  />
</template>

<script lang="ts" setup>
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import { useTheme } from "vuetify";

const { t } = useI18n({ useScope: "global" });
const theme = useTheme();

const selectedTheme = computed({
  get: () => (theme.global.current.value.dark ? "dark" : "light"),
  set: (value: string) => {
    theme.global.name.value = value;
  },
});

const themeOptions = computed(() => [
  { title: t("dashboard.theme.bright"), value: "light" },
  { title: t("dashboard.theme.dark"), value: "dark" },
]);
</script>

<style scoped>
.theme-switcher {
  max-width: 180px;
}
</style>
