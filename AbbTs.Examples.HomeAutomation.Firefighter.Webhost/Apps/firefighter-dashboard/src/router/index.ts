/**
 * router/index.ts
 *
 * Manual routes for ./src/pages/*.vue
 */

// Composables
import { createRouter, createWebHashHistory } from "vue-router";
import Index from "@/pages/index.vue";
import Settings from "@/pages/settings.vue";
import SmartHomeDetail from "@/pages/smart-home-detail.vue";
import SmartHomes from "@/pages/smart-homes.vue";

declare module "vue-router" {
  interface RouteMeta {
    appBar?: {
      icon: string;
      labelKey: string;
      order: number;
    };
  }
}

const router = createRouter({
  history: createWebHashHistory(import.meta.env.BASE_URL),
  routes: [
    {
      name: "dashboard",
      path: "/",
      component: Index,
      meta: {
        appBar: {
          icon: "mdi-view-dashboard",
          labelKey: "dashboard.nav",
          order: 10,
        },
      },
    },
    {
      path: "/smart-home",
      redirect: "/smart-homes",
    },
    {
      name: "smart-homes",
      path: "/smart-homes",
      component: SmartHomes,
      meta: {
        appBar: {
          icon: "mdi-home-automation",
          labelKey: "smartHomes.nav",
          order: 20,
        },
      },
    },
    {
      name: "settings",
      path: "/settings",
      component: Settings,
      meta: {
        appBar: {
          icon: "mdi-cog",
          labelKey: "settings.nav",
          order: 30,
        },
      },
    },
    {
      name: "smart-home-detail",
      path: "/smart-homes/:id",
      component: SmartHomeDetail,
    },
  ],
});

export default router;
