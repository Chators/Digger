<template>

<div id="main">
  <app-nav id="app-nav"></app-nav>
<el-row>
  <el-col :span="4">
    <admin-nav></admin-nav>
  </el-col>
  <div id="install">
  <el-col :span="23">
    
    <div id="result">
    <span v-if="this.softwareList.length > 0">
      <div id="logiciel">
      Logiciel installé et prêt a l'emploi :
      </div>
    </span>
    <span v-else>
      Aucun logiciel actuellement installé sur le serveur
    </span>
    </div>

    <el-collapse v-if="this.softwareList.length > 0" accordion>
      <span >
      <el-collapse-item v-for="software of this.softwareList" :title=software.software.name :name=software.software.id >
        <table class="table table-striped table-hover table-bordered" style="width:100%">
            <thead>
                <tr>
                    <th>Description</th>
                    <th>Temps d'exécution moyen</th>
                    <th>Opération</th>
                </tr>
            </thead>

            <tbody>
                <tr>
                    <td>{{ software.software.description }}</td>
                    <td>{{ software.software.averageExecutionTime }}</td>
                    <td>
                      <el-button round
                      icon="el-icon-plus"
                      size="mini"
                      type="success"
                      @click="getDataForm(software.software.id), form3.id = software.software.id"></el-button>

                      <el-dialog title="Créer un module de recherche" :visible.sync="dialogFormVisible2">
                        <el-form :model="form3">
                          <el-form-item label="Description" :label-width="formLabelWidth" required>
                            <el-input v-model="form3.desc" auto-complete="on" ></el-input>
                          </el-form-item>
                          <el-form-item label="LevelFootprint" :label-width="formLabelWidth" required>
                          <el-select v-model="form3.levelFootPrint2" placeholder="Select">
                              <el-option
                              v-for="(item,index) in levelFootPrint"
                              :key="item"
                              :label="item"
                              :value="index">
                            </el-option>
                          </el-select>
                          </el-form-item>
                          <el-form-item label="TypeEntity" :label-width="formLabelWidth" required><el-button type="text" @click="open3(software.software.id)">Ajouter</el-button>
                          <el-select v-model="form3.typeEntity" placeholder="Select">
                              <el-option
                              v-for="(item, index)  in typeEntity"
                              :key="item"
                              :label="item"
                              :value="index">
                            </el-option>
                          </el-select>
                          </el-form-item>
                          <el-form-item label="nameFileOsintAvaibles" :label-width="formLabelWidth" required>
                          <el-select v-model="form3.nameFileOsintAvaibles" placeholder="Select">
                              <el-option
                              v-for="item in nameFileOsintAvaibles"
                              :key="item"
                              :label="item"
                              :value="item">
                            </el-option>
                          </el-select>
                          </el-form-item>
                        </el-form>
                        <span slot="footer" class="dialog-footer">
                          <el-button @click="dialogFormVisible2 = false">Annuler</el-button>
                          <el-button type="danger" @click="createNewResearch(software.software.id)" >Confirmer</el-button>
                        </span>
                      </el-dialog>

                      <el-dialog title="Modifier un module de recherche" :visible.sync="dialogFormVisible3">
                        <el-form :model="form3">
                          <el-form-item label="Description" :label-width="formLabelWidth" required>
                            <el-input v-model="form3.desc" auto-complete="on" ></el-input>
                          </el-form-item>
                          <el-form-item label="LevelFootprint" :label-width="formLabelWidth" required>
                          <el-select v-model="form3.levelFootPrint2" placeholder="Select">
                              <el-option
                              v-for="(item,index) in levelFootPrint"
                              :key="item"
                              :label="item"
                              :value="index">
                            </el-option>
                          </el-select>
                          </el-form-item>
                          <el-form-item label="TypeEntity" :label-width="formLabelWidth" required>
                          <el-select v-model="form3.typeEntity" placeholder="Select">
                              <el-option
                              v-for="(item, index)  in typeEntity"
                              :key="item"
                              :label="item"
                              :value="index">
                            </el-option>
                          </el-select>
                          </el-form-item>
                        </el-form>
                        <span slot="footer" class="dialog-footer">
                          <el-button @click="dialogFormVisible3 = false">Annuler</el-button>
                          <el-button type="danger" @click="updateResearch()" >Confirmer</el-button>
                        </span>
                      </el-dialog>

                      <el-dialog title="Modifier" :visible.sync="dialogFormVisible">
                        <el-form :model="form2">
                          <el-form-item label="New name" :label-width="formLabelWidth" required>
                            <el-input v-model="form2.name" auto-complete="on" ></el-input>
                          </el-form-item>
                          <el-form-item label="New desc" :label-width="formLabelWidth" required>
                            <el-input v-model="form2.desc" auto-complete="off"></el-input>
                          </el-form-item>
                        </el-form>
                        <span slot="footer" class="dialog-footer">
                          <el-button @click="dialogFormVisible = false">Annuler</el-button>
                          <el-button type="danger" @click="updateSoftware(software.software.id,software.software.name)">Confirmer</el-button>
                        </span>
                      </el-dialog>

                      <el-button
                      size="mini"
                      round
                      icon="el-icon-edit-outline"
                      @click="dialogFormVisible = true"></el-button>
                      <el-button
                      size="mini"
                      round icon="el-icon-circle-close-outline"
                      type="danger"
                      @click="deleteSoftware(software.software.id)"></el-button>
                    </td>
                </tr>
            </tbody>
        </table>
        
        <table v-if="software.researchModule.length > 0" class="table table-striped table-hover table-bordered" style="width:100%">
            <thead>
                <tr>
                    <th>Nom</th>
                    <th>Description</th>
                    <th>Type de recherche</th>
                    <th>Temps d'exécution moyen</th>
                    <th>Opération</th>
                </tr>
            </thead>

            <tbody>
                <tr v-if="software.researchModule.length <= 0">
                  <td>
                    Vous n'avez pour le moment aucun recherche associé à ce logiciel !
                  </td>
                </tr>
                <tr v-else v-for="research of software.researchModule">
                    <td>{{ research.name }}</td>
                    <td>{{ research.description }}</td>
                    <td>{{ research.typeEntity }}</td>
                    <td>{{ research.averageExecutionTime }}</td>
                    <td>
                      <el-button
                      size="mini"
                      round icon="el-icon-edit-outline"
                      @click="getDataForm2(research.id,research.name)"></el-button>
                      <el-button
                      size="mini"
                      round icon="el-icon-circle-close-outline"
                      type="danger"
                      @click="deleteResearch(research.id)"></el-button>
                    </td>
                </tr>
            </tbody>
        </table>
        
      </el-collapse-item>
      </span>
      
    </el-collapse>
  </el-col>
  </div>

