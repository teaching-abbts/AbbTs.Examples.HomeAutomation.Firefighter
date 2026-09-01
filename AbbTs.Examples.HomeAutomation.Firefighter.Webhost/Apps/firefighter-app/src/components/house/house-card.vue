<template>
  <v-card :title="props.title" :subtitle="origin">
    <v-card-text class="py-0">
      <house-card-state
        :danger-kind="props.state.dangerKind"
        :endangerment="props.state.endangerment"
      />
    </v-card-text>
    <div class="d-flex py-3 justify-space-between">
      <house-card-property
        :title="props.humidity"
        :tooltip-text="$t('house.property.humidity')"
        icon="mdi-water-percent"
        tooltip-to="/wiki/humidity"
      />
      <house-card-property
        :title="props.gasLevel"
        :tooltip-text="$t('house.property.gas-smoke')"
        icon="mdi-smoke-detector"
        tooltip-to="/wiki/smoke"
      />
      <house-card-property
        :title="props.temperature"
        :tooltip-text="$t('house.property.temperature')"
        icon="mdi-thermometer"
        tooltip-to="/wiki/temperature"
      />
      <house-card-property
        :title="props.brightness"
        :tooltip-text="$t('house.property.brightness')"
        icon="mdi-brightness-5"
        tooltip-to="/wiki/brightness"
      />
    </div>
    <v-divider />
    <v-card-actions>
      <v-btn
        :to="`/house/${props.buildingId}`"
        :text="t('house.overview.navigate-to-detail')"
      />
    </v-card-actions>
  </v-card>
</template>

<script setup lang="ts">
import HouseCardProperty from "@/components/house/house-card-property.vue";
import HouseCardState, {
  type HouseCardStateProps,
} from "@/components/house/house-card-state.vue";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

export interface HouseCardProps {
  buildingId: string;
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
const { t } = useI18n();

const origin = computed(
  () => `(${props.coordinates.x}, ${props.coordinates.y})`,
);
</script>
