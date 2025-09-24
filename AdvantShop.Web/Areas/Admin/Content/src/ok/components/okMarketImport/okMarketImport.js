import okMarketImportTemplate from './okMarketImport.html';
(function (ng) {
    'use strict';

    var okMarketImportCtrl = function (toaster, okMarketService) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.getReports();
            ctrl.getImportState();
        };
        ctrl.import = function () {
            okMarketService.importProducts().then(function (data) {
                if (data.result === true) {
                    toaster.pop('success', '', 'Импорт товаров начался');
                    ctrl.ImportIsRun = true;
                    ctrl.getImportProgress();
                    ctrl.getReportsTimeout();
                } else {
                    data.errors.forEach(function (e) {
                        toaster.pop('error', '', e);
                    });
                }
            });
        };
        ctrl.getImportProgress = function () {
            okMarketService.getImportProgress().then(function (data) {
                ctrl.Total = data.Total;
                ctrl.Current = data.Current;
                ctrl.Percent = ctrl.Total > 0 ? parseInt((100 / ctrl.Total) * ctrl.Current) : 0;
                ctrl.ImportIsRun = data.IsRun;
                if (!ctrl.ImportIsRun && ctrl.Total > 0) {
                    toaster.pop('success', '', 'Импорт товаров закончен');
                } else {
                    setTimeout(function () {
                        ctrl.getImportProgress();
                    }, 500);
                }
            });
        };
        ctrl.getImportState = function () {
            okMarketService.getImportState().then(function (data) {
                ctrl.ImportIsRun = data.isRun;
                if (ctrl.ImportIsRun) ctrl.getImportProgress();
            });
        };
        ctrl.getReportsTimeout = function () {
            setTimeout(function () {
                ctrl.getReports().then(function () {
                    if (ctrl.Percent != 100) {
                        ctrl.getReportsTimeout();
                    }
                });
            }, 3000);
        };
        ctrl.getReports = function () {
            return okMarketService.getImportReports().then(function (data) {
                ctrl.Reports = data.reports;
            });
        };
    };
    okMarketImportCtrl.$inject = ['toaster', 'okMarketService'];
    ng.module('okMarketImport', []).controller('okMarketImportCtrl', okMarketImportCtrl).component('okMarketImport', {
        templateUrl: okMarketImportTemplate,
        controller: 'okMarketImportCtrl',
    });
})(window.angular);
