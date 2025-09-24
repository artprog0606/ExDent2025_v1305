(function (ng) {
    'use strict';

    var AnalyticsCtrl = function ($q, $location, $window, $interval, urlHelper, analyticsService, SweetAlert, toaster, $translate, isMobileService) {
        var ctrl = this;

        ctrl.exportOrdersSettings = {};
        ctrl.exportCustomersSettings = {};
        ctrl.exportProductsSettings = {};

        ctrl.isStartExport = false;

        ctrl.init = function () {};

        ctrl.exportOrders = function () {
            if (isMobileService.getValue()) {
                $window.scrollTo({
                    top: 0,
                });
            }

            analyticsService.getCommonStatistic().then(function (response) {
                if (!response.IsRun) {
                    analyticsService.exportOrders(ctrl.exportOrdersSettings).then(function (response) {
                        if (response) {
                            ctrl.isStartExport = true;
                        }
                    });
                } else {
                    toaster.error(
                        '',
                        $translate.instant('Admin.Js.CommonStatistic.AlreadyRunning') +
                            ' <a href="' +
                            response.CurrentProcess +
                            '">' +
                            (response.CurrentProcessName || response.CurrentProcess) +
                            '</a>',
                    );
                }
            });
        };

        ctrl.exportCustomers = function () {
            analyticsService.getCommonStatistic().then(function (response) {
                if (!response.IsRun) {
                    analyticsService.exportCustomers(ctrl.exportCustomersSettings).then(function (response) {
                        if (response) {
                            ctrl.isStartExport = true;
                        }
                    });
                } else {
                    toaster.error(
                        '',
                        $translate.instant('Admin.Js.CommonStatistic.AlreadyRunning') +
                            ' <a href="' +
                            response.CurrentProcess +
                            '">' +
                            (response.CurrentProcessName || response.CurrentProcess) +
                            '</a>',
                    );
                }
            });
        };
        ctrl.exportProducts = function () {
            if (ctrl.exportProductsSettings.ExportProductsType == 'OneProduct' && !ctrl.exportProductsSettings.OfferId)
                return toaster.error('', $translate.instant('Admin.Js.ExportProducts.ProductNotSelected'));
            else if (
                ctrl.exportProductsSettings.ExportProductsType == 'Categories' &&
                (!ctrl.exportProductsSettings.selectedCategories || ctrl.exportProductsSettings.selectedCategories.length == 0)
            )
                return toaster.error('', $translate.instant('Admin.Js.ExportProducts.CategoryNotSelected'));

            analyticsService.getCommonStatistic().then(function (response) {
                if (!response.IsRun) {
                    analyticsService.exportProducts(ctrl.exportProductsSettings).then(function (response) {
                        if (response) {
                            ctrl.isStartExport = true;
                        }
                    });
                } else {
                    toaster.error(
                        '',
                        $translate.instant('Admin.Js.CommonStatistic.AlreadyRunning') +
                            ' <a href="' +
                            response.CurrentProcess +
                            '">' +
                            (response.CurrentProcessName || response.CurrentProcess) +
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
                analyticsService.getCommonStatistic().then(function (response) {
                    ctrl.IsRun = response.IsRun;
                    if (!response.IsRun) {
                        $interval.cancel(ctrl.stop);
                        ctrl.FileName = response.FileName.indexOf('?') != -1 ? response.FileName : response.FileName + '?rnd=' + Math.random();
                    }
                    ctrl.progressTotal = response.Total;
                    ctrl.progressValue = response.Processed;
                    ctrl.progressCurrentProcess = response.CurrentProcess;
                    ctrl.progressCurrentProcessName = response.CurrentProcessName;
                });
            }, 100);
        };

        ctrl.treeCallbacks = {
            select_node: function (event, data) {
                var tree = ng.element(event.target).jstree(true);
                var selected = tree.get_selected(false);

                ctrl.exportProductsSettings.selectedCategories = selected;

                toaster.pop('success', $translate.instant('Admin.Js.Analytics.CategoryAddedToExport'));
            },

            deselect_node: function (event, data) {
                var tree = ng.element(event.target).jstree(true);
                var selected = tree.get_selected(false);
                ctrl.exportProductsSettings.selectedCategories = selected;

                toaster.pop('success', $translate.instant('Admin.Js.Analytics.CategoryRemovedFromExport'));
            },
        };

        ctrl.selectProduct = function (result) {
            if (result && result.ids) ctrl.exportProductsSettings.OfferId = result.ids[0];
            else toaster.pop('error', 'Не удалось выбрать товар');
        };
    };

    AnalyticsCtrl.$inject = [
        '$q',
        '$location',
        '$window',
        '$interval',
        'urlHelper',
        'analyticsService',
        'SweetAlert',
        'toaster',
        '$translate',
        'isMobileService',
    ];

    ng.module('analytics', ['urlHelper', 'isMobile']).controller('AnalyticsCtrl', AnalyticsCtrl);
})(window.angular);
