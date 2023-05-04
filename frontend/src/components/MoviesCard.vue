<script>
import axios from "axios";

export default {
    data() {
        return {
            movies: [],
            apiError: false,
            page: 1,
            isPreviousDisabled: true
        }
    },
    props: ["apiEndpoint", "title", "apiParams"],
    methods: {
        async fetchMovies(page = 1) {
            this.movies = [];
            const options = {
                method: 'GET',
                url: `https://moviesdatabase.p.rapidapi.com/${this.apiEndpoint}`,
                params: { list: this.apiParams, info: "base_info", limit: "9", page },
                headers: {
                    'X-RapidAPI-Key': '95d1eab7ddmsh0f51c34950daa93p1a8f5cjsnb298001e3f6f',
                    'X-RapidAPI-Host': 'moviesdatabase.p.rapidapi.com'
                }
            };

            try {
                let req = await axios.request(options);

                if (req.status == 200) {
                    if (req.data.error != undefined) {
                        console.log(req.data.error);
                        this.apiError = true;
                    } else {
                        // this.movies = req.data.results;
                        req.data.results.forEach(e => {
                            let img = e?.primaryImage;
                            let movie = e;
                            delete movie.primaryImage;

                            let splitted = img?.url.split("@");
                            let newImgUrl = "";

                            if (img != null) {
                                newImgUrl = `${splitted.splice(0, 1)}${'@'.repeat(splitted.length)}._V1_QL80_UY500_UX500.jpg`;
                            }

                            Object.assign(movie, {
                                primaryImage: {
                                    url: newImgUrl
                                }
                            })

                            this.movies.push(movie)
                        })
                        this.apiError = false;
                    }
                }
            } catch (error) {
                console.log(error);
                this.apiError = true;
            }

        },
        nextPage() {
            this.page++;
            this.isPreviousDisabled = false;
            this.fetchMovies(this.page);
            this.scrollToTop();
        },
        previousPage() {
            if (this.page > 1) this.page--;
            if (this.page == 1) this.isPreviousDisabled = true;
            this.fetchMovies(this.page);
            this.scrollToTop();
        },
        homePage() {
            this.page = 1;
            this.fetchMovies();
            this.scrollToTop();
        },
        scrollToTop() {
            window.scrollTo(0, 0);
        }
    },
    async mounted() {
        this.fetchMovies();
    }
}
</script>

<template>
    <div class="container">
        <h1 class="text-center m-3">{{ title }}</h1>

        <div class="alert alert-danger center-block" v-if="apiError">
            Something went wrong!
        </div>

        <div class="row d-flex flex-row flex-wrap justify-content-center">
            <div v-for="item in movies" class="card m-2">
                <img :src="item?.primaryImage?.url" class="img-fluid rounded" alt="Image not found">

                <div class="card-body">
                    <h5 class="card-title text-break">{{ item?.titleText?.text }}</h5>
                    <span class="badge text-bg-info">{{ item?.releaseYear?.year }}</span>
                    <span class="badge text-bg-warning mx-1">{{ item?.ratingsSummary?.aggregateRating }}</span>
                    <h6 class="card-text mt-2">{{ item?.plot?.plotText?.plainText }}</h6>
                </div>
                <router-link :to="`/movie/${item?.id}`" class="btn btn-md btn-block btn-primary m-2">Learn
                    more...</router-link>
            </div>
        </div>

        <nav class="mx-auto">
            <ul class="pagination justify-content-center">
                <li class="page-item"><button @click="previousPage"
                        :class="['page-link', { disabled: isPreviousDisabled }]">Previous</button></li>
                <li class="page-item"><button @click="homePage" class="page-link active">Home ({{ page }})</button></li>
                <li class="page-item"><button @click="nextPage" class="page-link">Next</button></li>
            </ul>
        </nav>
    </div>
</template>

<style scoped>
img {
    width: 100%;
    height: 365px;
}

.card {
    max-width: 325px;
}
</style>