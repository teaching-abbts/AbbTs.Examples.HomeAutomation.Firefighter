<template>
  <v-card rounded="xl" variant="outlined">
    <v-card-title class="d-flex align-center ga-2">
      <v-icon icon="mdi-map-marker-radius" />
      {{ title }}
    </v-card-title>
    <v-card-text>
      <div class="landscape-canvas">
        <TresCanvas clear-color="#d9e4ef" shadows>
          <TresAmbientLight :intensity="1.1" />
          <TresDirectionalLight
            :intensity="1.2"
            :position="[8, 14, 10]"
            cast-shadow
          />
          <TresPerspectiveCamera
            make-default
            :position="[0, 18, 20]"
            :fov="45"
            :look-at="[0, 0, 0]"
          />

          <TresMesh
            :position="[0, -0.05, 0]"
            :rotation="[-Math.PI / 2, 0, 0]"
            receive-shadow
          >
            <TresPlaneGeometry :args="[40, 40]" />
            <TresMeshStandardMaterial color="#b7c9d8" />
          </TresMesh>

          <TresMesh
            v-for="home in positionedHomes"
            :key="home.id"
            :position="[home.sceneX, 0.6, home.sceneZ]"
            cast-shadow
            @click="emit('select', home.id)"
          >
            <TresBoxGeometry :args="[1.8, 1.2, 1.8]" />
            <TresMeshStandardMaterial
              :color="home.isConnected ? '#2e7d32' : '#f57c00'"
            />
          </TresMesh>
        </TresCanvas>
      </div>

      <div class="d-flex flex-wrap ga-2 mt-4">
        <v-chip
          v-for="home in homes"
          :key="home.id"
          :color="home.isConnected ? 'success' : 'warning'"
          size="small"
          variant="flat"
          @click="emit('select', home.id)"
        >
          {{ home.id }}
        </v-chip>
      </div>
    </v-card-text>
  </v-card>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import { TresCanvas } from "@tresjs/core";

import type { SmartHomeSummary } from "@/types/smartHomes";

const props = defineProps<{
  homes: SmartHomeSummary[];
  title: string;
}>();

const emit = defineEmits<{
  select: [smartHomeId: string];
}>();

type PositionedHome = SmartHomeSummary & {
  sceneX: number;
  sceneZ: number;
};

const positionedHomes = computed<PositionedHome[]>(() => {
  if (props.homes.length === 0) {
    return [];
  }

  const minX = Math.min(...props.homes.map((home) => home.xCoordinate));
  const minY = Math.min(...props.homes.map((home) => home.yCoordinate));
  const maxX = Math.max(...props.homes.map((home) => home.xCoordinate));
  const maxY = Math.max(...props.homes.map((home) => home.yCoordinate));

  const width = Math.max(maxX - minX, 1);
  const depth = Math.max(maxY - minY, 1);

  return props.homes.map((home) => {
    const normalizedX = ((home.xCoordinate - minX) / width) * 16 - 8;
    const normalizedZ = ((home.yCoordinate - minY) / depth) * 16 - 8;

    return {
      ...home,
      sceneX: normalizedX,
      sceneZ: normalizedZ,
    };
  });
});
</script>

<style scoped>
.landscape-canvas {
  height: 320px;
  border-radius: 16px;
  overflow: hidden;
  border: 1px solid rgba(var(--v-theme-on-surface), 0.12);
}
</style>
