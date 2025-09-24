(function (ng) {
    'use strict';

    var ModalCopyTaskGroupCtrl = function ($uibModalInstance, $http, toaster) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.taskGroupId = params.taskGroupId;
            ctrl.name = params.name + ' - копия';
            ctrl.copyTasks = false;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.copyTaskGroup = function () {
            var params = {
                taskGroupId: ctrl.taskGroupId,
                name: ctrl.name,
                copyTasks: ctrl.copyTasks,
            };

            $http.post('taskgroups/CopyTaskGroup', params).then(function (response) {
                if (response.data.result == true) {
                    $uibModalInstance.close();
                } else {
                    toaster.pop('error', response.data.error);
                    ctrl.btnLoading = false;
                }
            });
        };
    };

    ModalCopyTaskGroupCtrl.$inject = ['$uibModalInstance', '$http', 'toaster'];

    ng.module('uiModal').controller('ModalCopyTaskGroupCtrl', ModalCopyTaskGroupCtrl);
})(window.angular);
