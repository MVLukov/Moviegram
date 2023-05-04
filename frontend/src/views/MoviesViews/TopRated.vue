<script>
import MoviesCardVue from '../../components/MoviesCard.vue';

export default {
    data() {
        return {
            movies: [{}]
        }
    },
    components: {
        MoviesCardVue
    },
}
</script>

<template>
    <div class="container">
        <MoviesCardVue title="Top rated movies" apiEndpoint="titles" apiParams="top_rated_250"></MoviesCardVue>
    </div>
</template>

<style scoped>

</style>