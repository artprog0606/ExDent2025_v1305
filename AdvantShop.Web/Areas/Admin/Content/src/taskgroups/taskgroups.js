import addEditTaskGroupTemplate from './modal/addEditTaskGroup.html';
import copyTaskGroupTemplate from './modal/copyTaskGroup/copyTaskGroup.html';
(function (ng) {
    'use strict';

    var TaskGroupsCtrl = function (
        $q,
        $location,
        $window,
        $http,
        $uibModal,
        uiGridConstants,
        uiGridCustomConfig,
        uiGridCustomParamsConfig,
        uiGridCustomService,
        SweetAlert,
        $translate,
        toaster,
    ) {
        var ctrl = this;
        ctrl.init = function (isAdmin) {
            var columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.Taskgroups.Taskgroups.Project'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><a ng-if="!grid.appScope.$ctrl.isMobile" href=\'projects/{{row.entity.Id}}\' class="link">{{COL_FIELD}}</a></div>',
                },
            ];
            columnDefs.push.apply(columnDefs, [
                {
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.Taskgroups.Taskgroups.Activity'),
                    cellTemplate: '<ui-grid-custom-switch row="row" readonly="' + !isAdmin + '"></ui-grid-custom-switch>',
                    width: 100,
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.Taskgroups.Taskgroups.Order'),
                    type: 'number',
                    enableCellEdit: isAdmin,
                    width: 150,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Taskgroups.Taskgroups.Sorting'),
                        type: 'range',
                        rangeOptions: {
                            from: {
                                name: 'SortingFrom',
                            },
                            to: {
                                name: 'SortingTo',
                            },
                        },
                    },
                },
                {
                    name: 'NewTaskCount',
                    displayName: $translate.instant('Admin.Js.TaskGroups.NewTaskCount'),
                    width: 90,
                },
                {
                    name: 'InProgressTaskCount',
                    displayName: $translate.instant('Admin.Js.TaskGroups.InProgressTaskCount'),
                    width: 90,
                },
                {
                    name: 'CompletedTaskCount',
                    displayName: $translate.instant('Admin.Js.TaskGroups.CompletedTaskCount'),
                    width: 110,
                },
                {
                    name: 'AcceptedTaskCount',
                    displayName: $translate.instant('Admin.Js.TaskGroups.AcceptedTaskCount'),
                    width: 100,
                },
                {
                    name: 'CanceledTaskCount',
                    displayName: $translate.instant('Admin.Js.TaskGroups.CanceledTaskCount'),
                    width: 90,
                },
            ]);
            if (isAdmin) {
                columnDefs.push.apply(columnDefs, [
                    {
                        name: '_serviceColumn',
                        displayName: '',
                        width: 100,
                        enableSorting: false,
                        useInSwipeBlock: true,
                        cellTemplate:
                            '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                            '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fa fa-copy js-task-group-edit" ng-click="grid.appScope.$ctrl.gridExtendCtrl.copyTaskGroup(row.entity.Id, row.entity.Name)" aria-label="Копировать"></button> ' +
                            '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt js-task-group-edit" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadTaskGroup(row.entity.Id)" aria-label="Редактировать"></button> ' +
                            '<button type="button" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteTaskGroup(row.entity.CanBeDeleted, row.entity.Id)" class="btn-icon js-task-group-edit" ' +
                            'ng-class="(!row.entity.CanBeDeleted ? \'ui-grid-custom-service-icon fa fa-times link-disabled\' : \'ui-grid-custom-service-icon fa fa-times link-invert\')" aria-label="Удалить"></button> ' +
                            '</div></div>' +
                            //'<div ng-if="grid.appScope.$ctrl.isMobile">' +
                            //'<a ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadTaskGroup(row.entity.Id)" class="btn btn-sm btn-success flex center-xs middle-xs" style="height:100%;">Редактировать</a>' +
                            //'</div>' +
                            '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="taskgroups/deletetaskgroup" params="{\'Id\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                    },
                ]);
            }
            ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: columnDefs,
                uiGridCustom: {
                    //rowUrl: 'projects/{{row.entity.Id}}',
                    //rowClick: function ($event, row) {
                    //    ctrl.loadTaskGroup(row.entity.Id);
                    //},
                    selectionOptions: [
                        {
                            text: $translate.instant('Admin.Js.Taskgroups.Taskgroups.DeleteSelected'),
                            url: 'taskgroups/deletetaskgroups',
                            field: 'Id',
                            before: function () {
                                return SweetAlert.confirm($translate.instant('Admin.Js.Taskgroups.Taskgroups.AreYouSureDelete'), {
                                    title: $translate.instant('Admin.Js.Taskgroups.Taskgroups.Deleting'),
                                }).then(function (result) {
                                    return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                                });
                            },
                        },
                    ],
                },
            });
        };
        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };
        ctrl.loadTaskGroup = function (id) {
            $uibModal
                .open({
                    bindToController: true,
                    size: 'sidebar-unit-modal-trigger',
                    controller: 'ModalAddEditTaskGroupCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: addEditTaskGroupTemplate,
                    resolve: {
                        id: function () {
                            return id;
                        },
                    },
                })
                .result.then(
                    function (result) {
                        ctrl.grid.fetchData();
                        return result;
                    },
                    function (result) {
                        ctrl.grid.fetchData();
                        return result;
                    },
                );
        };
        ctrl.deleteTaskGroup = function (canBeDeleted, id) {
            if (canBeDeleted) {
                SweetAlert.confirm($translate.instant('Admin.Js.Taskgroups.Taskgroups.AreYouSureDelete'), {
                    title: $translate.instant('Admin.Js.Taskgroups.Taskgroups.Deleting'),
                }).then(function (result) {
                    if (result === true || result.value) {
                        $http
                            .post('taskgroups/deletetaskgroup', {
                                Id: id,
                            })
                            .then(function (response) {
                                if (response.data.result == true) {
                                    ctrl.grid.fetchData();
                                } else {
                                    response.data.errors.forEach(function (error) {
                                        toaster.error(error);
                                    });
                                }
                            });
                    }
                });
            } else {
                SweetAlert.alert($translate.instant('Admin.Js.Taskgroups.Taskgroups.ProjectHasTasks'), {
                    title: $translate.instant('Admin.Js.Taskgroups.Taskgroups.DeletingIsImpossible'),
                });
            }
        };
        ctrl.copyTaskGroup = function (id, taskGroupName) {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalCopyTaskGroupCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: copyTaskGroupTemplate,
                    resolve: {
                        taskGroupId: function () {
                            return id;
                        },
                        name: function () {
                            return taskGroupName;
                        },
                    },
                })
                .result.then(
                    function (result) {
                        ctrl.grid.fetchData();
                        return result;
                    },
                    function (result) {
                        return result;
                    },
                );
        };
    };
    TaskGroupsCtrl.$inject = [
        '$q',
        '$location',
        '$window',
        '$http',
        '$uibModal',
        'uiGridConstants',
        'uiGridCustomConfig',
        'uiGridCustomParamsConfig',
        'uiGridCustomService',
        'SweetAlert',
        '$translate',
        'toaster',
    ];
    ng.module('taskgroups', ['uiGridCustom', 'urlHelper']).controller('TaskGroupsCtrl', TaskGroupsCtrl);
})(window.angular);
