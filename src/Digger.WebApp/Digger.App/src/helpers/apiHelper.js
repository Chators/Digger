async function checkErrors(resp) {
    if(resp.ok) return resp;

    let errorMsg = `ERROR ${resp.status} (${resp.statusText})`;
    let serverText = await resp.text();
    if(serverText) errorMsg = `${errorMsg}: ${serverText}`;

    var error = new Error(errorMsg);
    error.response = resp;
    throw error;
}

async function toJSON(resp) {
    const result = await resp.text();
    // Si la réponse n'est pas un json, renvoit une string
    try {
        if (result) return JSON.parse(result);
    }
    catch (e) {
        return result;
    }
}

export async function postAsync(url, data) {
    return await fetch(url, {
        credentials: 'same-origin',
        method: 'POST',
        body: JSON.stringify(data),
        headers: {
            'Content-Type': 'application/json'
        }
    })
    .then(checkErrors)
    .then(toJSON);
}

export async function putAsync(url, data) {
    return await fetch(url, {
        credentials: 'same-origin',
        method: 'PUT',
        body: JSON.stringify(data),
        headers: {
            'Content-Type': 'application/json'
        }
    })
    .then(checkErrors)
    .then(toJSON);
}

export async function getAsync(url) {
    return await fetch(url, {
        credentials: 'same-origin',
        method: 'GET'
    })
    .then(checkErrors)
    .then(toJSON);
}

export async function deleteAsync(url,data) {
    return await fetch(url, {
        credentials: 'same-origin',
        method: 'DELETE',
        body: JSON.stringify(data),
        headers: {
            'Content-Type': 'application/json'
        }
    })
    .then(checkErrors)
    .then(toJSON);
}