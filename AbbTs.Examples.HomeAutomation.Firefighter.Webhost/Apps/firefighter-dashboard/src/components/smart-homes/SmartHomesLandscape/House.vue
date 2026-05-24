<template>
  <TresGroup :position="position" @click="$emit('select')">
    <CientosHtml
      :position="[0, 1.95, 0]"
      transform
      sprite
      :distance-factor="12"
      pointer-events="none"
    >
      <div class="house-label">{{ houseName }}</div>
    </CientosHtml>

    <!-- Main house base (rectangular walls) -->
    <TresMesh cast-shadow>
      <TresBoxGeometry :args="[1.8, 1.2, 2.2]" />
      <TresMeshStandardMaterial :color="color" />
    </TresMesh>

    <!-- Front gable (triangle) -->
    <TresMesh cast-shadow>
      <TresBufferGeometry ref="frontGableGeom">
        <TresBufferAttribute
          :array="frontGableVertices"
          :item-size="3"
          attribute-name="position"
        />
      </TresBufferGeometry>
      <TresMeshStandardMaterial :color="gableColor" :side="2" />
    </TresMesh>

    <!-- Back gable (triangle) -->
    <TresMesh :position="[0, 0, -2.2]" cast-shadow>
      <TresBufferGeometry ref="backGableGeom">
        <TresBufferAttribute
          :array="frontGableVertices"
          :item-size="3"
          attribute-name="position"
        />
      </TresBufferGeometry>
      <TresMeshStandardMaterial :color="gableColor" :side="2" />
    </TresMesh>

    <!-- Roof panel 1 (left side) -->
    <TresMesh
      :position="[-0.95, 1.0, 0]"
      :rotation="[Math.PI * 0.5, Math.PI * 0.22, 0]"
      cast-shadow
    >
      <TresPlaneGeometry :args="[2.4, 2.8]" />
      <TresMeshStandardMaterial :color="roofColor" :side="2" />
    </TresMesh>

    <!-- Roof panel 2 (right side) -->
    <TresMesh
      :position="[0.95, 1.0, 0]"
      :rotation="[Math.PI * 0.5, Math.PI * -0.22, 0]"
      cast-shadow
    >
      <TresPlaneGeometry :args="[2.4, 2.8]" />
      <TresMeshStandardMaterial :color="roofColor" :side="2" />
    </TresMesh>

    <!-- Door -->
    <TresMesh :position="[0, 0.15, 1.15]" cast-shadow>
      <TresBoxGeometry :args="[0.5, 0.8, 0.1]" />
      <TresMeshStandardMaterial color="#8B4513" />
    </TresMesh>

    <!-- Window 1 -->
    <TresMesh :position="[-0.4, 0.4, 1.15]" cast-shadow>
      <TresBoxGeometry :args="[0.35, 0.35, 0.05]" />
      <TresMeshStandardMaterial color="#4FC3F7" />
    </TresMesh>

    <!-- Window 2 -->
    <TresMesh :position="[0.4, 0.4, 1.15]" cast-shadow>
      <TresBoxGeometry :args="[0.35, 0.35, 0.05]" />
      <TresMeshStandardMaterial color="#4FC3F7" />
    </TresMesh>
  </TresGroup>
</template>

<script lang="ts" setup>
import { computed, ref, watch } from "vue";
import { Html as CientosHtml } from "@tresjs/cientos";

interface Props {
  position: [number, number, number];
  color: string;
  houseName: string;
  isConnected?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  isConnected: false,
});

const emit = defineEmits<{
  select: [];
}>();

const frontGableGeom = ref<any>(null);
const backGableGeom = ref<any>(null);

// Triangle gable geometry vertices
const frontGableVertices = computed(() => {
  return new Float32Array([
    -0.9,
    0,
    0, // left bottom
    0,
    0.85,
    0, // top
    0.9,
    0,
    0, // right bottom
  ]);
});

// Compute normals when geometry updates
watch([frontGableGeom, backGableGeom], () => {
  if (frontGableGeom.value?.computeVertexNormals) {
    frontGableGeom.value.computeVertexNormals();
  }
  if (backGableGeom.value?.computeVertexNormals) {
    backGableGeom.value.computeVertexNormals();
  }
});

const gableColor = computed(() => {
  return props.isConnected ? "#1b5e20" : "#d32f2f";
});

const roofColor = computed(() => {
  return props.isConnected ? "#0d3817" : "#b71c1c";
});
</script>

<style scoped>
.house-label {
  padding: 2px 8px;
  border-radius: 999px;
  border: 1px solid rgba(0, 0, 0, 0.18);
  background: rgba(255, 255, 255, 0.86);
  color: #1f2937;
  font-size: 10px;
  font-weight: 700;
  line-height: 1.2;
  letter-spacing: 0.02em;
  white-space: nowrap;
  user-select: none;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.18);
}
</style>
