(function (ng) {
    'use strict';

    var ModalCertificateSettingsCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.getCertificate();
        };

        ctrl.getCertificate = function () {
            $http.get('certificates/getSettings').then(function (response) {
                ctrl.settings = response.data;
                ctrl.settings.Tax = ctrl.settings.Tax + ''; //toString
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function () {
            ctrl.btnSleep = true;

            var params = ctrl.settings;
            $http.post('certificates/saveSettings', params).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Certificates.ChangesSaved'));
                    $uibModalInstance.close();
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.Certificates.ErrorWhileSaving'));
                }
                ctrl.btnSleep = false;
            });
        };
    };

    ModalCertificateSettingsCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalCertificateSettingsCtrl', ModalCertificateSettingsCtrl);
})(window.angular);
