<template>

<div id="main">
  <app-nav id="app-nav"></app-nav>
  <div v-if="this.graph == null">
    <img src="../../assets/loadingMagnifyingGlass.gif">
    <p>En creusage ...</p>
    <img src="../../assets/diggerInAction.gif">
  </div>
  <div v-else>
    {{ graph }}
  </div>
</div>

</template>

<script>
import AppNav from '../AppNav.vue'
import SearchApiService from '../../services/SearchApiService'

export default {
  components: { AppNav },
  data() {
    return {
      name: 'ShowSearch',
      form: { domain : this.$route.query.name},
      graph: null
    }
  },
  methods: {
  },
  mounted: async function() {
    async function CreateGraph(theForm) {
      return await SearchApiService.SearchAsync(theForm);
    }
    console.log(this.graph);
    this.graph = await CreateGraph(this.form);
    console.log(this.graph);
  },

  
}
</script>

<style>

#main {
}

</style>