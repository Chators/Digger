<template>

<div id="main">
  <app-nav id="app-nav"></app-nav>
    <el-main> 
      <div id="button">

       
       
      </div>
      <div id="tableau">
       <span v-if="this.projectList.length > 0">
      <div id="projet">
      <el-button type="success" syze="mini" icon="el-icon-circle-plus-outline" v-on:click="dialogFormVisible = true" circle></el-button> Administration de vos Projets   
      </div>
    </span>
    <span v-else>
      <div id="projet">
      <el-button type="success" syze="mini" icon="el-icon-circle-plus-outline" v-on:click="dialogFormVisible = true" circle></el-button>
      Aucun projet à administer
      </div>
    </span>
        
       <table id="private" v-if="this.projectList.length > 0" class="table table-striped table-hover table-bordered" style="width:100%">
            <thead >
                <tr>
                    <th>Nom</th>
                    <th>Description</th>
                    <th>Publication</th>
                    <th>Role</th>
                    <th>Opération</th>
                    
                </tr>
            </thead>
             <tbody>
                <tr v-for="Project of this.projectList">
                  
                    <td>{{ Project.name }}</td>
                    <td>{{ Project.description }}</td>
                    <td v-if="Project.isPublic == 0">Privé</td>
                    <td v-else>Publique</td>
                    <td>{{Project.accessRight}}</td>
                    <td>
                      <el-button 
                      round=""
                      size="mini"
                      icon="el-icon-view"
                      type="success"
                      @click="GoToGraph(Project.id)"></el-button>
                      
                      <el-button v-if="Project.accessRight == 'Admin'" round icon="el-icon-circle-close-outline"
                      size="mini"
                      type="danger"
                      @click="deleteProject(Project.id)"></el-button>
                      
                      <el-button v-if="Project.accessRight == 'Admin'" round icon="el-icon-edit-outline"
                      size="mini"
                      @click="dialogFormVisible2 = true"></el-button>
                      <el-button v-if="Project.accessRight == 'Admin'"
                      size="mini"
                      round
                      icon="el-icon-plus"
                      @click="GetUser(Project.id)"></el-button>
                      <el-button v-if="Project.accessRight == 'Admin'"
                      size="mini"
                      round
                      @click="userInProject(Project.id)">Droits</el-button>
                      <el-button  round
                      icon="el-icon-close"
                      size="mini"
                      @click="leaveProject(Project.id)">Quitter</el-button>
                      </td>

                      <el-dialog title="Modifier un projet" :visible.sync="dialogFormVisible2">
                        <el-form :model="form">
                          <el-form-item label="Nom du projet"  required>
                            <el-input v-model="form.name" auto-complete="on" ></el-input>
                          </el-form-item>
                          <el-form-item label="Description du projet"  required>
                            <el-input v-model="form.desc" auto-complete="off"></el-input>
                          </el-form-item>
                            <el-form-item label="Type du projet"  required>
                          <el-select v-model="form.isPublic" placeholder="Select">
                              <el-option
                                v-for="item in valeurProjet"
                                :key="item.value"
                                :label="item.label"
                                :value="item.value">
                            </el-option>
                          </el-select>
                            </el-form-item>
                        </el-form>
                        <span slot="footer" class="dialog-footer">
                          <el-button @click="dialogFormVisible2 = false">Annuler</el-button>
                          <el-button type="danger" @click="UpdateProject(Project.id)">Confirmer</el-button>
                        </span>
                      </el-dialog>
                </tr>
             </tbody>
       </table>
      

          <el-dialog title="Créer un projet" :visible.sync="dialogFormVisible">
                        <el-form :model="form">
                          <el-form-item label="Nom du projet"  required>
                            <el-input v-model="form.name" auto-complete="on" ></el-input>
                          </el-form-item>
                          <el-form-item label="Description du projet"  required>
                            <el-input v-model="form.desc" auto-complete="off"></el-input>
                          </el-form-item>
                            <el-form-item label="Type du projet"  required>
                          <el-select v-model="form.isPublic" placeholder="Select">
                              <el-option
                                v-for="item in valeurProjet"
                                :key="item.value"
                                :label="item.label"
                                :value="item.value">
                            </el-option>
                          </el-select>
                            </el-form-item>
                        </el-form>
                        <span slot="footer" class="dialog-footer">
                          <el-button @click="dialogFormVisible = false">Annuler</el-button>
                          <el-button type="danger" @click="CreateProject()">Confirmer</el-button>
                        </span>
                      </el-dialog>

                      <el-dialog title="Modification des droits utilisateurs" :visible.sync="dialogFormVisible5">
                        <el-form :model="form">
                          <el-form-item label="Membre"  required>
                            <el-select v-model="form2.member" placeholder="Select">
                              <el-option
                                v-for="item in userProjectList"
                                :key="item.userId"
                                :label="item.userPseudo"
                                :value="item.userId">
                            </el-option>
                            </el-select>
                          </el-form-item>
                          <el-form-item label="Nouveau grade"  required>
                            <el-select v-model="form2.grade" placeholder="Select">
                              <el-option
                                v-for="item in valeurGrade"
                                :key="item.value"
                                :label="item.label"
                                :value="item.value">
                            </el-option>
                          </el-select>
                          </el-form-item>
                        </el-form>
                        <span slot="footer" class="dialog-footer">
                          <el-button @click="dialogFormVisible5 = false">Annuler</el-button>
                          <el-button type="danger" @click="ChangeAccess()">Confirmer</el-button>
                        </span>
                      </el-dialog>

                      <el-dialog title="Invitation dans un projet" :visible.sync="dialogFormVisible3">
                        <el-form :model="form3"> 
                          <el-form-item label="Membre à inviter"  required>
                            <el-autocomplete
                              class="inline-input"
                              v-model="form3.worker"
                              :fetch-suggestions="querySearch"
                              placeholder="Personne à inviter"
                              :trigger-on-focus="false"
                              @select="handleSelect">
                            </el-autocomplete>
                          </el-form-item>
                        </el-form>
                        <span slot="footer" class="dialog-footer">
                          <el-button @click="dialogFormVisible3 = false">Annuler</el-button>
                        </span>
                      </el-dialog>
      </div>
      <div id="public">
      <span v-if="this.projectPublicList.length > 0">
        Liste des projets publique
      </span>
      <span v-else>
        Aucun projet publique
      </span>

       <table id="public2" v-if="this.projectPublicList.length > 0" class="table table-striped table-hover table-bordered" style="width:100%">
         <thead>
           <tr>
             <th>Nom</th>
                    <th>Description</th>
                    <th>Publication</th>
                    
                    <th>Opération</th>
           </tr>
         </thead>
         <tbody>
                <tr v-for="Project of this.projectPublicList">
                  <td>{{ Project.name }}</td>
                    <td>{{ Project.description }}</td>
                    <td v-if="Project.isPublic == 0">Privé</td>
                    <td v-else>Publique</td>
                    <td> <el-button round
                    icon="el-icon-view"
                      size="mini"
                      type="success"
                      @click="GoToGraph(Project.id)"></el-button></td>
                </tr>
         </tbody>
       </table>
      </div>
    </el-main>
