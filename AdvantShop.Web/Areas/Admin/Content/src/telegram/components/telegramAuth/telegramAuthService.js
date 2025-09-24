(function (ng) {
    'use strict';

    var telegramAuthService = function ($http) {
        var service = this;

        service.getSettings = function () {
            return $http.get('telegram/getSettings').then(function (response) {
                return response.data;
            });
        };

        service.saveSettings = function (params) {
            return $http.post('telegram/saveSettings', params).then(function (response) {
                return response.data;
            });
        };

        service.deActivate = function () {
            return $http.post('telegram/deActivate').then(function (response) {
                return response.data;
            });
        };

        service.changeSalesFunnel = function (id) {
            return $http.post('telegram/changeSaleFunnel', { id: id }).then(function (response) {
                return response.data;
            });
        };
    };

    telegramAuthService.$inject = ['$http'];

    ng.module('telegramAuth').service('telegramAuthService', telegramAuthService);
})(window.angular);
