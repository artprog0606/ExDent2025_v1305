(function (ng) {
    'use strict';

    var ModalChangeOrderRecipientCtrl = function ($uibModalInstance, $http, $translate, toaster) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.orderId = params.orderId;
            ctrl.getOrderRecipient();
        };

        ctrl.getOrderRecipient = function () {
            return $http.get('orders/getOrderRecipient', { params: { orderId: ctrl.orderId } }).then(function (response) {
                ctrl.data = response.data.obj;
                ctrl.form.$setPristine();
                ctrl.formInited = true;
            });
        };

        ctrl.save = function () {
            var action = ctrl.data.IsNew ? 'addOrderRecipient' : 'updateOrderRecipient';
            return $http.post('orders/' + action, ctrl.data).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                    $uibModalInstance.close(ctrl.data);
                } else {
                    ctrl.btnLoading = false;
                    data.errors.forEach(function (error) {
                        toaster.pop('error', '', error);
                    });
                }

                return data;
            });
        };

        ctrl.close = function () {
            $uibModalInstance.close();
        };
    };

    ModalChangeOrderRecipientCtrl.$inject = ['$uibModalInstance', '$http', '$translate', 'toaster'];

    ng.module('uiModal').controller('ModalChangeOrderRecipientCtrl', ModalChangeOrderRecipientCtrl);
})(window.angular);
