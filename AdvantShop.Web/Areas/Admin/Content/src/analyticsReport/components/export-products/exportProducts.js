import exportProductsTemplate from './exportProducts.html';
(function (ng) {
    'use strict';

    var ExportProductsCtrl = function ($http, $q, $location, $window, $interval, urlHelper, SweetAlert, toaster, $translate) {
        var ctrl = this;
        ctrl.exportProductsSettings = {};
        ctrl.isStartExport = false;
        ctrl.$onInit = function () {
            $http.post('analytics/getexportproductssettings').then(function (response) {
                if (response.data.result) {
                    ctrl.exportProductsSettings = response.data.obj;
                    ctrl.exportProductsSettings.ExportProductsType = 'AllProducts';
                }
            });
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    exportProducts: ctrl,
                });
            }
        };
        ctrl.exportProducts = function () {
            if (ctrl.exportProductsSettings.ExportProductsType == 'OneProduct' && !ctrl.exportProductsSettings.OfferId)
                return toaster.error('', $translate.instant('Admin.Js.ExportProducts.ProductNotSelected'));
            else if (
                ctrl.exportProductsSettings.ExportProductsType == 'Categories' &&
                (!ctrl.exportProductsSettings.SelectedCategories || ctrl.exportProductsSettings.SelectedCategories.length == 0)
            )
                return toaster.error('', $translate.instant('Admin.Js.ExportProducts.CategoryNotSelected'));
            $http.post('ExportImportCommon/GetCommonStatistic').then(function (response) {
                if (!response.data.IsRun) {
                    $http
                        .post('analytics/exportproducts', {
                            settings: ctrl.exportProductsSettings,
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
        ctrl.treeCallbacks = {
            select_node: function (event, data) {
                var tree = ng.element(event.target).jstree(true);
                var selected = tree.get_selected(false);
                ctrl.exportProductsSettings.SelectedCategories = selected;
                toaster.pop('success', $translate.instant('Admin.Js.Analytics.CategoryAddedToExport'));
            },
            deselect_node: function (event, data) {
                var tree = ng.element(event.target).jstree(true);
                var selected = tree.get_selected(false);
                ctrl.exportProductsSettings.SelectedCategories = selected;
                toaster.pop('success', $translate.instant('Admin.Js.Analytics.CategoryRemovedFromExport'));
            },
        };
        ctrl.selectProduct = function (result) {
            if (result && result.ids) ctrl.exportProductsSettings.OfferId = result.ids[0];
            else toaster.pop('error', 'Не удалось выбрать товар');
        };
    };
    ExportProductsCtrl.$inject = ['$http', '$q', '$location', '$window', '$interval', 'urlHelper', 'SweetAlert', 'toaster', '$translate'];
    ng.module('analyticsReport')
        .controller('ExportProductsCtrl', ExportProductsCtrl)
        .component('exportProducts', {
            templateUrl: exportProductsTemplate,
            controller: ExportProductsCtrl,
            bindings: {
                onInit: '&',
            },
        });
})(window.angular);
