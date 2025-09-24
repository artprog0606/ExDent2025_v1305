(function (ng) {
    'use strict';

    var ModalFinishTaskCtrl = function ($http, $uibModalInstance, lastStatisticsService, toaster, $translate, tasksService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.taskId = ctrl.$resolve.id;
            if (!ctrl.taskId) {
                $uibModalInstance.dismiss();
                return;
            }
            ctrl.getFormData();
        };

        ctrl.getFormData = function () {
            $http.post('tasks/getAcceptTaskForm', { taskId: ctrl.taskId }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    ctrl.successStatus = data.obj.successStatus;
                    ctrl.cancelStatus = data.obj.cancelStatus;
                } else {
                    toaster.pop('error', '', 'Невозможно завершить задачу');
                    $uibModalInstance.dismiss();
                }
            });
        };

        ctrl.acceptTask = function () {
            ctrl.btnSleep = true;
            tasksService.acceptTask(ctrl.taskId).then(function (response) {
                if (response.result === true) {
                    toaster.pop('success', '', 'Изменения сохранены');
                    $uibModalInstance.close(true);
                    lastStatisticsService.getLastStatistics();
                } else {
                    toaster.pop('error', '', 'Невозможно завершить задачу');
                    $uibModalInstance.close(false);
                }
                ctrl.btnSleep = false;
            });
        };

        ctrl.cancelTask = function () {
            ctrl.btnCancelSleep = true;
            tasksService.cancelTask(ctrl.taskId).then(function (response) {
                if (response.result === true) {
                    toaster.pop('success', '', 'Изменения сохранены');
                    $uibModalInstance.close(true);
                    lastStatisticsService.getLastStatistics();
                } else {
                    toaster.pop('error', '', 'Невозможно завершить задачу');
                    $uibModalInstance.close(false);
                }
                ctrl.btnCancelSleep = false;
            });
        };
    };

    ModalFinishTaskCtrl.$inject = ['$http', '$uibModalInstance', 'lastStatisticsService', 'toaster', '$translate', 'tasksService'];

    ng.module('uiModal').controller('ModalFinishTaskCtrl', ModalFinishTaskCtrl);
})(window.angular);
