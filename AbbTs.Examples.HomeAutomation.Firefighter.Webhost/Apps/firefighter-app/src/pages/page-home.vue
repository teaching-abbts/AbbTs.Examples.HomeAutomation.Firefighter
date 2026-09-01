<template>
  <v-container fluid>
    <v-row class="d-flex flex-wrap align-start">
      <v-col v-for="house in houseStore.houses" :key="house.buildingId">
        <house-card v-bind="house" />
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts" setup>
import HouseCard from "@/components/house/house-card.vue";
import { useHouseStore } from "@/stores/houseStore";

const houseStore = useHouseStore();
</script>
