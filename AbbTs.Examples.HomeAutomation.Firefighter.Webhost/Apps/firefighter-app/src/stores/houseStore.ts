import type { HouseCardProps } from "@/components/house/house-card.vue";
import { defineStore } from "pinia";
import { ref } from "vue";

export const useHouseStore = defineStore("house", () => {
  const houses = ref<HouseCardProps[]>([
    {
      buildingId: "house-1",
      owner: "Max",
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
      buildingId: "house-2",
      owner: "Moritz",
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
      buildingId: "house-3",
      owner: "Lehrer Lämpel",
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
      buildingId: "house-4",
      owner: "Onkel Fritz",
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
      buildingId: "house-5",
      owner: "Meister Bäcker",
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
