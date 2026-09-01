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
          <li>
            {{ t("house.state.endangerment.label") }}:
            <router-link :to="{ name: 'wiki-endangerment' }">
              {{ t(`house.state.endangerment.${props.endangerment}`) }}
            </router-link>
          </li>
          <li>
            {{ t("house.state.danger-kind.label") }}:
            <router-link :to="{ name: 'wiki-danger-kind' }">
              {{ t(`house.state.danger-kind.${props.dangerKind}`) }}
            </router-link>
          </li>
        </ul>
      </div>
    </v-tooltip>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

type HouseEndangerment = "Normal" | "Warning" | "Critical";

type HouseDangerKind = "None" | "Gas" | "Smoke";

export interface HouseCardStateProps {
  endangerment: HouseEndangerment;
  dangerKind: HouseDangerKind;
}

const props = defineProps<HouseCardStateProps>();
const { t } = useI18n({ useScope: "global" });

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

<style scoped>
a {
  color: mediumslateblue;
}
</style>
