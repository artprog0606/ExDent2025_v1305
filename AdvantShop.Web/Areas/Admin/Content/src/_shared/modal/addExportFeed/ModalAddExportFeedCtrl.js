(function (ng) {
    'use strict';

    var ModalAddExportFeedCtrl = function ($uibModalInstance, $http, $window, $translate, toaster) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.exportfeedTypes = [];
            ctrl.openInNewTab = params.openInNewTab;

            $http.post('exportfeeds/GetAvalableTypes', { type: params.type }).then(function (response) {
                var data = response.data;
                ctrl.exportfeedTypes = response.data.obj;
                if (data.result != true || !ctrl.exportfeedTypes || ctrl.exportfeedTypes.length == 0) {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.AddExportFeed.ErrorGettingTypes'));
                } else {
                    ctrl.type = ctrl.exportfeedTypes[0];
                }
            });
        };

        ctrl.add = function () {
            var params = {
                name: ctrl.name,
                description: ctrl.description,
                type: ctrl.type.value,
            };

            $http.post('exportfeeds/add', params).then(function (response) {
                if (response.data.result == true) {
                    $uibModalInstance.close();
                    if (ctrl.openInNewTab)
                        $window.open(`exportfeeds/exportfeed${response.data.obj.typeUrlPostfix}/${response.data.obj.id}`, '_blank');
                    else $window.location.assign(`exportfeeds/exportfeed${response.data.obj.typeUrlPostfix}/${response.data.obj.id}`);
                } else {
                    toaster.error('', $translate.instant('Admin.Js.AddExportFeed.ErrorCreatingNewExport'));
                }
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getInstructionLink = function (type) {
            switch (type) {
                case 'Csv':
                    return 'https://www.advantshop.net/help/pages/export-csv-columns';
                case 'CsvV2':
                    return 'https://www.advantshop.net/help/pages/export-csv-columns-v2';
                case 'YandexMarket':
                    return 'https://www.advantshop.net/help/pages/export-yandex-market';
            }
            return '';
        };
    };

    ModalAddExportFeedCtrl.$inject = ['$uibModalInstance', '$http', '$window', '$translate', 'toaster'];

    ng.module('uiModal').controller('ModalAddExportFeedCtrl', ModalAddExportFeedCtrl);
})(window.angular);