</el-row>

</div>

</template>

<script>
import AppNav from '../AppNav.vue'
import AdminNav from "./AdminNav.vue"
import ResearchModuleApiService from '../../services/ResearchModuleApiService.js'
import SoftwareApiService from '../../services/SoftwareApiService.js'

export default {
  components: { AdminNav, AppNav },
  data() {
    return {
      dialogFormVisible: false,
      dialogFormVisible2: false,
      dialogFormVisible3:false,
      formLabelWidth: '120px',
      form2:{
        name: '',
        desc:'',
      },
      form3:{
        id: '',
        desc:'',
        levelFootPrint2 : '',
        typeEntity : '',
        nameFileOsintAvaibles : '',
      },
      name: 'AdminSoftware',
      levelFootPrint : [],
      typeEntity : [],
      nameFileOsintAvaibles : [],
      softwareList: []
    }
  },
  async mounted() {
    await this.refreshList();
  },
  methods: {
    async refreshList() {
      this.softwareList = await SoftwareApiService.getAllSoftwareAndResearchAsync();
    },
    async deleteResearch(idResearch) {
      
      await ResearchModuleApiService.DeleteResearchModule(idResearch);
      await this.refreshList();
    },
    async getDataForm(name){
       var data = await ResearchModuleApiService.GetInfoFormResearchModule(name);
       this.levelFootPrint = data.levelFootprint;
       this.typeEntity = data.typeEntity;
       this.nameFileOsintAvaibles = data.nameFileOsintAvaibles;
       this.dialogFormVisible2 = true;
      
    },
    async getDataForm2(id,name){
       var data = await ResearchModuleApiService.GetInfoFormResearchModule(id);
       this.form3.id = id;
       this.levelFootPrint = data.levelFootprint;
       this.typeEntity = data.typeEntity;
       this.nameFileOsintAvaibles = data.nameFileOsintAvaibles;
       this.form3.nameFileOsintAvaibles = name;
       this.dialogFormVisible3 = true;
      
    },

     open3(id) {
        this.$prompt('Ajouter un type entity', 'Tip', {
          confirmButtonText: 'OK',
          cancelButtonText: 'Cancel',
          
        }).then(value => {
          this.$message({
            type: 'success',
            message: 'enregistré'
          });
          ResearchModuleApiService.AddTypeEntityAsync(value.value);
          this.getDataForm(id);
        }).catch(() => {
          this.$message({
            type: 'info',
            message: 'Annulé'
          });       
        });
      },

    async updateResearch() {
      
            var json = {Id:this.form3.id,FktStaticEntity:this.form3.typeEntity+1,FktStaticFootprint:this.form3.levelFootPrint2+1,Name:this.form3.nameFileOsintAvaibles,Description:this.form3.desc};
            await ResearchModuleApiService.UpdateResearchModule(json);
            await this.refreshList();
            this.dialogFormVisible3 = false;
    },
    async createNewResearch(id) {
      
      var json = {FktSoftwareOnDiggos:id,FktStaticEntity:this.form3.typeEntity+1,FktStaticFootprint:this.form3.levelFootPrint2+1,Name:this.form3.nameFileOsintAvaibles,Description:this.form3.desc};
      await ResearchModuleApiService.UploadResearchModule(json);
      await this.refreshList();
      this.dialogFormVisible2 = false;
    }, 
    async updateSoftware(id,name) {
      this.dialogFormVisible = false;
      
      var json = {Id:id,NewName:this.form2.name,Description:this.form2.desc};
      
      await SoftwareApiService.updateSoftwareAsync(json);
     
      await this.refreshList();
    },
    async deleteSoftware(id,name) {
      
      await SoftwareApiService.deleteSoftwareAsync(id);
      await this.refreshList();
    },
    
  }
}
</script>

<style>
#install{
  margin-left: 330px;
  margin-top: 35px;
}



#logiciel{
  margin-left: -1000px;
  margin-bottom: 20px;
}

#result{
  margin-top: 20px;
}


</style>