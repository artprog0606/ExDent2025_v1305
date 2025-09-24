(function (ng) {
    'use strict';

    var ModalCompleteTaskCtrl = function ($translate, $uibModalInstance, lastStatisticsService, tasksService, toaster) {
        var ctrl = this;

        ctrl.$onInit = function () {
            const emptyOptions = {
                label: $translate.instant(`Admin.Js.Tasks.NotSelected`),
                value: null,
                selected: true,
            };

            ctrl.task = ctrl.$resolve.task;
            ctrl.taskResult = ctrl.task.taskResult;
            if (ctrl.task == null) {
                ctrl.close();
                return;
            }
            tasksService.getTaskGroupsCanBeCompleted().then(function (data) {
                if (data) {
                    ctrl.isNotBeCompleted = data.every((x) => parseFloat(x.value, 10) !== ctrl.task.taskGroupId);

                    ctrl.taskGroups = data;

                    if (ctrl.isNotBeCompleted) {
                        ctrl.taskGroups.unshift(emptyOptions);
                        ctrl.task.taskGroupId = emptyOptions.value;
                    } else if (ctrl.task.statusList != null) {
                        if (ctrl.task.statusList.length > 1) {
                            ctrl.statusList = ctrl.task.statusList;
                        } else if (ctrl.task.statusList.length === 1) {
                            ctrl.taskStatusId = ctrl.task.statusList[0].id;
                        }
                    }
                }
            });

            if (ctrl.task.orderId != null) {
                tasksService.getOrderStatuses(ctrl.task.orderId).then(function (data) {
                    ctrl.orderStatuses = data;
                    for (var i = 0, len = ctrl.orderStatuses.length; i < len; i++) {
                        if (ctrl.orderStatuses[i].selected === true) {
                            ctrl.orderStatusIdCurrent = ctrl.orderStatuses[i].value;
                            ctrl.orderStatusId = ctrl.orderStatuses[i].value;
                        }
                    }
                });
            } else if (ctrl.task.leadId != null) {
                tasksService.getDealStatuses(ctrl.task.leadId).then(function (data) {
                    ctrl.dealStatuses = data;
                    for (var i = 0, len = ctrl.dealStatuses.length; i < len; i++) {
                        if (ctrl.dealStatuses[i].selected === true) {
                            ctrl.dealStatusIdCurrent = ctrl.dealStatuses[i].value;
                            ctrl.dealStatusId = ctrl.dealStatuses[i].value;
                        }
                    }
                });
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.completeTask = function () {
            ctrl.btnSleep = true;
            tasksService
                .completeTask(ctrl.task.id, ctrl.taskResult, ctrl.orderStatusId, ctrl.dealStatusId, ctrl.task.taskGroupId, ctrl.taskStatusId)
                .then(function (data) {
                    if (data != null && data.result === true) {
                        toaster.success(
                            '',
                            $translate.instant('Admin.Js.Tasks.Tasks.Task') +
                                ' <a  href="tasks/view/' +
                                ctrl.task.id +
                                '">№' +
                                ctrl.task.id +
                                '</a> ' +
                                $translate.instant('Admin.Js.EditTask.Completed') +
                                '<br>' +
                                (data.obj != null && data.obj.orderId != null
                                    ? $translate.instant('Admin.Js.CompleteTask.OrderCreated') +
                                      ' <a href="orders/edit/' +
                                      data.obj.orderId +
                                      '">' +
                                      data.obj.orderNumber +
                                      '</a>'
                                    : ''),
                        );
                        $uibModalInstance.close({
                            taskResult: ctrl.taskResult,
                            taskId: ctrl.task.id,
                            taskGroupId: ctrl.task.taskGroupId,
                            statusId: ctrl.taskStatusId,
                            statusType: ctrl.statusList?.find((x) => x.id === ctrl.taskStatusId)?.name,
                        });
                        lastStatisticsService.getLastStatistics();
                    } else {
                        toaster.error($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.FailedToCompleteTask'));
                        $uibModalInstance.close(false);
                    }
                    ctrl.btnSleep = false;
                });
        };

        ctrl.getStatusList = function (taskGroupId) {
            ctrl.statusList = null;
            ctrl.taskStatusId = null;

            tasksService.getAvailableStatusesByTaskGroupId(taskGroupId).then((response) => {
                //1 - status Complete
                const statusListComplete = response.obj.filter((x) => x.statusType === 1);
                if (statusListComplete.length > 1) {
                    ctrl.statusList = statusListComplete;
                } else if (statusListComplete.length === 0) {
                    ctrl.statusList = response.obj;
                }

                if (ctrl.statusList != null) {
                    ctrl.taskStatusId = ctrl.statusList[0].id;
                }
            });
        };
    };

    ModalCompleteTaskCtrl.$inject = ['$translate', '$uibModalInstance', 'lastStatisticsService', 'tasksService', 'toaster'];

    ng.module('uiModal').controller('ModalCompleteTaskCtrl', ModalCompleteTaskCtrl);
})(window.angular);
