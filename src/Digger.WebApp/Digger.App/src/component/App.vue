<template>
  <div id="app">

    <router-view></router-view>
  
    
  </div>

</template>

<script>
import AppNav from './AppNav.vue'
import ProjectApiService from '../services/ProjectApiService.js'


export default {
  name: 'app',
  components: { AppNav },
  data () {
    return {
      msg: 'Welcome little Digger !'
    };
  },
  methods: {
    displayNotificaiton(t, mess, duration, icon = null) {
        this.$notify.info({
          title: t,
          message: mess,
          iconClass : icon,
          duration : duration,
          dangerouslyUseHTMLString : true
        });
      },
      getUppercase(word) {
        if (word == null) return word;
        this.newWord = word.charAt(0).toUpperCase() + word.substring(1).toLowerCase();
        return this.newWord;
      },
      async acceptInvitationProject(userId, projectId) {
        var json = {UserId:userId,ProjectId:projectId};
        await ProjectApiService.AcceptInvitationProjectAsync(json);
      },
      async declineInvitiationProject(userAuthorId, userInvitedId, projectId) {
        var json = {UserAuthorId:userAuthorId,UserInvitedId:userInvitedId,ProjectId:projectId};
        await ProjectApiService.CancelInvitationProjectAsync(json);
      }
  },
  created: function() {
    const connection = new this.$signalR.HubConnectionBuilder()
      .withUrl("/ProjectHub")
      .configureLogging(this.$signalR.LogLevel.Information)
      .build();

    connection.on("ReceiveInvitationInProject", (authorInvitation, nameProject, userAuthorId, userInvitedId, projectId) => {
     this.displayNotificaiton("Invitation", 
        this.getUppercase(authorInvitation) + " vous à invité à rejoindre le projet " + this.getUppercase(nameProject), 
        4500, 
        "el-icon-message");
    });
    connection.on("ReceiveRequestdone", (dataEntity, nameProject) => {
    if (this.$route.name != "GraphDraw")
    {
     this.displayNotificaiton("Requête", 
        "Une requête avec pour donnée " + this.getUppercase(dataEntity) + " vient de ce terminer dans le projet " + this.getUppercase(nameProject), 
        4500, 
        "el-icon-success");
    }
    });

    connection.on("ReceiveUserJoinedProject", (nameUser, nameProject) => {
     this.displayNotificaiton("Project", 
        this.getUppercase(nameUser) + " vient de rejoindre le projet " + this.getUppercase(nameProject), 
        4500, 
        "el-icon-share");
    });

    connection.on("ReceiveUserLeavedProject", (nameUser, nameProject) => {
     this.displayNotificaiton("Project", 
        this.getUppercase(nameUser) + " ne fait plus partie du projet " + this.getUppercase(nameProject), 
        4500, 
        "el-icon-error");
    });

    
    connection.start().catch(err => console.error(err.toString()));
  }
}


</script>

<style lang="scss">
#app-nav {
  margin-bottom : 60px;
}

body {
  background-color: rgb(240, 239, 239);
}

#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #000000;
  margin-top: 5px;
}

h1, h2 {
  font-weight: normal;
}

ul {
  list-style-type: none;
  padding: 0;
}

li {
  display: inline-block;
  margin: 0 10px;
}
.el-collapse-item__content{
  margin-left: 50px;
  margin-right: 50px
}
.el-menu--horizontal{
  border: solid rgb(170, 170, 170) 1px;
}

a {
  color: #42b983;
}
</style>
