import { createI18n } from "vue-i18n";

const messages = {
  en: {
    dashboard: {
      sections: {
        events: "Events",
        actions: "Actions",
      },
      houseName: "House {id}",
      events: {
        fire: "Fire!",
        gas: "Gas!",
      },
      houseStatus: {
        fireDetected: "Fire detected",
        monitoring: "Monitoring",
        safe: "Safe",
        gasAlert: "Gas alert",
      },
      actions: {
        extinguishFire: "Extinguish fire",
        openDoors: "Open doors",
        execute: "Execute",
        observe: "Observe",
      },
      language: {
        label: "Language",
        german: "Deutsch",
        english: "English",
        japanese: "日本語",
      },
      theme: {
        label: "Theme",
        bright: "Bright",
        dark: "Dark",
      },
    },
    message: {
      hello: "hello world",
    },
  },
  de: {
    dashboard: {
      sections: {
        events: "Ereignisse",
        actions: "Aktionen",
      },
      houseName: "Haus {id}",
      events: {
        fire: "Feuer!",
        gas: "Gas!",
      },
      houseStatus: {
        fireDetected: "Feuer erkannt",
        monitoring: "Beobachtung",
        safe: "Sicher",
        gasAlert: "Gasalarm",
      },
      actions: {
        extinguishFire: "Brandlöschung",
        openDoors: "Türen öffnen",
        execute: "Ausführen",
        observe: "Beobachten",
      },
      language: {
        label: "Sprache",
        german: "Deutsch",
        english: "English",
        japanese: "日本語",
      },
      theme: {
        label: "Design",
        bright: "Hell",
        dark: "Dunkel",
      },
    },
    message: {
      hello: "Hallo Welt",
    },
  },
  ja: {
    dashboard: {
      sections: {
        events: "イベント",
        actions: "アクション",
      },
      houseName: "家 {id}",
      events: {
        fire: "火災!",
        gas: "ガス!",
      },
      houseStatus: {
        fireDetected: "火災検知",
        monitoring: "監視中",
        safe: "安全",
        gasAlert: "ガス警報",
      },
      actions: {
        extinguishFire: "消火",
        openDoors: "ドアを開く",
        execute: "実行",
        observe: "監視",
      },
      language: {
        label: "言語",
        german: "Deutsch",
        english: "English",
        japanese: "日本語",
      },
      theme: {
        label: "テーマ",
        bright: "ライト",
        dark: "ダーク",
      },
    },
    message: {
      hello: "こんにちは、世界",
    },
  },
};

export default createI18n({
  legacy: false,
  locale: "de",
  fallbackLocale: "en",
  messages,
});
