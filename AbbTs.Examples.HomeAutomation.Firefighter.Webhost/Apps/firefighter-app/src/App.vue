<template>
  <v-app>
    <v-app-bar>
      <v-app-bar-nav-icon @click="drawer = !drawer"></v-app-bar-nav-icon>
      <v-app-bar-title>Application</v-app-bar-title>
      <app-theme-switcher />
    </v-app-bar>
    <v-navigation-drawer v-model="drawer" temporary>
      <!--  -->
    </v-navigation-drawer>
    <v-main>
      <router-view />
    </v-main>
  </v-app>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import AppThemeSwitcher from "@/components/app-theme-switcher.vue";

const drawer = ref<boolean | null>(null);
</script>
