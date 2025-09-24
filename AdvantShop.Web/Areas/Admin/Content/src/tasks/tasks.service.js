import completeTaskTemplate from './modal/completeTask/completeTask.html';
import editTaskTemplate from './modal/editTask/editTask.html';
import changeTaskStatusTemplate from './modal/changeTaskStatus/changeTaskStatus.html';

(function (ng) {
    'use strict';
    /* @ngInject */
    var tasksService = function ($http, $uibModal, toaster, $translate, $q) {
        var service = this;

        service.getTasks = function () {
            return $http.get('tasks/getTasks').then(function (response) {
                return response.data;
            });
        };

        service.getFormData = function (id, taskGroupId) {
            return $http.get('tasks/getTaskFormData', { params: { id: id, taskGroupId: taskGroupId } }).then(function (response) {
                return response.data;
            });
        };

        service.getTaskGroups = function () {
            return $http.get('taskgroups/getTaskGroupsSelectOptions').then(function (response) {
                return response.data;
            });
        };

        service.getTaskPriorities = function () {
            return $http.get('tasks/getTaskPrioritiesSelectOptions').then(function (response) {
                return response.data;
            });
        };

        service.getTaskStatuses = function () {
            return $http.get('tasks/getTaskStatusesSelectOptions').then(function (response) {
                return response.data;
            });
        };

        service.getTaskAttachments = function (id) {
            return $http.post('tasks/getTaskAttachments', { id: id }).then(function (response) {
                return response.data;
            });
        };

        service.getTask = function (id) {
            return $http.post('tasks/getTask', { id: id }).then(function (response) {
                return response.data;
            });
        };

        service.deleteTask = function (id) {
            return $http.post('tasks/deleteTask', { id: id }).then(function (response) {
                return response.data;
            });
        };

        service.addTask = function (params) {
            return $http.post('tasks/addTask', params).then(function (response) {
                return response.data;
            });
        };

        service.editTask = function (params) {
            return $http.post('tasks/editTask', params).then(function (response) {
                return response.data;
            });
        };

        service.changeTaskStatus = function (id, status) {
            return $http.post('tasks/changeTaskStatus', { id: id, status: status }).then(function (response) {
                return response.data;
            });
        };

        service.changeAssignedManager = function (id, managerIds) {
            return $http.post('tasks/changeAssignedManager', { id: id, managerIds: managerIds }).then(function (response) {
                return response.data;
            });
        };

        service.changeAssignedManager = function (id, managerIds) {
            return $http.post('tasks/changeAssignedManager', { id: id, managerIds: managerIds }).then(function (response) {
                return response.data;
            });
        };

        service.changeAppointedManager = function (id, appointedManagerId) {
            return $http.post('tasks/changeAppointedManager', { id, appointedManagerId }).then(function (response) {
                return response.data;
            });
        };

        service.changeDueDate = function (id, date) {
            return $http.post('tasks/changeDueDate', { id, date }).then(function (response) {
                return response.data;
            });
        };

        service.changeReminder = function (id, reminder) {
            return $http.post('tasks/changeReminder', { id, reminder }).then(function (response) {
                return response.data;
            });
        };

        service.removeReminder = function (id) {
            return $http.post('tasks/removeReminder', { id }).then(function (response) {
                return response.data;
            });
        };

        service.changeTaskStatuses = function (params) {
            return $http.post('tasks/changeTaskStatuses', params).then(function (response) {
                return response.data;
            });
        };

        service.changePriority = function (id, priority) {
            return $http.post('tasks/changePriority', { id, priority }).then(function (response) {
                return response.data;
            });
        };

        service.completeTask = function (id, result, orderStatusId, dealStatusId, taskGroupId, taskStatusId) {
            return $http
                .post('tasks/completeTask', {
                    id,
                    taskResult: result,
                    orderStatusId,
                    dealStatusId,
                    taskGroupId,
                    taskStatusId,
                })
                .then(function (response) {
                    return response.data;
                });
        };

        service.getOrderStatuses = function (orderId) {
            return $http.post('tasks/getOrderStatuses', { orderId: orderId }).then(function (response) {
                return response.data;
            });
        };

        service.getDealStatuses = function (leadId) {
            return $http.post('tasks/getDealStatuses', { leadId: leadId }).then(function (response) {
                return response.data;
            });
        };

        service.acceptTask = function (id) {
            return $http.post('tasks/acceptTask', { taskId: id }).then(function (response) {
                return response.data;
            });
        };

        service.cancelTask = function (id) {
            return $http.post('tasks/cancelTask', { taskId: id }).then(function (response) {
                return response.data;
            });
        };

        service.acceptTasks = function (params) {
            return $http.post('tasks/accepttasks', params).then(function (response) {
                return response.data;
            });
        };

        service.changeSorting = function (id, prevId, nextId) {
            return $http.post('tasks/changeSorting', { id: id, prevId: prevId, nextId: nextId }).then(function (response) {
                return response.data;
            });
        };

        service.deleteAttachment = function (id, taskId) {
            return $http.post('tasks/deleteAttachment', { id: id, taskId: taskId }).then(function (response) {
                return response.data;
            });
        };

        service.loadTask = function (id, modalOptions) {
            const options = Object.assign(
                {},
                {
                    bindToController: true,
                    controller: 'ModalEditTaskCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: editTaskTemplate,
                    resolve: {
                        id: function () {
                            return id;
                        },
                    },
                    size: 'lg',
                    backdrop: 'static',
                    windowClass: 'modal__window--scrollbar-no',
                },
                modalOptions,
            );

            return $uibModal.open(options);
        };

        service.getHistory = function (id) {
            return $http.get('tasks/getHistory', { params: { id: id } }).then(function (response) {
                return response.data;
            });
        };

        service.validateTaskGroupManager = function (managerIds, taskGroupId) {
            return $http
                .get('tasks/validateTaskGroupManager', { params: { managerIds: managerIds, taskGroupId: taskGroupId } })
                .then(function (response) {
                    return response.data;
                });
        };

        service.validateTaskGroupManagerByRoles = function (managerIds, managerRoleIds, participantIds) {
            return $http
                .get('tasks/validateTaskGroupManagerByRoles', {
                    params: { managerIds: managerIds, managerRoleIds: managerRoleIds, participantIds: participantIds },
                })
                .then(function (response) {
                    return response.data;
                });
        };

        service.validateTaskData = function (appointedManagerId, taskGroupId) {
            return $http
                .get('tasks/validateTaskData', {
                    params: { appointedManagerId: appointedManagerId, taskGroupId: taskGroupId },
                })
                .then(function (response) {
                    return response.data;
                });
        };

        service.getTaskManagers = function (id, taskGroupId) {
            return $http.get('tasks/getManagers', { params: { id: id, taskGroupId: taskGroupId } }).then(function (response) {
                return response.data;
            });
        };

        service.copyTask = function (id) {
            return $http.post('tasks/copyTask', { id: id }).then(function (response) {
                return response.data;
            });
        };

        service.completeTaskShowModal = function (taskData) {
            return $uibModal.open({
                bindToController: true,
                controller: 'ModalCompleteTaskCtrl',
                controllerAs: 'ctrl',
                templateUrl: completeTaskTemplate,
                windowClass: 'modal--strecth',
                resolve: {
                    task: taskData,
                },
            }).result;
        };

        service.changeTaskGroup = function (params) {
            return $http.post('tasks/changeTaskGroup', params).then(function (response) {
                return response.data;
            });
        };

        service.changeTaskGroups = function (params) {
            return $http.post('tasks/changeTaskGroups', params).then(function (response) {
                return response.data;
            });
        };

        service.changeObserver = function (id, observerIds) {
            return $http.post('tasks/changeObserver', { id: id, observerIds: observerIds }).then(function (response) {
                return response.data;
            });
        };

        service.getTaskGroupsCanBeCompleted = function () {
            return $http.get('tasks/getTaskGroupsCanBeCompleted').then(function (response) {
                return response.data;
            });
        };

        service.getProjectStatuses = function (taskGroupId, statusType) {
            return $http.get('tasks/getProjectStatusesByStatusType', { params: { taskGroupId, statusType } }).then(function (response) {
                return response.data;
                //return response;
            });
        };

        service.getAvailableStatusesByTaskGroupId = function (taskGroupId, currentStatusId) {
            return $http.get('tasks/getAvailableStatusesByTaskGroupId', { params: { taskGroupId, currentStatusId } }).then(function (response) {
                return response.data;
                //return response;
            });
        };

        service.selectStatusByList = function (statusList) {
            if (statusList == null || statusList.length === 0) {
                return $q.resolve(null);
            } else if (statusList.length === 1) {
                return $q.resolve(statusList[0]);
            } else {
                return $uibModal
                    .open({
                        bindToController: true,
                        controller: 'ModalChangeTaskStatusCtrl',
                        controllerAs: 'ctrl',
                        templateUrl: changeTaskStatusTemplate,
                        resolve: {
                            statusList: function () {
                                return statusList;
                            },
                        },
                    })
                    .result.then(function (result) {
                        if (result !== false) {
                            return $q.resolve(result);
                        } else {
                            return $q.resolve(null);
                        }
                    })
                    .catch((result) => {
                        if ([`backdrop`, `crossClick`, `escape`].some((x) => result.includes(x))) {
                            return $q.resolve(null);
                        } else {
                            return $q.reject(result);
                        }
                    });
            }
        };
    };

    ng.module('tasks').service('tasksService', tasksService);
})(window.angular);