</div>

</template>

<script>
import AppNav from '../AppNav.vue'
import SearchApiService from "../../services/SearchApiService";
import ProjectApiService from "../../services/ProjectApiService";
import UserApiService from "../../services/UserApiService";

export default {
  components: { AppNav },
  data() {
    return {
      projectList: [],
      projectPublicList : [],
      userProjectList : [],
      dialogFormVisible: false,
      dialogFormVisible2: false,
      dialogFormVisible3: false,
      dialogFormVisible5: false,
      valeurGrade: [
        {
          label : "Admin",
          value : 3
        },
        {
          label : "Voyeur",
          value : 1
        },
        {
          label : "Worker",
          value : 2
        }
      ],
      valeurProjet: [
        {
          value: 1,
          label: "publique"
        },
        {
          value: 0,
          label: "privé"
        }
      ],
      projectId :'',
      form: {
        name: "",
        desc: "",
        isPublic: ""
      },
      form2: {
        member : '',
        grade : '',
        projectId : ''
      },
      form3: {
        worker: ""
      },
      suggest: []
    };
  },
  async mounted() {
    await this.refreshList();
    await this.refreshPublicList();
  },
  methods: {
    async refreshList() {
      this.projectList = await ProjectApiService.GetProjectByUserIdAsync(0);
    },
    async refreshPublicList(){
      this.projectPublicList = await ProjectApiService.GetProjectPublicByUserIdAsync(0);
    },
    querySearch(queryString, cb) {
      var links = this.suggest;
      var results = queryString
        ? links.filter(this.createFilter(queryString))
        : links;
      // call callback function to return suggestions
      cb(results);
    },
    async leaveProject(projectId){
      var json = {UserId : 0,ProjectId : projectId };
      await ProjectApiService.UnassignUserToProjectAsync(json);
      await this.refreshList();
    },
    createFilter(queryString) {
      return link => {
        return (
          link.value.toLowerCase().indexOf(queryString.toLowerCase()) === 0
        );
      };
    },
    async userInProject(projectId){
      this.userProjectList =  await ProjectApiService.GetUserInProjectAsync(projectId);
      this.dialogFormVisible5 = true;
      this.form2.projectId = projectId;
      
    },

    async ChangeAccess(){
      var json = {UserId : this.form2.member, ProjectId : this.form2.projectId , AccessRightId : this.form2.grade};
      await ProjectApiService.ChangeAccessRightUserInProjectAsync(json);
      this.dialogFormVisible5 = false;
    },

    GoToGraph(id){
        this.$router.push('/Home/Graph/'+id);
      },
    async handleSelect(item) {
      var json = {
        UserAuthorId : 0,
        UserInvitedId : item.id,
        ProjectId : this.projectId
      };
      await ProjectApiService.SendInvitationProjectAsync(json);
      this.dialogFormVisible3 = false;
    },
    async deleteProject(id) {
      await ProjectApiService.DeleteProjectAsync(id);
      await this.refreshList();
    },
    async GetUser(id) {
      this.suggest = await UserApiService.GetUserForInvitByProjectIdAsync(id);
      this.projectId = id;
      this.dialogFormVisible3 = true;
    },
    async test() {
      await this.refreshList();
      
    },
    async CreateProject() {
      var json = {
        FktUser: 0,
        Name: this.form.name,
        Description: this.form.desc,
        IsPublic: this.form.isPublic
      };
      await ProjectApiService.CreateProjectAsync(json);
      this.dialogFormVisible = false;
      await this.refreshList();
    },
    async UpdateProject(id) {
      var json = {
        Id: id,
        Name: this.form.name,
        Description: this.form.desc,
        IsPublic: this.form.isPublic
      };
      await ProjectApiService.UpdateProjectAsync(json);
      this.dialogFormVisible2 = false;
      await this.refreshList();
    },
    SendInvitation(item) {
      console.log(item.link);
    }
  }
};
</script>

<style>
#rechercher {
  margin-left: 300px;
  margin-right: 250px;
}
#test {
  margin-left: 0px;
}
#tableau {
  margin-top: 50px;
}

#public{
  margin-top: 70px
}

#private{
  margin-top: 15px
}

table{
  background-color: white
}

#public2{
  margin-top: 15px
}
td{
  padding: 8px
}

</style>