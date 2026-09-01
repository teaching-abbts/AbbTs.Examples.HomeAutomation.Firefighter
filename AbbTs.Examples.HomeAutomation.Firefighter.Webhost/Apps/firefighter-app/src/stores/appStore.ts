import { computed } from "vue";
import { defineStore } from "pinia";
import { useI18n } from "vue-i18n";
import { useStorageBackedRef } from "@/composables/use-storage-backed-ref";
import { useTheme } from "vuetify";

export const useAppStore = defineStore("app", () => {
  const theme = useTheme();
  const i18n = useI18n({ useScope: "global" });

  const isDarkTheme = useStorageBackedRef("isDarkTheme", {
    get: () => theme.current.value.dark,
    set: (isDark) => theme.change(isDark ? "dark" : "light"),
  });

  const selectedLocale = useStorageBackedRef("selectedLocale", {
    get: () => i18n.locale.value,
    set: (locale) => {
      i18n.locale.value = locale;
    },
  });

  const languageOptions = computed(() =>
    i18n.availableLocales.map((locale) => ({
      title: i18n.t(`language.${locale}`),
      value: locale,
    })),
  );

  return {
    selectedLocale,
    languageOptions,
    isDarkTheme,
  };
});
