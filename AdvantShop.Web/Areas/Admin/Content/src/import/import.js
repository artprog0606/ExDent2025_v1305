(function (ng) {
    'use strict';

    var ImportCtrl = function ($q, $location, $window, $interval, urlHelper, importService, SweetAlert, toaster, Upload, isMobileService) {
        var ctrl = this;

        ctrl.importSettings = { selectedFields: [] };
        ctrl.firstObjectFields = [];
        ctrl.headers = [];
        ctrl.allFields = [];
        ctrl.showFields = false;
        ctrl.warningMsg = null;

        ctrl.$onInit = function () {
            ctrl.fileUploaderCtrl = {};
        };

        ctrl.processFieldsFromCsv = function (data) {
            ctrl.importSettings.selectedFields = [];
            if (data.result) {
                ctrl.firstObjectFields = data.obj.FirstItem;
                ctrl.headers = data.obj.Headers;
                ctrl.allFields = data.obj.AllFields;
                ctrl.warningMsg = data.obj.WarningMsg;
                ctrl.showFields = true;

                if (data.obj.SelectedFields == null) {
                    for (var i = 0; i < ctrl.headers.length; i++) {
                        ctrl.importSettings.selectedFields[i] = ctrl.allFields[ctrl.headers[i]] ? ctrl.headers[i] : 'none';
                    }
                } else {
                    ctrl.importSettings.selectedFields = data.obj.SelectedFields;
                }
            } else {
                ctrl.firstObjectFields = [];
                ctrl.headers = [];
                ctrl.allFields = [];
                ctrl.warningMsg = null;

                ctrl.changeNewFile();
            }
        };

        ctrl.getFieldsFromCsvFile = function () {
            return importService.getFieldsFromCsvFile(ctrl.importSettings).then(function (data) {
                ctrl.processFieldsFromCsv(data);
                return data;
            });
        };

        ctrl.getFieldsFromCategoriesCsvFile = function () {
            return importService.getFieldsFromCategoriesCsvFile(ctrl.importSettings).then(function (data) {
                ctrl.processFieldsFromCsv(data);
                return data;
            });
        };

        ctrl.getFieldsFromCustomersCsvFile = function () {
            return importService.getFieldsFromCustomersCsvFile(ctrl.importSettings).then(function (data) {
                ctrl.processFieldsFromCsv(data);
                return data;
            });
        };

        ctrl.getFieldsFromLeadsCsvFile = function () {
            return importService.getFieldsFromLeadsCsvFile(ctrl.importSettings).then(function (data) {
                ctrl.processFieldsFromCsv(data);
                return data;
            });
        };

        ctrl.getFieldsFromBrandsCsvFile = function () {
            return importService.getFieldsFromBrandsCsvFile(ctrl.importSettings).then(function (data) {
                ctrl.processFieldsFromCsv(data);
                return data;
            });
        };

        ctrl.getFieldsFromOrdersCsvFile = function () {
            return importService.getFieldsFromOrdersCsvFile(ctrl.importSettings).then(function (data) {
                ctrl.processFieldsFromCsv(data);
                return data;
            });
        };

        ctrl.startImport = function (func) {
            ctrl.inProgress = true;
            func(ctrl.importSettings)
                .then(function (data) {
                    if (data.result) {
                        ctrl.isStartExport = true;
                    } else {
                        toaster.error('', (data.errors || [])[0]);
                    }
                })
                .finally(function () {
                    ctrl.inProgress = false;
                });
        };

        ctrl.startProductsImport = function () {
            ctrl.startImport(importService.startProductsImport);
        };

        ctrl.startCategoriesImport = function () {
            ctrl.startImport(importService.startCategoriesImport);
        };

        ctrl.startCustomersImport = function () {
            ctrl.startImport(importService.startCustomersImport);
        };

        ctrl.startLeadsImport = function () {
            ctrl.startImport(importService.startLeadsImport);
        };

        ctrl.startBrandsImport = function () {
            ctrl.startImport(importService.startBrandsImport);
        };

        ctrl.startOrdersImport = function () {
            ctrl.startImport(importService.startOrdersImport);
        };

        //ctrl.isSaas = false;
        //ctrl.productsCount = 0;
        //ctrl.tariffProductsCount = 0;

        //ctrl.initProgress = function () {
        //    ctrl.getSaasDataInformation().then(function () {
        //        if (!ctrl.isSaas) {
        //            return;
        //        }
        //        ctrl.stop = $interval(function () {
        //            ctrl.getSaasDataInformation();
        //        }, 500);
        //    });

        //};

        ctrl.abortImport = function () {
            importService.abortImport().then(function (data) {
                ctrl.IsRun = false;
            });
        };

        //ctrl.getSaasDataInformation = function () {
        //    return importService.getSaasBlockInformation().then(function (data) {
        //        ctrl.productsCount = data.productsCount;
        //        ctrl.tariffProductsCount = data.productsInTariff;
        //        ctrl.isSaas = data.isSaas;
        //    });
        //};

        ctrl.getLogFile = function () {
            importService.getLogFile();
        };

        ctrl.getExampleCustomersFile = function () {
            importService.getExampleCustomersFile(ctrl.importSettings);
        };

        ctrl.getExampleLeadsFile = function () {
            importService.getExampleLeadsFile(ctrl.importSettings);
        };

        ctrl.getExampleBrandsFile = function () {
            importService.getExampleBrandsFile(ctrl.importSettings);
        };

        ctrl.getExampleOrdersFile = function () {
            importService.getExampleOrdersFile(ctrl.importSettings);
        };

        ctrl.changeNewFile = function () {
            ctrl.isStartExport = false;
            ctrl.showFields = false;
        };

        ctrl.onSelectTab = function (indexTab) {
            ctrl.tabActiveIndex = indexTab;
        };

        ctrl.getFileUploaderCtrlForZipFile = function (param) {
            ctrl.fileUploaderCtrlForZipFile = param;
        };

        ctrl.getFileUploaderCtrlForCsvFile = function (param) {
            ctrl.fileUploaderCtrlForCsvFile = param;
        };
    };

    ImportCtrl.$inject = [
        '$q',
        '$location',
        '$window',
        '$interval',
        'urlHelper',
        'importService',
        'SweetAlert',
        'toaster',
        'Upload',
        'isMobileService',
    ];

    ng.module('import', ['urlHelper', 'ngFileUpload', 'isMobile', 'fileUploader']).controller('ImportCtrl', ImportCtrl);
})(window.angular);
