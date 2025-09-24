import './offersSelectvizrModal.html';

(function (ng) {
    'use strict';

    var ModalOffersSelectvizrCtrl = /* @ngInject */ function ($uibModalInstance, uiGridCustomConfig, uiGridConstants, $http, domService, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            const resolve = ctrl.$resolve;
            const columnDefsCustom = resolve?.gridOptions?.columnDefs;

            ctrl.selectvizrProperty = resolve?.options?.selectvizrProperty;
            ctrl.selectvizrTreeUrl = 'catalog/categoriestree';
            ctrl.selectvizrGridUrl = 'catalog/getOffersCatalog';

            ctrl.initLoaded = false;

            let columnDefs = [
                {
                    name: 'ArtNo',
                    displayName: $translate.instant('Admin.Js.OfferSelect.VendorCode'),
                    width: 100,
                    enableSorting: false,
                    visible: 500,
                },
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.OffersSelect.Name'),
                    enableSorting: false,
                },
                {
                    name: 'ColorName',
                    displayName: $translate.instant('Admin.Js.OffersSelect.Color'),
                    width: 100,
                    enableSorting: false,
                    visible: 900,
                },
                {
                    name: 'SizeNameFormatted',
                    displayName: $translate.instant('Admin.Js.OffersSelect.Size'),
                    width: 100,
                    enableSorting: false,
                    visible: 1210,
                },
                {
                    name: 'PriceFormatted',
                    displayName: $translate.instant('Admin.Js.OffersSelect.Price'),
                    width: 120,
                    enableSorting: false,
                },
                {
                    name: 'Amount',
                    displayName: $translate.instant('Admin.Js.OffersSelect.ProductCount'),
                    width: 100,
                    enableSorting: false,
                    visible: 1250,
                },

                {
                    name: '_noopColumnArtNo',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.OfferSelect.VendorCode'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'ArtNo',
                    },
                },
                {
                    name: '_noopColumnName',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.OffersSelect.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                },
                {
                    name: '_noopColumnBrandId',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.ProductSelect.Manufacturer'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'BrandId',
                        fetch: 'catalog/getBrandList',
                        dynamicSearch: true,
                    },
                },
                {
                    name: '_noopColumnColorId',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.ProductSelect.Color'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'ColorId',
                        fetch: 'catalog/GetColorList',
                        dynamicSearch: true,
                    },
                },
                {
                    name: '_noopColumnSizeId',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.ProductSelect.Size'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'SizeId',
                        fetch: 'catalog/GetSizeList',
                        dynamicSearch: true,
                    },
                },
                {
                    name: '_noopColumnPropertyId',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.ProductSelect.Property'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'PropertyId',
                        fetch: 'catalog/GetPropertyList',
                        dynamicSearch: true,
                        change: function (params, item, filterCtrl) {
                            let colPropertyValue;

                            if (filterCtrl.blocks != null) {
                                for (let i = 0, len = filterCtrl.blocks.length; i < len; i++) {
                                    if (filterCtrl.blocks[i].name === 'PropertyValueId') {
                                        colPropertyValue = filterCtrl.blocks[i];
                                        break;
                                    }
                                }
                            }
                            if (colPropertyValue != undefined) {
                                colPropertyValue.filter.term = null;
                                filterCtrl.fill(item.filter.type, colPropertyValue, null, 'PropertyValueId');
                            }
                        },
                    },
                },
                {
                    name: '_noopColumnPropertyValueId',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.ProductSelect.PropertyValue'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'PropertyValueId',
                        fetch: 'catalog/GetPropertyValueList',
                        dynamicSearch: true,
                        dynamicSearchRelations: ['PropertyId'],
                    },
                },
                {
                    name: '_noopColumnPrice',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.ProductSelect.Price'),
                        type: 'range',
                        rangeOptions: {
                            from: {
                                name: 'PriceFrom',
                            },
                            to: {
                                name: 'PriceTo',
                            },
                        },
                        fetch: 'catalog/getpricerangeforpaging',
                    },
                },
                {
                    name: '_noopColumnAmount',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.ProductSelect.Quantity'),
                        type: 'range',
                        rangeOptions: {
                            from: {
                                name: 'AmountFrom',
                            },
                            to: {
                                name: 'AmountTo',
                            },
                        },
                        fetch: 'catalog/getamountrangeforpaging',
                    },
                },
                {
                    name: '_noopColumnEnabled',
                    visible: false,
                    filter: {
                        name: 'Enabled',
                        placeholder: $translate.instant('Admin.Js.ProductSelect.Activity'),
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            { label: $translate.instant('Admin.Js.ProductSelect.TheyActive'), value: true },
                            { label: $translate.instant('Admin.Js.ProductSelect.Inactive'), value: false },
                        ],
                    },
                },
                {
                    name: '_noopColumnBarCode',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Catalog.BarCode'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'BarCode',
                    },
                },
            ];

            ctrl.getProductSelectvizrSettings()
                .then(function (data) {
                    if (data != null) {
                        if (data.IsWarehouseFilterVisible) {
                            columnDefs = columnDefs.concat([
                                {
                                    name: '_noopColumnWarehouses',
                                    visible: false,
                                    filter: {
                                        placeholder: $translate.instant('Admin.Js.Catalog.Warehouses'),
                                        name: 'WarehouseIds',
                                        type: 'selectMultiple',
                                        fetch: 'warehouse/getWarehousesList',
                                    },
                                },
                            ]);
                        }
                    }

                    ctrl.selectvizrGridOptions = ng.extend({}, uiGridCustomConfig, {
                        columnDefs: columnDefs.concat(columnDefsCustom || []),
                        enableFullRowSelection: true,
                        showTreeExpandNoChildren: false,
                        uiGridCustom: {
                            rowClick: function ($event, row, grid) {
                                if (
                                    row.treeNode.children &&
                                    row.treeNode.children.length > 0 &&
                                    domService.closest($event.target, '.ui-grid-tree-base-row-header-buttons') == null
                                ) {
                                    grid.gridApi.treeBase.toggleRowTreeState(row);
                                }
                            },
                            rowClasses: function (row) {
                                return row.treeNode.children == null || row.treeNode.children.length === 0 ? 'ui-grid-custom-prevent-pointer' : '';
                            },
                        },
                    });

                    uiGridCustomConfig.enableHorizontalScrollbar = 1;

                    if (ctrl.$resolve.multiSelect === false) {
                        ng.extend(ctrl.selectvizrGridOptions, {
                            multiSelect: false,
                            modifierKeysToMultiSelect: false,
                            enableRowSelection: true,
                            enableRowHeaderSelection: true,
                        });
                    }
                })
                .finally(function () {
                    ctrl.initLoaded = true;
                });
        };

        ctrl.getProductSelectvizrSettings = function () {
            return $http.get('catalog/getProductSelectvizrSettings').then(function (response) {
                return response.data;
            });
        };

        ctrl.onChange = function (categoryId, ids, selectMode) {
            ctrl.data = {
                categoryId: categoryId,
                ids: ids,
                selectMode: selectMode,
            };
        };

        ctrl.gridOnFetch = function (grid) {
            if (grid != null && grid.gridOptions != null && grid.gridOptions.data != null && grid.gridOptions.data.length > 0) {
                for (var i = 0, len = grid.gridOptions.data.length; i < len; i++) {
                    if (grid.gridOptions.data[i].Main === true) {
                        grid.gridOptions.data[i].$$treeLevel = 0;
                    }
                }
            }
        };

        ctrl.select = function () {
            if (ctrl.data.selectMode == 'all') {
                $http.get('catalog/getCatalogOfferIds', { params: ctrl.data }).then(function (response) {
                    if (response.data != null) {
                        ctrl.data.selectMode = 'none';
                        ctrl.data.ids = response.data.ids.filter(function (item) {
                            return ctrl.data.ids.indexOf(item) === -1;
                        });
                    }
                    $uibModalInstance.close(ctrl.data);
                });
            } else {
                $uibModalInstance.close(ctrl.data);
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.gridSelectionItemsSelectedFn = function (rowEntity) {
            if (
                ctrl.$resolve != null &&
                ctrl.$resolve.gridSelectionItemsSelectedFn != null &&
                ctrl.$resolve.gridSelectionItemsSelectedFn.fn != null
            ) {
                return ctrl.$resolve.gridSelectionItemsSelectedFn.fn(rowEntity);
            }
        };
    };

    ng.module('uiModal').controller('ModalOffersSelectvizrCtrl', ModalOffersSelectvizrCtrl);
})(window.angular);
