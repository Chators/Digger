import { getAsync, postAsync, putAsync, deleteAsync } from '../helpers/apiHelper'
import { get } from 'https';

const endpoint = "/api/Software";

class SoftwareApiService {
    constructor() {

    }
    async uploadSoftwareAsync(model){
        return await postAsync(endpoint + '/UploadSoftware', model);
    }

    async updateSoftwareAsync(model){
        return await putAsync(endpoint + '/UpdateSoftware', model);
    }

    async deleteSoftwareAsync(id) {
        return await deleteAsync(endpoint + '/DeleteSoftware/'+ id);
    }

    async getAllSoftwareAndResearchAsync() {
        return await getAsync(endpoint + '/GetAllSoftwareAndResearch');
    }
}
export default new SoftwareApiService()