import exportOrdersTemplate from './exportOrders.html';
(function (ng) {
    'use strict';

    var ExportOrdersCtrl = function ($http, $q, $location, $window, $interval, urlHelper, analyticsService, SweetAlert, toaster, $translate) {
        var ctrl = this;
        ctrl.exportOrdersSettings = {};
        ctrl.isStartExport = false;
        ctrl.$onInit = function () {
            $http.post('analytics/getexportorderssettings').then(function (response) {
                if (response.data.result) {
                    ctrl.exportOrdersSettings = response.data.obj;
                }
            });
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    exportProducts: ctrl,
                });
            }
        };
        ctrl.exportOrders = function () {
            $http.post('ExportImportCommon/GetCommonStatistic').then(function (response) {
                if (!response.data.IsRun) {
                    $http
                        .post('analytics/exportorders', {
                            settings: ctrl.exportOrdersSettings,
                        })
                        .then(function (response) {
                            if (response) {
                                ctrl.isStartExport = true;
                            }
                        });
                } else {
                    toaster.error(
                        '',
                        $translate.instant('Admin.Js.CommonStatistic.AlreadyRunning') +
                            ' <a href="' +
                            response.data.CurrentProcess +
                            '">' +
                            (response.data.CurrentProcessName || response.data.CurrentProcess) +
                            '</a>',
                    );
                }
            });
        };
        ctrl.progressValue = 0;
        ctrl.progressTotal = 0;
        ctrl.progressPercent = 0;
        ctrl.progressCurrentProcess = '';
        ctrl.progressCurrentProcessName = '';
        ctrl.IsRun = true;
        ctrl.FileName = '';
        ctrl.stop = 0;
        ctrl.initProgress = function () {
            ctrl.stop = $interval(function () {
                $http.post('ExportImportCommon/GetCommonStatistic').then(function (response) {
                    ctrl.IsRun = response.data.IsRun;
                    if (!response.data.IsRun) {
                        $interval.cancel(ctrl.stop);
                        ctrl.FileName =
                            response.data.FileName.indexOf('?') != -1 ? response.data.FileName : response.data.FileName + '?rnd=' + Math.random();
                    }
                    ctrl.progressTotal = response.data.Total;
                    ctrl.progressValue = response.data.Processed;
                    ctrl.progressCurrentProcess = response.data.CurrentProcess;
                    ctrl.progressCurrentProcessName = response.data.CurrentProcessName;
                });
            }, 100);
        };
    };
    ExportOrdersCtrl.$inject = [
        '$http',
        '$q',
        '$location',
        '$window',
        '$interval',
        'urlHelper',
        'analyticsService',
        'SweetAlert',
        'toaster',
        '$translate',
    ];
    ng.module('analyticsReport')
        .controller('ExportOrdersCtrl', ExportOrdersCtrl)
        .component('exportOrders', {
            templateUrl: exportOrdersTemplate,
            controller: ExportOrdersCtrl,
            bindings: {
                onInit: '&',
                cssClass: '@',
                hiddenTitle: '<?',
            },
        });
})(window.angular);
