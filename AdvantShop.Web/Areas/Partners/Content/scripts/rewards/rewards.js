(function (ng) {
    'use strict';

    var RewardsCtrl = function ($http, toaster) {
        var ctrl = this;

        ctrl.saveNaturalPersonPaymentData = function (form) {
            $http
                .post('rewards/saveNaturalPersonPaymentData', {
                    paymentTypeId: ctrl.paymentTypeId,
                    paymentAccountNumber: ctrl.paymentAccountNumber,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result == true) {
                        toaster.success('', 'Изменения сохранены');
                        form.$setPristine();
                    }
                });
        };
    };

    RewardsCtrl.$inject = ['$http', 'toaster'];

    ng.module('rewards', []).controller('RewardsCtrl', RewardsCtrl);
})(window.angular);
