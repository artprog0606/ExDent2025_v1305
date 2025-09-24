(function (ng) {
    'use strict';

    var ModalImportCardsCtrl = function ($uibModalInstance, $http, $window, toaster, $q, $translate) {
        var ctrl = this;
        ctrl.exportNotStarted = false;

        ctrl.params = {
            accrueBonuses: false,
        };

        ctrl.isStartExport = false;

        ctrl.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.close = function () {
            $uibModalInstance.close('close');
        };

        ctrl.onBeforeSend = function () {
            $http.post('ExportImportCommon/GetCommonStatistic').then(function (response) {
                var data = response.data;

                if (!data.IsRun) {
                    ctrl.btnLoading = true;
                } else {
                    ctrl.exportNotStarted = true;
                    toaster.error(
                        '',
                        $translate.instant('Admin.Js.CommonStatistic.AlreadyRunning') +
                            ' <a href="' +
                            data.CurrentProcess +
                            '">' +
                            (data.CurrentProcessName || data.CurrentProcess) +
                            '</a>',
                    );
                }
            });
        };

        ctrl.onSuccess = function () {
            if (ctrl.exportNotStarted) {
                $uibModalInstance.dismiss('cancel');
            } else {
                ctrl.isStartExport = true;
                toaster.pop('success', $translate.instant('Admin.Js.Cards.CardsUploaded'));
            }
        };
    };

    ModalImportCardsCtrl.$inject = ['$uibModalInstance', '$http', '$window', 'toaster', '$q', '$translate'];

    ng.module('uiModal').controller('ModalImportCardsCtrl', ModalImportCardsCtrl);
})(window.angular);
