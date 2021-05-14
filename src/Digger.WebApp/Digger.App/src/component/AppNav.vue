<template>
  <div id="main">
    <el-header>
    <el-row>
      <el-col :span="4">
        <div id="logo">
        <router-link to="/Home"><img id="logoDigger"  src="../assets/logoDigger.png"/></router-link>
        <span id="digger">Digger</span>
        </div>
      </el-col>
      <div id="navbar">
      <el-col :span="22">
        <el-menu :default-active="activeIndex" class="el-menu-demo" mode="horizontal" @select="handleSelect">
          <el-menu-item index="1" v-on:click="GoTo('/Home')">Accueil</el-menu-item>
          <el-menu-item index="5" v-on:click="GoTo('/Home/Admin')">Mode Admin</el-menu-item>
          <el-menu-item index="3" v-on:click="GoTo('/Home/invitation')">Invitation</el-menu-item>
          <el-menu-item index="4" disabled>À propos</el-menu-item>
          
      <el-menu-item index="6" ><a href = "/Authentification/Logout/">Deconnexion</a></el-menu-item>
    
          
        </el-menu>
        
      </el-col>
      </div>
    </el-row>
    </el-header>
  </div>
</template>

<script>
import AuthentifiactionApiService from '../services/AuthentificationApiService.js'

export default {
  name: 'appNav',
  data () {
    return {
      msg: 'NavBar!',
      activeIndex: '1',
      activeIndex2: '1'
    };
  },
  methods: {
    handleSelect(key, keyPath) {
    },
    GoTo(path){
        this.$router.push(path);
      },
    async Logout(){
       await AuthentifiactionApiService.Logout();
       
    }
  }
}
</script>

<style lang="scss">
#logoDigger {
  width:65px;
  height:65px;
}

@font-face {
   font-family: codeB;
   src: url('../assets/CODE3X-R.ttf');
}
#digger {
  font-family: codeB;
  color: rgb(0, 0, 0);
  font-size: 430%;
}
#navbar{
  margin-left: 300px;
}

#logo{
  background-color:white;border:1px solid grey
}
</style>
