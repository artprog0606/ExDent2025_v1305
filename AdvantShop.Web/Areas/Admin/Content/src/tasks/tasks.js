import finishTaskTemplate from './modal/finishTask/finishTask.html';
import changeTaskStatusTemplate from './modal/changeTaskStatus/changeTaskStatus.html';
import addTaskTemplate from '../_shared/modal/addTask/addTask.html';
import changeTaskStatusesTemplate from './modal/changeTaskStatuses/ChangeTaskStatuses.html';
import changeTaskGroupTemplate from './modal/changeTaskGroup/ChangeTaskGroup.html';

(function (ng) {
    'use strict';
    const filters = {
        ALL: 'all',
        ASSIGNED_TO_ME: 'assignedtome',
        APPOINTED_BY_ME: 'appointedbyme',
        OBSERVED_BY_ME: 'observedbyme',
    };

    const stateTasks = {
        OPEN: 'open',
        INPROGRESS: 'inprogress',
        COMPLETED: 'completed',
        ACCEPTED: 'accepted',
        CANCELED: 'canceled',
    };

    const URL_FILTER_PARAM = 'filterby';
    const revertSortable = (event) => {
        event.dest.sortableScope.removeItem(event.dest.index);
        event.source.itemScope.sortableScope.insertItem(event.source.index, event.source.itemScope.modelValue);
    };
    /* @ngInject */
    var TasksCtrl = function (
        $cookies,
        $location,
        $q,
        $rootScope,
        $uibModal,
        $window,
        adminWebNotificationsEvents,
        adminWebNotificationsService,
        lastStatisticsService,
        SweetAlert,
        tasksService,
        toaster,
        uiGridConstants,
        uiGridCustomConfig,
        uiGridCustomParamsConfig,
        uiGridCustomService,
        urlHelper,
        $translate,
        $uibModalStack,
        isMobileService,
    ) {
        var ctrl = this;

        const NAME_FILTER_IN_LOCAL_STORAGE = `adv_tasks_filter_${urlHelper.transformBaseUriToKey()}`;

        //remove old static key
        localStorage.removeItem('adv_tasks_filter');

        ctrl.init = function (useKanban, selectTasks, prefilter, taskGroupId, isAdmin, showAcceptedTasks) {
            ctrl.gridParams = { projectStatusId: null };
            ctrl.useKanban = isMobileService.getValue() ? false : useKanban;
            // for grid
            ctrl.prefilter = prefilter || '';
            // for kanban

            //взять из url || localStorage ||  filters.All
            const taskFilterFromLocalStorage = localStorage.getItem(NAME_FILTER_IN_LOCAL_STORAGE);
            ctrl.selectTasks =
                (ctrl.isCorrectFilter(selectTasks, filters) ? selectTasks : null) ||
                (ctrl.isCorrectFilter(taskFilterFromLocalStorage, filters) ? taskFilterFromLocalStorage : null) ||
                filters.ALL;

            if (ctrl.useKanban) {
                $location.search(URL_FILTER_PARAM, ctrl.selectTasks);
            }

            ctrl.showAcceptedTasks = showAcceptedTasks || false;
            ctrl.gridParams.showAcceptedTasks = ctrl.showAcceptedTasks;

            ctrl.isAdmin = isAdmin;
            ctrl.taskGroupId = taskGroupId;
            if (!ctrl.useKanban) {
                ctrl.gridParams.filterby = prefilter;
            } else {
                ctrl.gridParams.selectTasks = ctrl.selectTasks;
            }
            if (taskGroupId) ctrl.gridParams.TaskGroupId = taskGroupId;

            var selectionOptions = [
                //{
                //    template:
                //        '<ui-modal-trigger data-controller="\'ModalChangeTaskStatusesCtrl\'" controller-as="ctrl" data-resolve="{params:$ctrl.getSelectedParams(\'Id\'), canAccept:' +
                //        (ctrl.prefilter == stateTasks.COMPLETED) +
                //        '}" ' +
                //        'template-url="' + changeTaskStatusesTemplate + '" ' +
                //        'data-on-close="$ctrl.gridOnAction()">' +
                //        $translate.instant(
                //            'Admin.Js.Tasks.Tasks.ChangeStatusForSelected',
                //        ) +
                //        '</ui-modal-trigger>',
                //},
                {
                    template:
                        '<ui-modal-trigger data-controller="\'ModalChangeTaskGroupCtrl\'" controller-as="ctrl" data-resolve="{params:$ctrl.getSelectedParams(\'Id\')}" ' +
                        'template-url="' +
                        changeTaskGroupTemplate +
                        '" ' +
                        'data-on-close="$ctrl.gridOnAction()">' +
                        $translate.instant('Admin.Js.Tasks.Tasks.ChangeTaskGroup') +
                        '</ui-modal-trigger>',
                },
            ];
            if (ctrl.isAdmin) {
                selectionOptions.splice(0, 0, {
                    text: $translate.instant('Admin.Js.Tasks.Tasks.DeleteSelected'),
                    url: 'tasks/deletetasks',
                    field: 'Id',
                    before: function () {
                        return SweetAlert.confirm($translate.instant('Admin.Js.Tasks.Tasks.AreYouSureDelete'), {
                            title: $translate.instant('Admin.Js.Tasks.Tasks.Deleting'),
                        }).then(function (result) {
                            return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                        });
                    },
                });
            }

            if (!isMobileService.getValue()) {
                selectionOptions.push({
                    text: $translate.instant('Admin.Js.Tasks.Tasks.MarkAsViewed'),
                    url: 'tasks/markviewed',
                    field: 'Id',
                });
            }

            if (!isMobileService.getValue()) {
                selectionOptions.push({
                    text: $translate.instant('Admin.Js.Tasks.Tasks.MarkAsNotViewed'),
                    url: 'tasks/marknotviewed',
                    field: 'Id',
                });
            }

            ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
                enableExpandAll: false,
                enableGridMenu: !isMobileService.getValue(),
                columnDefs: ctrl.getColumnDefs(useKanban, selectTasks, prefilter, taskGroupId),
                uiGridCustom: {
                    rowClick: function ($event, row) {
                        ctrl.loadTask(row.entity.Id);
                    },
                    groupByField: ctrl.taskGroupId == null ? 'TaskGroupName' : null,
                    selectionOptions: selectionOptions,
                    rowClasses: function (row) {
                        var classes = '';
                        if (!row.entity.Viewed || row.entity.NewCommentsCount > 0) classes += 'ui-grid-custom-row-bold ';
                        //if (row.entity.Overdue)
                        //    classes += 'ui-grid-custom-row-red ';
                        //if (row.entity.InProgress)
                        //    classes += 'ui-grid-custom-row-blue ';
                        if (row.entity.Completed) classes += 'ui-grid-custom-row-linethrough ';
                        return classes;
                    },
                },
            });
        };

        ctrl.isCorrectFilter = function (filterName, filters) {
            return filterName != null && Object.values(filters).indexOf(filterName) !== -1;
        };

        ctrl.getColumnDefs = function (useKanban, selectTasks, prefilter, taskGroupId) {
            // visible columns
            var columnDefs = [
                {
                    name: 'Viewed',
                    displayName: '',
                    enableHiding: false,
                    width: 25,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><span ng-if="!row.entity.Viewed" class="fa fa-circle text-warning" title="' +
                        $translate.instant('Admin.Js.Tasks.Tasks.NotViewed') +
                        '"> </span></div>',
                },
                {
                    name: 'Id',
                    displayName: '№',
                    width: 60,
                },
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.Tasks.Tasks.Task'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><a href="tasks?modal={{row.entity.Id}}" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadTask(row.entity.Id, $event)">{{COL_FIELD}}</a> <span ng-if="row.entity.NewCommentsCount > 0" class="badge badge-pink" title="' +
                        $translate.instant('Admin.Js.Tasks.Tasks.CountNewComments') +
                        '">{{row.entity.NewCommentsCount}}</span></div>',
                },
                {
                    name: 'PriorityFormatted',
                    displayName: $translate.instant('Admin.Js.Tasks.Tasks.Priority'),
                    width: 110,
                },
                {
                    name: 'DateAppointedFormatted',
                    displayName: $translate.instant('Admin.Js.Tasks.Tasks.DateOfCreation'),
                    width: 125,
                    cellTemplate: '<div class="ui-grid-cell-contents">{{COL_FIELD}}</div>',
                },
                {
                    name: 'DueDateFormatted',
                    displayName: $translate.instant('Admin.Js.Tasks.Tasks.Deadline'),
                    width: 125,
                    visible: 1441,
                    cellTemplate: '<div class="ui-grid-cell-contents" ng-class="{\'ui-grid-cell-red\': row.entity.Overdue}">{{COL_FIELD}}</div>',
                },
                {
                    name: 'StatusFormatted',
                    displayName: $translate.instant('Admin.Js.Tasks.Tasks.Status'),
                    width: 110,
                    cellTemplate: '<div class="ui-grid-cell-contents" ng-class="{\'ui-grid-cell-blue\': row.entity.InProgress}">{{COL_FIELD}}</div>',
                },
                {
                    name: 'Managers',
                    displayName: $translate.instant('Admin.Js.Tasks.Tasks.Executor'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents ui-grid-cell-contents--flex-nowrap js-grid-not-clicked">' +
                        '<sidebar-user-trigger customer-id="manager.CustomerId" class="ui-grid-cell-contents" ng-repeat="manager in row.entity.Managers track by $index">' +
                        '<div class="ui-grid-cell-avatar" ng-if="manager.AvatarSrc != null"><img ng-src="{{manager.AvatarSrc}}" alt="{{manager.FullName}}" title="{{manager.FullName}}"/></div>' +
                        '<a href="" class="text-decoration-invert" ng-if="row.entity.Managers.length == 1">{{manager.FullName}}</a>' +
                        '</sidebar-user-trigger>' +
                        '</div>',
                    //width: 200,
                },
                {
                    name: 'AppointedName',
                    displayName: $translate.instant('Admin.Js.Tasks.Tasks.TaskManager'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents js-grid-not-clicked">' +
                        '<sidebar-user-trigger customer-id="row.entity.AppointedCustomerId" ng-if="row.entity.AppointedCustomerId != null" class="ui-grid-cell-contents">' +
                        '<div class="ui-grid-cell-avatar" ng-if="row.entity.AppointedCustomerAvatarSrc != null"><img ng-src="{{row.entity.AppointedCustomerAvatarSrc}}"/></div>' +
                        '<a href="" class="text-decoration-invert">{{COL_FIELD}}</a>' +
                        '</sidebar-user-trigger>' +
                        '</div>',
                    //width: 200,
                },
                {
                    name: '_serviceColumn',
                    enableHiding: false,
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        ctrl.isAdmin && isMobileService.getValue()
                            ? '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="tasks/deletetask" params="{\'Id\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>'
                            : '<div class="ui-grid-cell-contents ui-grid-custom-ignore-row-style js-grid-not-clicked"><div><button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadTask(row.entity.Id, $event)" aria-label="Редактировать"></button>' +
                              (ctrl.isAdmin
                                  ? '<ui-grid-custom-delete url="tasks/deletetask" params="{\'Id\': row.entity.Id}"></ui-grid-custom-delete></div></div>'
                                  : ''),
                },
            ];

            // filters
            if (taskGroupId == null) {
                columnDefs.push({
                    name: '_noopColumnTaskGroups',
                    enableHiding: false,
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tasks.Tasks.Project'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'TaskGroupId',
                        fetch: 'taskgroups/getTaskGroupsSelectOptions',
                    },
                });
            }
            columnDefs.push.apply(columnDefs, [
                {
                    name: '_noopColumnDateCreated',
                    enableHiding: false,
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tasks.Tasks.Created'),
                        type: 'datetime',
                        term: {
                            from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                            to: new Date(),
                        },
                        datetimeOptions: {
                            from: { name: 'DateCreatedFrom' },
                            to: { name: 'DateCreatedTo' },
                        },
                    },
                },
                {
                    name: '_noopColumnPriorities',
                    enableHiding: false,
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tasks.Tasks.Priority'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'Priority',
                        fetch: 'tasks/getTaskPrioritiesSelectOptions',
                    },
                },
                {
                    name: '_noopColumnDueDateFormatted',
                    enableHiding: false,
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tasks.Tasks.Deadline'),
                        type: 'datetime',
                        term: {
                            from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                            to: new Date(),
                        },
                        datetimeOptions: {
                            from: { name: 'DueDateFrom' },
                            to: { name: 'DueDateTo' },
                        },
                    },
                },
                {
                    name: '_noopColumnViewed',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tasks.Tasks.Viewed'),
                        name: 'Viewed',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.Tasks.Tasks.Yes'),
                                value: 'true',
                            },
                            {
                                label: $translate.instant('Admin.Js.Tasks.Tasks.No'),
                                value: 'false',
                            },
                        ],
                    },
                },
            ]);

            if ((useKanban && selectTasks != filters.ASSIGNED_TO_ME) || (!useKanban && prefilter != filters.ASSIGNED_TO_ME)) {
                columnDefs.push({
                    name: '_noopColumnAssigned',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tasks.Tasks.Executor'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'AssignedManagerId',
                        fetch: 'managers/getAllTaskManagers?includeEmpty=true&assigned=true',
                    },
                });
            }
            if ((useKanban && selectTasks != filters.APPOINTED_BY_ME) || (!useKanban && prefilter != filters.APPOINTED_BY_ME)) {
                columnDefs.push({
                    name: '_noopColumnAppointed',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tasks.Tasks.TaskManager'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'AppointedManagerId',
                        fetch: 'managers/getAllTaskManagers?appointed=true',
                    },
                });
            }
            if (!useKanban && prefilter != stateTasks.COMPLETED && prefilter != stateTasks.ACCEPTED) {
                columnDefs.push({
                    name: '_noopColumnStatuses',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tasks.Tasks.Status'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'StatusType',
                        fetch:
                            prefilter == '' || prefilter == 'none' || prefilter == filters.ASSIGNED_TO_ME
                                ? 'tasks/getNotCompletedTaskStatusesSelectOptions'
                                : 'tasks/getTaskStatusesSelectOptions',
                    },
                });
            }
            if ((useKanban && selectTasks != filters.OBSERVED_BY_ME) || (!useKanban && prefilter != filters.OBSERVED_BY_ME)) {
                columnDefs.push({
                    name: '_noopColumnObserved',
                    enableHiding: false,
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Tasks.Tasks.TaskObserver'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'ObserverId',
                        fetch: 'managers/getAllTaskManagers?observed=true',
                    },
                });
            }
            //if (prefilter == 'completed') {
            //    ctrl.gridOptions.uiGridCustom.selectionOptions.push({
            //        text: 'Принять выделенные',
            //        url: 'tasks/accepttasks',
            //        field: 'Id'
            //    });
            //}

            return columnDefs;
        };

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.fetchData = function (ignoreHistory) {
            if (!ctrl.useKanban) {
                ctrl.grid.fetchData(ignoreHistory);
            } else {
                ctrl.kanban.fetchData();
            }
        };

        ctrl.refresh = function (reload, data) {
            if (data != null) {
                $location.search(`modal`, data.taskId);
            }
            ctrl.fetchData();
        };

        ctrl.changeParam = function (statusId) {
            ctrl.gridParams['projectStatusId'] = statusId;
            ctrl.grid.setParams(ctrl.gridParams);
            ctrl.grid.fetchData();
        };

        ctrl.reload = function (query) {
            var loc = $window.location.href.split('#')[0];
            var url = loc.split('?')[0] + (query || '');
            url += (url.indexOf('?') === -1 ? '?' : '&') + 'rnd=' + Math.random();
            $window.location.href = url;
        };

        ctrl.modalDismiss = ctrl.modalClose = function (result) {
            if (result) {
                $location.search('modal', result.modal);
            }

            if (!ctrl.useKanban || !result || result.refresh) {
                ctrl.fetchData();
            }
        };

        ctrl.changeView = function (view) {
            ctrl.setCookie('tasks_viewmode', view);
            if (ctrl.useKanban) {
                ctrl.clearGridParams();
            }
            ctrl.reload(`?${URL_FILTER_PARAM}=${ctrl.selectTasks}`);
        };

        ctrl.clearGridParams = function () {
            const locationSearch = $location.search();
            if (locationSearch.grid) {
                $location.search('grid', null);
            }
        };

        ctrl.toggleViewTasks = function (selectTasks) {
            ctrl.gridParams.selectTasks = selectTasks;
            // ctrl.setCookie('tasks_mykanban', selectTasks);
            localStorage.setItem(NAME_FILTER_IN_LOCAL_STORAGE, selectTasks);
            $location.search(URL_FILTER_PARAM, selectTasks);

            ctrl.refreshColumnDefs(selectTasks);

            ctrl.kanbanFilter.updateColumns();
            ctrl.kanban.resetColumnsData();
            ctrl.fetchData();
        };

        ctrl.refreshColumnDefs = function (selectTasks = '') {
            ctrl.gridOptions.columnDefs.length = 0;
            ctrl.gridOptions.columnDefs.push.apply(
                ctrl.gridOptions.columnDefs,
                ctrl.getColumnDefs(ctrl.useKanban, selectTasks, ctrl.prefilter, ctrl.taskGroupId),
            );
        };

        ctrl.toggleAcceptedTasks = function (showAcceptedTask) {
            ctrl.setCookie('tasks_mykanban_showAccepted', showAcceptedTask);
            ctrl.gridParams.showAcceptedTasks = showAcceptedTask;
            ctrl.fetchData();
        };

        ctrl.setCookie = function (name, value) {
            var date = new Date();
            date.setFullYear(date.getFullYear() + 1);
            $cookies.put(name, value, { expires: date });
        };

        ctrl.loadTask = function (id, $event) {
            if ($event) {
                $event.preventDefault();
            }

            $uibModalStack.dismissAll('open other modal');

            $location.search(`modalOpened`, `true`);

            tasksService.loadTask(id, { animation: false }).result.then(
                function (result) {
                    ctrl.modalClose(result);
                    ctrl.fetchData();
                    return result;
                },
                function (result) {
                    ctrl.modalDismiss(result);
                    return result;
                },
            );
        };

        ctrl.newTask = function () {
            $uibModalStack.dismissAll('open other modal');

            $uibModal
                .open({
                    animation: false,
                    bindToController: true,
                    controller: 'ModalAddTaskCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: addTaskTemplate,
                    size: 'xs-8 modal-md-6',
                    backdrop: 'static',
                    windowClass: 'modal__window--scrollbar-no',
                })
                .result.then(
                    function (result) {
                        ctrl.refresh(!ctrl.useKanban, result);
                        return result;
                    },
                    function (result) {
                        $location.search('modalNewTask', null);
                        return result;
                    },
                );
        };

        /************ Kanban  +  filter **************/

        ctrl.sortableOptions = {
            containment: '#kanban',
            containerPositioning: 'relative',
            additionalPlaceholderClass: 'kanban__placeholder',
            itemMoved: async function (event) {
                const task = event.source.itemScope.modelValue,
                    { Id: columnId, StatusType: statusTypeIdDest } = event.dest.sortableScope.$parent.column;
                if (columnId === 'CompleteTask' || columnId === 'CancelTask') {
                    ctrl.finishTask(task.Id, ctrl.showAcceptedTasks, columnId).then(function (result) {
                        if (result === 'cancel') {
                            revertSortable(event);
                        } else if (result != 'redirect') {
                            ctrl.fetchData();
                        }
                    });
                } else {
                    let statusesList = null;
                    let statusSelected = null;
                    let statusDest;

                    const { result: isSuccessOperation, obj: availableStatuses } = await tasksService.getAvailableStatusesByTaskGroupId(
                        task.TaskGroupId,
                    );

                    if (!isSuccessOperation) {
                        toaster.error(``, $translate.instant('Admin.Js.ErrorWhileSaving'));
                        revertSortable(event);
                        await ctrl.fetchData();
                        return;
                    }

                    const statusIdDest = /\d+/.test(columnId) ? parseFloat(columnId) : null;

                    if (statusIdDest != null) {
                        statusDest = availableStatuses.find((x) => x.id === statusIdDest);
                    }

                    if (statusDest == null) {
                        //const data = await tasksService.getProjectStatuses(task.TaskGroupId, columnId);
                        // await ctrl.changeTaskStatus(task.Id, data[0].id, event);
                        // await ctrl.fetchData();
                        //const statusDestByType = availableStatuses.find(x => x.statusType === statusTypeIdDest);
                        const listStatusByType = availableStatuses.filter((x) => x.statusType === statusTypeIdDest);
                        const isFoundedAvalableStatuses = listStatusByType.length > 0;
                        statusesList = isFoundedAvalableStatuses ? listStatusByType : availableStatuses;

                        statusSelected = await tasksService.selectStatusByList(statusesList);

                        if (statusSelected == null) {
                            //toaster.pop('error', 'Не удалось получить список статусов');
                            revertSortable(event);
                            await ctrl.fetchData();
                            return 'cancel';
                        }
                    } else {
                        statusSelected = statusDest;
                    }

                    const { id: statusId, statusType } = statusSelected;

                    //2 - Завершена
                    //3 - Принята
                    //4 - Отменена
                    const isChangeOnNotBeCompleted = task.isNotBeCompleted && [3, 4].includes(statusSelected.statusType);
                    if (statusSelected.statusType === 2 || isChangeOnNotBeCompleted) {
                        try {
                            await tasksService.completeTaskShowModal({
                                id: task.Id,
                                name: task.Name,
                                leadId: task.LeadId,
                                orderId: task.OrderId,
                                taskGroupId: task.TaskGroupId,
                            });
                            if (isChangeOnNotBeCompleted) {
                                await tasksService.changeTaskStatus(task.Id, statusId);
                            }
                        } catch (e) {
                            revertSortable(event);
                        }
                    } else {
                        await tasksService.changeTaskStatus(task.Id, statusId);
                    }

                    await ctrl.fetchData();
                }
            },
            orderChanged: function (event) {
                ctrl.onOrderChanged(event, true);
            },
        };

        ctrl.changeTaskStatus = async function (taskId, statusId, event) {
            const data = await tasksService.changeTaskStatus(taskId, statusId);

            if (data.result) {
                toaster.pop('success', $translate.instant('Admin.Js.Tasks.Tasks.ChangesSaved'));
                ctrl.onOrderChanged(event);
            } else {
                if (data.errors) {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });
                } else {
                    toaster.pop('error', 'Не удалось получить настройки');
                }
            }
            ctrl.fetchData();
        };

        ctrl.onOrderChanged = function (event, showMessage) {
            var current = event.source.itemScope.card,
                prev = event.dest.sortableScope.modelValue[event.dest.index - 1],
                next = event.dest.sortableScope.modelValue[event.dest.index + 1];
            // высокий приоритет выводится выше, задачу со средним или низким приоритетом не вставлять между задачами с высоким приоритетом и наоборот
            if (prev != null && (prev.Priority == 2 || current.Priority == 2) && prev.Priority != current.Priority) {
                prev = null;
            }
            if (next != null && (next.Priority == 2 || current.Priority == 2) && next.Priority != current.Priority) {
                next = null;
            }
            tasksService.changeSorting(current.Id, prev != null ? prev.Id : null, next != null ? next.Id : null).then(function (data) {
                if (showMessage && data != null && data.result === true) {
                    toaster.success('', $translate.instant('Admin.Js.Tasks.Tasks.ChangesSaved'));
                }
            });
        };

        ctrl.kanbanOnInit = function (kanban) {
            ctrl.kanban = kanban;
        };

        ctrl.kanbanOnFilterInit = function (filter) {
            ctrl.kanbanFilter = filter;
        };

        ctrl.$onInit = function () {
            const locationSearch = $location.search();
            const locationSearchLegacy = $location.hash();
            let modalIdOpen,
                isModalNewTask = false;

            if (locationSearchLegacy != null && locationSearchLegacy.length > 0) {
                let searchLegacy = urlHelper.getUrlParamsAsObject(locationSearchLegacy.slice(1));
                modalIdOpen = searchLegacy.modal;
                isModalNewTask = locationSearch.modalNewTask;

                if (modalIdOpen != null || isModalNewTask != null) {
                    $location.hash('');
                    $location.search({
                        modal: modalIdOpen,
                        modalNewTask: isModalNewTask,
                    });
                }
            } else if (locationSearch != null) {
                if (locationSearch.modal != null) {
                    modalIdOpen = locationSearch.modal;
                } else if (locationSearch.modalNewTask) {
                    isModalNewTask = locationSearch.modalNewTask;
                }
                if (modalIdOpen != null) {
                    ctrl.loadTask(modalIdOpen);
                } else if (isModalNewTask) {
                    ctrl.newTask();
                }
            }

            $rootScope.$on('$locationChangeSuccess', function (event, newUrl, oldUrl) {
                var params = $location.search();
                if (newUrl !== oldUrl && params != null && params.modal != null && params.modalOpened !== 'true') {
                    //&& $location.search().modalShow != null
                    ctrl.loadTask($location.search().modal);
                }
            });

            adminWebNotificationsService.addListener(adminWebNotificationsEvents.updateTasks, function () {
                ctrl.fetchData(true);
            });
        };

        ctrl.finishTask = function (taskId, showAcceptedTasks, columnId) {
            if (showAcceptedTasks) {
                var data = null;
                if (columnId === 'CompleteTask') {
                    data = tasksService.acceptTask(taskId);
                }
                if (columnId === 'CancelTask') {
                    data = tasksService.cancelTask(taskId);
                }
                if (data !== null) {
                    return data.then(function (result) {
                        return result.result;
                    });
                }
            }
            return $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalFinishTaskCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: finishTaskTemplate,
                    resolve: { id: taskId },
                })
                .result.then(
                    function (result) {
                        return result;
                    },
                    function (result) {
                        return 'cancel';
                    },
                );
        };

        ctrl.editTaskGroupClose = function (result) {
            if (result != null) {
                ctrl.taskGroupName = result.name;
            }
        };

        ctrl.goToTab = function (event, prefilter) {
            ctrl.prefilter = prefilter;
            localStorage.setItem(NAME_FILTER_IN_LOCAL_STORAGE, prefilter);
            var params = ctrl.grid.getParams();
            ctrl.grid.setParams({ ...params, filterby: prefilter });
            event.preventDefault();

            ctrl.refreshColumnDefs();
            ctrl.selectTasks = ctrl.prefilter;
            $location.search(URL_FILTER_PARAM, ctrl.prefilter);
        };
    };

    ng.module('tasks', ['uiGridCustom', 'adminComments', 'urlHelper', 'ngFileUpload']).controller('TasksCtrl', TasksCtrl);
})(window.angular);
