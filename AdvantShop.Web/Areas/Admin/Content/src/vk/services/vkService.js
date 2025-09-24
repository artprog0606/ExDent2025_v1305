(function (ng) {
    const vkService = function ($http) {
        const service = this;

        service.getVkSettings = function () {
            return $http.get('vk/getVkSettings').then((response) => response.data);
        };

        service.saveAuthVkUser = function (params) {
            return $http.post('vk/saveAuthVkUser', params).then((response) => params);
        };

        service.getUserAccessToken = function (params) {
            return $http.post('vk/getUserAccessToken', params).then((response) => response.data);
        };

        service.getGroups = function () {
            return $http.post('vk/getVkGroups').then((response) => response.data);
        };

        service.saveAuthVkGroup = function (params) {
            return $http.post('vk/saveAuthVkGroup', params).then((response) => response.data);
        };

        service.deleteGroup = function (params) {
            return $http.post('vk/deleteGroup', params).then((response) => response.data);
        };

        service.saveSettings = function (id, createLeadFromMessages, createLeadFromComments, syncOrdersFromVk) {
            return $http
                .post('vk/saveSettings', {
                    id,
                    createLeadFromMessages,
                    createLeadFromComments,
                    syncOrdersFromVk,
                })
                .then((response) => response.data);
        };

        service.removeChannel = function () {
            return $http.post('salesChannels/delete', { type: 'vk' }).then((response) => response.data);
        };

        service.changeGroupMessageErrorStatus = function () {
            return $http.post('vk/changeGroupMessageErrorStatus').then((response) => response.data);
        };
    };

    vkService.$inject = ['$http'];

    ng.module('vkAuth').service('vkService', vkService);
})(window.angular);
