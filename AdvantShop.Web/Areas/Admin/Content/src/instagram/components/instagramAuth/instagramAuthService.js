(function (ng) {
    'use strict';

    var instagramAuthService = function ($http) {
        var service = this;

        service.getInstagramSettings = function () {
            return $http.get('instagram/getSettings').then(function (response) {
                return response.data;
            });
        };

        service.saveLoginSettings = function (params) {
            return $http.post('instagram/saveLoginSettings', params).then(function (response) {
                return response.data;
            });
        };

        service.deActivate = function () {
            return $http.post('instagram/deActivate').then(function (response) {
                return response.data;
            });
        };

        service.saveSettings = function (id, createLeadFromDirectMessages, createLeadFromComments) {
            return $http
                .post('instagram/saveSettings', {
                    id: id,
                    createLeadFromDirectMessages: createLeadFromDirectMessages,
                    createLeadFromComments: createLeadFromComments,
                })
                .then(function (response) {
                    return response.data;
                });
        };

        service.requireChallengeCode = function (apiPath, choiceMethod) {
            return $http.post('instagram/requireChallengeCode', { apiPath: apiPath, choiceMethod: choiceMethod }).then(function (response) {
                return response.data;
            });
        };

        service.sendChallengeCode = function (apiPath, code) {
            return $http.post('instagram/sendChallengeCode', { apiPath: apiPath, code: code }).then(function (response) {
                return response.data;
            });
        };

        service.removeSaleChannel = function () {
            return $http.post('salesChannels/delete', { type: 'Instagram' }).then(function (response) {
                return response.data;
            });
        };
    };

    instagramAuthService.$inject = ['$http'];

    ng.module('instagramAuth').service('instagramAuthService', instagramAuthService);
})(window.angular);
