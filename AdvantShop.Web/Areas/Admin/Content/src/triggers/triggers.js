import addEditCategoryTemplate from './modal/addEditCategory/addEditCategoryTriggers.html';
(function (ng) {
    'use strict';

    var TriggersCtrl = function (
        toaster,
        triggersService,
        SweetAlert,
        $translate,
        uiGridCustomConfig,
        uiGridConstants,
        $q,
        $uibModal,
        $http,
        isMobileService,
        $location,
        $window,
    ) {
        var ctrl = this;
        ctrl.gridCategoriesInited = false;

        ctrl.$onInit = function () {
            ctrl.gridTriggersOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: [
                    {
                        name: 'Name',
                        displayName: $translate.instant('Admin.Js.Triggers.Name'),
                        cellTemplate:
                            '<div class="ui-grid-cell-contents"><a class="link-invert word-break" ng-href="triggers/edit/{{row.entity.Id}}" title="{{COL_FIELD}}">{{COL_FIELD}}</a></div>',
                        enableCellEdit: false,
                        filter: {
                            placeholder: $translate.instant('Admin.Js.Triggers.Name'),
                            type: uiGridConstants.filter.INPUT,
                            name: 'Name',
                        },
                    },
                    {
                        name: 'Description',
                        displayName: $translate.instant('Admin.Js.Triggers.Description'),
                        enableCellEdit: false,
                        enableSorting: false,
                    },
                    {
                        name: 'CategoryName',
                        displayName: $translate.instant('Admin.Js.Triggers.CategoryName'),
                        cellTemplate: '<div class="ui-grid-cell-contents">{{COL_FIELD || \'Общая\'}}</div>',
                        enableCellEdit: false,
                    },
                    {
                        name: 'Enabled',
                        displayName: $translate.instant('Admin.Js.Catalog.Activ'),
                        enableCellEdit: false,
                        cellTemplate: '<ui-grid-custom-switch row="row"></ui-grid-custom-switch>',
                        filter: {
                            placeholder: $translate.instant('Admin.Js.Catalog.Activity'),
                            type: uiGridConstants.filter.SELECT,
                            selectOptions: [
                                {
                                    label: $translate.instant('Admin.Js.Catalog.TheyActive'),
                                    value: true,
                                },
                                {
                                    label: $translate.instant('Admin.Js.Catalog.Inactive'),
                                    value: false,
                                },
                            ],
                        },
                    },
                    {
                        name: 'CategoryId',
                        enableCellEdit: false,
                        visible: false,
                        filter: {
                            placeholder: $translate.instant('Admin.Js.Triggers.Category'),
                            type: uiGridConstants.filter.SELECT,
                            name: 'CategoryId',
                            fetch: 'triggers/getCategoriesList',
                        },
                    },
                    {
                        name: '_serviceColumn',
                        displayName: '',
                        width: 75,
                        enableSorting: false,
                        useInSwipeBlock: true,
                        cellTemplate:
                            '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div class="js-grid-not-clicked"><a ng-href="triggers/edit/{{row.entity.Id}}" class="ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></a><ui-grid-custom-delete url="triggers/deleteTrigger" params="{\'id\': row.entity.Id}"></ui-grid-custom-delete></div></div>' +
                            '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="triggers/deleteTrigger" params="{\'id\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                    },
                ],
                uiGridCustom: {
                    rowUrl: 'triggers/edit/{{row.entity.Id}}',
                    selectionOptions: [
                        {
                            text: $translate.instant('Admin.Js.Catalog.DeleteSelected'),
                            url: 'triggers/deleteTriggers',
                            field: 'Id',
                            before: function () {
                                return SweetAlert.confirm($translate.instant('Admin.Js.Catalog.AreYouSureDelete'), {
                                    title: $translate.instant('Admin.Js.Catalog.Deleting'),
                                }).then(function (result) {
                                    if (result === true || result.value) {
                                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                                        return $q.resolve('sweetAlertConfirm');
                                    }

                                    return $q.reject('sweetAlertCancel');
                                });
                            },
                        },
                        {
                            text: $translate.instant('Admin.Js.Catalog.MakeActive'),
                            url: 'triggers/activateTriggers',
                            field: 'Id',
                        },
                        {
                            text: $translate.instant('Admin.Js.Catalog.MakeInactive'),
                            url: 'triggers/disableTriggers',
                            field: 'Id',
                        },
                    ],
                },
            });
            ctrl.gridCategoriesOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: [
                    {
                        name: 'Name',
                        displayName: 'Название',
                        enableCellEdit: true,
                        //cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{COL_FIELD}}</span></div>'
                    },
                    {
                        name: 'SortOrder',
                        displayName: 'Порядок',
                        type: 'number',
                        enableCellEdit: true,
                        width: 150,
                        filter: {
                            placeholder: 'Сортировка',
                            type: 'range',
                            rangeOptions: {
                                from: { name: 'SortingFrom' },
                                to: { name: 'SortingTo' },
                            },
                        },
                    },
                    {
                        name: '_serviceColumn',
                        displayName: '',
                        width: 80,
                        enableSorting: false,
                        useInSwipeBlock: true,
                        cellTemplate:
                            '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                            '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt js-task-group-edit" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadCategory(row.entity.Id)" aria-label="Редактировать"></button> ' +
                            '<button type="button" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteCategory(row.entity.Id)" class="btn-icon js-task-group-edit ui-grid-custom-service-icon fa fa-times link-invert" aria-label="Удалить"></button> ' +
                            '</div></div>' +
                            '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="triggers/deleteCategory" params="{\'Id\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                    },
                ],
                uiGridCustom: {
                    rowClick: function ($event, row) {
                        ctrl.loadCategory(row.entity.Id);
                    },
                    selectionOptions: [
                        {
                            text: 'Удалить выделенные',
                            url: 'triggers/deleteCategories',
                            field: 'Id',
                            before: function () {
                                return SweetAlert.confirm($translate.instant('Admin.Js.TriggerCategories.Categories.AreYouSureDelete'), {
                                    title: $translate.instant('Admin.Js.TriggerCategories.Categories.Deleting'),
                                }).then(function (result) {
                                    if (result === true || result.value) {
                                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                                        return $q.resolve('sweetAlertConfirm');
                                    }

                                    return $q.reject('sweetAlertCancel');
                                });
                            },
                        },
                    ],
                },
            });
        };

        ctrl.deleteTrigger = function (id) {
            SweetAlert.confirm('Вы уверены что хотите удалить?', {
                title: $translate.instant('Admin.Js.GridCustomComponent.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    triggersService.deleteTrigger(id).then(function (result) {
                        toaster.pop('success', '', 'Изменения успешно сохранены.');

                        ctrl.grid.fetchData();
                    });
                }
            });
        };

        ctrl.gridCategoriesOnInit = function (grid) {
            ctrl.grid = grid;
            ctrl.gridCategoriesInited = true;
        };

        ctrl.loadCategory = function (id) {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddEditCategoryCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: addEditCategoryTemplate,
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
                        return result;
                    },
                );
        };

        ctrl.deleteCategory = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.TriggerCategories.Categories.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.TriggerCategories.Categories.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http.post('triggers/deleteCategory', { Id: id }).then(function (response) {
                        if (response.data.result === true) {
                            ctrl.grid.fetchData();
                        } else {
                            response.data.errors.forEach(function (error) {
                                toaster.error(error);
                            });
                        }
                    });
                }
            });
        };

        ctrl.onSelectTab = function (indexTab) {
            ctrl.tabActiveIndex = indexTab;
        };

        ctrl.getTriggerEditCtrl = function (triggerEditCtrl) {
            ctrl.triggerEditCtrl = triggerEditCtrl;
        };

        ctrl.getTriggeremailings = function () {
            $window.location.href = 'emailings/triggeremailings/' + ctrl.triggerEditCtrl.id;
        };

        ctrl.setName = function (name) {
            ctrl.triggerEditCtrl.name = name;
        };

        ctrl.deleteTriggers = function () {
            SweetAlert.confirm('Вы уверены, что хотите отключить канал?', {
                title: 'Удаление',
            }).then(function (result) {
                if (result && !result.isDismissed) {
                    triggersService.deleteTriggers().then(function () {
                        var basePath = document.getElementsByTagName('base')[0].getAttribute('href');
                        $window.location.assign(basePath);
                    });
                }
            });
        };
    };

    TriggersCtrl.$inject = [
        'toaster',
        'triggersService',
        'SweetAlert',
        '$translate',
        'uiGridCustomConfig',
        'uiGridConstants',
        '$q',
        '$uibModal',
        '$http',
        'isMobileService',
        '$location',
        '$window',
    ];

    ng.module('triggers', ['isMobile', 'coupons']).controller('TriggersCtrl', TriggersCtrl);
})(window.angular);
