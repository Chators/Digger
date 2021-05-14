import { getAsync, postAsync, putAsync, deleteAsync } from '../helpers/apiHelper'
import { get } from 'https';

const endpoint = "/api/User";

class UserApiService {
    constructor() {

    }

    async GetUserForInvitByProjectIdAsync(id){
        return await getAsync(endpoint+'/GetUserForInvitByProjectId/'+id);
    }

    async GetAllUsersAsync(){
        return await getAsync(endpoint+'/GetAllUsers');
    }

    async DeleteUserAsync(id){
        return await deleteAsync(endpoint+'/'+id);
    }

    async UpgradeUserInAdminAsync(id){
        return await putAsync(endpoint + '/UpgradeUserInAdmin/'+ id);
    }

    
}

export default new UserApiService()