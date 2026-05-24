import { createI18n } from "vue-i18n";
import en from "@/i18n/en.json";
import de from "@/i18n/de.json";
import hu from "@/i18n/hu.json";
import it from "@/i18n/it.json";
import jp from "@/i18n/jp.json";

const messages = {
  en,
  de,
  hu,
  it,
  jp,
};

export default createI18n({
  legacy: false,
  locale: "de",
  fallbackLocale: "en",
  messages,
});
