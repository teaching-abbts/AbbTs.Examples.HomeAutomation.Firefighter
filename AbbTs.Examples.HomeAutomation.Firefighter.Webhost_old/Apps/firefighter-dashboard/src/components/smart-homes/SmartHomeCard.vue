<template>
  <v-card class="smart-home-card" rounded="xl" hover @click="$emit('select')">
    <v-card-item>
      <template #prepend>
        <v-avatar
          :color="summary.isConnected ? 'success' : 'surface-variant'"
          size="40"
        >
          <v-icon
            :icon="
              summary.isConnected
                ? 'mdi-home-lightning-bolt-outline'
                : 'mdi-home-off-outline'
            "
          />
        </v-avatar>
      </template>

      <v-card-title>{{ summary.id }}</v-card-title>
      <v-card-subtitle>{{ summary.owner }}</v-card-subtitle>

      <template #append>
        <v-chip
          :color="summary.isConnected ? 'success' : 'warning'"
          size="small"
          variant="flat"
        >
          {{
            summary.isConnected
              ? t("smartHomes.online")
              : t("smartHomes.offline")
          }}
        </v-chip>
      </template>
    </v-card-item>

    <v-card-text>
      <div class="text-body-2 mb-2">
        {{
          t("smartHomes.coordinates", {
            x: summary.xCoordinate,
            y: summary.yCoordinate,
          })
        }}
      </div>
      <div class="text-body-2 mb-2">
        {{
          t("smartHomes.recentMessages", { count: summary.recentMessageCount })
        }}
      </div>
      <div class="text-caption text-medium-emphasis">
        {{ lastSeenLabel }}
      </div>
    </v-card-text>
  </v-card>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { SmartHomeSummary } from "@/types/smartHomes";

const props = defineProps<{
  summary: SmartHomeSummary;
}>();

defineEmits<{
  select: [];
}>();

const { t } = useI18n({ useScope: "global" });

const lastSeenLabel = computed(() => {
  if (!props.summary.lastSeenUtc) {
    return t("smartHomes.lastSeenNever");
  }

  return t("smartHomes.lastSeenAt", {
    timestamp: new Date(props.summary.lastSeenUtc).toLocaleString(),
  });
});
</script>

<style scoped>
.smart-home-card {
  cursor: pointer;
  transition: transform 0.18s ease;
}

.smart-home-card:hover {
  transform: translateY(-2px);
}
</style>
