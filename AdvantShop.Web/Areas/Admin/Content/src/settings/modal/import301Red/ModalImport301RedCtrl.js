(function (ng) {
    'use strict';

    var ModalImport301RedCtrl = function ($uibModalInstance, toaster, $translate) {
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

    ModalImport301RedCtrl.$inject = ['$uibModalInstance', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalImport301RedCtrl', ModalImport301RedCtrl);
})(window.angular);
