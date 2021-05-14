<template>
    <div>
      <app-nav id="app-nav"></app-nav>
        <el-col>
        <span v-if="this.invitList.length > 0">
      <div id="invit">
      Invitation recu :
      </div>
    </span>
    <span v-else>
      Aucune nouvelle Invitation recu
    </span>

            <table v-if="this.invitList.length > 0" class="table table-striped table-hover table-bordered" style="width:100%">
                <thead >
                <tr>
                    <th>Nom Projet</th>
                    <th>Autheur</th>
                    <th>Operation</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="Invit of this.invitList" >
                    <td>{{Invit.projectName}}</td>
                    <td>{{Invit.userAuthorPseudo}}</td>
                    <td>
                      
                        <el-button type="success"
                      size="mini"
                      @click="AcceptInvit(Invit.projectId)">Accepter</el-button>
                      <el-button type="danger"
                      size="mini"
                      @click="CancelInvit(Invit.projectId,Invit.userAuthorId)">Refuser</el-button>
                      </td>
                </tr>
            </tbody>
            </table>
        </el-col>
        <el-col>
        <span v-if="this.invitToUserList.length > 0">
      <div id="invit2">
      Invitation envoyée :
      </div>
    </span>
    <span v-else>
      Aucune invitation envoyée
    </span>

        <table v-if="this.invitToUserList.length > 0" class="table table-striped table-hover table-bordered" style="width:100%">
            <thead >
                <tr>
                    <th>Nom Projet</th>
                    <th>Membre invité</th>
                    <th>Operation</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="InvitUser of this.invitToUserList">
                    <td>{{InvitUser.projectName}}</td>
                    <td>{{InvitUser.userInvitedPseudo}}</td>
                    <td>
                      <el-button type="danger"
                      size="mini"
                      @click="CancelInvitFromAuthor(InvitUser.projectId,InvitUser.userInvitedId)">Annuler</el-button>
                    </td>
                </tr>
            </tbody>
        </table>
        </el-col>
    </div>
</template>

<script>
import AppNav from '../AppNav.vue'
import ProjectApiService from "../../services/ProjectApiService";

export default {
  components: { AppNav },
  data() {
    return {
      invitList: [],
      invitToUserList: []
    };
  },
  async mounted() {
    this.refreshInvit();
    this.refreshInvitToUser();
  },
  methods: {
    async refreshInvit() {
      this.invitList = await ProjectApiService.GetUserInvitationByUserIdAsync(
        0
      );
    },

    async refreshInvitToUser() {
      this.invitToUserList = await ProjectApiService.GetAuthorInvitationByUserIdAsync(
        0
      );
    },

    

    async AcceptInvit(projectId) {
      var json = { UserId: 0, ProjectId: projectId };
      await ProjectApiService.AssignUserToProjectAsync(json);
      this.refreshInvit();
    },

    async CancelInvit(projectId, authorId) {
      var json = {
        UserAuthorId: authorId,
        UserInvitedId: 0,
        ProjectId: projectId
      };
      await ProjectApiService.CancelInvitationProjectAsync(json);
      this.refreshInvit();
    },

    async CancelInvitFromAuthor(projectId,invitedId){
      var json = {UserAuthorId:0,UserInvitedId:invitedId,ProjectId:projectId};
      await ProjectApiService.CancelInvitationProjectAsync(json);
      this.refreshInvitToUser();
    }
  }
};
</script>

<style>
</style>


