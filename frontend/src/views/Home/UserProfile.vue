<script>
import axios from 'axios';
import { useUserStore } from '@/stores/user';
import PostVue from '@/components/Post.vue';

export default {
    name: "UserProfile",
    components: { PostVue },
    props: ['pushPostUpdate'],
    setup() {
        const userStore = useUserStore();
        return { userStore }
    },
    data() {
        return {
            user: {},
            posts: [],
            userId: "",
            page: 1
        }
    },
    watch: {
        pushPostUpdate(newFlag, oldFlag) {
            if (newFlag) {
                this.fetchPersonalPosts();
            }
        }
    },
    mounted() {
        this.userId = this.$route.params?.id;

        this.fetchUser();
        this.fetchPersonalPosts();

        window.onscroll = () => {
            let bottomOfWindow = document.documentElement.scrollTop + window.innerHeight === document.documentElement.offsetHeight;

            if (bottomOfWindow) {
                this.page++;
                this.fetchPersonalPosts()
            }
        }
    },
    unmounted() {
        window.onscroll = () => {
            return;
        }
    },
    methods: {
        async fetchUser() {
            let req = await axios.get("/api/users/getUserById", {
                params: { userId: this.userId },
                validateStatus: () => true
            });

            if (req.status == 200) {
                this.user = req.data;
            }
            else {
                this.$router.push("/");
            }
        },
        async fetchPersonalPosts() {
            console.log("test");
            let token = this.userStore.getJwt;

            let req = await axios.get("/api/posts/getPostByAuthorId", {
                headers: {
                    Authorization: `Bearer ${token}`
                },
                params: { authorId: this.userId, page: parseInt(this.page) },
                validateStatus: () => true
            });

            if (req.status == 200) {
                if (this.page == 1 && req.data.length == 0) {
                    this.posts = [];
                } else if (this.page == 1 && req.data.length > 0) {
                    this.posts = [];
                    this.posts.push(...req.data);
                } else if (req.data.length == 0) {
                    this.page--;
                } else {
                    this.posts.push(...req.data);
                }
            }
            else {
                this.$router.push("/");
            }
        }
    },
}

</script>
<template>
    <div class="container mt-3">
        <div class="card w-50 mx-auto">
            <div class="card-body">
                <img src="https://cdn-icons-png.flaticon.com/512/21/21104.png" class="float-start profile-pic"
                    height="50" width="50" />
                <h3 class="card-title d-inline-block ms-2">
                    <span class="align-middle">{{ user.username }}</span>
                </h3>
                <p class="d-inline-block mx-3 float-end mb-0">Following:
                    <span class="d-block text-center">
                        {{ user?.following?.length }}
                    </span>
                </p>
                <p class="d-inline-block float-end mb-0">Followers:
                    <span class="d-block text-center">
                        {{ user?.followers?.length }}
                    </span>
                </p>
            </div>
        </div>
        <div class="my-3">
            <PostVue v-for="post in posts" :key="post.id" :post="post" @fetch-posts="fetchPersonalPosts"></PostVue>
        </div>
    </div>
</template>

<style scoped>
.card {
    min-width: 23rem;
}
</style>