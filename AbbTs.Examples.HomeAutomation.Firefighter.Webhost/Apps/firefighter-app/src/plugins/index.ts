import i18n from "./i18n";
import router from "../router";
import type { App } from "vue";
import vuetify from "./vuetify";
import { createPinia } from "pinia";

export function registerPlugins(app: App) {
  app.use(vuetify);
  app.use(createPinia());
  app.use(i18n);
  app.use(router);
}
