<script>
import axios from 'axios';

export default {
    name: "PostModal",
    props: {
        isOpen: {
            type: Boolean,
            default: false
        },
        currentData: {
            type: Object,
            default: {}
        },
        publishStatus: {
            type: String,
            default: ""
        },
        btnText: {
            type: String,
            default: "Publish"
        }
    },
    emits: ["close", "submit"],
    mounted() {
        if (this.isOpen) {
            document.getElementById("openModal").click();
        }

        let { title, description, movieId } = this.currentData;
        if (title && description && movieId) {
            this.title = title;
            this.description = description;
            this.movieId = movieId;

            this.fetchMovieById();
        }
    },
    data() {
        return {
            searchInput: "",
            previousSearch: "",
            movieSearchResult: [],
            filter: "movie",
            pageNumber: 1,
            isPreviousDisabled: true,
            title: "",
            description: "",
            movieId: "",
            rating: 0,
            isNotFound: false,
            titleErr: { error: false, errorMsg: "" },
            descriptionErr: { error: false, errorMsg: "" },
            movieIdErr: { error: false, errorMsg: "" },
            ratingErr: { error: false, errorMsg: "" },
            publishErr: false,
            timeout: null
        }
    },
    computed: {
        isSearchResult() {
            if (this.movieSearchResult == undefined) return false;
            return this.movieSearchResult.length > 0 ? true : false;
        },
        isPublishStatus() {
            return this.publishStatus.length > 0 ? true : false;
        },
        isError() {
            if (this.movieId.length == 0) {
                return true;
            }
            if (this.title.length == 0) {
                return true;
            }
            if (this.description.length == 0) {
                return true;
            }

            if (!Number.isInteger(this.rating) || this.rating > 10 || this.rating < 0) {
                return true;
            }

            return false;
        }
    },

    methods: {
        async fetchMovieById() {
            const options = {
                method: 'GET',
                url: `https://moviesdatabase.p.rapidapi.com/titles/${this.movieId}`,
                params: { info: "base_info" },
                headers: {
                    'X-RapidAPI-Key': '95d1eab7ddmsh0f51c34950daa93p1a8f5cjsnb298001e3f6f',
                    'X-RapidAPI-Host': 'moviesdatabase.p.rapidapi.com'
                }
            };

            axios.request(options).then(res => {
                this.movieSearchResult.push(res.data.results);
                this.searchInput = res.data.results?.titleText?.text;
            }).catch(function (error) {
                console.error(error);
            });
        },

        async fetchMoviesBySearch() {
            if (this.searchInput.length == 0) {
                this.movieIdErr = { error: true, errorMsg: "Please enter a movie name!" }
                return true;
            }

            this.movieIdErr = { error: false, errorMsg: "" }

            if (this.previousSearch != this.searchInput) {
                this.pageNumber = 1;
                this.isPreviousDisabled = true;
            }

            const options = {
                method: 'GET',
                url: `https://moviesdatabase.p.rapidapi.com/titles/search/title/${this.searchInput.toLocaleLowerCase()}`,
                params: { titleType: this.filter, page: this.pageNumber, info: "base_info" },
                // params: { exact: false, info: "base_info" },
                headers: {
                    'X-RapidAPI-Key': '95d1eab7ddmsh0f51c34950daa93p1a8f5cjsnb298001e3f6f',
                    'X-RapidAPI-Host': 'moviesdatabase.p.rapidapi.com'
                }
            };

            try {
                let req = await axios.request(options);
                let movies = req.data.results;
                movies = movies.filter(e => e.primaryImage != undefined);

                movies.map(e => {
                    let img = e?.primaryImage;
                    let movie = e;
                    delete movie.primaryImage;

                    let splitted = img?.url.split("@");
                    let newImgUrl = "";

                    if (img != null) {
                        if (splitted?.length > 2) {
                            newImgUrl = `${splitted.splice(0, 1)}@@._V1_QL100_UY500_UX500.jpg`;
                        } else {
                            newImgUrl = `${splitted.splice(0, 1)}@._V1_QL100_UY500_UX500.jpg`;
                        }

                        Object.assign(movie, {
                            primaryImage: {
                                url: newImgUrl
                            }
                        })

                        return movie;
                    }
                })

                this.movieSearchResult = movies;

                if (this.movieSearchResult.length == 0) {
                    if (this.pageNumber > 1) this.pageNumber--;
                    if (this.pageNumber == 1) this.isPreviousDisabled = true;
                }

            } catch (error) {
                console.log(error);
            }

            this.previousSearch = this.searchInput;
        },
        setFilter(filter) {
            this.filter = filter;
            this.pageNumber = 1;
            this.isPreviousDisabled = true;
        },
        async nextPage() {
            this.pageNumber++;
            this.isPreviousDisabled = false;
            await this.fetchMoviesBySearch();
            this.scrollTo();
        },
        async previousPage() {
            if (this.pageNumber > 1) this.pageNumber--;
            if (this.pageNumber == 1) this.isPreviousDisabled = true;
            await this.fetchMoviesBySearch();
            this.scrollTo();
        },
        async homePage() {
            this.pageNumber = 1;
            await this.fetchMoviesBySearch();
            this.scrollTo();
        },
        selectMovie(id) {
            this.movieId = id;
        },
        close() {
            this.$emit("close");
            this.reset();

            this.titleErr = { error: false, errorMsg: "" };
            this.descriptionErr = { error: false, errorMsg: "" };
            this.movieIdErr = { error: false, errorMsg: "" };
            this.publishErr = false;
            // this.publishStatus = "";
        },
        reset() {
            this.movieId = "";
            this.title = "";
            this.description = "";
            this.searchInput = this.previousSearch = "";
            this.pageNumber = 1;
            this.isPreviousDisabled = true;
            this.movieSearchResult = [];
        },
        submit() {
            this.titleErr = { error: false, errorMsg: "" };
            this.descriptionErr = { error: false, errorMsg: "" };
            this.movieIdErr = { error: false, errorMsg: "" };

            if (this.movieId.length == 0) {
                this.movieIdErr = { error: true, errorMsg: "Please choose a movie!" };
            }
            if (this.title.length == 0) {
                this.titleErr = { error: true, errorMsg: "Please enter a title!" };
            }
            if (this.description.length == 0) {
                this.descriptionErr = { error: true, errorMsg: "Please enter a description!" }
            }

            let titleErr = this.titleErr.error;
            let descriptionErr = this.descriptionErr.error;
            let movieIdErr = this.movieIdErr.error;

            if (!titleErr && !descriptionErr && !movieIdErr) {
                this.$emit("submit", {
                    title: this.title,
                    description: this.description,
                    movieId: this.movieId,
                    rating: this.rating
                })

                setTimeout(() => {
                    document.getElementById("close").click();
                }, 2 * 1000);
                return;
            }

            this.reset();
        },
        scrollTo() {
            document.getElementById("modal-body").scrollTo(0, 0);
        }
    }
}
</script>

