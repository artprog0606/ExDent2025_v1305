import exportCustomersTemplate from './exportCustomers.html';
(function (ng) {
    'use strict';

    var ExportCustomersCtrl = function ($http, $q, $location, $window, $interval, urlHelper, SweetAlert, toaster, $translate) {
        var ctrl = this;
        ctrl.exportCustomersSettings = {};
        ctrl.isStartExport = false;
        ctrl.tab = 'params';
        ctrl.$onInit = function () {
            ctrl.getExportSettings();
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    exportProducts: ctrl,
                });
            }
        };
        ctrl.getExportSettings = function () {
            $http.post('customers/getExportCustomersSettings').then(function (response) {
                if (response.data.result) {
                    ctrl.exportCustomersSettings = response.data.obj;
                    ctrl.exportCustomersSettings.DefaultSelectedExportFields = ctrl.exportCustomersSettings.SelectedExportFields.slice();
                    ctrl.SocialTypes = [
                        {
                            label: $translate.instant('Admin.Js.Customers.VKontakte'),
                            value: 'vk',
                        },
                        {
                            label: 'Instagram',
                            value: 'instagram',
                        },
                        {
                            label: 'Telegram',
                            value: 'telegram',
                        },
                        {
                            label: $translate.instant('Admin.Js.Customers.AnySocialNetwork'),
                            value: 'all',
                        },
                    ];
                    ctrl.YesNos = [
                        {
                            label: 'Да',
                            value: true,
                        },
                        {
                            label: 'Нет',
                            value: false,
                        },
                    ];
                }
            });
        };
        ctrl.setNoneExportFeedFields = function () {
            for (let i = 0; i < ctrl.exportCustomersSettings.SelectedExportFields.length; i++) {
                ctrl.exportCustomersSettings.SelectedExportFields[i] = 'None';
            }
        };
        ctrl.setDefaultExportFeedFields = function () {
            ctrl.exportCustomersSettings.SelectedExportFields = ctrl.exportCustomersSettings.DefaultSelectedExportFields.slice();
        };
        ctrl.exportCustomers = function () {
            if (!ctrl.showRegistrationDateFilter) {
                ctrl.exportCustomersSettings.RegistrationDateFrom = null;
                ctrl.exportCustomersSettings.RegistrationDateTo = null;
            }
            if (!ctrl.showOrdersCountFilter) {
                ctrl.exportCustomersSettings.OrdersCountFrom = null;
                ctrl.exportCustomersSettings.OrdersCountTo = null;
            }
            if (!ctrl.showOrderSumFilter) {
                ctrl.exportCustomersSettings.OrderSumFrom = null;
                ctrl.exportCustomersSettings.OrderSumTo = null;
            }
            if (!ctrl.showLastOrderFilter) {
                ctrl.exportCustomersSettings.LastOrderFrom = null;
                ctrl.exportCustomersSettings.LastOrderTo = null;
            }
            if (!ctrl.showLastOrderDateTimeFilter) {
                ctrl.exportCustomersSettings.LastOrderDateTimeFrom = null;
                ctrl.exportCustomersSettings.LastOrderDateTimeTo = null;
            }
            if (!ctrl.showAverageCheckFilter) {
                ctrl.exportCustomersSettings.AverageCheckFrom = null;
                ctrl.exportCustomersSettings.AverageCheckTo = null;
            }
            if (!ctrl.showSocialTypeFilter) {
                ctrl.exportCustomersSettings.SocialType = null;
            }
            if (!ctrl.showHasBonusCardFilter) {
                ctrl.exportCustomersSettings.HasBonusCard = null;
            }
            if (!ctrl.showSubscriptionFilter) {
                ctrl.exportCustomersSettings.Subscription = null;
            }
            if (!ctrl.showManagerFilter) {
                ctrl.exportCustomersSettings.ManagerId = null;
            }
            if (!ctrl.showCustomerTypeFilter) {
                ctrl.exportCustomersSettings.CustomerType = null;
            }
            $http.post('ExportImportCommon/GetCommonStatistic').then(function (response) {
                if (!response.data.IsRun) {
                    $http
                        .post('customers/exportCustomers', {
                            settings: ctrl.exportCustomersSettings,
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
            }, 250);
        };
    };
    ExportCustomersCtrl.$inject = ['$http', '$q', '$location', '$window', '$interval', 'urlHelper', 'SweetAlert', 'toaster', '$translate'];
    ng.module('exportCustomers', [])
        //.controller('ExportCustomersCtrl', ExportCustomersCtrl)
        .component('exportCustomers', {
            templateUrl: exportCustomersTemplate,
            controller: ExportCustomersCtrl,
            bindings: {
                onInit: '&',
            },
        });
})(window.angular);
