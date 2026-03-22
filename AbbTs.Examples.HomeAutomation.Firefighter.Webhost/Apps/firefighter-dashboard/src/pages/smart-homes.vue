<template>
  <v-container class="py-8 mt-8" fluid>
    <v-row class="mb-4" justify="space-between" align="center">
      <v-col cols="12" md="8">
        <div class="text-overline text-primary">
          {{ t("smartHomes.badge") }}
        </div>
        <h1 class="text-h4 font-weight-bold mb-2">
          {{ t("smartHomes.title") }}
        </h1>
        <p class="text-body-1 text-medium-emphasis mb-0">
          {{ t("smartHomes.subtitle") }}
        </p>
      </v-col>
      <v-col cols="12" md="4" class="d-flex justify-md-end gap-2">
        <v-btn prepend-icon="mdi-view-dashboard" to="/" variant="outlined">
          {{ t("smartHomes.back") }}
        </v-btn>
      </v-col>
    </v-row>

    <v-alert v-if="errorMessage" class="mb-4" type="error" variant="tonal">
      {{ errorMessage }}
    </v-alert>

    <v-progress-linear
      v-if="loading"
      class="mb-4"
      color="primary"
      indeterminate
    />

    <v-row v-if="smartHomes.length > 0">
      <v-col
        v-for="smartHome in smartHomes"
        :key="smartHome.id"
        cols="12"
        md="6"
        xl="4"
      >
        <SmartHomeCard
          :summary="smartHome"
          @select="openSmartHome(smartHome.id)"
        />
      </v-col>
    </v-row>

    <v-alert v-else-if="!loading" type="info" variant="tonal">
      {{ t("smartHomes.empty") }}
    </v-alert>
    <v-row v-if="smartHomes.length > 0" class="mt-2">
      <v-col cols="12">
        <SmartHomesLandscape
          :homes="smartHomes"
          :title="t('smartHomes.landscapeTitle')"
          @select="openSmartHome"
        />
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts" setup>
import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { onMounted, onUnmounted, ref } from "vue";
import { useRouter } from "vue-router";
import { useI18n } from "vue-i18n";

import { Client } from "@/api/AbbTs.Examples.HomeAutomation.Firefighter.Webhost";
import SmartHomeCard from "@/components/smart-homes/SmartHomeCard.vue";
import SmartHomesLandscape from "@/components/smart-homes/SmartHomesLandscape.vue";

import type { SmartHomeSummary } from "@/types/smartHomes";

const router = useRouter();
const { t } = useI18n({ useScope: "global" });

const smartHomes = ref<SmartHomeSummary[]>([]);
const loading = ref(true);
const errorMessage = ref("");
const apiClient = new Client();

const hubConnection = new HubConnectionBuilder()
  .withUrl("/hubs/smart-homes")
  .withAutomaticReconnect()
  .configureLogging(LogLevel.Warning)
  .build();

const loadSmartHomes = async () => {
  loading.value = true;
  errorMessage.value = "";

  try {
    const result = await apiClient.getSmartHomes();
    smartHomes.value = (result ?? []).map((item) => ({
      id: item.id ?? "",
      owner: item.owner ?? "",
      xCoordinate: item.xCoordinate ?? 0,
      yCoordinate: item.yCoordinate ?? 0,
      isConnected: item.isConnected ?? false,
      connectedAtUtc: item.connectedAtUtc?.toISOString() ?? null,
      lastSeenUtc: item.lastSeenUtc?.toISOString() ?? null,
      recentMessageCount: item.recentMessageCount ?? 0,
    }));
  } catch {
    errorMessage.value = t("smartHomes.loadError");
  } finally {
    loading.value = false;
  }
};

const openSmartHome = (smartHomeId: string) => {
  void router.push(`/smart-homes/${encodeURIComponent(smartHomeId)}`);
};

onMounted(async () => {
  await loadSmartHomes();

  hubConnection.on("smartHomesChanged", (payload: SmartHomeSummary[]) => {
    smartHomes.value = payload;
  });

  try {
    await hubConnection.start();
  } catch {
    errorMessage.value ||= t("smartHomes.liveUnavailable");
  }
});

onUnmounted(() => {
  hubConnection.off("smartHomesChanged");
  void hubConnection.stop();
});
</script>
