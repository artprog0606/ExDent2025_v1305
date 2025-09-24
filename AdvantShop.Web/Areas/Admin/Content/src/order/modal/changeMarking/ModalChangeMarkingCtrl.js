(function (ng) {
    'use strict';

    var ModalChangeMarkingCtrl = function ($uibModalInstance, $window, toaster, $q, $http, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.orderItemId = params.orderItemId;

            ctrl.getItems();
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getItems = function () {
            $http.get('orders/getMarking', { params: { orderItemId: ctrl.orderItemId } }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    ctrl.codes = data.obj.Codes;
                    ctrl.name = data.obj.Name;
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', '', error);
                    });
                }
            });
        };

        ctrl.save = function () {
            $http.post('orders/saveMarking', { orderItemId: ctrl.orderItemId, codes: ctrl.codes }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Order.DataSavedSuccessfully'));
                    $uibModalInstance.close();
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', '', error);
                    });
                }
            });
        };
    };

    ModalChangeMarkingCtrl.$inject = ['$uibModalInstance', '$window', 'toaster', '$q', '$http', '$translate'];

    ng.module('uiModal').controller('ModalChangeMarkingCtrl', ModalChangeMarkingCtrl);
})(window.angular);
