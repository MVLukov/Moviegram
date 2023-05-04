<script>
// import PostModal from './PostModal.vue';
import axios from 'axios';
import PostModalVue from './PostModal.vue';
import { useUserStore } from '../stores/user';

export default {
    name: "Post",
    props: ["post", "id", "key"],
    emits: ["fetchPosts"],
    components: { PostModalVue },
    setup() {
        const userStore = useUserStore();
        return { userStore };
    },
    data() {
        return {
            flag: false,
            currentData: {},
            publishStatus: ""
        }
    },
    mounted() {
        this.fetchMovie();
    },
    watch: {
        post() {
            this.fetchMovie();
        }
    },
    computed: {
        isEditedAt() {
            return this.post.editedAt == null ? false : true;
        },
        isAuthor() {
            let userId = this.userStore.getUserDetails?.publicId;
            let authorId = this.post.authorId;

            return userId == authorId;
        },
        isFollowing() {
            let authorId = this.post.authorId;

            let isFollowing = this.userStore.getUserDetails?.following?.find(x => x.publicId == authorId);

            if (isFollowing) {
                return true;
            }
            return false;
        }
    },
    methods: {
        edit() {
            this.flag = !this.flag;
            this.currentData = {
                title: this.post.title,
                description: this.post.description,
                movieId: this.post.movieId
            }
        },
        async deletePost() {
            let data = JSON.stringify(this.post);

            await axios.delete("/api/posts/delete", {
                headers: {
                    'Content-Type': "application/json"
                },
                data: data
            });

            this.$emit("fetchPosts");

        },
        close(data) {
            this.flag = false;
            this.publishStatus = "";
        },
        async submit({ title, description, movieId, rating }) {
            if (title && description && movieId) {
                let data = JSON.stringify({
                    title,
                    description,
                    movieId,
                    id: this.post.id,
                    personalRating: rating
                });

                let req = await axios.patch("/api/posts/update", data, {
                    validateStatus: () => true,
                    headers: {
                        "Content-Type": "application/json"
                    }
                });

                let { status } = req;

                if (status == 200) {
                    this.publishStatus = "Post updated successfully!";
                } else if (status == 400) {
                    this.publishStatus = "Something went wrong!"
                } else if (status == 401 || status == 404) {
                    this.$router.push("/");
                }

                this.$emit("fetchPosts");
            }
        },
        async fetchMovie() {
            const getBaseInfo = {
                method: 'GET',
                url: `https://moviesdatabase.p.rapidapi.com/titles/${this.post.movieId}`,
                params: { info: "base_info" },
                headers: {
                    'X-RapidAPI-Key': '95d1eab7ddmsh0f51c34950daa93p1a8f5cjsnb298001e3f6f',
                    'X-RapidAPI-Host': 'moviesdatabase.p.rapidapi.com'
                }
            };

            axios.request(getBaseInfo).then((req) => {
                let movie = req.data.results;

                if (movie.primaryImage.url != undefined) {
                    let img = movie.primaryImage;
                    delete movie.primaryImage;

                    let splitted = img?.url.split("@");
                    let newImgUrl = "";

                    if (img != null) {
                        newImgUrl = `${splitted.splice(0, 1)}${'@'.repeat(splitted.length)}._V1_QL100_UY500_UX500.jpg`

                    }

                    Object.assign(this.post, {
                        url: newImgUrl
                    });
                }
            })
        },
        async toggleFollow() {
            if (!this.isFollowing) {
                let res = await axios.get("/api/users/follow", {
                    params: { userIdToFollow: this.post.authorId }
                });

                let getUserData = await axios.get("/api/users/getUserById", {
                    params: { userId: this.userStore.getUserDetails.publicId },
                    validateStatus: () => true
                });

                if (getUserData.status == 200) {
                    this.userStore.setUserDetails(getUserData.data);
                }
            } else {
                let res = await axios.get("/api/users/unfollow", {
                    params: { userIdToUnfollow: this.post.authorId }
                });

                let getUserData = await axios.get("/api/users/getUserById", {
                    params: { userId: this.userStore.getUserDetails.publicId },
                    validateStatus: () => true
                });

                if (getUserData.status == 200) {
                    this.userStore.setUserDetails(getUserData.data);
                }
            }

            this.$emit("fetchPosts");
        }
    }
}
</script>

<template>
    <div class="card w-50 mx-auto mb-3">
        <div class="card-header d-flex align-items-center" style="width: 100%;">
            <img src="https://cdn-icons-png.flaticon.com/512/21/21104.png" class="profile-pic" height="35" width="35" />
            <h4 class="ms-2 flex-grow-1">
                <router-link :to="`/user/${post.authorId}`" class="link-dark align-middle">
                    {{ post.authorName }}
                </router-link>
                <a v-if="!isAuthor" @click="toggleFollow" href="javascript:void(0)" class="ms-1 fs-6">
                    <span v-if="isFollowing" class="text-muted">Following</span>
                    <span v-if="!isFollowing">Follow</span>
                </a>
            </h4>
            <div v-if="isAuthor" class="btn-group flex-grow-0" role="group">
                <button type="button" class="btn btn-sm btn-success" @click="edit()"><span
                        class="align-middle">Edit</span></button>
                <button type="button" class="btn btn-sm btn-danger" @click="deletePost()"><span
                        class="align-middle">Delete</span></button>
            </div>
        </div>

        <div class="card-body">
            <img :src="post.url" class="rounded movie-pic mb-1 ms-1" alt="Image not found" width="300" height="300">
            <h4 class="card-title text-break">{{ post.title }}</h4>
            <span class="badge text-bg-info">
                {{ new Date(post.publishedAt).toLocaleString() }}
            </span>
            <span class="badge text-bg-warning ms-1">
                {{ post.personalRating }}
            </span>
            <h6 class="card-text mt-4 fw-semibold">Description</h6>
            <p class="card-text">{{ post.description }}</p>
            <div v-html="`<script>alert('test')</script>`"></div>
        </div>
        <div class="card-footer">
            <span v-if="isEditedAt" class="text-muted">Last edit:
                {{ new Date(post.editedAt).toLocaleString() }}
            </span>

            <router-link :to="`/movie/${post.movieId}`" class="float-end">
                Check this movie
            </router-link>
        </div>
    </div>

    <PostModalVue v-if="flag" :isOpen="true" :currentData="currentData" :publishStatus="publishStatus" btnText="Edit"
        @close="close" @submit="submit">
    </PostModalVue>
</template>

<style scoped>
.movie-pic {
    width: 115px;
    height: 150px;
    float: right;
    border: 2px solid #d2d2d2;
}

.card {
    min-width: 23rem;
    /* max-width: 35rem; */
}
</style>