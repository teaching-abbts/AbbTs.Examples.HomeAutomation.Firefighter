<template>
  <v-container class="py-8 smart-home-page" fluid>
    <v-row class="mb-4" justify="space-between" align="center">
      <v-col cols="12" md="8">
        <div class="text-overline text-primary">
          {{ t("smartHomes.detailBadge") }}
        </div>
        <h1 class="text-h4 font-weight-bold mb-2">
          {{ detail?.id ?? smartHomeId }}
        </h1>
        <p class="text-body-1 text-medium-emphasis mb-0">
          {{ detailSubtitle }}
        </p>
      </v-col>
      <v-col cols="12" md="4" class="d-flex justify-md-end ga-2 flex-wrap">
        <v-btn
          prepend-icon="mdi-home-group"
          to="/smart-homes"
          variant="outlined"
        >
          {{ t("smartHomes.backToList") }}
        </v-btn>
      </v-col>
    </v-row>

    <v-alert v-if="errorMessage" class="mb-4" type="error" variant="tonal">
      {{ errorMessage }}
    </v-alert>

    <v-alert v-if="notFound" class="mb-4" type="warning" variant="tonal">
      {{ t("smartHomes.notFound") }}
    </v-alert>

    <v-row>
      <v-col cols="12" lg="5">
        <v-card class="mb-4" rounded="xl">
          <v-card-title class="d-flex align-center ga-2">
            <v-icon icon="mdi-transmission-tower" />
            {{ t("smartHomes.connection") }}
          </v-card-title>
          <v-card-text>
            <v-chip
              :color="detail?.isConnected ? 'success' : 'warning'"
              size="small"
              variant="flat"
            >
              {{
                detail?.isConnected
                  ? t("smartHomes.online")
                  : t("smartHomes.offline")
              }}
            </v-chip>
            <div class="mt-3 text-body-2 text-medium-emphasis">
              {{ t("smartHomes.ownerLabel", { owner: detail?.owner ?? "-" }) }}
            </div>
            <div class="mt-2 text-body-2 text-medium-emphasis">
              {{
                t("smartHomes.coordinates", {
                  x: detail?.xCoordinate ?? "-",
                  y: detail?.yCoordinate ?? "-",
                })
              }}
            </div>
            <div class="mt-2 text-body-2">{{ liveStatus }}</div>
          </v-card-text>
        </v-card>

        <v-card class="mb-4" rounded="xl">
          <v-card-title>{{ t("smartHomes.quick") }}</v-card-title>
          <v-card-text>
            <div class="d-flex flex-wrap ga-2">
              <v-btn
                :disabled="!detail?.isConnected || !hubConnected"
                @click="requestState()"
              >
                {{ t("smartHomes.getState") }}
              </v-btn>
              <v-btn
                :disabled="!detail?.isConnected || !hubConnected"
                @click="requestMeasurement()"
              >
                {{ t("smartHomes.getMeasurement") }}
              </v-btn>
            </div>
          </v-card-text>
        </v-card>

        <v-card rounded="xl">
          <v-card-title>{{ t("smartHomes.sendCommand") }}</v-card-title>
          <v-card-text>
            <v-select
              v-model="selectedDevice"
              :items="devices"
              :label="t('smartHomes.device')"
              density="comfortable"
              variant="outlined"
            />
            <v-select
              v-model="selectedCommand"
              :items="availableCommands"
              :label="t('smartHomes.command')"
              density="comfortable"
              variant="outlined"
            />
            <v-text-field
              v-model="commandValue"
              :disabled="!commandNeedsValue"
              :label="t('smartHomes.value')"
              :placeholder="valuePlaceholder"
              density="comfortable"
              variant="outlined"
            />
            <v-btn
              block
              color="primary"
              :disabled="
                !detail?.isConnected || !hubConnected || !selectedCommand
              "
              prepend-icon="mdi-send"
              variant="elevated"
              @click="sendCommand()"
            >
              {{ t("smartHomes.send") }}
            </v-btn>
          </v-card-text>
        </v-card>
      </v-col>

      <v-col cols="12" lg="7">
        <v-card rounded="xl" class="log-card">
          <v-card-title class="d-flex align-center justify-space-between">
            <span>{{ t("smartHomes.liveLog") }}</span>
            <v-btn size="small" variant="text" @click="clearMessages()">
              {{ t("smartHomes.clear") }}
            </v-btn>
          </v-card-title>
          <v-divider />
          <v-card-text class="log-area">
            <v-alert v-if="messages.length === 0" type="info" variant="tonal">
              {{ t("smartHomes.noMessages") }}
            </v-alert>

            <v-sheet
              v-for="(item, index) in messages"
              :key="`${item.receivedAtUtc ?? 'na'}-${item.messageType}-${index}`"
              class="pa-3 mb-3 rounded-lg"
              color="surface-variant"
            >
              <div class="d-flex align-center justify-space-between mb-1">
                <v-chip
                  size="x-small"
                  variant="flat"
                  :color="chipColor(item.messageType)"
                >
                  {{ item.messageType }}
                </v-chip>
                <span class="text-caption">{{
                  formatTimestamp(item.receivedAtUtc)
                }}</span>
              </div>
              <pre class="text-caption code-block">{{
                formatPayload(item.payload)
              }}</pre>
            </v-sheet>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts" setup>
