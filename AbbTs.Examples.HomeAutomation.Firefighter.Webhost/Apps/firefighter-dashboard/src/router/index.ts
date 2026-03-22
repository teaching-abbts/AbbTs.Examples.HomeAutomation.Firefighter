/**
 * router/index.ts
 *
 * Manual routes for ./src/pages/*.vue
 */

// Composables
import { createRouter, createWebHashHistory } from "vue-router";
import Index from "@/pages/index.vue";
import SmartHomeDetail from "@/pages/smart-home-detail.vue";
import SmartHomes from "@/pages/smart-homes.vue";

const router = createRouter({
  history: createWebHashHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      component: Index,
    },
    {
      path: "/smart-home",
      redirect: "/smart-homes",
    },
    {
      path: "/smart-homes",
      component: SmartHomes,
    },
    {
      path: "/smart-homes/:id",
      component: SmartHomeDetail,
    },
  ],
});

export default router;
