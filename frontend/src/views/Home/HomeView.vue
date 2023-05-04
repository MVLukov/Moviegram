<script>
import PostModalVue from '@/components/PostModal.vue';
import Post from "@/components/Post.vue";
import { useUserStore } from '@/stores/user';
import axios from 'axios';

export default {
  components: { PostModalVue, Post },
  setup() {
    const userStore = useUserStore();
    return { userStore }
  },
  data() {
    return {
      showFlag: false,
      publishStatus: "",
      page: 1,
      posts: [],
      pushPostUpdate: false
    }
  },
  methods: {
    async submit({ title, description, movieId, rating }) {
      this.pushPostUpdate = false;

      if (title && description && movieId) {
        let data = JSON.stringify({ title, description, movieId, personalRating: rating })

        let result = await axios.post("/api/posts/add", data, {
          headers: {
            'Content-Type': "application/json",
          },
          withCredentials: true,
          validateStatus: () => true
        });

        if (result.status == 200) {
          this.publishErr = false;
          this.publishStatus = "Post published successfully!";
        }
        if (result.status == 403) {
          this.publishErr = true;
          this.publishStatus = "You can't add posts!";
        }
        if (result.status == 401) this.$router.push("/")

        this.page = 1;
        this.posts = [];

        let personalUserProfilePath = `/user/${this.userStore.getUserDetails?.publicId}`

        if (this.$router.currentRoute._value.path == personalUserProfilePath) {
          this.pushPostUpdate = true;
        }
      }
    },
    // async fetchPosts() {
    //   let result = await axios.get("/api/posts/feed", {
    //     params: { page: parseInt(this.page) }
    //   });

    //   if (this.page == 1 && result.data.length == 0) {
    //     this.posts = [];
    //   } else if (this.page == 1 && result.data.length > 0) {
    //     this.posts = [];
    //     this.posts.push(...result.data);
    //   } else if (result.data.length == 0) {
    //     this.page--;
    //   } else {
    //     this.posts.push(...result.data);
    //   }
    // },
    toggle() {
      this.showFlag = true;
      this.publishStatus = "";
    },
    close() {
      this.showFlag = false;
      this.publishStatus = "";
    },
  }
}
</script>

<template>
  <nav class="navbar navbar-expand py-0" style="background-color: #e3f2fd;">
    <div class="container-fluid">
      <ul class="navbar-nav mx-auto">
        <li class="nav-item">
          <router-link to="/" class="nav-link fs-5" aria-current="page">My feed</router-link>
        </li>
        <li class="nav-item">
          <router-link to="/discover" class="nav-link fs-5" aria-current="page">Discover</router-link>
        </li>
        <li class="nav-item me-1">
          <router-link class="nav-link fs-5" :to="`/user/${userStore.getUserDetails?.publicId}`">My
            posts</router-link>
        </li>
      </ul>
    </div>
  </nav>
  <div class="container mt-3">
    <PostModalVue v-if="showFlag" :isOpen="showFlag" :publishStatus="publishStatus" btnText="Publish" @submit="submit"
      @close="close">
    </PostModalVue>
    <button class="btn btn-primary btn-modal" @click="toggle">
      <img class="mx-auto" src="@/assets/plus-solid.svg" width="50" height="50" alt="not found">
    </button>

    <router-view :pushPostUpdate="pushPostUpdate"></router-view>
  </div>
</template>

<style scoped>
.btn-modal {
  position: fixed;
  z-index: 10;
  bottom: 0;
  right: 0;
  border-radius: 50%;
  margin: 35px;
  padding: 5px;
}

body {
  overflow: visible !important;
}
</style>