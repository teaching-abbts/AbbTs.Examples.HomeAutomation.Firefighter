<template>
  <v-select
    :items="appStore.languageOptions"
    :label="t('app.language-selector.label')"
    class="language-switcher"
    density="compact"
    hide-details
    item-title="title"
    item-value="value"
    prepend-inner-icon="mdi-translate"
    v-model="appStore.selectedLocale"
    variant="outlined"
  />
</template>

<script lang="ts" setup>
import { useAppStore } from "@/stores/appStore";
import { useI18n } from "vue-i18n";

const appStore = useAppStore();
const { t } = useI18n({ useScope: "global" });
</script>

<style scoped>
.language-switcher {
  max-width: 150px;
}
</style>
