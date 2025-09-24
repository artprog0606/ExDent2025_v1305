(function (ng) {
    'use strict';

    var ModalProductsSelectvizrCtrl = function ($uibModalInstance, uiGridCustomConfig, uiGridConstants, $http, $q, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            const resolve = ctrl.$resolve;
            const columnDefsCustom = resolve?.gridOptions?.columnDefs;
            ctrl.selectvizrProperty = resolve?.options?.selectvizrProperty;
            ctrl.selectvizrTreeUrl = 'catalog/categoriestree';
            ctrl.selectvizrGridUrl = 'catalog/getcatalog';
            ctrl.data = [];
            ctrl.itemsSelected = ctrl.$resolve != null && ctrl.$resolve.value != null ? ng.copy(ctrl.$resolve.value.itemsSelected) : null;
            ctrl.selectvizrGridParams = { isProductSelector: true };
            ctrl.initLoaded = false;

            let columnDefs = [
                {
                    name: 'ProductArtNo',
                    displayName: $translate.instant('Admin.Js.ProductSelect.VendorCode'),
                    width: 100,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.ProductSelect.VendorCode'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'ArtNo',
                    },
                },
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.ProductSelect.Name'),
                    filter: {
                        placeholder: $translate.instant('Admin.Js.ProductSelect.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                },
                {
                    visible: false,
                    name: 'BrandId',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.ProductSelect.Manufacturer'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'BrandId',
                        fetch: 'catalog/getBrandList',
                        dynamicSearch: true,
                    },
                },
                {
                    visible: false,
                    name: 'ColorId',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.ProductSelect.Color'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'ColorId',
                        fetch: 'catalog/GetColorList',
                        dynamicSearch: true,
                    },
                },
                {
                    visible: false,
                    name: 'SizeId',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.ProductSelect.Size'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'SizeId',
                        fetch: 'catalog/GetSizeList',
                        dynamicSearch: true,
                    },
                },
                {
                    visible: false,
                    name: 'PropertyId',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.ProductSelect.Property'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'PropertyId',
                        fetch: 'catalog/GetPropertyList',
                        dynamicSearch: true,
                        change: function (params, item, filterCtrl) {
                            var colPropertyValue;

                            if (filterCtrl.blocks != null) {
                                for (var i = 0, len = filterCtrl.blocks.length; i < len; i++) {
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
                    visible: false,
                    name: 'PropertyValueId',
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
                    visible: false,
                    name: 'Price',
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
                    visible: false,
                    name: 'Amount',
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
                    visible: true,
                    name: 'Enabled',
                    displayName: 'Активность',
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.ProductSelect.Activity'),
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            { label: $translate.instant('Admin.Js.ProductSelect.TheyActive'), value: true },
                            { label: $translate.instant('Admin.Js.ProductSelect.Inactive'), value: false },
                        ],
                    },
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><label class="adv-checkbox-label">' +
                        '<input type="checkbox" disabled ng-model="row.entity.Enabled" class="adv-checkbox-input control-checkbox" data-e2e="switchOnOffSelect" />' +
                        '<span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span>' +
                        '</label></div>',
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
                    });

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

        ctrl.onChange = function (categoryId, gridParams, rows) {
            if (gridParams == null) {
                return;
            }

            if (rows != undefined) {
                if (rows.length > 0) {
                    ctrl.artNoByProductSelect = rows.find((x) => x.isSelected)?.entity.ProductArtNo;
                }
            }

            var itemIndex;
            for (var i = 0, len = ctrl.data.length; i < len; i++) {
                if (ctrl.data[i].categoryId === categoryId) {
                    itemIndex = i;
                    break;
                }
            }

            if (itemIndex != null) {
                ng.extend(ctrl.data[itemIndex], gridParams);
                //ctrl.data[itemIndex].ids = gridParams.ids;
                //ctrl.data[itemIndex].selectMode = gridParams.selectMode;
            } else {
                ctrl.data.push(
                    ng.extend({ categoryId: categoryId }, gridParams),
                    //{ categoryId: categoryId, ids: gridParams.ids, selectMode: gridParams.selectMode }
                );
            }
        };

        ctrl.getProductSelectvizrSettings = function () {
            let deferred = $q.defer();
            $http.get('catalog/getProductSelectvizrSettings').then(function (response) {
                deferred.resolve(response.data);
            });
            return deferred.promise;
        };

        ctrl.select = function () {
            var promiseArray;

            ctrl.data.forEach(function (dataItem) {
                if (dataItem.selectMode == 'all') {
                    var promise = $http.get('catalog/getCatalogIds', { params: dataItem }).then(function (response) {
                        if (response.data != null) {
                            dataItem.selectMode = 'none';
                            dataItem.ids = response.data.ids.filter(function (item) {
                                return dataItem.ids.indexOf(item) === -1;
                            });
                        }

                        return dataItem;
                    });

                    promiseArray = promiseArray || [];

                    promiseArray.push(promise);
                }
            });

            $q.all(promiseArray || ctrl.data).then(function (data) {
                var allIds = data.reduce(function (previousValue, currentValue) {
                    return previousValue.concat(currentValue.ids);
                }, []);

                var uniqueItems = [];

                allIds.concat(ctrl.itemsSelected || []).forEach(function (item) {
                    uniqueItems.indexOf(item) === -1 ? uniqueItems.push(item) : null;
                });

                $uibModalInstance.close({ ids: uniqueItems, artNo: ctrl.artNoByProductSelect });
            });
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

    ModalProductsSelectvizrCtrl.$inject = ['$uibModalInstance', 'uiGridCustomConfig', 'uiGridConstants', '$http', '$q', '$translate'];

    ng.module('uiModal').controller('ModalProductsSelectvizrCtrl', ModalProductsSelectvizrCtrl);
})(window.angular);
