<template>
<div id="main">
    <h5>Menu</h5>
    <el-menu class="el-menu-vertical-demo">
      <el-menu-item index="1" v-on:click="GoTo('/Home/Admin')">
        <i class="el-icon-menu" ></i><span>Gestion des utilisateurs</span>
      </el-menu-item>
      <el-menu-item index="1" v-on:click="GoTo('/Home/Admin/Software')">
        <i class="el-icon-menu" ></i><span>Gestion logiciel sur Diggos</span>
      </el-menu-item>
      <el-menu-item index="2" v-on:click="GoTo('/Home/Admin/Software/SoftwareEdit')">
        <i class="el-icon-menu"></i><span>Installer un nouveau logiciel</span>
      </el-menu-item>
    </el-menu>
</div>
</template>

<script>

export default {
  data() {
    return {
      name: 'AdminNav'
    }
  },
  methods: {
      GoTo(path){
        this.$router.push(path);
      }
    }
}
</script>

<style>

.el-menu{
  border: solid 2px rgb(170, 170, 170);
}
.el-menu-item.is-active{
  color: red;
}
.el-menu-item:focus, .el-menu-item:hover{
  background-color: rgba(212, 212, 212, 0.377);
}
.el-menu-item{
  color: black;
}

</style>