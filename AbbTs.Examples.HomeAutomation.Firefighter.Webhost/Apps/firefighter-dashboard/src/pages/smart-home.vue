<template>
  <v-container class="py-8 smart-home-page" fluid>
    <v-row class="mb-4" justify="space-between" align="center">
      <v-col cols="12" md="8">
        <div class="text-overline text-primary">{{ t("smartHome.badge") }}</div>
        <h1 class="text-h4 font-weight-bold mb-2">
          {{ t("smartHome.title") }}
        </h1>
        <p class="text-body-1 text-medium-emphasis mb-0">
          {{ t("smartHome.subtitle") }}
        </p>
      </v-col>
      <v-col cols="12" md="4" class="d-flex justify-md-end ga-2">
        <v-btn prepend-icon="mdi-view-dashboard" to="/" variant="outlined">
          {{ t("smartHome.back") }}
        </v-btn>
        <v-btn
          :color="isConnected ? 'error' : 'success'"
          :prepend-icon="
            isConnected ? 'mdi-close-network-outline' : 'mdi-lan-connect'
          "
          variant="elevated"
          @click="isConnected ? disconnect() : connect()"
        >
          {{ isConnected ? t("smartHome.disconnect") : t("smartHome.connect") }}
        </v-btn>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" lg="5">
        <v-card class="mb-4" rounded="xl">
          <v-card-title class="d-flex align-center ga-2">
            <v-icon icon="mdi-transmission-tower" />
            {{ t("smartHome.connection") }}
          </v-card-title>
          <v-card-text>
            <v-chip
              :color="isConnected ? 'success' : 'warning'"
              size="small"
              variant="flat"
            >
              {{
                isConnected
                  ? t("smartHome.connected")
                  : t("smartHome.disconnected")
              }}
            </v-chip>
            <div class="mt-3 text-body-2 text-medium-emphasis">{{ wsUrl }}</div>
            <div class="mt-2 text-body-2">{{ connectionMessage }}</div>
          </v-card-text>
        </v-card>

        <v-card class="mb-4" rounded="xl">
          <v-card-title>{{ t("smartHome.quick") }}</v-card-title>
          <v-card-text>
            <v-alert
              v-if="isConnected && !smartHomeConnected"
              type="warning"
              variant="tonal"
              density="compact"
              class="mb-3"
              icon="mdi-home-off-outline"
            >
              {{ t("smartHome.smartHomeOffline") }}
            </v-alert>
            <div class="d-flex flex-wrap ga-2">
              <v-btn
                :disabled="!isConnected || !smartHomeConnected"
                @click="sendSimple('get state')"
              >
                {{ t("smartHome.getState") }}
              </v-btn>
              <v-btn
                :disabled="!isConnected || !smartHomeConnected"
                @click="sendSimple('get measurement')"
              >
                {{ t("smartHome.getMeasurement") }}
              </v-btn>
            </div>
          </v-card-text>
        </v-card>

        <v-card rounded="xl">
          <v-card-title>{{ t("smartHome.sendCommand") }}</v-card-title>
          <v-card-text>
            <v-select
              v-model="selectedDevice"
              :items="devices"
              :label="t('smartHome.device')"
              density="comfortable"
              variant="outlined"
            />
            <v-select
              v-model="selectedCommand"
              :items="availableCommands"
              :label="t('smartHome.command')"
              density="comfortable"
              variant="outlined"
            />
            <v-text-field
              v-model="commandValue"
              :disabled="!commandNeedsValue"
              :label="t('smartHome.value')"
              :placeholder="valuePlaceholder"
              density="comfortable"
              variant="outlined"
            />
            <v-btn
              block
              color="primary"
              :disabled="
                !isConnected || !smartHomeConnected || !selectedCommand
              "
              prepend-icon="mdi-send"
              variant="elevated"
              @click="sendCommand()"
            >
              {{ t("smartHome.send") }}
            </v-btn>
          </v-card-text>
        </v-card>
      </v-col>

      <v-col cols="12" lg="7">
        <v-card rounded="xl" class="log-card">
          <v-card-title class="d-flex align-center justify-space-between">
            <span>{{ t("smartHome.liveLog") }}</span>
            <v-btn size="small" variant="text" @click="messages = []">
              {{ t("smartHome.clear") }}
            </v-btn>
          </v-card-title>
          <v-divider />
          <v-card-text class="log-area">
            <v-alert v-if="messages.length === 0" type="info" variant="tonal">
              {{ t("smartHome.noMessages") }}
            </v-alert>
            <v-sheet
              v-for="item in messages"
              :key="item.id"
              class="pa-3 mb-3 rounded-lg"
              color="surface-variant"
            >
              <div class="d-flex align-center justify-space-between mb-1">
                <v-chip
                  size="x-small"
                  variant="flat"
                  :color="
                    item.direction === 'error'
                      ? 'error'
                      : item.direction === 'outbound'
                        ? 'secondary'
                        : item.direction === 'system'
                          ? 'warning'
                          : 'primary'
                  "
                  >{{ item.direction }}</v-chip
                >
                <span class="text-caption">{{ item.timestamp }}</span>
              </div>
              <div class="text-subtitle-2 mb-1">{{ item.messageType }}</div>
              <pre class="text-caption code-block">{{ item.payloadText }}</pre>
            </v-sheet>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts" setup>
