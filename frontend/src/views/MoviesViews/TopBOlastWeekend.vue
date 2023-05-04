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
        <MoviesCardVue title="Top box office last weekend" api-endpoint="titles"
            api-params="top_boxoffice_last_weekend_10"></MoviesCardVue>
    </div>
</template>