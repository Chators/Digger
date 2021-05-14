<template>
    <div id="main">

        <!-- Menu when click on node -->
        <el-dialog title="Gestion du noeud" :visible.sync="dialogFormVisible2">
            <el-collapse v-model="activeName" accordion>
                <el-collapse-item title="Modifier le noeud" name="1">
                    <el-form :model="form">
                        <el-form-item label="Donnée"  required>
                            <el-input v-model="form.data" auto-complete="off" ></el-input>
                        </el-form-item>
                        <el-form-item label="Type de donnée"  required>
                        <el-autocomplete
                            class="inline-input"
                            
                            v-model="form.typeOfData"
                            :fetch-suggestions="querySearch">
                        </el-autocomplete>
                        </el-form-item>
                        <el-form-item label="Source"  required>
                            <el-input class="inline-input" v-model="form.source" auto-complete="off"></el-input>
                        </el-form-item>
                        <el-form-item label="Note"  required>
                            <el-input v-model="form.note" auto-complete="off"></el-input>
                        </el-form-item>
                        <el-form-item label="Uid"  required style="display:none">
                            <el-input v-model="form.uid" auto-complete="off"></el-input>
                        </el-form-item>
                    </el-form>
                    <el-button type="primary" @click="modifyNodeForm()">Confirmer</el-button>
                </el-collapse-item>

                <el-collapse-item title="Faire des recherches" name="2">
                    

                <span v-if="softwareAndResearchModule.length > 0">
                <table class="table table-striped table-hover table-bordered" style="width:100%">
                <thead>
                <tr>
                    <th>Logiciel associé</th>
                    <th>Nom</th>
                    <th>Description</th>
                    <th>Niveau d'empreinte</th>
                    <th>Envoyer une recherche</th>
                </tr>
                </thead>
                <tbody v-for="(software, indexSoftware) of softwareAndResearchModule">
                <input v-model="formResearch.Softwares.Name" style="display:none">
                <tr v-for="(researchModule, indexResearchModule) of software.researchModule">
                    <td> {{ software.software.name }} </td>
                    <td> {{ researchModule.name }} </td>
                    <td> {{ researchModule.description }} </td>
                    <td> {{ researchModule.levelFootprint }} </td>
                    <td> <el-button center type="primary" @click="startResearch(software.software.id, researchModule.name)">Confirmer</el-button> </td>
                       <!-- <input type="checkbox" v-model="formResearch.Softwares.ResearchModules"> -->
                </tr> 
                <el-input v-model="formResearch.UidNode" style="display:none"></el-input>
                <el-input v-model="formResearch.DataEntity" style="display:none"></el-input>             
                </tbody>
                </table>
                
                </span>
                <div v-else>
                    Vous n'avez aucun logiciel associé avec ce type de donnée !
                </div>
                </el-collapse-item>
            </el-collapse>
        </el-dialog>

        <!-- Menu when create node -->
        <el-dialog title="Créer un noeud" :visible.sync="dialogFormVisible">
            <el-form :model="form">
            <el-form-item label="Donnée"  required>
                <el-input v-model="form.data" auto-complete="off" ></el-input>
                </el-form-item>
                <el-form-item label="Type de donnée"  required>
                    <el-autocomplete
                              class="inline-input"
                              v-model="form.typeOfData"
                              :fetch-suggestions="querySearch"
                              required>
                    </el-autocomplete>
                </el-form-item>
                <el-form-item label="Source"  required>
                    <el-input v-model="form.source" auto-complete="off"></el-input>
                </el-form-item>
                <el-form-item label="Note"  required>
                    <el-input v-model="form.note" auto-complete="off"></el-input>
                </el-form-item>
            </el-form>
            <span slot="footer" class="dialog-footer">
            <el-button @click="dialogFormVisible = false">Annuler</el-button>
            <el-button type="primary" @click="addNodeForm()">Confirmer</el-button>
            </span>
        </el-dialog>

        <!-- Menu for see request -->
        <el-dialog title="Requête en cours" :visible.sync="dialogFormVisible3">
            <table v-if="currentRequest.length > 0" class="table table-striped table-hover table-bordered" style="width:100%">
                <thead>
                <tr>
                    <th>Donnée</th>
                    <th>Auteur</th>
                    <th>Date de lancement</th>
                    <th>Status</th>
                </tr>
                </thead>
                <tbody>
                <tr v-for="request of currentRequest">
                    <td> {{ request.dataEntity }} </td>
                    <td> {{ request.author }} </td>
                    <td> {{ request.date }} </td>
                    <td> {{ request.status }} <i class="el-icon-loading"></i></td>
                </tr> 
                </tbody>
                </table>
                <div v-else>
                    Il n'y aucune requête en cours
                </div>
        </el-dialog>

        Nombre de noeuds : {{ nodes.length }} | Nombre de liens : {{ links.length }}
        <d3-network 
        :net-nodes="nodes" 
        :net-links="links" 
        :options="options"
        @node-click="nodeClick" 
        @link-click="linkClick"/>

        <router-link to="/Home/Project" style="display:inline"><img id="logoDigger" src="../../assets/logoDigger.png"/></router-link>
        <router-link to="/Home/Project" style="display:inline"><img id="leftArrow" src="../../assets/left-arrow.png"/></router-link>
        
        <div id="actionMenu">
        <div>
        <a v-on:click="changeActionClick(menuButton.seeButton.name)"><icon id="seeButton" v-bind:class="{active : actionClick == menuButton.seeButton.name}" name="info"></icon></a>
        <a v-on:click="changeActionClick(menuButton.selectButton.name)"><icon id="selectButton" v-bind:class="{active : actionClick == menuButton.selectButton.name}" name="mouse-pointer"></icon></a>
        <!--<a v-on:click="changeActionClick(menuButton.copyButton.name)"><icon id="copyButton" v-bind:class="{active : actionClick == menuButton.copyButton.name}" name="copy"></icon></a>-->
        <a v-on:click="changeActionClick(menuButton.deleteButton.name)"><icon id="deleteButton" v-bind:class="{active : actionClick == menuButton.deleteButton.name}" name="trash"></icon></a>
        <a @click="clickAddNode()"><icon id="addNodeButton" v-bind:class="{active : actionClick == menuButton.addNodeButton.name}" name="circle"></icon></a>
        <a v-on:click="changeActionClick(menuButton.addEdgeButton.name)"><icon id="addEdgeButton" v-bind:class="{active : actionClick == menuButton.addEdgeButton.name}" name="code-branch"></icon></a>
        <a @click="clickSeeRequest()"><icon id="seeRequest" v-bind:class="{active : actionClick == menuButton.seeRequest.name}" name="search-plus"></icon></a>
        </div>
        <div id="descriptionActionMenu">
            {{ menuButton[actionClick].description }}
        </div>
        </div>

        <el-card v-if="this.infoCard.isDisplayCard" id="infoCard" class="box-card">
            <div slot="header" class="clearfix">
                <span>Description du noeud</span>
            </div>
            <div class="text item">
                {{ "Donnée : " + infoCard.data }}
            </div>
            <div class="text item">
                {{ "Type de donnée : " + infoCard.typeOfData }}
            </div>
            <div class="text item">
                {{ "Note : " + infoCard.note }}
            </div>
            <div class="text item">
                {{ "Source : " + infoCard.source }}
            </div>
            <div class="text item">
                {{ "Auteur : " + infoCard.author }}
            </div>
            <div class="text item">
                {{ "Dernière modification : " + new Date(infoCard.lastUpdate)}}
            </div>
        </el-card>
        <el-card v-else id="infoCard" class="box-card" style="display:none">
            <div slot="header" class="clearfix">
                <span>Description du noeud</span>
            </div>
            <div class="text item">
                {{ "Donnée : " + infoCard.data }}
            </div>
            <div class="text item">
                {{ "Type de donnée : " + infoCard.typeOfData }}
            </div>
            <div class="text item">
                {{ "Note : " + infoCard.note }}
            </div>
            <div class="text item">
                {{ "Source : " + infoCard.source }}
            </div>
            <div class="text item">
                {{ "Auteur : " + infoCard.author }}
            </div>
            <div class="text item">
                {{ "Dernière modification : " + infoCard.lastUpdate}}
            </div>
        </el-card>
    </div>
