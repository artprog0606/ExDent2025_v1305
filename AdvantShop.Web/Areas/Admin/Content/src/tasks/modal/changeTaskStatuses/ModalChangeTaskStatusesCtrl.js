(function (ng) {
    'use strict';

    var ModalChangeTaskStatusesCtrl = function ($uibModalInstance, tasksService, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.params = ctrl.$resolve.params;
            ctrl.canAccept = ctrl.$resolve.canAccept;
            tasksService.getTaskStatuses().then(function (result) {
                ctrl.statuses = result;
                if (ctrl.canAccept) {
                    ctrl.statuses.push({
                        label: $translate.instant('Admin.Js.Tasks.ModalChangeTask.Accepted'),
                        value: 'accept',
                    });
                }
                if (result.length > 0) {
                    ctrl.status = result[0];
                }
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.changeStatuses = function () {
            if (ctrl.status.value === 'accept') {
                tasksService.acceptTasks(ctrl.params).then(function (result) {
                    $uibModalInstance.close('taskAccepted');
                });
            } else {
                tasksService.changeTaskStatuses(ng.extend(ctrl.params || {}, { status: ctrl.status.value })).then(function (result) {
                    $uibModalInstance.close('changedStatus');
                });
            }
        };
    };

    ModalChangeTaskStatusesCtrl.$inject = ['$uibModalInstance', 'tasksService', '$translate'];

    ng.module('uiModal').controller('ModalChangeTaskStatusesCtrl', ModalChangeTaskStatusesCtrl);
})(window.angular);
