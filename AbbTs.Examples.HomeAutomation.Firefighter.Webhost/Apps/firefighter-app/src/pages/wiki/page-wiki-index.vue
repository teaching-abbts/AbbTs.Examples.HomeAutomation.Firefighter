<template>
  <v-container fluid>
    <h1>Firefighter Wiki</h1>
    <ul>
      <li v-for="route in childRoutes" :key="route.name">
        <router-link :to="`/wiki/${route.path}`">{{ route.path }}</router-link>
      </li>
    </ul>
    <router-view />
  </v-container>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import { useRouter } from "vue-router";

const router = useRouter();

const childRoutes = computed(() => {
  const wikiRoute = router.options.routes.find((r) => r.name === "wiki");
  return wikiRoute?.children || [];
});
</script>