<template>
    <a id="openModal" data-bs-toggle="modal" :href="`#${btnText}`" class="visually-hidden"></a>

    <div class="modal fade" :id="btnText" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1"
        aria-labelledby="staticBackdropLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title fs-5" id="staticBackdropLabel">Share what you watched</h1>
                    <button type="button" @click.prevent="close()" class="btn-close" data-bs-dismiss="modal"
                        aria-label="Close"></button>
                </div>
                <div id="modal-body" class="modal-body">
                    <div :class="['alert', {
                        'alert-danger': publishErr,
                        'alert-success': !publishErr
                    }]" v-if="isPublishStatus">
                        {{ publishStatus }}
                    </div>
                    <form>
                        <div class="mb-3">
                            <label for="recipient-name" class="col-form-label">Title:</label>
                            <input type="text" :class="['form-control', { 'is-invalid': titleErr?.error }]"
                                v-model="title">
                            <div class="invalid-feedback" v-if="titleErr?.error">
                                {{ titleErr?.errorMsg }}
                            </div>
                        </div>
                        <div class="mb-3">
                            <label for="message-text" class="col-form-label">Description:</label>
                            <textarea :class="['form-control', { 'is-invalid': descriptionErr?.error }]"
                                v-model="description"></textarea>
                            <div class="invalid-feedback" v-if="descriptionErr?.error">
                                {{ descriptionErr?.errorMsg }}
                            </div>
                        </div>
                        <div class="mb-3">
                            <label for="message-text" class="col-form-label">Personal rating:</label>
                            <input type="number" class="form-control" min="0" max="10" v-model="rating">
                        </div>
                        <div class="mb-3">
                            <label for="recipient-name" class="col-form-label">Movie:</label>
                            <div class="input-group ">
                                <input type="text" :class="['form-control', { 'is-invalid': movieIdErr?.error }]"
                                    v-model.lazy="searchInput">
                                <button class="btn btn-outline-success "
                                    @click.prevent="fetchMoviesBySearch">Search</button>
                                <button type="button"
                                    class="btn btn-outline-success dropdown-toggle dropdown-toggle-split"
                                    data-bs-toggle="dropdown" aria-expanded="false">
                                    <span class="visually-hidden">Toggle Dropdown</span>
                                </button>
                                <div class="invalid-feedback" v-if="movieIdErr?.error">
                                    {{ movieIdErr?.errorMsg }}
                                </div>
                                <ul class="dropdown-menu dropdown-menu-end">
                                    <li><a class="dropdown-item" href="javascript:void(0)"
                                            @click="setFilter('movie')">movie</a></li>
                                    <li><a class="dropdown-item" href="javascript:void(0)"
                                            @click="setFilter('tvMovie')">tvMovie</a></li>
                                    <li><a class="dropdown-item" href="javascript:void(0)"
                                            @click="setFilter('tvSeries')">tvSeries</a></li>
                                    <li><a class="dropdown-item" href="javascript:void(0)"
                                            @click="setFilter(null)">all</a></li>
                                    <li>
                                        <hr class="dropdown-divider">
                                    </li>
                                    <li class="dropdown-item disabled">Chosen filter: {{ filter }}</li>
                                </ul>

                            </div>
                        </div>
                        <div class="alert alert-info" role="alert" v-if="!isSearchResult">
                            Empty search result!
                        </div>
                        <div class="mb-3" v-if="isSearchResult">
                            <ul class="list-group">
                                <li :class="['list-group-item', {
                                    'active-movie': movieId == movie?.id
                                }]" v-for="movie in movieSearchResult" @click="selectMovie(movie?.id)">
                                    <div class="d-flex flex-row">
                                        <img class="mx-3" :src="movie?.primaryImage?.url" alt="" width="75"
                                            height="100">
                                        <div class="d-flex flex-column">
                                            <h5>{{ movie?.titleText?.text }}</h5>
                                            <div class="d-flex flex-row">
                                                <span class="badge text-bg-info me-1">
                                                    {{ movie?.releaseYear?.year }}
                                                </span>
                                                <span class="badge text-bg-success me-1">
                                                    {{ movie?.titleType?.text }}
                                                </span>
                                                <span class="badge text-bg-warning">
                                                    {{ movie?.ratingsSummary?.aggregateRating }}
                                                </span>
                                            </div>
                                            <ul>
                                                <li v-for="genre in movie?.genres?.genres">{{ genre?.text }}
                                                </li>
                                            </ul>
                                        </div>
                                    </div>
                                </li>
                            </ul>
                            <ul class="pagination justify-content-center mt-3">
                                <li class="page-item">
                                    <button @click.prevent="previousPage"
                                        :class="['page-link', { disabled: isPreviousDisabled }]">
                                        Previous
                                    </button>
                                </li>
                                <li class="page-item">
                                    <button @click.prevent="homePage" class="page-link active">
                                        Home({{ pageNumber }})
                                    </button>
                                </li>
                                <li class="page-item">
                                    <button @click.prevent="nextPage" class="page-link">Next</button>
                                </li>
                            </ul>
                        </div>
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" id="close" class="btn btn-secondary" data-bs-dismiss="modal"
                        @click.prevent="close()">Close</button>
                    <button type="button" :class="['btn', 'btn-primary', {
                        'disabled': isError
                    }]" @click.prevent="submit()">{{ btnText }}</button>
                </div>
            </div>

        </div>
    </div>
</template>

<style scoped>
.btn-modal {
    position: absolute;
    bottom: 0;
    right: 0;
    border-radius: 50%;
    margin: 40px;
    padding: 5px;
}

.list-group-item:hover {
    background-color: #cccccc;
    cursor: pointer;
}

.active-movie {
    background-color: #cccccc;
}
</style>