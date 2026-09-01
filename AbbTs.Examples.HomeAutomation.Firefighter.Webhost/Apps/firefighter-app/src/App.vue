<template>
  <v-app>
    <v-app-bar>
      <v-app-bar-nav-icon @click="toggleDrawerLeft()" />
      <v-app-bar-title>
        <v-btn variant="text" to="/">{{
          $t("app.main-navigation.home.title")
        }}</v-btn>
        <v-btn variant="text" to="/wiki">{{
          $t("app.main-navigation.wiki.title")
        }}</v-btn>
      </v-app-bar-title>
      <app-theme-switcher />
      <app-language-selector />
      <v-app-bar-nav-icon @click="toggleDrawerRight()" />
    </v-app-bar>
    <app-drawer-events v-model="drawerLeft" location="left" />
    <app-drawer-actions v-model="drawerRight" location="right" />
    <v-main>
      <router-view />
    </v-main>
  </v-app>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import AppThemeSwitcher from "@/components/app-theme-switcher.vue";
import AppLanguageSelector from "@/components/app-language-selector.vue";
import AppDrawerEvents from "@/components/app-drawer-events.vue";
import AppDrawerActions from "@/components/app-drawer-actions.vue";

const drawerLeft = ref<boolean | null>(null);
const drawerRight = ref<boolean | null>(null);

function toggleDrawerLeft() {
  drawerLeft.value = !drawerLeft.value;
}

function toggleDrawerRight() {
  drawerRight.value = !drawerRight.value;
}
</script>