import { computed, onUnmounted, ref } from "vue";
import { useI18n } from "vue-i18n";

type SmartHomeEnvelope = {
  messageType: string;
  payload?: unknown;
  receivedAtUtc?: string;
};

type DashboardCommand = {
  messageType: string;
  payload?: {
    device: string;
    command: string;
    value: string;
  };
};

type LogItem = {
  id: number;
  direction: "inbound" | "outbound" | "system" | "error";
  messageType: string;
  payloadText: string;
  timestamp: string;
};

const { t } = useI18n({ useScope: "global" });

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

const selectedDevice = ref("LightControl");
const selectedCommand = ref("on");
const commandValue = ref("");
const isConnected = ref(false);
const smartHomeConnected = ref(false);
const connectionMessage = ref("");
const messages = ref<LogItem[]>([]);

let socket: WebSocket | null = null;
let nextLogId = 1;

const wsUrl = computed(() => {
  const protocol = window.location.protocol === "https:" ? "wss" : "ws";
  return `${protocol}://${window.location.host}/smart-home/ws`;
});

const availableCommands = computed(
  () => commandMap[selectedDevice.value] ?? [],
);

const commandNeedsValue = computed(() => {
  return (
    selectedCommand.value === "setpoint" || selectedDevice.value === "Display"
  );
});

const valuePlaceholder = computed(() => {
  if (selectedDevice.value === "Display") {
    return "Line 1;Line 2";
  }

  if (selectedCommand.value === "setpoint") {
    return "e.g. 174";
  }

  return "";
});

const addLog = (
  direction: LogItem["direction"],
  messageType: string,
  payload: unknown,
) => {
  const timestamp = new Date().toLocaleTimeString();
  const payloadText =
    payload === undefined || payload === null
      ? "-"
      : JSON.stringify(payload, null, 2);

  messages.value.unshift({
    id: nextLogId++,
    direction,
    messageType,
    payloadText,
    timestamp,
  });

  if (messages.value.length > 120) {
    messages.value = messages.value.slice(0, 120);
  }
};

const connect = () => {
  if (socket && socket.readyState === WebSocket.OPEN) {
    return;
  }

  socket = new WebSocket(wsUrl.value);

  socket.onopen = () => {
    isConnected.value = true;
    connectionMessage.value = t("smartHome.ready");
    addLog("system", "connected", { url: wsUrl.value });
  };

  socket.onmessage = (event) => {
    try {
      const envelope = JSON.parse(event.data as string) as SmartHomeEnvelope;
      const direction = envelope.messageType.startsWith("outbound")
        ? "outbound"
        : envelope.messageType === "error"
          ? "error"
          : envelope.messageType.startsWith("system")
            ? "system"
            : "inbound";

      if (envelope.messageType === "system status" && envelope.payload) {
        const status = envelope.payload as { smartHomeConnected?: boolean };
        smartHomeConnected.value = status.smartHomeConnected ?? false;
      }

      addLog(direction, envelope.messageType, envelope.payload ?? null);
    } catch {
      addLog("system", "parse-error", event.data);
    }
  };

  socket.onclose = () => {
    smartHomeConnected.value = false;
  };

  socket.onerror = () => {
    connectionMessage.value = t("smartHome.error");
    addLog("system", "error", { message: "WebSocket error" });
  };

  socket.onclose = () => {
    isConnected.value = false;
    smartHomeConnected.value = false;
    connectionMessage.value = t("smartHome.closed");
    addLog("system", "disconnected", null);
  };
};

const disconnect = () => {
  if (!socket) {
    return;
  }

  socket.close(1000, "User disconnect");
  socket = null;
};

const sendRaw = (payload: DashboardCommand) => {
  if (!socket || socket.readyState !== WebSocket.OPEN) {
    connectionMessage.value = t("smartHome.notConnected");
    return;
  }

  socket.send(JSON.stringify(payload));
  addLog("outbound", payload.messageType, payload.payload ?? null);
};

const sendSimple = (messageType: "get state" | "get measurement") => {
  sendRaw({ messageType });
};

const sendCommand = () => {
  const payload = {
    device: selectedDevice.value,
    command: selectedDevice.value === "Display" ? "" : selectedCommand.value,
    value: commandNeedsValue.value ? commandValue.value : "",
  };

  sendRaw({
    messageType: "send command",
    payload,
  });
};

onUnmounted(() => {
  disconnect();
});
</script>

<style scoped>
.smart-home-page {
  min-height: 100vh;
  background:
    radial-gradient(
      circle at 10% 10%,
      rgba(65, 105, 179, 0.12),
      transparent 45%
    ),
    radial-gradient(
      circle at 90% 80%,
      rgba(236, 142, 56, 0.16),
      transparent 40%
    );
}

.log-card {
  height: 100%;
}

.log-area {
  max-height: 70vh;
  overflow-y: auto;
}

.code-block {
  white-space: pre-wrap;
  word-break: break-word;
  margin: 0;
}
</style>
