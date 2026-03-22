import { createPinia } from "pinia";

import router from "../router";
import { useAppStore } from "@/stores/app";
import i18n from "./i18n";
/**
 * plugins/index.ts
 *
 * Automatically included in `./src/main.ts`
 */

// Types
import type { App } from "vue";

// Plugins
import vuetify from "./vuetify";

export function registerPlugins(app: App) {
  const pinia = createPinia();

  app.use(vuetify);
  app.use(pinia);

  const appStore = useAppStore(pinia);
  i18n.global.locale.value = appStore.locale;
  vuetify.theme.global.name.value = appStore.theme;

  app.use(i18n);
  app.use(router);
}
