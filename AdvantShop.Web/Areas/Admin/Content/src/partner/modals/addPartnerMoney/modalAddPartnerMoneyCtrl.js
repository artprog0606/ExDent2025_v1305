(function (ng) {
    'use strict';

    var ModalAddPartnerMoneyCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.partnerId = params.partnerId;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.submit = function () {
            $http.post('partners/addMoney', { partnerId: ctrl.partnerId, amount: ctrl.amount, basis: ctrl.basis }).then(function (response) {
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

    ModalAddPartnerMoneyCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalAddPartnerMoneyCtrl', ModalAddPartnerMoneyCtrl);
})(window.angular);
