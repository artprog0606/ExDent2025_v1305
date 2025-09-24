(function (ng) {
    'use strict';

    var ModalSubtractPartnerMoneyCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.partnerId = params.partnerId;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.submit = function () {
            $http.post('partners/subtractMoney', { partnerId: ctrl.partnerId, amount: ctrl.amount, basis: ctrl.basis }).then(function (response) {
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

    ModalSubtractPartnerMoneyCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalSubtractPartnerMoneyCtrl', ModalSubtractPartnerMoneyCtrl);
})(window.angular);
