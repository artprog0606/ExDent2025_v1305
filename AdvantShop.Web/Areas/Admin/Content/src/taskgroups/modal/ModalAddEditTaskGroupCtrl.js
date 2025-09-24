import copyTaskGroupTemplate from './copyTaskGroup/copyTaskGroup.html';
(function (ng) {
    'use strict';

    var ModalAddEditTaskGroupCtrl = function ($uibModalInstance, $http, $window, $q, toaster, $translate, tasksService, $uibModal) {
        var ctrl = this;
        ctrl.formInited = false;
        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.id = params.id != null ? params.id : 0;
            ctrl.goToProjectPage = params.goToProjectPage;
            ctrl.mode = ctrl.id != 0 ? 'edit' : 'add';
            ctrl.managerIds = [];
            ctrl.managerRoleIds = [];
            ctrl.participantIds = [];
            ctrl.projectStatuses = [];
            ctrl.systemProjectStatuses = [];
            ctrl.getFormData().then(function () {
                if (ctrl.mode == 'add') {
                    ctrl.name = '';
                    ctrl.sortOrder = 0;
                    ctrl.enabled = true;
                    ctrl.isPrivateComments = false;
                    ctrl.formInited = true;
                    ctrl.managersTaskGroupConstraint = -1;
                    ctrl.isNotBeCompleted = false;
                } else {
                    ctrl.getTaskGroup(ctrl.id);
                }
            });
        };
        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
        ctrl.getTaskGroup = function (id) {
            $http
                .get('taskgroups/getTaskGroup', {
                    params: {
                        id: id,
                        rnd: Math.random(),
                    },
                })
                .then(function (response) {
                    if (response.data.result == true) {
                        var data = response.data.obj;
                        if (data != null) {
                            ctrl.name = data.Name;
                            ctrl.sortOrder = data.SortOrder;
                            ctrl.enabled = data.Enabled;
                            ctrl.isPrivateComments = data.IsPrivateComments;
                            ctrl.managerIds = data.ManagerIds || [];
                            ctrl.managerRoleIds = data.ManagerRoleIds || [];
                            ctrl.participantIds = data.ParticipantIds || [];
                            ctrl.managersTaskGroupConstraint = data.ManagersTaskGroupConstraint;
                            ctrl.isNotBeCompleted = data.IsNotBeCompleted;
                        }
                        ctrl.formAddEditTaskGroup.$setPristine();
                        ctrl.formInited = true;
                    } else {
                        ctrl.close();
                        response.data.errors.forEach(function (error) {
                            toaster.error(error);
                        });
                    }
                });
        };
        ctrl.save = function () {
            ctrl.validateTaskGroupManagerByRoles().then(function (result) {
                if (!result) {
                    return;
                }
                var params = {
                    id: ctrl.id,
                    name: ctrl.name,
                    enabled: ctrl.enabled,
                    isPrivateComments: ctrl.isPrivateComments,
                    sortOrder: ctrl.sortOrder,
                    managerIds: ctrl.managerIds,
                    managerRoleIds: ctrl.managerRoleIds,
                    participantIds: ctrl.participantIds,
                    managersTaskGroupConstraint: ctrl.managersTaskGroupConstraint,
                    isNotBeCompleted: ctrl.isNotBeCompleted,
                    projectStatuses: ctrl.projectStatuses.concat(ctrl.systemProjectStatuses),
                };
                var url = ctrl.mode == 'add' ? 'taskgroups/addTaskGroup' : 'taskgroups/updateTaskGroup';
                $http.post(url, params).then(function (response) {
                    var data = response.data;
                    if (data.result == true) {
                        toaster.pop(
                            'success',
                            '',
                            ctrl.mode == 'add'
                                ? $translate.instant('Admin.Js.Taskgroups.ModalAddEdit.ProjectAdded')
                                : $translate.instant('Admin.Js.Taskgroups.ModalAddEdit.ChangesSaved'),
                        );
                        if (ctrl.goToProjectPage === true) {
                            $uibModalInstance.close(params);
                            $window.location.assign('projects/' + data.obj);
                        } else {
                            $uibModalInstance.close(params);
                        }
                    } else {
                        if (data.errors) {
                            response.data.errors.forEach(function (error) {
                                toaster.error(error);
                            });
                        } else {
                            toaster.error(
                                $translate.instant('Admin.Js.Taskgroups.ModalAddEdit.Error'),
                                $translate.instant('Admin.Js.Taskgroups.ModalAddEdit.ErrorWhile') +
                                    (ctrl.mode == 'add'
                                        ? $translate.instant('Admin.Js.Taskgroups.ModalAddEdit.Creating')
                                        : $translate.instant('Admin.Js.Taskgroups.ModalAddEdit.Editing')),
                            );
                        }
                    }
                });
            });
        };
        ctrl.getFormData = function () {
            return $http
                .get('taskgroups/getFormData', {
                    params: {
                        taskGroupId: ctrl.id,
                    },
                })
                .then(function (response) {
                    if (response.data != null) {
                        ctrl.managers = response.data.managers;
                        ctrl.managerRoles = response.data.managerRoles;
                        ctrl.participants = response.data.participants;
                        ctrl.managersTaskConstraints = response.data.managersTaskConstraints;
                    }
                });
        };
        ctrl.validateTaskGroupManagerByRoles = function () {
            if (
                ctrl.id == null ||
                ctrl.managerIds == null ||
                ctrl.managerIds.length == 0 ||
                ((ctrl.managerRoleIds == null || ctrl.managerRoleIds.length == 0) && (ctrl.participantIds == null || ctrl.participantIds.length == 0))
            ) {
                return $q.resolve(true);
            }
            return tasksService.validateTaskGroupManagerByRoles(ctrl.managerIds, ctrl.managerRoleIds, ctrl.participantIds).then(function (data) {
                if (data.errors != null) {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', '', error, 10000);
                    });
                    return false;
                }
                return true;
            });
        };
        ctrl.copyTaskGroup = function () {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalCopyTaskGroupCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: copyTaskGroupTemplate,
                    resolve: {
                        taskGroupId: function () {
                            return ctrl.id;
                        },
                        name: function () {
                            return ctrl.name;
                        },
                    },
                })
                .result.then(
                    function (result) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Taskgroups.ModalAddEdit.ChangesSaved'));
                        return result;
                    },
                    function (result) {
                        return result;
                    },
                );
        };

        ctrl.onChangeProjectStatuses = function (items, systemItems) {
            ctrl.projectStatuses = items;
            ctrl.systemProjectStatuses = systemItems;
        };
    };
    ModalAddEditTaskGroupCtrl.$inject = ['$uibModalInstance', '$http', '$window', '$q', 'toaster', '$translate', 'tasksService', '$uibModal'];
    ng.module('uiModal').controller('ModalAddEditTaskGroupCtrl', ModalAddEditTaskGroupCtrl);
})(window.angular);
