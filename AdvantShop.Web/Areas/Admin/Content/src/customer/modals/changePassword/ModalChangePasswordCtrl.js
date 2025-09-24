(function (ng) {
    'use strict';

    var ModalChangePasswordCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.customerId = params.customerId;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.change = function () {
            if (ctrl.password != ctrl.password2 || ctrl.password.length < 6) {
                toaster.pop('error', '', $translate.instant('Admin.Js.Customer.PasswordMustBe6Char'));
                return;
            }

            $http
                .post('customers/changePassword', {
                    customerId: ctrl.customerId,
                    pass: ctrl.password,
                    pass2: ctrl.password2,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result) {
                        $uibModalInstance.close();
                        toaster.pop('success', '', $translate.instant('Admin.Js.Customer.PasswordChanged'));
                    } else {
                        toaster.pop('error', '', data.error);
                    }
                });
        };
    };

    ModalChangePasswordCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalChangePasswordCtrl', ModalChangePasswordCtrl);
})(window.angular);
