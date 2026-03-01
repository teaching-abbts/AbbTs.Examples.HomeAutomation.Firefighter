import { createI18n } from "vue-i18n";

const messages = {
  en: {
    dashboard: {
      title: "Firefighter Dashboard",
      loading: "Loading...",
      sections: {
        events: "Events",
        actions: "Actions",
      },
      houseName: "House {id}",
      events: {
        fire: "Fire!",
        gas: "Gas!",
        motion: "Motion!",
        sound: "Sound!",
        rfid: "RFID!",
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
        open: "Open",
        done: "Done",
        markDone: "Mark done",
        reopen: "Reopen",
        onlyOpen: "Only open actions",
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
      houseDetails: {
        title: "House details",
        timestamp: "Timestamp",
        type: "Type",
        details: "Details",
        noData: "No history data available",
        error: "History could not be loaded",
        close: "Close",
      },
    },
    message: {
      hello: "hello world",
    },
  },
  de: {
    dashboard: {
      title: "Feuerwehr-Dashboard",
      loading: "Lädt...",
      sections: {
        events: "Ereignisse",
        actions: "Aktionen",
      },
      houseName: "Haus {id}",
      events: {
        fire: "Feuer!",
        gas: "Gas!",
        motion: "Bewegung!",
        sound: "Geräusch!",
        rfid: "RFID!",
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
        open: "Offen",
        done: "Erledigt",
        markDone: "Als erledigt markieren",
        reopen: "Wieder öffnen",
        onlyOpen: "Nur offene Aktionen",
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
      houseDetails: {
        title: "Hausdetails",
        timestamp: "Zeitstempel",
        type: "Typ",
        details: "Details",
        noData: "Keine Verlaufsdaten verfügbar",
        error: "Verlauf konnte nicht geladen werden",
        close: "Schließen",
      },
    },
    message: {
      hello: "Hallo Welt",
    },
  },
  ja: {
    dashboard: {
      title: "消防ダッシュボード",
      loading: "読み込み中...",
      sections: {
        events: "イベント",
        actions: "アクション",
      },
      houseName: "家 {id}",
      events: {
        fire: "火災!",
        gas: "ガス!",
        motion: "動体!",
        sound: "音!",
        rfid: "RFID!",
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
        open: "未完了",
        done: "完了",
        markDone: "完了にする",
        reopen: "再オープン",
        onlyOpen: "未完了のみ表示",
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
      houseDetails: {
        title: "家の詳細",
        timestamp: "タイムスタンプ",
        type: "種類",
        details: "詳細",
        noData: "履歴データがありません",
        error: "履歴を読み込めませんでした",
        close: "閉じる",
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
