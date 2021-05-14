import { getAsync, postAsync, putAsync, deleteAsync } from '../helpers/apiHelper'
import { get } from 'https';

const endpoint = "/api/ResearchModule";

class ResearchModuleApiService {
    constructor() {

    }

    async DeleteResearchModule(id){
        return await deleteAsync(endpoint+'/DeleteResearchModule/'+id);
    }

    async UpdateResearchModule(model){
        return await putAsync(endpoint+'/UpdateResearchModule',model);
    }

    async UploadResearchModule(model){
        return await postAsync(endpoint + '/UploadResearchModule', model);
    }

    async GetInfoFormResearchModule(name){
        return await getAsync(endpoint+'/GetInfoFormResearchModule/'+name);
    }
    
    async GetSoftwareModuleResearchByTypeEntity(typeEntity){
        return await getAsync(endpoint+'/GetSoftwareModuleResearchByTypeEntity/'+typeEntity);
    }

    async AddTypeEntityAsync(name){
        return await postAsync(endpoint + '/AddTypeEntity/'+ name);
    }
}

export default new ResearchModuleApiService()