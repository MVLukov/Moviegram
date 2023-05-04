<script>
import axios from 'axios';
import Post from "@/components/Post.vue";

export default {
    name: "Discover",
    components: { Post },
    data() {
        return {
            showFlag: false,
            publishStatus: "",
            page: 1,
            posts: [],
        }
    },
    watch: {
        usersSearch(newInput, oldInput) {
            if (newInput) {
                if (!this.debounce) {
                    this.debounce = true;

                    setTimeout(() => {
                        this.getUsers();
                        this.debounce = false;
                    }, 500)
                }
            } else {
                this.usersFound = {};
            }
        }
    },
    mounted() {
        this.fetchPosts();

        this.fetchPosts();

        window.onscroll = () => {
            let bottomOfWindow = document.documentElement.scrollTop + window.innerHeight === document.documentElement.offsetHeight;

            if (bottomOfWindow) {
                this.page++;
                this.fetchPosts()
            }
        }
    },
    unmounted() {
        window.onscroll = () => {
            return;
        }
    },
    methods: {
        async fetchPosts() {
            let result = await axios.get("/api/posts/discover", {
                params: { page: parseInt(this.page) }
            });

            if (this.page == 1 && result.data.length == 0) {
                this.posts = [];
            } else if (this.page == 1 && result.data.length > 0) {
                this.posts = [];
                this.posts.push(...result.data);
            } else if (result.data.length == 0) {
                this.page--;
            } else {
                this.posts.push(...result.data);
            }
        },
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
    <div class="container mt-3">
        <div class="container mt-3">
            <h2 class="text-center mb-4">Discover</h2>
            <Post v-for="post in posts" :post="post" :key="post.id" @fetch-posts="fetchPosts"></Post>
        </div>
    </div>
</template>