</template>

<script>
window.onscroll = function () {
  window.scroll(0,0);
}

import D3Network from 'vue-d3-network';
import GraphApiService from '../../services/GraphApiService.js';
import ResearchModuleApiService from '../../services/ResearchModuleApiService.js';
import RequestApiService from '../../services/RequestApiService.js';
import * as utils from './utils.js'

export default {
    data() {
        return {
            suggestTypeEntity : [],
            maxSizeNode : 60,
            currentRequest : [],
            defaultNodeSize : 15,
            dialogFormVisible3 : false,
            nodesSize : new Map(),
            softwareAndResearchModule : {
                Software : []
            },
            nodeSelected : {},
            form : {},
            formResearch : {
                Softwares : [
                ]
            },
            activeName : 1,
            dialogFormVisible: false,
            dialogFormVisible2: false,
            jsonNodes : {},
            nodes: [
            ],
            links: [
            ],
            actionClick : "selectButton",
            nodeForce : 600,
            projectId: this.$route.params.projectId,
            infoCard : {
                "isDisplayCard" : false, 
                "data" : "",
                "author" : "",
                "lastUpdate" : "",
                "note" : "",
                "source" : "",
                "typeOfData" : ""
            },
            menuButton : {
                "seeButton" : {
                    "name" : "seeButton",
                    "description" : "Permet de voir les informations d'un noeud"
                },
                "selectButton" : {
                    "name" : "selectButton",
                    "description" : "Permet de selectionner un noeud pour y executer des actions"
                },
                /*"copyButton" : {
                    "name" : "copyButton",
                    "description" : "Permet de selectionner plusieurs noeuds pour les copiers"
                },*/
                "deleteButton" : {
                    "name" : "deleteButton",
                    "description" : "Permet de détuire un noeud ou une arrête"
                },
                "addNodeButton" : {
                    "name" : "addNodeButton",
                    "description" : "Permet d'ajouter un noeud"
                },
                "addEdgeButton" : {
                    "name" : "addEdgeButton",
                    "description" : "Permet de relier 2 noeuds",
                    "selectNode" : []
                },
                "seeRequest" : {
                    "name" : "seeRequest",
                    "description" : "Permet de voir les requêtes en cours",
                    "selectNode" : []
                }
                
            },
            offset : {
                x : 0,
                y : 0
            }
        }
    },
    
    async mounted(){
        await this.refreshList();
    },
    computed: {
        options(){
            return{
                force: this.nodeForce,
                nodeLabels: true,
                size: {
                    w : window.innerWidth - 15,
                    h : window.innerHeight - 20
                },
                offset : {
                    x : this.offset.x,
                    y : this.offset.y
                },
                canvas: false,
                linkWidth: 3
            }
        }
    },
    components: {
        
    D3Network

    },
    methods:{
        querySearch(queryString, cb) {
            var links = this.suggestTypeEntity;
            var results = queryString
            ? links.filter(this.createFilter(queryString))
            : links;
            // call callback function to return suggestions
            cb(results);
        },
        createFilter(queryString) {
            return link => {
                
                return (
                link.value.toLowerCase().indexOf(queryString.toLowerCase()) === 0
                );
            };
        },
        async clickSeeRequest() {
            this.currentRequest = await RequestApiService.GetRequestByProjectId(this.projectId);
            this.dialogFormVisible3 = true;
        },
        sizeNodes() {
            var nodesSize = new Map();

            for (var node of this.nodes) 
            {
                node._size = this.defaultNodeSize;
                nodesSize.set(node.id, 0);
            }

            for (var link of this.links)
            {
                nodesSize[link.sid] = (nodesSize[link.sid]+2 || 1);
                nodesSize[link.tid] = (nodesSize[link.tid]+2 || 1);
            }       

            for (var node of this.nodes)
            {
                if (nodesSize[node.id] != undefined) 
                {
                    if (nodesSize[node.id] > this.maxSizeNode) node._size = this.maxSizeNode;
                    else node._size += nodesSize[node.id];
                }
            } 
        },
        async GetSoftwareModuleResearchByTypeEntity(typeEntity) {
            this.softwareAndResearchModule = await ResearchModuleApiService.GetSoftwareModuleResearchByTypeEntity(typeEntity);
        },
        startResearch(softwareId, researchModuleName) {
            this.formResearch.Softwares = 
            [
                {
                    Name : softwareId,
                    ResearchModules : 
                    [
                        researchModuleName
                    ]
                }
            ];
            RequestApiService.StartRequest(this.projectId, this.formResearch);
            this.displayNotificaiton("Requête en cours", "Votre requête à bien été prise en compte par le serveur", 4500, "el-icon-share");
        },
        async clickAddNode() {
            var suggestTypeEntity = await RequestApiService.GetTypeEntity();
                this.suggestTypeEntity = [];
                for (var i = 0; i < suggestTypeEntity.length; i++)
                {
                    this.suggestTypeEntity.push({ value : suggestTypeEntity[i]});
                }
            this.form = {};
            this.dialogFormVisible = true;
        },
        async modifyNodeForm() {
            await GraphApiService.ModifyNode(this.projectId, this.form.uid, this.form.data, this.form.note, this.form.source, this.form.typeOfData);
            this.dialogFormVisible2 = false;
            this.nodeSelected.name = this.form.data;
            this.nodeSelected.note = this.form.note;
            this.nodeSelected.source = this.form.source;
            this.nodeSelected.typeOfData = this.form.typeOfData;
            this.displayNotificaiton("Noeud modifié", "Vous avez modifié un noeud", 4500, "el-icon-success");
            //this.selectNode();
        },
        async selectNode(node) {
            this.nodeSelected._color = "";
            this.nodeSelected = node;
            console.log(node);
            this.nodeSelected._color = "red";
        },
        async unSelectNode() {
            this.nodeSelected._color = "";
            this.nodeSelected = {};
        },
        async refreshList() {
            //Get the good project by params in url
            this.jsonNodes = await GraphApiService.GetProjectGraphById(this.projectId);
        
            // Init nodes
            for (var i = 0; i < this.jsonNodes.FindByIdProject.length; i++)
            {
                var currentNode = this.jsonNodes.FindByIdProject[i];

                var node = {
                    _size : this.defaultNodeSize,
                    id : currentNode.uid,
                    name : currentNode.data,
                    author : currentNode.author,
                    lastUpdate : currentNode.lastUpdate,
                    note : currentNode.note,
                    source : currentNode.source,
                    typeOfData : currentNode.typeOfData
                }

                this.nodes.push(node);

                if (currentNode.link != null)
                {
                    for (var y = 0; y < currentNode.link.length; y++)
                    {
                        var currentLink = currentNode.link[y];

                        var link = {
                            sid : currentNode.uid,
                            tid : currentLink.uid
                        }

                        this.links.push(link);
                    }
                }            
            }

            this.sizeNodes();

        },
        async addNodeForm() {
            this.dialogFormVisible = false;
            var nodeDGraph = await GraphApiService.AddNode(this.projectId, this.form.data, this.form.typeOfData, this.form.source, this.form.note);
            var node = {
                    id : nodeDGraph.FindNode[0].uid,
                    name : nodeDGraph.FindNode[0].data,
                    author : nodeDGraph.FindNode[0].author,
                    lastUpdate : nodeDGraph.FindNode[0].lastUpdate,
                    note : nodeDGraph.FindNode[0].note,
                    source : nodeDGraph.FindNode[0].source,
                    typeOfData : nodeDGraph.FindNode[0].typeOfData
            }
            this.nodes.push(node);
            this.displayNotificaiton("Noeud crée", "Vous avez crée un noeud", 4500, "el-icon-success");
            this.sizeNodes()
        },
        handleScroll(event) {
            if (event.deltaY < 0) this.nodeForce += 300;
            else if (event.deltaY > 0) this.nodeForce -= 300;
        },
        GoTo(path){
            this.$router.push(path);
        },
        nodeCardChange(node) {
            this.infoCard.isDisplayCard = true;
            this.infoCard.data = node.name;
            this.infoCard.author = node.author;
            this.infoCard.lastUpdate = node.lastUpdate;
            this.infoCard.note = node.note;
            this.infoCard.source = node.source;
            this.infoCard.typeOfData = node.typeOfData;
        },
        async nodeClick(event, node) {
            this.selectNode(node);
            if (this.actionClick == this.menuButton.seeButton.name)
            {
                this.nodeCardChange(node);
            }
            else if (this.actionClick == this.menuButton.selectButton.name)
            {
                this.nodeCardChange(node);
                var suggestTypeEntity = await RequestApiService.GetTypeEntity();
                this.suggestTypeEntity = [];
                for (var i = 0; i < suggestTypeEntity.length; i++)
                {
                    this.suggestTypeEntity.push({ value : suggestTypeEntity[i]});
                }
                await this.GetSoftwareModuleResearchByTypeEntity(node.typeOfData);
                
                this.form.data = node.name;
                this.form.typeOfData = node.typeOfData;
                this.form.source = node.source;
                this.form.note = node.note;
                this.form.uid = node.id;
                this.dialogFormVisible2 = true;
                
                for (var i = 0; i < this.softwareAndResearchModule.length; i++)
                {
                    var software = this.softwareAndResearchModule[i];
                    this.formResearch.Softwares.push(
                        {
                            Name : software.software.id
                        }
                    );
                    if (software.researchModule.length > 0)
                    {
                        this.formResearch.Softwares[i].ResearchModules = [];
                        for (var y = 0; y < software.researchModule.length; y++)
                        {
                            var researchModule = software.researchModule;
                            this.formResearch.Softwares[i].ResearchModules.push(researchModule[y].name)
                        }
                    }
                }
                this.formResearch.UidNode = node.id;
                this.formResearch.DataEntity = node.name;
            }
            /*else if (this.actionClick == this.menuButton.copyButton.name)
            {
                this.nodeCardChange(node);
            }*/
            else if (this.actionClick == this.menuButton.deleteButton.name)
            {
                // A ENLEVER
                this.infoCard.isDisplayCard = false;

                GraphApiService.DeleteNodes(this.projectId, [node.id]);
                this.removeNode(node.id);
                this.displayNotificaiton("Noeud supprimé", "Vous avez supprimé un noeud", 4500, "el-icon-delete");
                this.sizeNodes();
            }
            else if (this.actionClick == this.menuButton.addNodeButton.name)
            {
                this.nodeCardChange(node);
            }
            else if (this.actionClick == this.menuButton.addEdgeButton.name)
            {
                this.nodeCardChange(node);

                this.menuButton.addEdgeButton.selectNode.push(node.id);
                if (this.menuButton.addEdgeButton.selectNode.length == 2)
                {
                    var sourceNode = this.menuButton.addEdgeButton.selectNode[0];
                    var targetNode = this.menuButton.addEdgeButton.selectNode[1];
                    // On vérifie que le lien n'existe pas déjà
                    var nodeExist = false;
                    for (var i = 0; i < this.links.length; i++)
                    {
                        link = this.links[i];
                        var counter = 0;
                        if (link.sid == sourceNode || link.tid == sourceNode) counter++;
                        if (link.sid == targetNode || link.tid == targetNode) counter++;
                        if (counter == 2) 
                        {
                            nodeExist = true;
                            break;
                        }
                    }

                    if (!nodeExist)
                    {
                        GraphApiService.AddEdge(this.projectId, sourceNode, targetNode);
                        var link = {
                            sid : sourceNode,
                            tid : targetNode
                        }
                        this.links.push(link);
                        this.displayNotificaiton("Lien crée", "Vous avez crée un lien", 4500, "el-icon-success");
                    }
                    this.sizeNodes()
                    this.menuButton.addEdgeButton.selectNode = [];
                }
                else if (this.menuButton.addEdgeButton.selectNode.length > 2) this.menuButton.addEdgeButton.selectNode = [];
            }
        },
        linkClick(event, link) {
            if (this.actionClick == this.menuButton.selectButton.name)
            {

            }
            /*else if (this.actionClick == this.menuButton.copyButton.name)
            {
                
            }*/
            else if (this.actionClick == this.menuButton.deleteButton.name)
            {
                GraphApiService.DeleteEdge(this.projectId, link.sid, link.tid);
                this.deleteLink(link);
                this.displayNotificaiton("Lien supprimer", "Vous avez supprimé un lien", 4500, "el-icon-delete");
                this.sizeNodes();
            }
        },
        changeActionClick(value) {
            this.unSelectNode();
            this.actionClick = value;
        },
        deleteLink (link) {
            this.links.splice(link.index, 1);
        },
        removeNode (nodeId) {
            utils.removeNode(nodeId, this.nodes, (nodes) => {
                if (nodes) {
                    this.links = this.rebuildLinks(nodes)
                    this.nodes = utils.rebuildNodes(nodeId, this.links, nodes)
                }
            })
        },
        rebuildLinks (nodes) {
            if (!nodes) nodes = this.nodes
            let links = utils.rebuildLinks(nodes, this.links)
            return links.newLinks
        },
        getUppercase(word) {
            if (word == null) return word;
            this.newWord = word.charAt(0).toUpperCase() + word.substring(1).toLowerCase();
            return this.newWord;
        },
        displayNotificaiton(t, mess, duration, icon = null) {
        this.$notify.info({
          title: t,
          message: mess,
          iconClass : icon,
          duration : duration,
          dangerouslyUseHTMLString : true
        });
      }
    },
    created: function (){
        // CREER UN TRUC TANT QUE TRUE ON A LE DROIT DE DEPLACER
        window.addEventListener('keydown', (event) => {
            if (event.key == "ArrowLeft")
            {
                this.offset.x += 25;
            }
            else if (event.key == "ArrowUp")
            {
                this.offset.y += 25;
            }
            else if (event.key == "ArrowDown")
            {
                this.offset.y -= 25;
            }
            else if (event.key == "ArrowRight")
            {
                this.offset.x -= 25;
            }
            else if (event.key == "&")
            {
                this.changeActionClick(this.menuButton.seeButton.name)
            }
            else if (event.key == "é")
            {
                this.changeActionClick(this.menuButton.selectButton.name)
            }
            else if (event.key == "\"")
            {
                this.changeActionClick(this.menuButton.deleteButton.name)
            }
            else if (event.key == "'")
            {
                this.clickAddNode()
            }
            else if (event.key == "(")
            {
                this.changeActionClick(this.menuButton.addEdgeButton.name)
            }
            else if (event.key == "-")
            {
                this.clickSeeRequest()
            }
        });
        window.addEventListener('wheel', (event) => {
            this.handleScroll(event)
        });

        const connection = new this.$signalR.HubConnectionBuilder()
            .withUrl("/GraphHub")
            .configureLogging(this.$signalR.LogLevel.Information)
            .build();
        connection.on("ReceiveRequestDoneGiveNewNode", (newNode, dataEntity) => {
            newNode = JSON.parse(newNode);
            
            // On enregistre les links pour plus tard
            var links = [];

            // Init nodes
            for (var i = 0; i < newNode.FindByIdProject.length; i++)
            {
                var currentNodeIsFound = false;
                var currentNode = newNode.FindByIdProject[i];

                
                if (currentNode.link != null)
                {
                    for (var y = 0; y < currentNode.link.length; y++)
                    {
                        var currentLink = currentNode.link[y];

                            var link = {
                                sid : currentNode.uid,
                                tid : currentLink.uid
                            }
                            links.push(link);
                    }
                }

                for (var z = 0; z < this.nodes.length; z++)
                {
                    if (this.nodes[z].id == currentNode.uid)
                    {
                        currentNodeIsFound = true;
                        break;
                    }
                }

                if (!currentNodeIsFound) 
                {
                    var node = {
                        _size : this.defaultNodeSize,
                        id : currentNode.uid,
                        name : currentNode.data,
                        author : currentNode.author,
                        lastUpdate : currentNode.lastUpdate,
                        note : currentNode.note,
                        source : currentNode.source,
                        typeOfData : currentNode.typeOfData
                    }

                    this.nodes.push(node);       
                }
                
            }
            for (var i = 0; i < links.length; i++)
            {
                var linkFound = false;
                for (var y = 0; y < this.links.length; y++)
                {
                    if (links[i].sid == this.links[y].sid || links[i].sid == this.links[y].tid) 
                    {
                        
                        if (links[i].tid == this.links[y].sid || links[i].tid == this.links[y].tid)
                        {
                            
                            linkFound = true;
                            break;
                        }
                    }
                }
                if (!linkFound) this.links.push(links[i]);
            }
            var mess = "Ajouts de nouveaux noeuds sur " + this.getUppercase(dataEntity);
            this.displayNotificaiton("De nouveaux noeuds viennent d'arriver", mess, 4500, "el-icon-share");
            this.currentRequest = RequestApiService.GetRequestByProjectId(this.projectId);
            this.sizeNodes();
            });

        connection.start().catch(err => console.error(err.toString()));
    },
    destroyed () {
        window.removeEventListener('wheel', (event) => {
            this.handleScroll(event)
        });
    }
}
</script>

<style lang="scss">
    @import '../../../node_modules/vue-d3-network/dist/vue-d3-network.css';
    html, body {
        max-width: 100% !important;
        overflow-x: hidden !important;
        overflow-y: hidden !important;
    }
    #logoDigger {
        width:65px;
        height:65px;
        float:left;
        position:absolute;
        left:5px;
        top:5px;
    }
    #leftArrow {
        width:65px;
        height:65px;
        float:right;
        position:absolute;
        right:5px;
        top:5px;
    }
    #actionMenu {
        right: 3%;
        bottom: 3%;
        position:absolute;
    }
    #selectButton, #copyButton, #deleteButton, #addNodeButton, #addEdgeButton, #seeButton, #seeRequest {
        width:30px;
        height:30px;
        :hover {
            color:orangered;
        }
    }
    .active {
        color: orange;
    }
    #idActionMenu {
        height:30px;
    }

    #infoCard {
        bottom : 0;
        left : 0;
        position : absolute;
    }
    .text {
        font-size: 14px;
    }
    .item {
        margin-bottom: 18px;
    }
    .clearfix:before,
    .clearfix:after {
        display: table;
        content: "";
    }
    .clearfix:after {
        clear: both
    }
    .box-card {
        width: 300px;
    }
</style>
