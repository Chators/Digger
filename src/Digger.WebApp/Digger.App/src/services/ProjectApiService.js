import { getAsync, postAsync, putAsync, deleteAsync } from '../helpers/apiHelper'
import { get } from 'https';

const endpoint = "/api/Project";

class SoftwareApiService {
    constructor() {

    }
    async AcceptInvitationProjectAsync(model){
        return await postAsync(endpoint + '/AssignUserToProject', model);
    }

    async CancelInvitationProjectAsync(model){
        return await deleteAsync(endpoint + '/CancelInvitationProject', model);
    }

    async CreateProjectAsync(model){
        return await postAsync(endpoint+'/CreateProject', model);
    }

    async GetProjectByUserIdAsync(id){
        return await getAsync(endpoint+'/GetProjectByUserId/'+id);
    }

    async DeleteProjectAsync(id){
        return await deleteAsync(endpoint+'/DeleteProject/'+id);
    }

    async UpdateProjectAsync(model){
        return await putAsync(endpoint+'/UpdateProject',model);
    }

    async SendInvitationProjectAsync(model){
        return await postAsync(endpoint+'/SendInvitationProject',model);
    }

    async GetUserInvitationByUserIdAsync(id){
        return await getAsync(endpoint+'/GetUserInvitationByUserId/'+id);
    }

    async AssignUserToProjectAsync(model){
        return await postAsync(endpoint+'/AssignUserToProject',model);
    }

    async CancelInvitationProjectAsync(model){
        return await deleteAsync(endpoint+'/CancelInvitationProject',model);
    }

    async GetAuthorInvitationByUserIdAsync(id){
        return await getAsync(endpoint+'/GetAuthorInvitationByUserId/'+id);
    }
    async UnassignUserToProjectAsync(model){
        return await deleteAsync(endpoint + '/UnassignUserToProject',model);
    }

    async GetUserInProjectAsync(id){
        return await getAsync(endpoint + '/GetUserInProject/'+id);
    }

    async ChangeAccessRightUserInProjectAsync(model){
        return await putAsync(endpoint + '/ChangeAccessRightUserInProject',model);
    }

    async GetProjectPublicByUserIdAsync(id){
        return await getAsync(endpoint + '/GetProjectPublicByUserId/'+id);
    }
}
export default new SoftwareApiService()