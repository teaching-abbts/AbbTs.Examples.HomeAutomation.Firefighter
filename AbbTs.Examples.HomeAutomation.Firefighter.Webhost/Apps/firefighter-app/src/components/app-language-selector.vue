<template>
  <v-select
    :items="languageOptions"
    :label="t('app.language-selector.label')"
    class="language-switcher"
    density="compact"
    hide-details
    item-title="title"
    item-value="value"
    prepend-inner-icon="mdi-translate"
    v-model="selectedLocale"
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
