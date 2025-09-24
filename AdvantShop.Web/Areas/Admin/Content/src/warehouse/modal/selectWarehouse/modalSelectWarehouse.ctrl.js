import './selectWarehouse.html';

(function (ng) {
    'use strict';

    var ModalSelectWarehouseCtrl = function ($q, uiGridConstants, uiGridCustomConfig, $uibModalInstance, $translate) {
        var ctrl = this;
        ctrl.selectedWarehouseObj = {};

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'WarehouseName',
                    displayName: 'Название',
                    filter: {
                        placeholder: 'Название',
                        type: uiGridConstants.filter.INPUT,
                        name: 'WarehouseName',
                    },
                },
                {
                    name: 'CityName',
                    displayName: 'Город',
                    width: 150,
                },
                {
                    visible: false,
                    name: 'CityId',
                    filter: {
                        placeholder: 'Город',
                        type: uiGridConstants.filter.SELECT,
                        name: 'TypeId',
                        fetch: 'cities/getCitiesList',
                        dynamicSearch: true,
                    },
                },
                {
                    name: 'TypeName',
                    displayName: 'Тип',
                    width: 200,
                },
                {
                    visible: false,
                    name: 'TypeId',
                    filter: {
                        placeholder: 'Тип',
                        type: uiGridConstants.filter.SELECT,
                        name: 'TypeId',
                        fetch: 'warehouse/getWarehouseTypesList',
                        dynamicSearch: true,
                    },
                },
                {
                    name: 'Enabled',
                    displayName: 'Актив.',
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><label class="adv-checkbox-label">' +
                        '<input type="checkbox" disabled ng-model="row.entity.Enabled" class="adv-checkbox-input control-checkbox" />' +
                        '<span class="adv-checkbox-emul"></span>' +
                        '</label></div>',
                    width: 76,
                    filter: {
                        placeholder: 'Активность',
                        name: 'Enabled',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: 'Активные',
                                value: true,
                            },
                            { label: 'Неактивные', value: false },
                        ],
                    },
                },
                {
                    name: 'Amount',
                    displayName: 'Остатки',
                    enableCellEdit: false,
                    width: 100,
                },
            ],
            enableFullRowSelection: true,
        });

        ctrl.$onInit = function () {
            ctrl.itemsSelected = ctrl.$resolve != null && ctrl.$resolve.params != null ? ng.copy(ctrl.$resolve.params.itemsSelected) : [];

            if (ctrl.itemsSelected != null) {
                ctrl.convertSelectedListToObj(ctrl.itemsSelected);
            }

            ctrl.multiSelect = ctrl.$resolve.params != null ? ctrl.$resolve.params.multiSelect === true : false;

            if (!ctrl.multiSelect) {
                ctrl.gridOptions.columnDefs.push({
                    name: '_serviceColumn',
                    displayName: '',
                    width: 100,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                        '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.choose(row.entity.WarehouseId, row.entity.WarehouseName)">' +
                        $translate.instant('Admin.Js.AddCategory.Select') +
                        '</a> ' +
                        '</div></div>',
                });
            }

            if (ctrl.multiSelect === false) {
                ng.extend(ctrl.gridOptions, {
                    multiSelect: false,
                    modifierKeysToMultiSelect: false,
                    enableRowSelection: true,
                    enableRowHeaderSelection: true,
                });
            }
        };

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.choose = function (warehouseId, warehouseName) {
            $uibModalInstance.close({ warehouseId: warehouseId, warehouseName: warehouseName });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.gridSelectionOnInit = function (selectionCustom) {
            ctrl.selectionCustom = selectionCustom;
        };

        ctrl.selectWarehouse = function () {
            ctrl.itemsSelected = ctrl.selectionCustom.getRowsFromStorage();
            ctrl.convertSelectedListToObj(ctrl.itemsSelected);
            $uibModalInstance.close(ctrl.itemsSelected);
        };

        ctrl.convertSelectedListToObj = function (warehouseList) {
            warehouseList.forEach(function (it) {
                ctrl.selectedWarehouseObj[it.WarehouseId] = it.WarehouseId;
            });
        };

        ctrl.getSelectedWarehousesItems = function (rowEntity) {
            if (rowEntity != null) {
                return ctrl.selectedWarehouseObj[rowEntity.WarehouseId] != null;
            }
            return false;
        };
    };

    ModalSelectWarehouseCtrl.$inject = ['$q', 'uiGridConstants', 'uiGridCustomConfig', '$uibModalInstance', '$translate'];

    ng.module('uiModal').controller('ModalSelectWarehouseCtrl', ModalSelectWarehouseCtrl);
})(window.angular);
