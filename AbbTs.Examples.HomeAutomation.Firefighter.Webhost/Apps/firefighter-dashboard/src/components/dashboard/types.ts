export type EventItem = {
  id: number;
  titleKey: string;
  houseNumber: number;
  time: string;
  icon: string;
  color: string;
  textColor: string;
};

export type HouseItem = {
  id: number;
  number: number;
  statusKey: string;
  statusIcon: string;
  color: string;
  textColor: string;
};

export type ActionItem = {
  id: number;
  titleKey: string;
  houseNumber: number;
  color: string;
  textColor: string;
};

export type ObservedHouseItem = {
  id: number;
  number: number;
  active: boolean;
};
