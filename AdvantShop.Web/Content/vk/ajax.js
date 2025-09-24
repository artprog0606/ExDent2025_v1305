class Ajax {
    constructor() {}

    makeRequest(url, options) {
        return fetch(url, options)
            .then(function (response) {
                if (!response.ok) {
                    //throw Error(response.statusText);
                    return Promise.reject(new Error(response.statusText));
                }
                return response;
            })
            .catch(function (response) {
                return Promise.reject(response.statusText);
                //return Promise.reject(new Error(response.statusText));
                //throw Error(response.statusText);
            });
    }
}
