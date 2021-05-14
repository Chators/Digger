import { getAsync, postAsync, putAsync, deleteAsync } from '../helpers/apiHelper'

const endpoint = "/api/search";

class SearchApiService {
    constructor() {

    }

    async SearchAsync(model) {
        return await postAsync(endpoint + '/StartSearch', model);
    }

}

export default new SearchApiService()