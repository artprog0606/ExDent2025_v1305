import editTaskTemplate from '../../tasks/modal/editTask/editTask.html';
import changeTaskStatusesTemplate from '../../tasks/modal/changeTaskStatuses/ChangeTaskStatuses.html';
(function (ng) {
    'use strict';

    var TasksGridCtrl = function (
        $location,
        $q,
        $rootScope,
        $uibModal,
        uiGridConstants,
        uiGridCustomConfig,
        uiGridCustomParamsConfig,
        uiGridCustomService,
        SweetAlert,
        tasksService,
        $translate,
        toaster,
    ) {
        var ctrl = this,
            columnDefs = [
                {
                    name: '_noopColumnTaskGroups',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.TasksGrid.Project'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'TaskGroupId',
                        fetch: 'taskgroups/getTaskGroupsSelectOptions',
                    },
                },
                {
                    name: '_noopColumnDateCreated',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.TasksGrid.Created'),
                        type: 'datetime',
                        term: {
                            from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                            to: new Date(),
                        },
                        datetimeOptions: {
                            from: {
                                name: 'DateCreatedFrom',
                            },
                            to: {
                                name: 'DateCreatedTo',
                            },
                        },
                    },
                },
                {
                    name: '_noopColumnPriorities',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.TasksGrid.Priority'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'Priority',
                        fetch: 'tasks/getTaskPrioritiesSelectOptions',
                    },
                },
                {
                    name: '_noopColumnAssigned',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.TasksGrid.Executor'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'AssignedManagerId',
                        fetch: 'managers/getManagersSelectOptions?includeEmpty=true',
                    },
                },
                {
                    name: '_noopColumnAppointed',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.TaskGrid.TaskManager'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'AppointedManagerId',
                        fetch: 'managers/getManagersSelectOptions',
                    },
                },
                {
                    name: '_noopColumnStatuses',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.TaskGrid.Status'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'Status',
                        fetch: 'tasks/getTaskStatusesSelectOptions',
                    },
                },
                {
                    name: 'Viewed',
                    displayName: '',
                    width: 25,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><span ng-if="!row.entity.Viewed" class="fa fa-circle text-warning" title="' +
                        $translate.instant('Admin.Js.TaskGrid.NotViewed') +
                        '"> </span></div>',
                },
                {
                    name: 'Id',
                    displayName: '№',
                    width: 50,
                },
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.TaskGrid.Task'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div><span class="link">{{COL_FIELD}}</span> <span ng-if="row.entity.NewCommentsCount > 0" class="badge badge-pink" title="' +
                        $translate.instant('Admin.Js.TaskGrid.NumberOfNewComments') +
                        '">{{row.entity.NewCommentsCount}}</span></div></div>',
                },
                //{
                //    name: 'PriorityFormatted',
                //    displayName: 'Приоритет',
                //    width: 110,
                //},
                {
                    name: 'DueDateFormatted',
                    displayName: $translate.instant('Admin.Js.TaskGrid.Deadline'),
                    width: 130,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents" ng-class="{\'ui-grid-cell-red\': row.entity.Overdue}">{{COL_FIELD}}</span></div>',
                    visible: 1383,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.TaskGrid.Deadline'),
                        type: 'datetime',
                        term: {
                            from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                            to: new Date(),
                        },
                        datetimeOptions: {
                            from: {
                                name: 'DueDateFrom',
                            },
                            to: {
                                name: 'DueDateTo',
                            },
                        },
                    },
                },
                {
                    name: 'StatusFormatted',
                    displayName: $translate.instant('Admin.Js.TaskGrid.Status'),
                    width: 110,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents" ng-class="{\'ui-grid-cell-blue\': row.entity.InProgress}">{{COL_FIELD}}</span></div>',
                },
                {
                    name: 'Managers',
                    displayName: $translate.instant('Admin.Js.TasksGrid.Executor'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents js-grid-not-clicked">' +
                        '<sidebar-user-trigger customer-id="manager.CustomerId" ng-repeat="manager in row.entity.Managers track by $index" class="p-xs">' +
                        '<div class="ui-grid-cell-avatar" ng-if="manager.AvatarSrc != null"><img ng-src="{{manager.AvatarSrc}}" alt="{{manager.FullName}}" title="{{manager.FullName}}"/></div>' +
                        '<a href="" class="text-decoration-invert" ng-if="row.entity.Managers.length == 1">{{manager.FullName}}</a>' +
                        '</sidebar-user-trigger>' +
                        '</div>',
                    width: 150,
                },
                {
                    name: 'AppointedName',
                    displayName: $translate.instant('Admin.Js.TaskGrid.TaskManager'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents js-grid-not-clicked">' +
                        '<sidebar-user-trigger customer-id="row.entity.AppointedCustomerId" ng-if="row.entity.AppointedCustomerId != null" class="p-xs">' +
                        '<div class="ui-grid-cell-avatar" ng-if="row.entity.AppointedCustomerAvatarSrc != null"><img ng-src="{{row.entity.AppointedCustomerAvatarSrc}}"/></div>' +
                        '<a href="" class="text-decoration-invert">{{COL_FIELD}}</a>' +
                        '</sidebar-user-trigger>' +
                        '</div>',
                    width: 150,
                    visible: 1600,
                },
                {
                    name: 'ResultFull',
                    displayName: $translate.instant('Admin.Js.TaskGrid.Result'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents" ng-class="{\'ui-grid-cell-blue\': row.entity.InProgress}">{{COL_FIELD}}</span></div>',
                    visible: 1600,
                },
                {
                    name: '_noopColumnViewed',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.TaskGrid.Viewed'),
                        name: 'Viewed',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.TaskGrid.Yes'),
                                value: true,
                            },
                            {
                                label: $translate.instant('Admin.Js.TaskGrid.No'),
                                value: false,
                            },
                        ],
                    },
                },
                {
                    name: '_serviceColumnEdit',
                    displayName: '',
                    width: 40,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<ui-modal-trigger controller="\'ModalEditTaskCtrl\'" controller-as="ctrl" on-dismiss="grid.appScope.$ctrl.gridExtendCtrl.modalDismiss()" on-close="grid.appScope.$ctrl.gridExtendCtrl.modalClose()" size="lg" backdrop="static" \
                                           template-url="' +
                        editTaskTemplate +
                        '" resolve="{\'id\': row.entity.Id}" modal-id="{{row.entity.Id}}">' +
                        uiGridCustomService.getTemplateCellEdit() +
                        '</ui-modal-trigger>',
                },
            ];

        if (ctrl.isAdmin) {
            columnDefs.push({
                name: '_serviceColumn',
                displayName: '',
                width: 40,
                enableSorting: false,
                useInSwipeBlock: true,
                cellTemplate: uiGridCustomService.getTemplateCellDelete(
                    'tasks/deletetask',
                    '{Id: row.entity.Id}',
                    'on-delete="grid.appScope.$ctrl.gridExtendCtrl.modalClose()"',
                ),
            });
        }

        var selectionOptions = [
            {
                template:
                    '<ui-modal-trigger data-controller="\'ModalChangeTaskStatusesCtrl\'" controller-as="ctrl" data-resolve="{params:$ctrl.getSelectedParams(\'Id\')}" ' +
                    'template-url="' +
                    changeTaskStatusesTemplate +
                    '" ' +
                    'data-on-close="$ctrl.gridOnAction()">' +
                    $translate.instant('Admin.Js.TaskGrid.ChangeStatusForSelected') +
                    '</ui-modal-trigger>',
            },
            {
                text: $translate.instant('Admin.Js.TaskGrid.MarkAsViewed'),
                url: 'tasks/markviewed',
                field: 'Id',
            },
            {
                text: $translate.instant('Admin.Js.TaskGrid.MarkAsUnviewed'),
                url: 'tasks/marknotviewed',
                field: 'Id',
            },
        ];
        if (ctrl.isAdmin) {
            selectionOptions.splice(0, 0, {
                text: $translate.instant('Admin.Js.TaskGrid.DeleteSelected'),
                url: 'tasks/deletetasks',
                field: 'Id',
                before: function () {
                    return SweetAlert.confirm($translate.instant('Admin.Js.TaskGrid.AreYouSureDelete'), {
                        title: $translate.instant('Admin.Js.TaskGrid.Deleting'),
                    }).then(function (result) {
                        if (result === true || result.value) {
                            toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                            return $q.resolve('sweetAlertConfirm');
                        }

                        return $q.reject('sweetAlertCancel');
                    });
                },
            });
        }

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            enableExpandAll: false,
            columnDefs: columnDefs,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.loadTask(row.entity.Id);
                },
                groupByField: 'TaskGroupName',
                selectionOptions: selectionOptions,
                rowClasses: function (row) {
                    var classes = '';
                    if (!row.entity.Viewed || row.entity.NewCommentsCount > 0) classes += 'ui-grid-custom-row-bold ';
                    if (row.entity.Completed) classes += 'ui-grid-custom-row-linethrough ';
                    return classes;
                },
            },
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.gridTasks = grid;

            if (ctrl.onInit != null) {
                ctrl.onInit({ taskGrid: ctrl });
            }
        };

        ctrl.modalDismiss = ctrl.modalClose = function () {
            ctrl.gridTasks.fetchData();
            $location.search('modal', null);
            $location.search('modalShow', null);
            ctrl.update();
        };

        ctrl.update = function () {
            if (ctrl.onUpdate != null) {
                ctrl.onUpdate();
            }
        };

        ctrl.loadTask = function (id) {
            tasksService.loadTask(id).result.then(
                function (result) {
                    ctrl.modalClose();
                    return result;
                },
                function (result) {
                    ctrl.modalDismiss();
                    return result;
                },
            );
        };

        ctrl.$onInit = function () {
            if ($location.search() != null && $location.search().modal != null) {
                ctrl.loadTask($location.search().modal);
            }

            $rootScope.$on('$locationChangeSuccess', function (event, newUrl, oldUrl) {
                if (newUrl !== oldUrl && $location.search() != null && $location.search().modal != null && $location.search().modalShow != null) {
                    ctrl.loadTask($location.search().modal);
                }
            });
        };
    };

    TasksGridCtrl.$inject = [
        '$location',
        '$q',
        '$rootScope',
        '$uibModal',
        'uiGridConstants',
        'uiGridCustomConfig',
        'uiGridCustomParamsConfig',
        'uiGridCustomService',
        'SweetAlert',
        'tasksService',
        '$translate',
        'toaster',
    ];

    ng.module('tasksGrid', []).controller('TasksGridCtrl', TasksGridCtrl);
})(window.angular);
