/* @ngInject */
export default function WarehouseTypesListCtrl(
    $q,
    uiGridConstants,
    uiGridCustomConfig,
    uiGridCustomParamsConfig,
    uiGridCustomService,
    SweetAlert,
    warehouseTypesService,
    $translate,
) {
    var ctrl = this;

    ctrl.gridOptions = angular.extend({}, uiGridCustomConfig, {
        columnDefs: [
            {
                name: 'TypeName',
                displayName: $translate.instant('Admin.Js.WarehouseTypes.TypeName'),
                enableCellEdit: true,
                filter: {
                    placeholder: $translate.instant('Admin.Js.WarehouseTypes.TypeNameFilter'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'TypeName',
                },
            },
            {
                name: 'SortOrder',
                displayName: $translate.instant('Admin.Js.WarehouseTypes.SortOrder'),
                type: 'number',
                width: 80,
                enableCellEdit: true,
                filter: {
                    placeholder: $translate.instant('Admin.Js.WarehouseTypes.SortOrderFilter'),
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
                displayName: $translate.instant('Admin.Js.WarehouseTypes.Enabled'),
                enableCellEdit: false,
                cellTemplate: '<ui-grid-custom-switch row="row"></ui-grid-custom-switch>',
                width: 76,
                filter: {
                    placeholder: $translate.instant('Admin.Js.WarehouseTypes.EnabledFilter'),
                    name: 'Enabled',
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: [
                        {
                            label: $translate.instant('Admin.Js.WarehouseTypes.EnabledFilter.Active'),
                            value: true,
                        },
                        {
                            label: $translate.instant('Admin.Js.WarehouseTypes.EnabledFilter.Inactive'),
                            value: false,
                        },
                    ],
                },
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 75,
                enableSorting: false,
                useInSwipeBlock: true,
                cellTemplate:
                    '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div class="js-grid-not-clicked">' +
                    '<button type="button" ng-click="grid.appScope.$ctrl.gridExtendCtrl.showType(row.entity.TypeId); $event.preventDefault();" class="btn-icon ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="{{\'Admin.Js.WarehouseTypes.Edit\' | translate}}"></button>' +
                    '<ui-grid-custom-delete url="warehouse/deletewarehousetype" params="{\'WarehouseTypeId\': row.entity.TypeId}"></ui-grid-custom-delete></div></div>' +
                    '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="warehouse/deletewarehousetype" params="{\'WarehouseTypeId\': row.entity.TypeId}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">{{\'Admin.Js.WarehouseTypes.Delete\' | translate}}</ui-grid-custom-delete>',
            },
        ],
        uiGridCustom: {
            selectionOptions: [
                {
                    text: $translate.instant('Admin.Js.WarehouseTypes.DeleteSelected'),
                    url: 'warehouse/deletewarehousetypes',
                    field: 'TypeId',
                    before: function () {
                        return SweetAlert.confirm($translate.instant('Admin.Js.WarehouseTypes.DeleteSelected.Title'), {
                            title: $translate.instant('Admin.Js.WarehouseTypes.DeleteSelected.Notification'),
                            confirmButtonText: $translate.instant('Admin.Js.WarehouseTypes.DeleteSelected.Confirm'),
                            cancelButtonText: $translate.instant('Admin.Js.WarehouseTypes.DeleteSelected.Cancel'),
                        }).then(function (result) {
                            return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                        });
                    },
                },
                {
                    text: $translate.instant('Admin.Js.WarehouseTypes.MakeActive'),
                    url: 'warehouse/activatewarehousetypes',
                    field: 'TypeId',
                },
                {
                    text: $translate.instant('Admin.Js.WarehouseTypes.MakeInactive'),
                    url: 'warehouse/disablewarehousetypes',
                    field: 'TypeId',
                },
            ],
        },
    });

    ctrl.gridOnInit = function (grid) {
        ctrl.grid = grid;
    };

    ctrl.showType = function (typeId) {
        warehouseTypesService.showWarehouseType(typeId).result.then(
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
}

//    WarehouseTypesListCtrl.$inject = [
//        '$q',
//        'uiGridConstants',
//        'uiGridCustomConfig',
//        'uiGridCustomParamsConfig',
//        'uiGridCustomService',
//        'SweetAlert',
//        'warehouseTypesService',
//    ];

//    ng.module('warehouseTypesList', ['uiGridCustom']).controller('WarehouseTypesListCtrl', WarehouseTypesListCtrl).service();
//})(window.angular);
