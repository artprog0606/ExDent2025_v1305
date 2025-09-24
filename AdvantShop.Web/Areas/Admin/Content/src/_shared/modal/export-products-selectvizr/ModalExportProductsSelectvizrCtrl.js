(function (ng) {
    'use strict';

    var ModalExportProductsSelectvizrCtrl = function ($uibModalInstance, uiGridCustomConfig, uiGridConstants, $http, $q, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.data = [];
            ctrl.itemsSelected = ctrl.$resolve != null && ctrl.$resolve.value != null ? ng.copy(ctrl.$resolve.value.itemsSelected) : null;
            ctrl.exportFeedId = ctrl.$resolve != null && ctrl.$resolve.value != null ? ng.copy(ctrl.$resolve.value.exportFeedId) : null;
            ctrl.allowIncludedProducts =
                ctrl.$resolve != null && ctrl.$resolve.value != null ? ng.copy(ctrl.$resolve.value.allowIncludedProducts) : null;

            //ctrl.selectvizrTreeUrl = 'exportFeeds/categoriestree?exportFeedId=' + ctrl.exportFeedId;
            ctrl.selectvizrTreeUrl = 'catalog/categoriestree';
            ctrl.selectvizrGridUrl = 'exportFeeds/getcatalog';
            ctrl.inplaceExcludedProducts = 'exportFeeds/inplaceExcludedProducts';
            ctrl.selectvizrGridParams = {
                exportFeedId: ctrl.exportFeedId,
                allowIncludedProducts: ctrl.allowIncludedProducts,
            };
            ctrl.selectvizrGridOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: [
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
                        },
                    },
                    {
                        visible: true,
                        name: 'BrandName',
                        displayName: $translate.instant('Admin.Js.ProductSelect.Manufacturer'),
                    },
                    {
                        visible: true,
                        name: 'PriceString',
                        displayName: $translate.instant('Admin.Js.ProductSelect.Price'),
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
                        name: 'ExcludeFromExport',
                        displayName: $translate.instant('Admin.Js.Catalog.ExcludeFromExport'),
                        //width: 80,
                        enableCellEdit: true,
                        type: 'checkbox',
                        cellTemplate:
                            '<div class="ui-grid-cell-contents"><label class="ui-grid-custom-edit-field adv-checkbox-label" data-e2e="switchOnOffLabel"><input type="checkbox" class="adv-checkbox-input" ng-model="MODEL_COL_FIELD " data-e2e="switchOnOffSelect" /><span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span></label></div>',
                        filter: {
                            placeholder: $translate.instant('Admin.Js.Catalog.ExcludeFromExport'),
                            type: uiGridConstants.filter.SELECT,
                            name: 'ExcludeFromExport',
                            selectOptions: [
                                { label: $translate.instant('Admin.Js.Properties.Yes'), value: true },
                                { label: $translate.instant('Admin.Js.Properties.No'), value: false },
                            ],
                        },
                    },
                    {
                        visible: false,
                        name: 'Enabled',
                        filter: {
                            placeholder: $translate.instant('Admin.Js.ProductSelect.Activity'),
                            type: uiGridConstants.filter.SELECT,
                            selectOptions: [
                                { label: $translate.instant('Admin.Js.ProductSelect.TheyActive'), value: true },
                                { label: $translate.instant('Admin.Js.ProductSelect.Inactive'), value: false },
                            ],
                        },
                    },
                ],
                uiGridCustom: {
                    selectionOptions: [
                        {
                            text: $translate.instant('Admin.Js.ExportFeeds.Exclude'),
                            url: 'exportFeeds/excludeproducts',
                            field: 'ProductId',
                        },
                        {
                            text: $translate.instant('Admin.Js.ExportFeeds.Include'),
                            url: 'exportFeeds/includeproducts',
                            field: 'ProductId',
                        },
                    ],
                },
                enableFullRowSelection: true,
            });

            if (ctrl.$resolve.multiSelect === false) {
                ng.extend(ctrl.selectvizrGridOptions, {
                    multiSelect: false,
                    modifierKeysToMultiSelect: false,
                    enableRowSelection: true,
                    enableRowHeaderSelection: false,
                });
            }
        };

        ctrl.onChange = function (categoryId, gridParams) {
            if (gridParams == null) {
                return;
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
                //ctrl.data[itemIndex].ids = ids;
                //ctrl.data[itemIndex].selectMode = selectMode;
            } else {
                ctrl.data.push(
                    ng.extend({ categoryId: categoryId }, gridParams),
                    //{ categoryId: categoryId, ids: ids, selectMode: selectMode }
                );
            }
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

                $uibModalInstance.close({ ids: uniqueItems });
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalExportProductsSelectvizrCtrl.$inject = ['$uibModalInstance', 'uiGridCustomConfig', 'uiGridConstants', '$http', '$q', '$translate'];

    ng.module('uiModal').controller('ModalExportProductsSelectvizrCtrl', ModalExportProductsSelectvizrCtrl);
})(window.angular);
