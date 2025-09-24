const REGEX_SITE_URL = /((?:http|https):\/\/((?:[\w-]+)(?:\.[\w-]+)+))(?:[\w.,@?^=%&amp;:\/~+#-]*[\w@?^=%&amp;\/~+#-])?/g;
const REGEX_APIKEY = /apikey=(.+)?&/i;
const SUFFIX = '/api/vk';
const REGEX_VK_USER_ID = /https\:\/\/vk\.com\/id(\d+)$/;
//const REGEX_VK_USER_ID_MESSAGE = /https\:\/\/vk\.com\/im\?.*sel=(\d+)$/;
const REGEX_VK_USER_ID_MESSAGE = /https\:\/\/vk\.com\/.*sel=(\d+)$/;
const REGEX_VK_USER_SYNONYM = /https\:\/\/vk\.com\/([\w\d_]+)$/;
class Widget {
    constructor(siteUrl, apikey) {
        this.apikey = apikey;
        this.siteUrl = siteUrl;
        this.ajax = new Ajax();
        this.url = new Url();
    }

    parseUrl(url) {
        return this.url.parseLocation(url);
    }

    getCustomerInfo(userVkId) {
        return this.ajax.makeRequest(this.getUrlForRequest('getCustomerInfo', userVkId)).then(function (response) {
            return response.json();
        });
    }

    createCustomer(userVkId) {
        return this.ajax.makeRequest(this.getUrlForRequest('createCustomer', userVkId)).then(function (response) {
            return response.json();
        });
    }

    createOrder(userVkId) {
        return this.ajax.makeRequest(this.getUrlForRequest('createOrder', userVkId)).then(function (response) {
            return response.json();
        });
    }

    createLead(userVkId) {
        return this.ajax.makeRequest(this.getUrlForRequest('createLead', userVkId)).then(function (response) {
            return response.json();
        });
    }

    createCustomer(userVkId) {
        return this.ajax.makeRequest(this.getUrlForRequest('createCustomer', userVkId)).then(function (response) {
            return response.json();
        });
    }

    getUrlForRequest(path, params) {
        return this.siteUrl + SUFFIX + '/' + path + '?apikey=' + this.apikey + (params != null ? '&' + this.url.toQueryString(params) : '');
    }

    authorize(url) {
        const self = this;

        const urlParsed = this.parseUrl(url);

        this.siteUrl = urlParsed.protocol + '//' + urlParsed.host + ':' + urlParsed.port + urlParsed.path.replace(SUFFIX, '');

        this.apikey = urlParsed.search.apikey;

        return this.ajax.makeRequest(this.getUrlForRequest('getInfo')).then(function (response) {
            self.setValueInStorage({
                siteUrl: self.siteUrl,
                apikey: self.apikey,
            });

            return response.json();
        });
    }

    signOut() {
        this.apikey = null;
        this.siteUrl = null;
        this.removeFromStorage();
    }

    isAuthorized(callback) {
        this.getValueFromStorage(null, function (result) {
            callback(result.siteUrl != null && result.apikey != null);
        });
    }

    getVkId(url) {
        const userIdParsed = url.match(REGEX_VK_USER_ID),
            userIdMessageParser = url.match(REGEX_VK_USER_ID_MESSAGE),
            userSynonym = url.match(REGEX_VK_USER_SYNONYM),
            id = userIdParsed != null ? userIdParsed[1] : userIdMessageParser != null ? userIdMessageParser[1] : null,
            stringId = userSynonym != null ? userSynonym[1] : null;

        let result = null;

        if (id != null || stringId != null) {
            result = {
                id: id,
                stringId: stringId,
            };
        }

        return result;
    }

    setValueInStorage(data) {
        chrome.storage.sync.set(data);
    }

    getValueFromStorage(key, callback) {
        chrome.storage.sync.get(key, callback);
    }

    removeFromStorage() {
        chrome.storage.sync.clear();
    }
}
