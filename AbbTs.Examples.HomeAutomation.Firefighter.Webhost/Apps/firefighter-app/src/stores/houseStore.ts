import type { HouseCardProps } from "@/components/house/house-card.vue";
import { defineStore } from "pinia";
import { ref } from "vue";

export const useHouseStore = defineStore("house", () => {
  const houses = ref<HouseCardProps[]>([
    {
      title: "Haus 1",
      coordinates: {
        x: 0,
        y: 0,
      },
      temperature: "22°C",
      brightness: "80%",
      humidity: "48%",
      gasLevel: "5%",
      state: {
        dangerKind: "None",
        endangerment: "Normal",
      },
    },
    {
      title: "Haus 2",
      coordinates: {
        x: 1,
        y: 0,
      },
      temperature: "21°C",
      brightness: "75%",
      humidity: "50%",
      gasLevel: "4%",
      state: {
        dangerKind: "None",
        endangerment: "Normal",
      },
    },
    {
      title: "Haus 3",
      coordinates: {
        x: 0,
        y: 1,
      },
      temperature: "23°C",
      brightness: "90%",
      humidity: "45%",
      gasLevel: "6%",
      state: {
        dangerKind: "None",
        endangerment: "Normal",
      },
    },
    {
      title: "Haus 4",
      coordinates: {
        x: 1,
        y: 1,
      },
      temperature: "22°C",
      brightness: "85%",
      humidity: "47%",
      gasLevel: "5%",
      state: {
        dangerKind: "None",
        endangerment: "Normal",
      },
    },
    {
      title: "Haus 5",
      coordinates: {
        x: 2,
        y: 0,
      },
      temperature: "20°C",
      brightness: "70%",
      humidity: "52%",
      gasLevel: "3%",
      state: {
        dangerKind: "None",
        endangerment: "Normal",
      },
    },
  ]);

  return {
    houses,
  };
});
