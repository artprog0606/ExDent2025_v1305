(function (ng) {
    'use strict';

    var ModalSelectTypeWarehouseCtrl = function ($uibModalInstance, uiGridConstants, uiGridCustomConfig, $translate) {
        var ctrl = this;

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            multiSelect: false,
            modifierKeysToMultiSelect: false,
            enableRowSelection: false,
            enableRowHeaderSelection: false,
            columnDefs: [
                {
                    name: 'TypeName',
                    displayName: $translate.instant('Admin.Js.SelectTypeWarehouse.TypeName'),
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SelectTypeWarehouse.TypeNameFilter'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'TypeName',
                    },
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.SelectTypeWarehouse.SortOrder'),
                    type: 'number',
                    width: 80,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SelectTypeWarehouse.SortOrderFilter'),
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
                    displayName: $translate.instant('Admin.Js.SelectTypeWarehouse.Enabled'),
                    enableCellEdit: false,
                    width: 76,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.SelectTypeWarehouse.EnabledFilter'),
                        name: 'Enabled',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.SelectTypeWarehouse.EnabledFilter.Active'),
                                value: true,
                            },
                            {
                                label: $translate.instant('Admin.Js.SelectTypeWarehouse.EnabledFilter.Inactive'),
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
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.choose(row.entity.TypeId, row.entity.TypeName)">{{\'Admin.Js.SelectTypeWarehouse.Select\' | translate}}</a> ' +
                        '</div></div>',
                },
            ],
        });

        ctrl.$onInit = function () {};

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.choose = function (typeId, typeName) {
            $uibModalInstance.close({ typeId: typeId, typeName: typeName });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalSelectTypeWarehouseCtrl.$inject = ['$uibModalInstance', 'uiGridConstants', 'uiGridCustomConfig', '$translate'];

    ng.module('uiModal').controller('ModalSelectTypeWarehouseCtrl', ModalSelectTypeWarehouseCtrl);
})(window.angular);
