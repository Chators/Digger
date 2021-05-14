import { getAsync, postAsync, putAsync, deleteAsync } from '../helpers/apiHelper'
import { get } from 'https';

const endpoint = "/api/Request";

class GraphApiService {
    constructor() {

    }

    async GetRequestByProjectId(projectId) {
        return await getAsync(endpoint+'/GetRequestByProjectId/'+projectId);
    }

    async StartRequest(projectId, model) {
        return await postAsync(endpoint+'/StartRequest/'+projectId, model);
    }

    async GetTypeEntity() {
        return await getAsync(endpoint+'/GetTypeEntity');
    }
}

export default new GraphApiService()