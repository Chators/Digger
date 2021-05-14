<template>

<div id="main">
  <app-nav id="app-nav"></app-nav>
    Commence ta recherche  !
    <el-main> 
       <div id="rechercher">
        <el-collapse>
          <div id="test">
        <el-collapse-item title="Recherche" name="1">
        <el-form>
        <el-form-item label="Nom de domaine">
          <el-input v-model="form.domain" placeholder="Entrer un nom de domaine"></el-input>
        </el-form-item>
        <el-form-item label="Groupe ou organisation">
          <el-input v-model="form.groupOrOrg" placeholder="Entrer un nom de groupe ou d'organisation"></el-input>
        </el-form-item>
        <el-form-item label="Mail">
          <el-input v-model="form.mail" placeholder="Entrer un mail"></el-input>
        </el-form-item>
        <el-form-item label="Pseudo">
          <el-input v-model="form.pseudo" placeholder="Entrer un pseudo"></el-input>
        </el-form-item>
        <el-button type="primary" @click="onSubmit">Lancer la recherche</el-button>
        </el-form>
        </el-collapse-item>
          </div>
        </el-collapse>
       </div>
    </el-main>
</div>

</template>

<script>
import AppNav from '../AppNav.vue'
import SearchApiService from '../../services/SearchApiService'

export default {
  components: { AppNav },
  data() {
    return {
      name: 'Search',
      form: {}
    }
  },
  methods: {
   async onSubmit(e){
     e.preventDefault();
     this.$router.push('Search/ShowSearch?name=' + this.form.domain);
   }
  },
  mounted() {
  }
}
</script>

<style>

#rechercher {
  margin-left: 300px;
  margin-right: 250px;
}
#test{
  margin-left: 0px;
}

</style>