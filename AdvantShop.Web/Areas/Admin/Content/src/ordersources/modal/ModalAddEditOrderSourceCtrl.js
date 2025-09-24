(function (ng) {
    'use strict';

    var ModalAddEditOrderSourceCtrl = function ($uibModalInstance, $http, $translate, toaster) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.id = params.id != null ? params.id : 0;
            ctrl.mode = ctrl.id != 0 ? 'edit' : 'add';

            ctrl.getTypes().then(function () {
                if (ctrl.mode == 'add') {
                    ctrl.data = {
                        Type: ctrl.types[0].value,
                        SortOrder: 0,
                    };
                } else {
                    ctrl.loadOrderSource();
                }
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getTypes = function () {
            return $http.get('ordersources/getTypes').then(function (response) {
                ctrl.types = response.data;
            });
        };

        ctrl.loadOrderSource = function () {
            $http.get('ordersources/getOrderSource', { params: { id: ctrl.id } }).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    ctrl.data = data.obj;
                } else {
                    ctrl.close();
                }
            });
        };

        ctrl.saveSource = function () {
            var url = ctrl.mode == 'add' ? 'ordersources/addOrderSource' : 'ordersources/updateOrderSource';
            $http.post(url, ctrl.data).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                    $uibModalInstance.close();
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        };
    };

    ModalAddEditOrderSourceCtrl.$inject = ['$uibModalInstance', '$http', '$translate', 'toaster'];

    ng.module('uiModal').controller('ModalAddEditOrderSourceCtrl', ModalAddEditOrderSourceCtrl);
})(window.angular);
