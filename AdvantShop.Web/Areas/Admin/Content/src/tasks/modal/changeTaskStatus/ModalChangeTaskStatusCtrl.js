(function (ng) {
    'use strict';

    var ModalChangeTaskStatusCtrl = function ($http, $uibModalInstance, toaster, $timeout) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.statuses = ctrl.$resolve.statusList;

            if (ctrl.statuses == null || ctrl.statuses.length === 0) {
                toaster.pop('error', 'Не удалось получить список статусов');
                ctrl.close();
            }
            ctrl.status = ctrl.statuses.length > 0 ? ctrl.statuses[0] : null;
        };

        ctrl.changeStatus = function () {
            $uibModalInstance.close(ctrl.status);
        };

        ctrl.close = function () {
            $uibModalInstance.close(false);
        };
    };

    ModalChangeTaskStatusCtrl.$inject = ['$http', '$uibModalInstance', 'toaster', '$timeout'];

    ng.module('uiModal').controller('ModalChangeTaskStatusCtrl', ModalChangeTaskStatusCtrl);
})(window.angular);
