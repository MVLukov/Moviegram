<script>
import { RouterLink, RouterView } from 'vue-router'
import Navbar from "./components/Navbar.vue"

export default {
  components: { Navbar }
}

</script>

<template>
  <Navbar></Navbar>

  <router-view :key="$route.fullPath"></router-view>
</template>

<style>
.profile-pic {
  background-color: #444444;
  border-radius: 50%;
  padding: 5px;
}

.modal {
  overflow: inherit !important;
}

.modal-open {
  overflow-y: auto !important;
  padding: 0 !important;
}

.link-dark {
  text-decoration: none;
}

body {
  overflow-y: scroll;
  overflow-x: hidden;
}
</style>
