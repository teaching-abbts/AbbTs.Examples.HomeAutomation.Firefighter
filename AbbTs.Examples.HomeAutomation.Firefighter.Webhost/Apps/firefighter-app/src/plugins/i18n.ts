import { createI18n } from "vue-i18n";
import de from "../i18n/de.json";
import en from "../i18n/en.json";

const messages = {
  en,
  de,
};

export default createI18n({
  legacy: false,
  locale: "en",
  fallbackLocale: "en",
  messages,
});
