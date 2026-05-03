<template>
  <TresGroup :position="position" @click="$emit('select')">
    <!-- Main house body -->
    <TresMesh cast-shadow>
      <TresBoxGeometry :args="[1.8, 1.2, 1.8]" />
      <TresMeshStandardMaterial :color="color" />
    </TresMesh>

    <!-- Roof (cone) -->
    <TresMesh :position="[0, 1.0, 0]" cast-shadow>
      <TresConeGeometry :args="[1.7, 1.0, 4]" />
      <TresMeshStandardMaterial :color="roofColor" />
    </TresMesh>

    <!-- Door -->
    <TresMesh :position="[0, 0.15, 0.95]" cast-shadow>
      <TresBoxGeometry :args="[0.5, 0.8, 0.1]" />
      <TresMeshStandardMaterial color="#8B4513" />
    </TresMesh>
  </TresGroup>
</template>

<script lang="ts" setup>
import { computed } from "vue";

interface Props {
  position: [number, number, number];
  color: string;
  isConnected?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  isConnected: false,
});

const emit = defineEmits<{
  select: [];
}>();

const roofColor = computed(() => {
  return props.isConnected ? "#1b5e20" : "#d84315";
});
</script>
