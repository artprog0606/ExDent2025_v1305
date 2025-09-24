(function (ng) {
    'use strict';

    var okMarketService = function ($http) {
        var service = this;

        service.getExportSettings = function () {
            return $http.get('okmarket/getExportSettings').then(function (response) {
                return response.data;
            });
        };

        service.saveExportSettings = function (params) {
            return $http.post('okmarket/saveExportSettings', params).then(function (response) {
                return response.data;
            });
        };

        service.export = function () {
            return $http.post('okmarket/Export').then(function (response) {
                return response.data;
            });
        };

        service.getExportProgress = function () {
            return $http.get('okmarket/getExportProgress').then(function (response) {
                return response.data;
            });
        };

        service.getExportReports = function () {
            return $http.get('okmarket/getExportReports').then(function (response) {
                return response.data;
            });
        };

        service.getExportState = function () {
            return $http.get('okmarket/getExportState').then(function (response) {
                return response.data;
            });
        };

        service.importProducts = function () {
            return $http.post('okmarket/importProducts').then(function (response) {
                return response.data;
            });
        };

        service.getImportProgress = function () {
            return $http.get('okmarket/getImportProgress').then(function (response) {
                return response.data;
            });
        };

        service.getImportReports = function () {
            return $http.get('okmarket/getImportReports').then(function (response) {
                return response.data;
            });
        };

        service.getImportState = function () {
            return $http.get('okmarket/getImportState').then(function (response) {
                return response.data;
            });
        };

        service.getDeleteState = function () {
            return $http.get('okmarket/getDeleteState').then(function (response) {
                return response.data;
            });
        };
    };

    okMarketService.$inject = ['$http'];

    ng.module('okMarketExport').service('okMarketService', okMarketService);
})(window.angular);
