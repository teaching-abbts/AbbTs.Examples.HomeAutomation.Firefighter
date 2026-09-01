<template>
  <v-container fluid>
    <h1>House Page</h1>
    <h2>{{ buildingId }}</h2>
  </v-container>
</template>

<script setup lang="ts">
import { useRoute } from "vue-router";

const route = useRoute();
const buildingId = route.params.buildingId as string;
</script>
