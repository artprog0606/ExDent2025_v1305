(function (ng) {
    'use strict';

    var ModalAddPhoneCtrl = function ($uibModalInstance, $http, $filter, toaster, $translate, settingsTelephonyService) {
        var ctrl = this;
        ctrl.formInited = false;

        ctrl.$onInit = function () {};

        ctrl.addPhone = function () {
            settingsTelephonyService.addPhone(ctrl.newPhone, ctrl.newOrderSourceId, ctrl.$resolve.params.phoneOrderSources).then(function () {
                $uibModalInstance.close();
            });
        };

        ctrl.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalAddPhoneCtrl.$inject = ['$uibModalInstance', '$http', '$filter', 'toaster', '$translate', 'settingsTelephonyService'];

    ng.module('settingsTelephony').controller('ModalAddPhoneCtrl', ModalAddPhoneCtrl);
})(window.angular);
