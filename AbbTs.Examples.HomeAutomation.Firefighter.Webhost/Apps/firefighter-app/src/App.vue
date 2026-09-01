<template>
  <v-app>
    <v-app-bar>
      <v-app-bar-nav-icon @click="toggleDrawerLeft()" />
      <v-app-bar-title>
        <v-btn variant="text" to="/">Firefighter App</v-btn>
      </v-app-bar-title>
      <app-theme-switcher />
      <v-app-bar-nav-icon @click="toggleDrawerRight()" />
    </v-app-bar>
    <v-navigation-drawer v-model="drawerLeft" :location="'left'">
      <!--  -->
    </v-navigation-drawer>
    <v-navigation-drawer v-model="drawerRight" :location="'right'">
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

const drawerLeft = ref<boolean | null>(null);
const drawerRight = ref<boolean | null>(null);

function toggleDrawerLeft() {
  drawerLeft.value = !drawerLeft.value;
}

function toggleDrawerRight() {
  drawerRight.value = !drawerRight.value;
}
</script>
