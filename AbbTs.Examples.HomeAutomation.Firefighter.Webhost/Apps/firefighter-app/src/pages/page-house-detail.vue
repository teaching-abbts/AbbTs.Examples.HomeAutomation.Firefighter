<template>
  <v-container fluid>
    <template v-if="currentHouse">
      <v-row>
        <v-col cols="12">
          <v-card>
            <v-card-title>{{ currentHouse.buildingId }}</v-card-title>
            <v-card-subtitle>{{ currentHouse.owner }}</v-card-subtitle>
            <v-card-text class="py-0">
              <div class="d-flex justify-space-around text-display-large">
                <v-chip prepend-icon="mdi-map-marker" size="large">
                  [ x = {{ currentHouse.coordinates.x }}, y =
                  {{ currentHouse.coordinates.y }} ]
                </v-chip>
                <house-card-state
                  :danger-kind="currentHouse.state.dangerKind"
                  :endangerment="currentHouse.state.endangerment"
                />
              </div>
            </v-card-text>
          </v-card>
        </v-col>
        <v-col cols="12" lg="1/2">
          <v-card min-width="500">
            <v-card-title>{{ t("house.detail.sensors-title") }}</v-card-title>
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
          </v-card>
        </v-col>
        <v-col cols="12" lg="1/2">
          <v-card min-width="500">
            <v-card-title>{{ t("house.detail.settings-title") }}</v-card-title>
            <v-table>
              <thead>
                <tr>
                  <th id="setting-key" class="text-left">
                    {{ t("house.detail.outline-table.setting-title") }}
                  </th>
                  <th id="setting-control" class="text-left">
                    {{ t("house.detail.outline-table.control-title") }}
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>{{ t("house.setting.display") }}</td>
                  <td>
                    <v-text-field
                      :label="t('house.setting.display-value')"
                      max="33"
                      v-model="displayValue"
                      :validate="validateDisplayValue"
                    />
                  </td>
                </tr>
                <tr>
                  <td>{{ t("house.setting.light") }}</td>
                  <td>
                    <v-switch
                      color="primary"
                      v-model="isLightOn"
                      :label="t('house.setting.light')"
                    />
                    <v-slider
                      v-model="lightIntensity"
                      :label="t('house.setting.light-intensity')"
                      min="0"
                      max="1023"
                      step="1"
                    >
                      <template #append>
                        <v-text-field
                          density="compact"
                          hide-details
                          max="1023"
                          min="0"
                          single-line
                          step="1"
                          type="number"
                          v-model="lightIntensity"
                        />
                      </template>
                    </v-slider>
                  </td>
                </tr>
                <tr>
                  <td>{{ t("house.setting.door-lock") }}</td>
                  <td>
                    <v-switch
                      color="primary"
                      v-model="isDoorLocked"
                      :label="t('house.setting.door-lock')"
                    />
                  </td>
                </tr>
                <tr>
                  <td>{{ t("house.setting.heating") }}</td>
                  <td>
                    <v-switch
                      color="primary"
                      v-model="isHeatingOn"
                      :label="t('house.setting.heating')"
                    />
                    <v-slider
                      v-model="heatingIntensity"
                      :label="t('house.setting.heating-intensity')"
                      min="0"
                      max="30"
                      step="1"
                    >
                      <template #append>
                        <v-text-field
                          density="compact"
                          hide-details
                          max="30"
                          min="0"
                          single-line
                          step="1"
                          type="number"
                          v-model="heatingIntensity"
                        />
                      </template>
                    </v-slider>
                  </td>
                </tr>
                <tr>
                  <td>{{ t("house.setting.alarm") }}</td>
                  <td>
                    <v-switch
                      color="primary"
                      v-model="isAlarmOn"
                      :label="t('house.setting.alarm')"
                    />
                  </td>
                </tr>
              </tbody>
            </v-table>
          </v-card>
        </v-col>
      </v-row>
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
    houseStore.readonlyHouses.find(
      (house) => house.buildingId === (route.params.buildingId as string),
    ) ?? null,
);

const isLightOn = computed({
  get: () => currentHouse.value?.sensors.isLightOn ?? false,
  set: (value: boolean) => {
    if (currentHouse.value) {
      houseStore.toggleLight(currentHouse.value.buildingId, value);
    }
  },
});
const isDoorLocked = computed({
  get: () => currentHouse.value?.sensors.isDoorLocked ?? false,
  set: (value: boolean) => {
    if (currentHouse.value) {
      houseStore.toggleDoorLock(currentHouse.value.buildingId, value);
    }
  },
});
const isHeatingOn = computed({
  get: () => currentHouse.value?.sensors.isHeatingOn ?? false,
  set: (value: boolean) => {
    if (currentHouse.value) {
      houseStore.toggleHeating(currentHouse.value.buildingId, value);
    }
  },
});
const isAlarmOn = computed({
  get: () => currentHouse.value?.sensors.isAlarmOn ?? false,
  set: (value: boolean) => {
    if (currentHouse.value) {
      houseStore.toggleAlarm(currentHouse.value.buildingId, value);
    }
  },
});

const lightIntensity = computed({
  get: () => currentHouse.value?.sensors.lightIntensity ?? 0,
  set: (value: number) => {
    if (currentHouse.value) {
      houseStore.setLightIntensity(currentHouse.value.buildingId, value);
    }
  },
});

const heatingIntensity = computed({
  get: () => currentHouse.value?.sensors.heatingIntensity ?? 0,
  set: (value: number) => {
    if (currentHouse.value) {
      houseStore.setHeatingIntensity(currentHouse.value.buildingId, value);
    }
  },
});

const displayValue = computed({
  get: () => currentHouse.value?.sensors.displayValue ?? "",
  set: (value: string) => {
    if (currentHouse.value) {
      houseStore.setDisplayValue(currentHouse.value.buildingId, value);
    }
  },
});

function validateDisplayValue(value: string) {
  if (value.trim().length > 33) {
    return "Display value must be 33 characters or less.";
  }

  if (value.split(";").length > 2) {
    return "Display value cannot contain more than one semicolon.";
  }

  return true;
}
</script>
