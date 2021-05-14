import Vue from 'vue'
import Vuex from 'vuex'
import VueRouter from 'vue-router'
import ElementUI from 'element-ui'
import 'element-ui/lib/theme-chalk/index.css';
//ICON
import 'vue-awesome/icons'
import Icon from 'vue-awesome/components/Icon'

import router from './routes.js'

import App from './component/App.vue'

Vue.component('icon', Icon)

const signalR = require("@aspnet/signalr")
Vue.prototype.$signalR = signalR;

Vue.use(ElementUI)
Vue.use(VueRouter)

new Vue({
  el: '#app',
  router,
  render: h => h(App),
  components: {
    Icon
  }
})

