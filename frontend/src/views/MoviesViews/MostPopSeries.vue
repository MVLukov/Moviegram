<script>
import MoviesCardVue from '../../components/MoviesCard.vue';

export default {
    data() {
        return {
        }
    },
    components: { MoviesCardVue },
}


</script>

<template>
    <div class="container">
        <MoviesCardVue title="Most popular series" api-endpoint="titles" api-params="most_pop_series"></MoviesCardVue>
    </div>
</template>
