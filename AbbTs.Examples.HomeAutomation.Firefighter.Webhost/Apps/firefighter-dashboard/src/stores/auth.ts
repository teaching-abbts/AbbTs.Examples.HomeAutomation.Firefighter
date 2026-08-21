import { computed, ref } from "vue";
import { defineStore } from "pinia";

import { Client } from "@/api/AbbTs.Examples.HomeAutomation.Firefighter.Webhost";
import type { CurrentUser } from "@/types/auth";

const ANONYMOUS_USER: CurrentUser = {
  isAuthenticated: false,
  userName: null,
  displayName: null,
  roles: [],
};

export const useAuthStore = defineStore("auth", () => {
  const currentUser = ref<CurrentUser>(ANONYMOUS_USER);
  const apiClient = new Client();

  const isAuthenticated = computed(function () {
    return currentUser.value.isAuthenticated;
  });
  const isOperator = computed(function () {
    return currentUser.value.roles.includes("operator");
  });

  async function fetchCurrentUser() {
    try {
      const response = await apiClient.getAccountUser();
      currentUser.value = {
        isAuthenticated: response.isAuthenticated ?? false,
        userName: response.userName ?? null,
        displayName: response.displayName ?? null,
        roles: response.roles ?? [],
      };
    } catch {
      currentUser.value = ANONYMOUS_USER;
    }
  }

  function login() {
    // Full-page navigation: the backend redirects the browser to Authentik's hosted login page.
    const returnUrl = `${location.pathname}${location.search}`;
    location.href = `/account/login?returnUrl=${encodeURIComponent(returnUrl)}`;
  }

  function logout() {
    location.href = "/account/logout";
  }

  return {
    currentUser,
    isAuthenticated,
    isOperator,
    fetchCurrentUser,
    login,
    logout,
  };
});
