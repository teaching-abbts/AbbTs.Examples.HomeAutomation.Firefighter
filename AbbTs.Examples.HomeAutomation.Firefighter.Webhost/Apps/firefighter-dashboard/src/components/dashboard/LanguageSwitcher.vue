<template>
  <v-select
    v-model="selectedLocale"
    :items="languageOptions"
    class="language-switcher"
    density="compact"
    hide-details
    item-title="title"
    item-value="value"
    :label="t('dashboard.language.label')"
    prepend-inner-icon="mdi-translate"
    variant="outlined"
  />
</template>

<script lang="ts" setup>
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import { useAppStore, type AppLocale } from "@/stores/app";

const { locale, t } = useI18n({ useScope: "global" });
const appStore = useAppStore();

const selectedLocale = computed({
  get: () => locale.value,
  set: (value: string) => {
    locale.value = value;
    appStore.setLocale(value as AppLocale);
  },
});

const languageOptions = computed(() => [
  { title: t("dashboard.language.german"), value: "de" },
  { title: t("dashboard.language.english"), value: "en" },
  { title: t("dashboard.language.japanese"), value: "jp" },
]);
</script>

<style scoped>
.language-switcher {
  max-width: 220px;
}
</style>