import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr";
import { computed, onMounted, onUnmounted, ref, watch } from "vue";
import { useRoute } from "vue-router";
import { useI18n } from "vue-i18n";

import {
  ApiException,
  Client,
} from "@/api/AbbTs.Examples.HomeAutomation.Firefighter.Webhost";
import type {
  SmartHomeCommand,
  SmartHomeDetails,
  SmartHomeEnvelope,
} from "@/types/smartHomes";

const devices = [
  "LightControl",
  "HeatingControl",
  "AlarmControl",
  "Door",
  "Display",
];
const commandMap: Record<string, string[]> = {
  LightControl: ["on", "off", "setpoint"],
  HeatingControl: ["on", "off", "setpoint"],
  AlarmControl: ["on", "off"],
  Door: ["open", "close"],
  Display: ["set"],
};

const route = useRoute();
const { t } = useI18n({ useScope: "global" });

const detail = ref<SmartHomeDetails | null>(null);
const hubConnected = ref(false);
const errorMessage = ref("");
const notFound = ref(false);
const selectedDevice = ref("LightControl");
const selectedCommand = ref("on");
const commandValue = ref("");
const apiClient = new Client();

let hubConnection: HubConnection | null = null;

const smartHomeId = computed(() => String(route.params.id ?? ""));
const messages = computed<SmartHomeEnvelope[]>(
  () => detail.value?.recentEnvelopes ?? [],
);
const availableCommands = computed(
  () => commandMap[selectedDevice.value] ?? [],
);
const commandNeedsValue = computed(
  () =>
    selectedCommand.value === "setpoint" || selectedDevice.value === "Display",
);
const valuePlaceholder = computed(() => {
  if (selectedDevice.value === "Display") {
    return "Line 1;Line 2";
  }

  return selectedCommand.value === "setpoint" ? "e.g. 174" : "";
});

const detailSubtitle = computed(() => {
  if (!detail.value) {
    return t("smartHomes.detailSubtitleUnknown");
  }

  return t("smartHomes.detailSubtitle", {
    owner: detail.value.owner,
    x: detail.value.xCoordinate,
    y: detail.value.yCoordinate,
  });
});

const liveStatus = computed(() => {
  if (!hubConnected.value) {
    return t("smartHomes.hubDisconnected");
  }

  return detail.value?.isConnected
    ? t("smartHomes.deviceConnected")
    : t("smartHomes.deviceDisconnected");
});

const createHubConnection = () => {
  return new HubConnectionBuilder()
    .withUrl("/hubs/smart-homes")
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Warning)
    .build();
};

