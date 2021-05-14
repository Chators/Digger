<template>
<div id="main">
  <app-nav id="app-nav"></app-nav>
<el-row>
  <el-col :span="4">
    <admin-nav></admin-nav>
  </el-col>
  <el-col :span="20">
    <div id="add">
  <el-button round
                    icon="el-icon-circle-plus-outline"
                      size="mini"
                      type="success"
                      @click="dialogFormVisible = true">Ajouter un utilisateur</el-button>
    </div>
  <div id="member">
    
  <table id="public2" v-if="this.userList.length > 0" class="table table-striped table-hover table-bordered" style="width:100%">
         <thead>
           <tr>
            <th>Membre</th>
            <th>Role</th>
            <th>Opération</th>
           </tr>
         </thead>
         <tbody>
                <tr v-for="Project of this.userList">
                  <td>{{ Project.pseudo }}</td>
                  <td>{{Project.role}}</td>
                    <td> <el-button round
                    icon="el-icon-close"
                      size="mini"
                      type="danger"
                      @click="DeleteUser(Project.id)"></el-button>
                      <el-button round v-if="Project.role == 'user'"
                    icon="el-icon-star-on"
                      size="mini"
                      type="success"
                      @click="AdminUser(Project.id)"></el-button>
                      </td>
                </tr>
         </tbody>
       </table>

       <el-dialog title="Nouvel utilisateur" :visible.sync="dialogFormVisible">
                        <el-form :model="form">
                          <el-form-item label="Pseudo"  required>
                            <el-input v-model="form.pseudo" auto-complete="on" ></el-input>
                          </el-form-item>
                          <el-form-item label="Mot de passe"  required>
                            <el-input v-model="form.mdp" auto-complete="off" type="password"></el-input>
                          </el-form-item>
                            <el-form-item label="Confirmation du mot de passe"  required>
                              <el-input v-model="form.mdp2" auto-complete="off" type="password"></el-input>
                            </el-form-item>
                        </el-form>
                        <span slot="footer" class="dialog-footer">
                          <el-button @click="dialogFormVisible2 = false">Annuler</el-button>
                          <el-button type="danger" @click="addUser()">Confirmer</el-button>
                        </span>
                      </el-dialog>
  </div>
  
  
    
  </el-col>
</el-row>
</div>

</template>

<script>
import AppNav from '../AppNav.vue'
import AdminNav from "./AdminNav.vue"
import UserApiService from '../../services/UserApiService.js'
import auth from '../../services/AuthentificationApiService.js'

export default {
  components: { AdminNav, AppNav },
  data() {
    return {
      name: 'Admin',
      dialogFormVisible : false,
      userList : [],
      form : {
        pseudo : '',
        mdp : '',
        mdp2 : ''
      }
    }
  },
  async mounted(){
    await this.GetAllUser();
  },
  methods : {
    async GetAllUser(){
      this.userList = await UserApiService.GetAllUsersAsync();
    },
    async addUser(){
      
      this.dialogFormVisible = false;
      var json = {pseudo : this.form.pseudo, password : this.form.mdp , confirmPassword : this.form.mdp2};
      
      await auth.RegisterAsync(json);
      this.GetAllUser();
      
    },
    async DeleteUser(id){
      await UserApiService.DeleteUserAsync(id);
      await this.GetAllUser();
    },
    async AdminUser(id){
      await UserApiService.UpgradeUserInAdminAsync(id);
      await this.GetAllUser();
    }
  }
}
</script>

<style>
#add{
  margin-left: -1100px;
  margin-top: 60px
}
#member{
  margin-top: 20px
}
#main {
}

</style>