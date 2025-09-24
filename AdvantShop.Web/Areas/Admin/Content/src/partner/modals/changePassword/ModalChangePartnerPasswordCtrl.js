(function (ng) {
    'use strict';

    var ModalChangePartnerPasswordCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.partnerId = params.partnerId;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.change = function () {
            if (ctrl.password != ctrl.password2 || ctrl.password.length < 6) {
                toaster.error('', $translate.instant('Admin.Js.Customer.PasswordMustBe6Char'));
                return;
            }

            $http
                .post('partners/changePassword', {
                    partnerId: ctrl.partnerId,
                    pass: ctrl.password,
                    pass2: ctrl.password2,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result) {
                        $uibModalInstance.close();
                        toaster.success('', $translate.instant('Admin.Js.Customer.PasswordChanged'));
                    } else {
                        toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                    }
                });
        };
    };

    ModalChangePartnerPasswordCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalChangePartnerPasswordCtrl', ModalChangePartnerPasswordCtrl);
})(window.angular);
