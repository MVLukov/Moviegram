import { defineStore } from "pinia";
import { useStorage } from "@vueuse/core";

export const useUserStore = defineStore("user", {
  state: () => ({
    auth: useStorage("auth", false),
    userDetail: useStorage("userDetails", {}),
  }),
  getters: {
    isAuthenticated() {
      return this.auth;
    },
    getUserDetails() {
      return this.userDetail;
    },
  },
  actions: {
    setAuth(flag) {
      this.auth = flag;
    },
    setUserDetails(details) {
      this.userDetail = details;
    },
    resetUser() {
      this.auth = false;
      this.userDetail = {};
    },
  },
});
