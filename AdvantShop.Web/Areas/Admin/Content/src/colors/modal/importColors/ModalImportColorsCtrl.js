(function (ng) {
    'use strict';

    var ModalImportColorsCtrl = function ($uibModalInstance, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.isStartExport = false;
            ctrl.btnLoading = false;

            ctrl.CreateNewColor = false;
            ctrl.UpdateOnlyColorWithoutCodeOrIcon = true;
            ctrl.DownloadIconByLink = true;
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

        ctrl.onInitCsvFileUploader = function (fileUploader) {
            ctrl.fileUploader = fileUploader;
        };

        ctrl.import = function () {
            ctrl.fileUploader.uploadParams = {
                createNewColor: ctrl.CreateNewColor,
                updateOnlyColorWithoutCodeOrIcon: ctrl.UpdateOnlyColorWithoutCodeOrIcon,
                downloadIconByLink: ctrl.DownloadIconByLink,
            };
            ctrl.fileUploader.onSend();
        };
    };

    ModalImportColorsCtrl.$inject = ['$uibModalInstance', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalImportColorsCtrl', ModalImportColorsCtrl);
})(window.angular);
