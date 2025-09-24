(function (ng) {
    'use strict';

    var vkMarketService = function ($http) {
        var service = this;

        service.getAuthSettings = function () {
            return $http.get('vkMarket/getAuthSettings').then(function (response) {
                return response.data;
            });
        };

        service.saveAuth = function (params) {
            return $http.post('vkMarket/saveAuth', params).then(function (response) {
                return params;
            });
        };

        service.getGroups = function () {
            return $http.get('vkMarket/getGroups').then(function (response) {
                return response.data;
            });
        };

        service.saveGroup = function (params) {
            return $http.post('vkMarket/saveGroup', params).then(function (response) {
                return response.data != null ? response.data.obj : null;
            });
        };

        service.deleteGroup = function () {
            return $http.post('vkMarket/deleteGroup').then(function (response) {
                return response.data;
            });
        };

        service.getExportSettings = function () {
            return $http.get('vkMarket/getExportSettings').then(function (response) {
                return response.data;
            });
        };

        service.saveExportSettings = function (params) {
            return $http.post('vkMarket/saveExportSettings', params).then(function (response) {
                return response.data;
            });
        };

        service.export = function () {
            return $http.post('vkMarket/export').then(function (response) {
                return response.data;
            });
        };

        service.getExportProgress = function () {
            return $http.get('vkMarket/getExportProgress').then(function (response) {
                return response.data;
            });
        };

        service.import = function (settings) {
            return $http.post('vkMarket/importProducts', settings).then(function (response) {
                return response.data;
            });
        };

        service.getImportProgress = function () {
            return $http.get('vkMarket/getImportProgress').then(function (response) {
                return response.data;
            });
        };

        service.getReports = function () {
            return $http.get('vkMarket/getReports').then(function (response) {
                return response.data;
            });
        };

        service.deleteAllProducts = function () {
            return $http.post('vkMarket/deleteAllProducts').then(function (response) {
                return response.data;
            });
        };
    };

    vkMarketService.$inject = ['$http'];

    ng.module('vkMarket', []).service('vkMarketService', vkMarketService);
})(window.angular);
