/**
 * router/index.ts
 *
 * Manual routes for ./src/pages/*.vue
 */

// Composables
import { createRouter, createWebHashHistory } from "vue-router";
import Index from "@/pages/index.vue";
import SmartHome from "@/pages/smart-home.vue";

const router = createRouter({
  history: createWebHashHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      component: Index,
    },
    {
      path: "/smart-home",
      component: SmartHome,
    },
  ],
});

export default router;
