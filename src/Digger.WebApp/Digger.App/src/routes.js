import Vue from "vue";
import Router from "vue-router";

// Routes

// Search
import Search from "./component/Search/Search.vue";
import ShowSearch from "./component/Search/ShowSearch.vue";
import Project from "./component/Search/Project.vue";
import Invitation from "./component/Search/Invitation.vue";

// Admin
import Admin from "./component/Admin/Admin.vue";
import AdminSoftware from "./component/Admin/AdminSoftware.vue";
import SoftwareEdit from "./component/Admin/SoftwareEdit.vue";

// Graph
import GraphDraw from "./component/Graph/Graph.vue";

export default new Router({
  routes: [
    {
      path: "/Home",
      name: "Project",
      component: Project
    },
    {
      path: '/Home/invitation',
      name: "Invitation",
      component: Invitation
    },
    {
      path:'/Home/Project',
      name:'Project',
      component: Project,
    },
    {
      path: "/Home/Search/ShowSearch",
      name: "ShowSearch",
      component: ShowSearch
    },
    {
      path: "/Home/Admin",
      name: "Admin",
      component: Admin
    },
    {
      path: "/Home/Admin/Software",
      name: "AdminSoftware",
      component: AdminSoftware
    },
    {
      path: "/Home/Admin/Software/SoftwareEdit",
      name: "SoftwareEdit",
      component : SoftwareEdit
    },
    {
      path: "/Home/Graph/:projectId",
      name: "GraphDraw",
      component : GraphDraw
    },
  ],
  base: "/",
  mode: "history"
});