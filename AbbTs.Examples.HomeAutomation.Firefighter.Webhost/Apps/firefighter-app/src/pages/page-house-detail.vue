<template>
  <v-container fluid>
    <template v-if="currentHouse">
      <h1>{{ currentHouse.buildingId }}</h1>
      <h2>{{ currentHouse.owner }}</h2>
      <h3>
        [{{ currentHouse.coordinates.x }}, {{ currentHouse.coordinates.y }}]
      </h3>
      <v-card-text class="py-0">
        <house-card-state
          :danger-kind="currentHouse.state.dangerKind"
          :endangerment="currentHouse.state.endangerment"
        />
      </v-card-text>
      <v-table>
        <thead>
          <tr>
            <th id="key" class="text-left">
              {{ t("house.detail.outline-table.key-title") }}
            </th>
            <th id="value" class="text-left">
              {{ t("house.detail.outline-table.value-title") }}
            </th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td>{{ t("house.property.humidity") }}</td>
            <td>
              <house-card-property
                :title="currentHouse.humidity"
                :tooltip-text="$t('house.property.humidity')"
                icon="mdi-water-percent"
                tooltip-to="/wiki/humidity"
              />
            </td>
          </tr>
          <tr>
            <td>{{ t("house.property.gas-smoke") }}</td>
            <td>
              <house-card-property
                :title="currentHouse.gasLevel"
                :tooltip-text="$t('house.property.gas-smoke')"
                icon="mdi-smoke-detector"
                tooltip-to="/wiki/smoke"
              />
            </td>
          </tr>
          <tr>
            <td>{{ t("house.property.temperature") }}</td>
            <td>
              <house-card-property
                :title="currentHouse.temperature"
                :tooltip-text="$t('house.property.temperature')"
                icon="mdi-thermometer"
                tooltip-to="/wiki/temperature"
              />
            </td>
          </tr>
          <tr>
            <td>{{ t("house.property.brightness") }}</td>
            <td>
              <house-card-property
                :title="currentHouse.brightness"
                :tooltip-text="$t('house.property.brightness')"
                icon="mdi-brightness-5"
                tooltip-to="/wiki/brightness"
              />
            </td>
          </tr>
        </tbody>
      </v-table>
    </template>
  </v-container>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useRoute } from "vue-router";
import { useHouseStore } from "@/stores/houseStore";
import { useI18n } from "vue-i18n";
import HouseCardState from "@/components/house/house-card-state.vue";
import HouseCardProperty from "@/components/house/house-card-property.vue";

const houseStore = useHouseStore();
const route = useRoute("house-detail");
const { t } = useI18n({ useScope: "global" });
const currentHouse = computed(
  () =>
    houseStore.houses.find(
      (house) => house.buildingId === (route.params.buildingId as string),
    ) ?? null,
);
</script>
