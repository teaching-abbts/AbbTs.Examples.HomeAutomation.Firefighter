<template>
  <v-container fluid>
    <h1>{{ $t("wiki.index.title") }}</h1>
    <ul>
      <li v-for="route in childRoutes" :key="route.name">
        <router-link :to="`/wiki/${route.path}`">{{
          route.meta?.titleKey
            ? $t(String(route.meta.titleKey))
            : String(route.path)
        }}</router-link>
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
