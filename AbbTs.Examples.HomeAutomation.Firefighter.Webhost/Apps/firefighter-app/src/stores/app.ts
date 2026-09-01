import { computed } from "vue";
import { defineStore } from "pinia";
import { useI18n } from "vue-i18n";
import { useTheme } from "vuetify";

export const useAppStore = defineStore("app", () => {
  const { locale, t, availableLocales } = useI18n({ useScope: "global" });
  const theme = useTheme();

  const isDarkTheme = computed({
    get: () => theme.current.value.dark,
    set: (isDark) => {
      const nextTheme = isDark ? "dark" : "light";
      theme.change(nextTheme);
    },
  });

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

  return {
    selectedLocale,
    languageOptions,
    isDarkTheme,
  };
});