const loadDetail = async () => {
  if (!smartHomeId.value) {
    return;
  }

  errorMessage.value = "";
  notFound.value = false;

  try {
    const result = await apiClient.getSmartHome(smartHomeId.value);
    detail.value = {
      id: result.id ?? "",
      owner: result.owner ?? "",
      xCoordinate: result.xCoordinate ?? 0,
      yCoordinate: result.yCoordinate ?? 0,
      isConnected: result.isConnected ?? false,
      connectedAtUtc: result.connectedAtUtc?.toISOString() ?? null,
      lastSeenUtc: result.lastSeenUtc?.toISOString() ?? null,
      recentEnvelopes: (result.recentEnvelopes ?? []).map(
        (envelope) =>
          ({
            messageType: envelope.messageType ?? "",
            payload: envelope.payload,
            receivedAtUtc: envelope.receivedAtUtc?.toISOString(),
          }) as SmartHomeEnvelope,
      ),
    };
  } catch (error: unknown) {
    if (ApiException.isApiException(error) && error.status === 404) {
      detail.value = null;
      notFound.value = true;
      return;
    }

    throw error;
  }
};

const connectHub = async () => {
  if (!smartHomeId.value) {
    return;
  }

  hubConnection = createHubConnection();

  hubConnection.onreconnected(async () => {
    hubConnected.value = true;
    await hubConnection?.invoke("Subscribe", smartHomeId.value);
  });

  hubConnection.onclose(() => {
    hubConnected.value = false;
  });

  hubConnection.on("smartHomeUpdated", (payload: SmartHomeDetails) => {
    if (payload.id === smartHomeId.value) {
      detail.value = payload;
      notFound.value = false;
    }
  });

  await hubConnection.start();
  hubConnected.value = true;
  await hubConnection.invoke("Subscribe", smartHomeId.value);
};

const disconnectHub = async () => {
  if (!hubConnection) {
    return;
  }

  hubConnection.off("smartHomeUpdated");

  try {
    if (hubConnected.value) {
      await hubConnection.invoke("Unsubscribe", smartHomeId.value);
    }
  } catch {}

  await hubConnection.stop();
  hubConnection = null;
  hubConnected.value = false;
};

const initializePage = async () => {
  await disconnectHub();

  try {
    await loadDetail();
    if (!notFound.value) {
      await connectHub();
    }
  } catch {
    errorMessage.value = t("smartHomes.loadDetailError");
  }
};

const requestState = async () => {
  try {
    await hubConnection?.invoke("RequestState", smartHomeId.value);
  } catch {
    errorMessage.value = t("smartHomes.commandError");
  }
};

const requestMeasurement = async () => {
  try {
    await hubConnection?.invoke("RequestMeasurement", smartHomeId.value);
  } catch {
    errorMessage.value = t("smartHomes.commandError");
  }
};

const sendCommand = async () => {
  const payload: SmartHomeCommand = {
    device: selectedDevice.value,
    command: selectedCommand.value,
    value: commandNeedsValue.value ? commandValue.value : "",
  };

  try {
    await hubConnection?.invoke("SendCommand", smartHomeId.value, payload);
    if (!commandNeedsValue.value) {
      commandValue.value = "";
    }
  } catch {
    errorMessage.value = t("smartHomes.commandError");
  }
};

const clearMessages = () => {
  if (!detail.value) {
    return;
  }

  detail.value = {
    ...detail.value,
    recentEnvelopes: [],
  };
};

const chipColor = (messageType: string) => {
  if (messageType.startsWith("outbound")) {
    return "secondary";
  }

  if (messageType.startsWith("send ")) {
    return "primary";
  }

  return "warning";
};

const formatTimestamp = (timestamp?: string) => {
  if (!timestamp) {
    return "-";
  }

  return new Date(timestamp).toLocaleString();
};

const formatPayload = (payload: unknown) => {
  if (payload === undefined || payload === null) {
    return "-";
  }

  return JSON.stringify(payload, null, 2);
};

onMounted(async () => {
  await initializePage();
});

onUnmounted(() => {
  void disconnectHub();
});

watch(smartHomeId, async () => {
  await initializePage();
});
</script>

<style scoped>
.log-card {
  min-height: 70vh;
}

.log-area {
  max-height: 70vh;
  overflow: auto;
}

.code-block {
  margin: 0;
  white-space: pre-wrap;
  word-break: break-word;
}
</style>
