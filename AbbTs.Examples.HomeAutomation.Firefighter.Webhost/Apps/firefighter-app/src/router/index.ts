/**
 * router/index.ts
 *
 * Manual routes for ./src/pages/*.vue
 */

// Composables
import { createRouter, createWebHashHistory } from "vue-router";
import Home from "@/pages/page-home.vue";
import WikiIndex from "@/pages/wiki/page-wiki-index.vue";

const router = createRouter({
  history: createWebHashHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "home",
      component: Home,
    },
    {
      path: "/wiki",
      name: "wiki",
      component: WikiIndex,
      children: [
        {
          path: "smoke",
          name: "wiki-smoke",
          component: () => import("@/pages/wiki/page-wiki-smoke.vue"),
        },
        {
          path: "humidity",
          name: "wiki-humidity",
          component: () => import("@/pages/wiki/page-wiki-humidity.vue"),
        },
        {
          path: "temperature",
          name: "wiki-temperature",
          component: () => import("@/pages/wiki/page-wiki-temperature.vue"),
        },
        {
          path: "brightness",
          name: "wiki-brightness",
          component: () => import("@/pages/wiki/page-wiki-brightness.vue"),
        },
      ],
    },
  ],
});

export default router;
