(function (ng) {
    'use strict';

    var exportfeedsService = function ($http, toaster) {
        var service = this;

        service.addCategoriesToExport = function (exportFeedId, categories) {
            return $http
                .post('exportfeeds/AddCategoriesToExport', {
                    exportFeedId: exportFeedId,
                    categories: categories,
                })
                .then(function (response) {
                    return response.data;
                });
        };

        service.saveExportFeedFields = function (exportFeedId, exportFeedFields) {
            return $http
                .post('exportfeeds/SaveExportFeedFields', {
                    exportFeedId: exportFeedId,
                    exportFeedFields: exportFeedFields,
                })
                .then(function (response) {
                    return response.data;
                });
        };

        service.getCommonStatistic = function () {
            return $http.post('ExportImportCommon/GetCommonStatistic').then(function (response) {
                return response.data;
            });
        };

        service.deleteExport = function (exportFeedId) {
            return $http.post('exportfeeds/DeleteExport', { exportFeedId: exportFeedId }).then(function (response) {
                return response.data;
            });
        };

        service.saveExportFeedSettings = function (exportFeedId, exportFeedName, exportFeedDescription, commonSettings, advancedSettings) {
            return $http
                .post('exportfeeds/SaveExportFeedSettings', {
                    exportFeedId: exportFeedId,
                    exportFeedName: exportFeedName,
                    exportFeedDescription: exportFeedDescription,
                    commonSettings: commonSettings,
                    advancedSettings: advancedSettings,
                })
                .then(function (response) {
                    return response.data;
                });
        };

        service.deleteExportFile = function (exportFeedId, fileFullName) {
            return $http.post('exportfeeds/deleteExportFile', { exportFeedId: exportFeedId, fileFullName: fileFullName }).then(function (response) {
                return response.data;
            });
        };

        service.getSaleChannel = function (type) {
            return $http.get('salesChannels/getItem', { params: { type: type } }).then(function (response) {
                return response.data;
            });
        };

        service.deleteSaleChannel = function (type) {
            return $http.post('salesChannels/delete', { type: type }).then(function (response) {
                return response.data;
            });
        };

        service.getWarehouses = function () {
            return $http.get('exportfeeds/getWarehouses').then(function (response) {
                return response.data;
            });
        };
    };

    exportfeedsService.$inject = ['$http', 'toaster'];

    ng.module('exportfeeds').service('exportfeedsService', exportfeedsService);
})(window.angular);
