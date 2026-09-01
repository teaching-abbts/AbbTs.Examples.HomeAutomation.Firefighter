<template>
  <v-card :title="props.title" :subtitle="origin">
    <v-card-text class="py-0">
      <house-card-state
        :endangerment="props.state.endangerment"
        :danger-kind="props.state.dangerKind"
      />
    </v-card-text>
    <div class="d-flex py-3 justify-space-between">
      <house-card-property
        :title="props.humidity"
        icon="mdi-water-percent"
        tooltip-to="/wiki/humidity"
        :tooltip-text="$t('house-property.humidity')"
      />
      <house-card-property
        :title="props.gasLevel"
        icon="mdi-smoke-detector"
        tooltip-to="/wiki/smoke"
        :tooltip-text="$t('house-property.gas-smoke')"
      />
      <house-card-property
        :title="props.temperature"
        icon="mdi-thermometer"
        tooltip-to="/wiki/temperature"
        :tooltip-text="$t('house-property.temperature')"
      />
      <house-card-property
        :title="props.brightness"
        icon="mdi-brightness-5"
        tooltip-to="/wiki/brightness"
        :tooltip-text="$t('house-property.brightness')"
      />
    </div>
  </v-card>
</template>

<script setup lang="ts">
import HouseCardProperty from "@/components/house/house-card-property.vue";
import HouseCardState, {
  type HouseCardStateProps,
} from "@/components/house/house-card-state.vue";
import { computed } from "vue";

export interface HouseCardProps {
  title: string;
  coordinates: {
    x: number;
    y: number;
  };
  temperature: string;
  brightness: string;
  humidity: string;
  gasLevel: string;
  state: HouseCardStateProps;
}

const props = defineProps<HouseCardProps>();

const origin = computed(
  () => `(${props.coordinates.x}, ${props.coordinates.y})`,
);
</script>
