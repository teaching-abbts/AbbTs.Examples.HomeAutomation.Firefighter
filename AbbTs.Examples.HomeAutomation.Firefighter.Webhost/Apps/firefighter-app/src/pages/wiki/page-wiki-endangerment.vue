<template>
  <v-container fluid>
    <h2>{{ $t("wiki.endangerment.title") }}</h2>
    <p>{{ $t("wiki.endangerment.description") }}</p>
    <ul>
      <li>{{ $t("wiki.endangerment.normal") }}</li>
      <li>{{ $t("wiki.endangerment.warning") }}</li>
      <li>{{ $t("wiki.endangerment.critical") }}</li>
    </ul>
  </v-container>
</template>
