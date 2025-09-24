(function (ng) {
    'use strict';

    var ModalimportLocalizationCtrl = function ($uibModalInstance, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.isStartExport = false;
            ctrl.btnLoading = false;
        };

        ctrl.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.onBeforeSend = function () {
            ctrl.isStartExport = true;
            ctrl.btnLoading = true;
        };

        ctrl.onSuccess = function (data) {
            toaster.pop('success', $translate.instant('Admin.Js.Import301Red.Success'));
            $uibModalInstance.close('close');
        };

        ctrl.onUpdate = function (data) {
            return { result: true };
        };
    };

    ModalimportLocalizationCtrl.$inject = ['$uibModalInstance', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalimportLocalizationCtrl', ModalimportLocalizationCtrl);
})(window.angular);
