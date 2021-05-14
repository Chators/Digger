import { getAsync, postAsync, putAsync, deleteAsync } from '../helpers/apiHelper'
import { get } from 'https';

const endpoint = "/api/Graph";

class GraphApiService {
    constructor() {

    }

    async GetProjectGraphById(projectId){
        return await getAsync(endpoint+'/GetProjectGraphById/'+projectId);
    }

    async ModifyNode(projectId, uid, data, note, source, typeOfData) {
        var model = { "Uid" : uid, "Data" : data, "Note" : note, "Source" : source, "TypeOfData" : typeOfData }
        return await putAsync(endpoint+'/ModifyNode/'+projectId, model);
    }

    async AddNode(projectId, data, typeOfData, source, note){
        var model = { "Data" : data, "Note" : note, "Source" : source, "TypeOfData" : typeOfData }
        return await postAsync(endpoint+'/AddNode/'+projectId, model);
    }

    async AddEdge(projectId, sourceNode, targetNode){
        var model = { "SourceNode" : sourceNode, "TargetNode" : targetNode }
        return await postAsync(endpoint+'/AddEdge/'+projectId, model);
    }

    async DeleteNodes(projectId, NodesId){
        var model = { "NodesId" : NodesId }
        return await deleteAsync(endpoint+'/DeleteNodes/'+projectId, model);
    }
    
    async DeleteEdge(projectId, sourceNode, targetNode){
        var model = { "SourceNode" : sourceNode, "TargetNode" : targetNode }
        return await deleteAsync(endpoint+'/DeleteEdge/'+projectId, model);
    }
}

export default new GraphApiService()