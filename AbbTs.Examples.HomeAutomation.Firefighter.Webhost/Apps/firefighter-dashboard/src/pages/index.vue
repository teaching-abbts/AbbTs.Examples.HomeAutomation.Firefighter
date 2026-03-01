<template>
  <v-layout class="dashboard-layout">
    <v-navigation-drawer permanent width="260">
      <div class="px-4 py-4 text-h5 font-weight-bold">Ereignisse</div>

      <v-list class="px-2">
        <v-list-item v-for="event in events" :key="event.id" class="mb-3 px-0" lines="three">
          <v-card :color="event.color" class="w-100" rounded="lg" variant="flat">
            <v-card-item>
              <v-card-title class="d-flex align-center ga-2 pb-1">
                <v-icon :icon="event.icon" />
                {{ event.title }}
              </v-card-title>
              <v-card-subtitle>{{ event.houseName }}</v-card-subtitle>
              <div class="text-body-2 mt-1">{{ event.time }}</div>
            </v-card-item>
          </v-card>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>

    <v-main>
      <v-container class="py-8" fluid>
        <v-row>
          <v-col v-for="house in houses" :key="house.id" cols="12" md="4" sm="6">
            <v-card :color="house.color" class="house-card" rounded="lg" variant="tonal">
              <v-card-item>
                <div class="d-flex align-center justify-space-between">
                  <v-icon icon="mdi-home-city-outline" size="44" />
                  <v-icon :icon="house.statusIcon" size="28" />
                </div>
                <v-card-title class="pt-4 text-h4">{{ house.name }}</v-card-title>
                <v-card-subtitle>{{ house.statusText }}</v-card-subtitle>
              </v-card-item>
            </v-card>
          </v-col>
        </v-row>
      </v-container>
    </v-main>

    <v-navigation-drawer location="right" permanent width="300">
      <div class="px-4 py-4 text-h5 font-weight-bold">Aktionen</div>

      <div class="px-3 pb-3">
        <v-card
          v-for="action in actions"
          :key="action.id"
          :color="action.color"
          class="mb-4"
          rounded="lg"
          variant="flat"
        >
          <v-card-item>
            <v-card-title class="text-h6">{{ action.title }}</v-card-title>
            <v-card-subtitle>{{ action.houseName }}</v-card-subtitle>
          </v-card-item>

          <v-card-actions>
            <v-btn block color="surface" variant="elevated">Ausführen</v-btn>
          </v-card-actions>
        </v-card>

        <v-card rounded="lg" variant="tonal">
          <v-card-title>Beobachten</v-card-title>
          <v-list class="pt-0" density="comfortable">
            <v-list-item v-for="house in observedHouses" :key="house.id" :title="house.name">
              <template #append>
                <v-icon v-if="house.active" color="success" icon="mdi-check-bold" />
              </template>
            </v-list-item>
          </v-list>
        </v-card>
      </div>
    </v-navigation-drawer>
  </v-layout>
</template>

<script lang="ts" setup>
  type EventItem = {
    id: number
    title: string
    houseName: string
    time: string
    icon: string
    color: string
  }

  type HouseItem = {
    id: number
    name: string
    statusText: string
    statusIcon: string
    color: string
  }

  type ActionItem = {
    id: number
    title: string
    houseName: string
    color: string
  }

  type ObservedHouseItem = {
    id: number
    name: string
    active: boolean
  }

  const events: EventItem[] = [
    {
      id: 1,
      title: 'Feuer!',
      houseName: 'Haus 1',
      time: '17.11.2033 14:34',
      icon: 'mdi-fire',
      color: 'deep-orange-lighten-1',
    },
    {
      id: 2,
      title: 'Gas!',
      houseName: 'Haus 5',
      time: '17.11.2033 14:31',
      icon: 'mdi-alert',
      color: 'amber-darken-1',
    },
  ]

  const houses: HouseItem[] = [
    { id: 1, name: 'Haus 1', statusText: 'Feuer erkannt', statusIcon: 'mdi-fire', color: 'deep-orange-lighten-1' },
    { id: 2, name: 'Haus 2', statusText: 'Beobachtung', statusIcon: 'mdi-eye-outline', color: 'blue-lighten-1' },
    { id: 3, name: 'Haus 3', statusText: 'Sicher', statusIcon: 'mdi-check-circle-outline', color: 'light-green-darken-1' },
    { id: 4, name: 'Haus 4', statusText: 'Beobachtung', statusIcon: 'mdi-eye-outline', color: 'blue-lighten-1' },
    { id: 5, name: 'Haus 5', statusText: 'Gasalarm', statusIcon: 'mdi-alert', color: 'amber-darken-1' },
    { id: 6, name: 'Haus 6', statusText: 'Beobachtung', statusIcon: 'mdi-eye-outline', color: 'blue-lighten-1' },
  ]

  const actions: ActionItem[] = [
    { id: 1, title: 'Brandlöschung', houseName: 'Haus 1', color: 'deep-orange-lighten-1' },
    { id: 2, title: 'Türen öffnen', houseName: 'Haus 5', color: 'amber-darken-1' },
  ]

  const observedHouses: ObservedHouseItem[] = [
    { id: 2, name: 'Haus 2', active: false },
    { id: 4, name: 'Haus 4', active: false },
    { id: 6, name: 'Haus 6', active: false },
    { id: 3, name: 'Haus 3', active: true },
  ]
</script>

<style scoped>
  .dashboard-layout {
    min-height: 100vh;
  }

  .house-card {
    min-height: 180px;
  }
</style>
