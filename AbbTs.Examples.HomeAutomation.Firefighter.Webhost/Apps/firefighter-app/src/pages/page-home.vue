<template>
  <v-container fluid>
    <v-row class="d-flex flex-wrap align-start">
      <v-col v-for="house in houses" :key="house.title">
        <v-card :title="house.title">
          <!-- <v-card-text class="py-0">
            <v-row class="align-center" density="compact">
              <v-col class="text-display-large font-weight-light">{{
                house.temperature
              }}</v-col>
              <v-col class="text-display-large font-weight-light">{{
                house.brightness
              }}</v-col>
            </v-row>
          </v-card-text> -->
          <div class="d-flex py-3 justify-space-between">
            <house-card-property
              :title="house.humidity"
              icon="mdi-water-percent"
              tooltip-to="/wiki/humidity"
              tooltip-text="Humidity levels"
            />
            <house-card-property
              :title="house.gasLevel"
              icon="mdi-smoke-detector"
              tooltip-to="/wiki/smoke"
              tooltip-text="Gas/Smoke levels"
            />
            <house-card-property
              :title="house.temperature"
              icon="mdi-thermometer"
              tooltip-to="/wiki/temperature"
              tooltip-text="Temperature levels"
            />
            <house-card-property
              :title="house.brightness"
              icon="mdi-brightness-5"
              tooltip-to="/wiki/brightness"
              tooltip-text="Brightness levels"
            />
          </div>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts" setup>
import { ref } from "vue";
import HouseCardProperty from "@/components/house/house-card-property.vue";

interface House {
  title: string;
  temperature: string;
  brightness: string;
  humidity: string;
  gasLevel: string;
}

const houses = ref<House[]>([
  {
    title: "Haus 1",
    temperature: "22°C",
    brightness: "80%",
    humidity: "48%",
    gasLevel: "5%",
  },
  {
    title: "Haus 2",
    temperature: "21°C",
    brightness: "75%",
    humidity: "50%",
    gasLevel: "4%",
  },
  {
    title: "Haus 3",
    temperature: "23°C",
    brightness: "90%",
    humidity: "45%",
    gasLevel: "6%",
  },
  {
    title: "Haus 4",
    temperature: "22°C",
    brightness: "85%",
    humidity: "47%",
    gasLevel: "5%",
  },
  {
    title: "Haus 5",
    temperature: "20°C",
    brightness: "70%",
    humidity: "52%",
    gasLevel: "3%",
  },
]);
</script>
