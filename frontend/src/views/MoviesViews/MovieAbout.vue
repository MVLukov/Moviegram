<script>
import axios from 'axios';

export default {
    data() {
        return {
            baseInfo: {},
            directors: [],
            actors: [],
            apiError: false,
            notFound: false
        }
    },
    methods: {
        async fetchMovie(id) {
            const getBaseInfo = {
                method: 'GET',
                url: `https://moviesdatabase.p.rapidapi.com/titles/${id}`,
                params: { info: "base_info" },
                headers: {
                    'X-RapidAPI-Key': '95d1eab7ddmsh0f51c34950daa93p1a8f5cjsnb298001e3f6f',
                    'X-RapidAPI-Host': 'moviesdatabase.p.rapidapi.com'
                }
            };

            const getDirectors = {
                method: 'GET',
                url: `https://moviesdatabase.p.rapidapi.com/titles/${id}`,
                params: { info: 'creators_directors_writers' },
                headers: {
                    'X-RapidAPI-Key': '95d1eab7ddmsh0f51c34950daa93p1a8f5cjsnb298001e3f6f',
                    'X-RapidAPI-Host': 'moviesdatabase.p.rapidapi.com'
                }
            };

            const getActors = {
                method: 'GET',
                url: `https://moviesdatabase.p.rapidapi.com/titles/${id}`,
                params: { info: 'extendedCast' },
                headers: {
                    'X-RapidAPI-Key': '95d1eab7ddmsh0f51c34950daa93p1a8f5cjsnb298001e3f6f',
                    'X-RapidAPI-Host': 'moviesdatabase.p.rapidapi.com'
                }
            };

            try {
                let req = await axios.request(getDirectors);
                if (req.status == 200) {
                    if (req.data.error != undefined) {
                        console.log(req.data.error);
                        this.apiError = true;
                    } else {
                        if (req.data.results.directors.length > 0) {
                            req.data.results.directors[0].credits.forEach(director => {
                                this.directors.push(director?.name?.nameText?.text)
                            });
                        }

                        if (req.data.results.writers.length > 0) {
                            req.data.results.writers[0].credits.forEach(director => {
                                this.directors.push(director?.name?.nameText?.text)
                            });
                        }

                        this.directors = [...new Set(this.directors)]

                        this.apiError = false;
                    }
                }
            } catch (error) {
                console.log(error);
                this.apiError = true;
            }

            try {
                let req = await axios.request(getBaseInfo);
                if (req.status == 200) {
                    if (req.data.results == null) {
                        this.notFound = true;
                    }

                    if (req.data.error != undefined) {
                        console.log(req.data.error);
                        this.apiError = true;
                    } else {
                        let img = req.data.results?.primaryImage;
                        let movie = req.data.results;
                        delete movie.primaryImage;

                        let splitted = img?.url.split("@");
                        let newImgUrl = "";

                        if (img != null) {
                            newImgUrl = `${splitted.splice(0, 1)}${'@'.repeat(splitted.length)}._V1_QL90_UY500_UX500.jpg`;
                        }

                        Object.assign(movie, {
                            primaryImage: {
                                url: newImgUrl
                            }
                        })

                        this.baseInfo = movie;
                        this.apiError = false;
                    }
                }
            } catch (error) {
                console.log(error);
                this.apiError = true;
            }

            try {
                let req = await axios.request(getActors);

                if (req.status == 200) {
                    if (req.data.error != undefined) {
                        console.log(req.data.error);
                        this.apiError = true;
                    } else {
                        req.data.results.cast.edges.forEach(actor => {
                            this.actors.push(actor?.node?.name?.nameText?.text);
                        });

                        this.apiError = false;
                    }
                }

                this.actors = this.actors.slice(0, 5);
            } catch (error) {

            }
        }
    },
    mounted() {
        this.fetchMovie(this.$route.params.id);
    }
}


</script>

<template>
    <div class="container">
        <h1 class="text-center my-3">More about...</h1>

        <div class="alert alert-danger center-block" v-if="notFound">
            Movie not found!
        </div>

        <div class="card mx-auto my-5" v-if="!notFound">
            <div class="row p-0">
                <div class="col-md-4 col-sm-12">
                    <img class="img-fluid rounded" :src="baseInfo?.primaryImage?.url" alt="Image not found">
                </div>
                <div class="col-md-8 col-sm-12">
                    <div class="card-body">
                        <h2 class="card-title">{{ baseInfo?.titleText?.text }}</h2>
                        <span class="badge text-bg-info">Year: {{ baseInfo?.releaseYear?.year }}</span>
                        <span class="badge text-bg-warning mx-1">
                            Rating: {{ baseInfo?.ratingsSummary?.aggregateRating }}
                        </span>

                        <h4 class="card-text mt-3">Directors and writers</h4>
                        <p>{{ directors.join(", ") }}</p>

                        <h4 class="card-text mt-3">Synopsis</h4>
                        <p class="card-text">{{ baseInfo?.plot?.plotText?.plainText }}</p>

                        <h4 class="card-title">Genres</h4>
                        <ul class="list-group list-group-horizontal ">
                            <li class="list-group-item" v-for="item in baseInfo?.genres?.genres">{{ item?.text }}</li>
                        </ul>

                        <h4 class="card-text mt-3">Actors</h4>
                        <ul class="list-group list-group-horizontal">
                            <li class="list-group-item" v-for="item in actors">{{ item }}</li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
img {
    width: 100%;
    height: 100%;
}

.card {
    max-width: 1000px !important;
}
</style>