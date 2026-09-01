<template>
  <v-select
    v-model="selectedLocale"
    :items="languageOptions"
    class="language-switcher"
    density="compact"
    hide-details
    item-title="title"
    item-value="value"
    :label="t('app.language-selector.label')"
    prepend-inner-icon="mdi-translate"
    variant="outlined"
  />
</template>

<script lang="ts" setup>
import { computed } from "vue";
import { useI18n } from "vue-i18n";

const { locale, t, availableLocales } = useI18n({ useScope: "global" });

const selectedLocale = computed({
  get: () => locale.value,
  set: (value: string) => {
    locale.value = value;
  },
});

const languageOptions = computed(() =>
  availableLocales.map((locale) => ({
    title: t(`language.${locale}`),
    value: locale,
  })),
);
</script>

<style scoped>
.language-switcher {
  max-width: 150px;
}
</style>
