/**
 * router/index.ts
 *
 * Manual routes for ./src/pages/*.vue
 */

// Composables
import { createRouter, createWebHashHistory } from "vue-router";
import Home from "@/pages/page-home.vue";

const router = createRouter({
  history: createWebHashHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "home",
      component: Home,
    },
    {
      path: "/house/:buildingId",
      name: "house",
      component: () => import("@/pages/page-house.vue"),
    },
    {
      path: "/wiki",
      name: "wiki",
      component: () => import("@/pages/wiki/page-wiki-index.vue"),
      children: [
        {
          path: "smoke",
          name: "wiki-smoke",
          component: () => import("@/pages/wiki/page-wiki-smoke.vue"),
          meta: { titleKey: "wiki.smoke.title" },
        },
        {
          path: "humidity",
          name: "wiki-humidity",
          component: () => import("@/pages/wiki/page-wiki-humidity.vue"),
          meta: { titleKey: "wiki.humidity.title" },
        },
        {
          path: "temperature",
          name: "wiki-temperature",
          component: () => import("@/pages/wiki/page-wiki-temperature.vue"),
          meta: { titleKey: "wiki.temperature.title" },
        },
        {
          path: "brightness",
          name: "wiki-brightness",
          component: () => import("@/pages/wiki/page-wiki-brightness.vue"),
          meta: { titleKey: "wiki.brightness.title" },
        },
        {
          path: "endangerment",
          name: "wiki-endangerment",
          component: () => import("@/pages/wiki/page-wiki-endangerment.vue"),
          meta: { titleKey: "wiki.endangerment.title" },
        },
        {
          path: "danger-kind",
          name: "wiki-danger-kind",
          component: () => import("@/pages/wiki/page-wiki-danger-kind.vue"),
          meta: { titleKey: "wiki.danger-kind.title" },
        },
      ],
    },
  ],
});

export default router;
