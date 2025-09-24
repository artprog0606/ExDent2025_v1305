(function (ng) {
    'use strict';

    var ModalAddLandingCtrl = function ($uibModalInstance, $http, $window, toaster) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve || {};
            ctrl.siteId = params.siteId;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.addLanding = function () {
            if (ctrl.name == '') return;

            var params = {
                name: ctrl.name,
                type: ctrl.type,
                productIds: ctrl.productIds,
                goal: ctrl.goal,
                siteId: ctrl.siteId,
            };

            $http.post('funnels/add', params).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    $window.location.assign(data.url);
                    $uibModalInstance.close();
                } else {
                    data.errors.forEach(function (err) {
                        toaster.pop('error', '', err);
                    });
                }
            });
        };
    };

    ModalAddLandingCtrl.$inject = ['$uibModalInstance', '$http', '$window', 'toaster'];

    ng.module('uiModal').controller('ModalAddLandingCtrl', ModalAddLandingCtrl);
})(window.angular);
