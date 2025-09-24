(function (ng) {
    'use strict';

    var okService = function ($http) {
        var service = this;

        service.removeChannel = function () {
            return $http.post('salesChannels/delete', { type: 'ok' }).then(function (response) {
                return response.data;
            });
        };

        service.getOkSettings = function () {
            return $http.get('ok/getOkSettings').then(function (response) {
                return response.data;
            });
        };

        service.validatePrimarySettings = function (params) {
            return $http.post('ok/validatePrimarySettings', params).then(function (response) {
                return response.data;
            });
        };

        service.changeMarketGroup = function (params) {
            return $http.post('ok/changeMarketGroup', params).then(function (response) {
                return response.data;
            });
        };

        service.removeBinding = function () {
            return $http.post('ok/removeBinding').then(function (response) {
                return response.data;
            });
        };

        service.changeSaleFunnel = function (params) {
            return $http.post('ok/changeSaleFunnel', params).then(function (response) {
                return response.data;
            });
        };

        service.toggleSubscriptionToMessages = function (subscribe) {
            return $http.post('ok/toggleSubscriptionToMessages', { subscribe: subscribe }).then(function (response) {
                return response.data;
            });
        };
    };

    okService.$inject = ['$http'];

    ng.module('okChannel').service('okService', okService);
})(window.angular);
