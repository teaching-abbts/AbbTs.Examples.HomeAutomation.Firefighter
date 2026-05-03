<template>
  <v-app>
    <v-main>
      <v-layout class="dashboard-layout">
        <v-app-bar class="px-6" color="surface" elevation="0">
          <v-app-bar-title class="d-flex align-center ga-2 text-h6">
            <span>{{ t("dashboard.title") }}</span>
            <v-progress-circular v-if="loading" color="primary" indeterminate />
            <span v-if="loading" class="text-caption">{{
              t("dashboard.loading")
            }}</span>
          </v-app-bar-title>
          <v-btn
            :color="isActiveRoute(item.path) ? 'primary' : 'default'"
            :key="item.path"
            :prepend-icon="item.icon"
            :to="item.path"
            :variant="isActiveRoute(item.path) ? 'elevated' : 'text'"
            class="mr-2"
            rounded="lg"
            v-for="item in appBarNavItems"
          >
            {{ t(item.labelKey) }}
          </v-btn>
          <LanguageSwitcher class="mr-4" />
          <ThemeSwitcher />
        </v-app-bar>
        <router-view />
      </v-layout>
    </v-main>
  </v-app>
</template>

<script lang="ts" setup>
import LanguageSwitcher from "@/components/dashboard/LanguageSwitcher.vue";
import ThemeSwitcher from "@/components/dashboard/ThemeSwitcher.vue";
import { storeToRefs } from "pinia";
import { useHouseDetailsStore } from "@/stores/houseDetails";
import { computed } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useI18n } from "vue-i18n";

const { t } = useI18n({ useScope: "global" });
const router = useRouter();
const route = useRoute();
const houseDetailsStore = useHouseDetailsStore();

const { loading } = storeToRefs(houseDetailsStore);

type AppBarNavItem = {
  path: string;
  icon: string;
  labelKey: string;
  order: number;
};

const appBarNavItems = computed<AppBarNavItem[]>(() => {
  return router
    .getRoutes()
    .filter(
      (routeRecord) =>
        !!routeRecord.meta.appBar &&
        !routeRecord.path.includes(":") &&
        !routeRecord.redirect,
    )
    .map((routeRecord) => ({
      path: routeRecord.path,
      icon: routeRecord.meta.appBar!.icon,
      labelKey: routeRecord.meta.appBar!.labelKey,
      order: routeRecord.meta.appBar!.order,
    }))
    .sort((left, right) => left.order - right.order);
});

const isActiveRoute = (targetPath: string): boolean => {
  if (targetPath === "/") {
    return route.path === "/";
  }

  return route.path === targetPath || route.path.startsWith(`${targetPath}/`);
};
</script>
