(function (ng) {
    'use strict';

    var ModalPartnerRewardPayoutCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.fpOptions = {
            dateFormat: 'd.m.Y',
            startDateFormat: 'Y-m-d',
            wrap: true,
            disable: [
                function (date) {
                    var lastDay = new Date(date.getFullYear(), date.getMonth() + 1, 0).getDate();
                    return date.getDate() != lastDay;
                },
            ],
        };

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.partnerId = params.partnerId;
            ctrl.getFormData();
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getFormData = function () {
            $http
                .post('partners/getRewardPayoutFormData', {
                    partnerId: ctrl.partnerId,
                    rewardPeriodTo: ctrl.rewardPeriodTo,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result) {
                        ctrl.amount = data.obj.amount;
                        ctrl.rewardPeriodFrom = data.obj.rewardPeriodFrom;
                        ctrl.rewardPeriodTo = data.obj.rewardPeriodTo;
                        ctrl.basis = data.obj.basis;
                    }
                });
        };

        ctrl.submit = function () {
            var params = {
                partnerId: ctrl.partnerId,
                amount: ctrl.amount,
                basis: ctrl.basis,
                rewardPeriodTo: ctrl.rewardPeriodTo,
            };
            $http.post('partners/rewardPayout', params).then(function (response) {
                var data = response.data;
                if (data.result) {
                    $uibModalInstance.close();
                    toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        };
    };

    ModalPartnerRewardPayoutCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalPartnerRewardPayoutCtrl', ModalPartnerRewardPayoutCtrl);
})(window.angular);
