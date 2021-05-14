import { getAsync, postAsync, putAsync, deleteAsync } from '../helpers/apiHelper'
import { get } from 'https';

const endpoint = "/Authentification";

class AuthentificationApiService {
    constructor() {

    }

    async Logout(){
        return await postAsync(endpoint + '/Logout');
    }

    async RegisterAsync(model){
        
        return await postAsync('/Register',model);
    }

    
}

export default new AuthentificationApiService()