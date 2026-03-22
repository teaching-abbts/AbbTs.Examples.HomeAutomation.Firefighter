import { createI18n } from "vue-i18n";
import en from "@/i18n/en.json";
import de from "@/i18n/de.json";
import jp from "@/i18n/jp.json";

const messages = {
  en,
  de,
  jp,
};

export default createI18n({
  legacy: false,
  locale: "de",
  fallbackLocale: "en",
  messages,
});
