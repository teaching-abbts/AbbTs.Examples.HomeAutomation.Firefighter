import { computed, watch } from "vue";
import { defineStore } from "pinia";
import { useI18n } from "vue-i18n";
import { useStorage } from "@vueuse/core";
import { useTheme } from "vuetify";

export const useAppStore = defineStore("app", () => {
  const theme = useTheme();
  const i18n = useI18n({ useScope: "global" });

  const isDarkTheme = useStorage("isDarkTheme", theme.current.value.dark);
  const selectedLocale = useStorage("selectedLocale", i18n.locale.value);

  watch(
    isDarkTheme,
    (isDark) => {
      const nextTheme = isDark ? "dark" : "light";
      theme.change(nextTheme);
    },
    {
      immediate: true,
    },
  );

  watch(
    theme.current,
    (newTheme) => {
      isDarkTheme.value = newTheme.dark;
    },
    {
      immediate: true,
    },
  );

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
