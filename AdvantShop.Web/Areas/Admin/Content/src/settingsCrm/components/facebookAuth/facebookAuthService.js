(function (ng) {
    'use strict';

    var facebookAuthService = function ($http) {
        var service = this;

        service.getSettings = function () {
            return $http.get('facebook/getSettings').then(function (response) {
                return response.data;
            });
        };

        service.saveAuthUser = function (params) {
            return $http.post('facebook/saveAuthUser', params).then(function (response) {
                return response.data;
            });
        };

        service.saveGroupToken = function (params) {
            return $http.post('facebook/saveGroupToken', params).then(function (response) {
                return response.data;
            });
        };

        service.deleteGroup = function (params) {
            return $http.post('facebook/deleteGroup', params).then(function (response) {
                return response.data;
            });
        };

        service.saveSettings = function (id, createLeadFromMessages, createLeadFromComments) {
            return $http
                .post('facebook/saveSettings', {
                    id: id,
                    createLeadFromMessages: createLeadFromMessages,
                    createLeadFromComments: createLeadFromComments,
                })
                .then(function (response) {
                    return response.data;
                });
        };
    };

    facebookAuthService.$inject = ['$http'];

    ng.module('facebookAuth').service('facebookAuthService', facebookAuthService);
})(window.angular);
