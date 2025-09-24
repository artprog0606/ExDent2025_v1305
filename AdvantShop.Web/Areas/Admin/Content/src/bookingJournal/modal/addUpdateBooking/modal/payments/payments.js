import './payments.html';

(function (ng) {
    'use strict';

    var ModalBookingPaymentsCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.paymentLoading = true;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            var summary = params.summary;

            ctrl.getPayments(summary);
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getPayments = function (summary) {
            $http
                .post('booking/getPayments', { summary: summary })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        ctrl.payments = data.obj.payments;
                    } else {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });

                        if (!data.errors) {
                            toaster.pop('error', 'Не удалось загрузить список оплаты');
                        }
                    }
                })
                .finally(function () {
                    ctrl.paymentLoading = false;
                });
        };

        ctrl.changePayment = function (payment) {
            ctrl.selectPayment = payment;
        };

        ctrl.save = function () {
            if (ctrl.selectPayment == null) {
                toaster.pop('error', '', $translate.instant('Admin.Js.Order.SelectThePaymentMethod'));
                return;
            }

            $uibModalInstance.close({ payment: ctrl.selectPayment });
        };
    };

    ModalBookingPaymentsCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalBookingPaymentsCtrl', ModalBookingPaymentsCtrl);
})(window.angular);
