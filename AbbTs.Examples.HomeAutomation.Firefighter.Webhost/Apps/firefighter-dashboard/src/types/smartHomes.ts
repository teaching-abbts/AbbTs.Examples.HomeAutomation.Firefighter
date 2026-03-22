export type SmartHomeEnvelope = {
  messageType: string;
  payload?: unknown;
  receivedAtUtc?: string;
};

export type SmartHomeSummary = {
  id: string;
  owner: string;
  xCoordinate: number;
  yCoordinate: number;
  isConnected: boolean;
  connectedAtUtc?: string | null;
  lastSeenUtc?: string | null;
  recentMessageCount: number;
};

export type SmartHomeDetails = {
  id: string;
  owner: string;
  xCoordinate: number;
  yCoordinate: number;
  isConnected: boolean;
  connectedAtUtc?: string | null;
  lastSeenUtc?: string | null;
  recentEnvelopes: SmartHomeEnvelope[];
};

export type SmartHomeCommand = {
  device: string;
  command: string;
  value: string;
};
