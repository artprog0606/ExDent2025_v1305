(function (ng) {
    'use strict';

    var WarehousesListCtrl = function (
        $q,
        uiGridConstants,
        uiGridCustomConfig,
        uiGridCustomParamsConfig,
        uiGridCustomService,
        SweetAlert,
        $translate,
    ) {
        var ctrl = this;

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'WarehouseName',
                    displayName: $translate.instant('Admin.Js.WarehousesList.WarehouseName'),
                    enableCellEdit: false,
                    cellTemplate: '<div class="ui-grid-cell-contents"><a ng-href="warehouse/edit/{{row.entity.WarehouseId}}">{{COL_FIELD}}</a></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.WarehousesList.WarehouseNameFilter'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'WarehouseName',
                    },
                },
                {
                    name: 'CityName',
                    displayName: $translate.instant('Admin.Js.WarehousesList.CityName'),
                    enableCellEdit: false,
                    width: 150,
                },
                {
                    visible: false,
                    name: 'CityId',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.WarehousesList.CityNameFilter'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'CityId',
                        fetch: 'cities/getCitiesList',
                        dynamicSearch: true,
                    },
                },
                {
                    name: 'TypeName',
                    displayName: $translate.instant('Admin.Js.WarehousesList.TypeName'),
                    enableCellEdit: false,
                    width: 200,
                },
                {
                    visible: false,
                    name: 'TypeId',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.WarehousesList.TypeNameFilter'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'TypeId',
                        fetch: 'warehouse/getWarehouseTypesList',
                        dynamicSearch: true,
                    },
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.WarehousesList.SortOrder'),
                    type: 'number',
                    width: 80,
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.WarehousesList.SortOrderFilter'),
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
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.WarehousesList.Enabled'),
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch row="row"></ui-grid-custom-switch>',
                    width: 76,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.WarehousesList.EnabledFilter'),
                        name: 'Enabled',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.WarehousesList.EnabledFilter.Enabled'),
                                value: true,
                            },
                            {
                                label: $translate.instant('Admin.Js.WarehousesList.EnabledFilter.Disabled'),
                                value: false,
                            },
                        ],
                    },
                },
                {
                    name: 'Amount',
                    displayName: $translate.instant('Admin.Js.WarehousesList.Amount'),
                    enableCellEdit: false,
                    width: 100,
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div class="js-grid-not-clicked"><a ng-href="warehouse/edit/{{row.entity.WarehouseId}}" class="ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="{{\'Admin.Js.WarehousesList.Edit\' | translate}}"></a><ui-grid-custom-delete url="warehouse/deletewarehouse" params="{\'WarehouseId\': row.entity.WarehouseId}" ng-if="row.entity.CanDelete"></ui-grid-custom-delete></div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile && row.entity.CanDelete" url="warehouse/deletewarehouse" params="{\'WarehouseId\': row.entity.WarehouseId}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">{{\'Admin.Js.WarehousesList.Delete\' | translate}}</ui-grid-custom-delete>',
                },
            ],
            uiGridCustom: {
                rowUrl: 'warehouse/edit/{{row.entity.WarehouseId}}',
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.WarehousesList.DeleteSelected'),
                        url: 'warehouse/deletewarehouses',
                        field: 'WarehouseId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.WarehousesList.DeleteSelected.Notification'), {
                                title: $translate.instant('Admin.Js.WarehousesList.DeleteSelected.Title'),
                                confirmButtonText: $translate.instant('Admin.Js.WarehousesList.DeleteSelected.Confirm'),
                                cancelButtonText: $translate.instant('Admin.Js.WarehousesList.DeleteSelected.Cancel'),
                            }).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                    {
                        text: $translate.instant('Admin.Js.WarehousesList.MakeActive'),
                        url: 'warehouse/activatewarehouses',
                        field: 'WarehouseId',
                    },
                    {
                        text: $translate.instant('Admin.Js.WarehousesList.MakeInactive'),
                        url: 'warehouse/disablewarehouses',
                        field: 'WarehouseId',
                    },
                ],
            },
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };
    };

    WarehousesListCtrl.$inject = [
        '$q',
        'uiGridConstants',
        'uiGridCustomConfig',
        'uiGridCustomParamsConfig',
        'uiGridCustomService',
        'SweetAlert',
        '$translate',
    ];

    ng.module('warehousesList', ['uiGridCustom']).controller('WarehousesListCtrl', WarehousesListCtrl);
})(window.angular);
