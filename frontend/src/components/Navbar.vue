<script>
import { useUserStore } from "@/stores/user.js";
import axios from "axios";
import { RouterLink, RouterView } from "vue-router";

export default {
  setup() {
    const userStore = useUserStore();
    return { userStore };
  },
  data() {
    return {
      debounce: false,
      usersFound: {},
      usersSearch: "",
    };
  },
  watch: {
    usersSearch(newInput, oldInput) {
      if (!newInput) {
        this.usersFound = {};
      }
    },
  },
  computed: {
    isAuthenticated() {
      return this.userStore.isAuthenticated;
    },
  },
  methods: {
    logout() {
      axios.get("/api/users/logout", {
        withCredentials: true,
        validateStatus: () => true,
      });

      this.userStore.resetUser();
      this.$router.push("/signin");
    },
    async searchUsers() {
      if (this.searchUsers) {
        let req = await axios("/api/users/getUsersByUsername", {
          params: { username: this.usersSearch },
        });

        this.usersFound = req.data;
      }
    },
    selectUser(id) {
      this.$router.push(`/user/${id}`);
      this.usersSearch = "";
      this.usersFound = [];
    },
  },
};
</script>

<template>
  <nav class="navbar navbar-dark bg-dark navbar-expand-lg py-2">
    <div class="container-fluid">
      <router-link to="/" class="navbar-brand text-light fs-4"
        >Moviegram</router-link
      >

      <button
        class="navbar-toggler"
        type="button"
        data-bs-toggle="collapse"
        data-bs-target="#navbarSupportedContent"
        aria-controls="navbarSupportedContent"
        aria-expanded="false"
        aria-label="Toggle navigation"
      >
        <span class="navbar-toggler-icon"></span>
      </button>

      <div class="collapse navbar-collapse" id="navbarSupportedContent">
        <ul
          :class="[
            'navbar-nav',
            {
              'me-auto': !isAuthenticated,
            },
          ]"
        >
          <li class="nav-item" v-if="isAuthenticated">
            <router-link to="/" class="nav-link">Home</router-link>
          </li>
          <li class="nav-item dropdown">
            <a
              class="nav-link dropdown-toggle"
              href="#"
              role="button"
              data-bs-toggle="dropdown"
              aria-expanded="false"
            >
              Movies and series
            </a>
            <ul class="dropdown-menu dropdown-menu-dark">
              <li class="nav-item">
                <router-link to="/topRated" class="dropdown-item"
                  >Top 250 rated</router-link
                >
              </li>
              <li class="nav-item">
                <router-link to="/mostPopMovies" class="dropdown-item"
                  >Most pop movies</router-link
                >
              </li>
              <li class="nav-item">
                <router-link to="/mostPopSeries" class="dropdown-item"
                  >Most pop series</router-link
                >
              </li>
              <li class="nav-item">
                <router-link to="/topBOlastWeekend" class="dropdown-item"
                  >Top BO last weekend</router-link
                >
              </li>
            </ul>
          </li>
        </ul>

        <div class="d-flex" v-if="!isAuthenticated">
          <ul class="navbar-nav me-auto mb-2 mb-lg-0">
            <li class="nav-item">
              <router-link to="/signin" class="nav-link text-light"
                >Sign in</router-link
              >
            </li>
            <li class="nav-item">
              <router-link to="/signup" class="nav-link text-light"
                >Sign up</router-link
              >
            </li>
          </ul>
        </div>

        <form
          @submit.prevent="searchUsers"
          class="ms-lg-3 me-lg-auto"
          role="search"
          v-if="isAuthenticated"
        >
          <div class="input-group input-group-sm">
            <input
              type="text"
              class="form-control"
              placeholder="Search user"
              v-model.trim="usersSearch"
            />
            <button class="btn btn-outline-success" type="submit">
              Search
            </button>
          </div>
          <div class="search-list">
            <ul class="list-group mt-1">
              <li
                :class="['list-group-item']"
                v-for="{ publicId, username } in usersFound"
                @click="selectUser(publicId)"
              >
                {{ username }}
              </li>
            </ul>
          </div>
        </form>

        <div class="d-flex" v-if="isAuthenticated">
          <ul class="navbar-nav me-auto mb-2 mb-lg-0">
            <li class="nav-item me-1">
              <router-link to="/myProfile" class="nav-link">
                Me: {{ userStore.getUserDetails.username }}
              </router-link>
            </li>
            <li class="nav-item">
              <a
                href="javascript:void(0)"
                @click="logout"
                class="btn btn-primary"
                >Logout</a
              >
            </li>
          </ul>
        </div>
        <!-- <form class="d-flex" role="search">
              <input class="form-control me-2" type="search" placeholder="Search" aria-label="Search">
              <button class="btn btn-success" type="submit">Search</button>
            </form> -->
      </div>
    </div>
  </nav>
</template>

<style scoped>
.search-list {
  position: absolute !important;
  top: 100%;
  max-width: 280px;
  width: 100%;
  z-index: 10;
}

.list-group-item:hover {
  background-color: #0d6efd;
  color: white;
}
</style>
