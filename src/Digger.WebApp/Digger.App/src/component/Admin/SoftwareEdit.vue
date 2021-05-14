<template>

<div id="main">
  <app-nav id="app-nav"></app-nav>
<el-row>
  <el-col :span="4">
    <admin-nav></admin-nav>
  </el-col>
<div id="form">
  <el-col :span="18">
    <el-form ref="form" :model="form" label-width="200px">
    <el-form-item label="Nom du logiciel" required>
      <el-input v-model="form.Name"></el-input>
    </el-form-item>
    <el-form-item label="Description du logiciel" required>
      <el-input type="textarea" auto-complete="off" v-model="form.Description"></el-input>
    </el-form-item>
    <el-form-item label="Lien GitHub du logciel" required>
      <el-input auto-complete="off" v-model="form.LinkProject"></el-input>
    </el-form-item>
    <el-form-item>
      <el-button type="primary" @click="submitForm()">Installer le logiciel</el-button>
      <router-link to="/Home/Admin/Software"><el-button>Retour</el-button></router-link>
      <el-button type="danger" @click="helpInstallSoftware()">Aide !</el-button>
    </el-form-item>
  </el-form>
  </el-col>
</div>
</el-row>
</div>

</template>

<script>
import AppNav from '../AppNav.vue'
import AdminNav from "./AdminNav.vue"
import ResearchModuleApiService from '../../services/ResearchModuleApiService'
import SoftwareApiService from '../../services/SoftwareApiService'


export default {
  components: { AdminNav, AppNav },
  data() {
    return {
      name: 'SoftwareEdit',
      form : {
        Name : '',
        Description : '',
        LinkProject : '',
      }
    }
  },
  async mounted() {
    await this.refreshList();
  },
  methods: {
    async refreshList() {
      //this.softwareResearchList = await SoftwareResearchApiService.getAllSoftwareAndResearchAsync();
    },
    async submitForm(){
      this.responseUpload = await SoftwareApiService.uploadSoftwareAsync(this.form);

      this.$router.push('./../Software');
    },
    helpInstallSoftware() {
        this.$alert("L'installation d'un logiciel sur le serveur se fait en 2 étapes ! \
                    Tout d'abord vous devez soit donner le lien GitHub du logiciel soit l'upload vous-même dessus. \
                    Il faudra ensuite remplir le formulaire et l'envoyer !", 
                    'Installer un logiciel',{
          confirmButtonText: 'OK',
          callback: action => {
            this.$message({
              type: 'info',
              message: `action: ${ action }`
          });
        }
      });
    }
  },
}
</script>

<style>

#form {
  padding-left: 400px;
  margin-top: 50px;
}

</style>