<template>
  <div class="d-flex justify-space-around text-display-large">
    <v-tooltip interactive location="bottom">
      <template #activator="{ props }">
        <v-icon v-bind="props" :color="iconProps.color">{{
          iconProps.icon
        }}</v-icon>
      </template>
      <div>
        <ul>
          <li>Endangerment: {{ props.endangerment }}</li>
          <li>Danger Kind: {{ props.dangerKind }}</li>
        </ul>
      </div>
    </v-tooltip>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";

type HouseEndangerment = "Normal" | "Warning" | "Critical";

type HouseDangerKind = "None" | "Gas" | "Smoke";

export interface HouseCardStateProps {
  endangerment: HouseEndangerment;
  dangerKind: HouseDangerKind;
}

const props = defineProps<HouseCardStateProps>();

const iconProps = computed<{ color: string; icon: string }>(() => {
  switch (props.endangerment) {
    case "Normal":
      return { color: "success", icon: "mdi-check-circle" };
    case "Warning":
      return { color: "warning", icon: "mdi-alert" };
    case "Critical":
      return { color: "error", icon: "mdi-alert-circle" };
    default:
      return { color: "info", icon: "mdi-help-circle" };
  }
});
</script>
