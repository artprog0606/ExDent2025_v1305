import './couponProductsSelectvizrModal.html';

(function (ng) {
    'use strict';

    var ModalCouponProductsSelectvizrCtrl = function (
        $uibModalInstance,
        uiGridCustomConfig,
        uiGridConstants,
        $http,
        $q,
        $translate,
        isMobileService,
    ) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.data = [];
            ctrl.itemsSelected = ctrl.$resolve != null && ctrl.$resolve.value != null ? ng.copy(ctrl.$resolve.value.itemsSelected) : null;
            ctrl.couponId = ctrl.$resolve != null && ctrl.$resolve.value != null ? ng.copy(ctrl.$resolve.value.couponId) : null;

            ctrl.selectvizrTreeUrl = 'catalog/categoriestree';
            ctrl.selectvizrGridUrl = 'coupons/getcatalog';
            ctrl.inplaceProducts = 'coupons/inplaceApllyCouponToProduct';
            ctrl.selectvizrGridParams = { couponId: ctrl.couponId, isProductSelector: true };
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
                        name: 'ApllyCoupon',
                        displayName: $translate.instant('Admin.Js.SettingsCoupon.ApplyCoupon'),
                        //width: 80,
                        enableCellEdit: true,
                        type: 'checkbox',
                        cellTemplate:
                            '<div class="ui-grid-cell-contents js-grid-not-clicked"><label class="ui-grid-custom-edit-field adv-checkbox-label" data-e2e="switchOnOffLabel"><input type="checkbox" class="adv-checkbox-input" ng-model="MODEL_COL_FIELD " data-e2e="switchOnOffSelect" /><span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span></label></div>',
                        filter: {
                            placeholder: $translate.instant('Admin.Js.SettingsCoupon.ApplyCoupon'),
                            type: uiGridConstants.filter.SELECT,
                            name: 'ApllyCoupon',
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
                            text: 'Применять купон к товарам',
                            url: 'coupons/ApllyCouponToProducts',
                            field: 'ProductId',
                        },
                        {
                            text: 'Не применять купон к товарам',
                            url: 'coupons/NotApllyCouponToProducts',
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
            } else {
                ctrl.data.push(ng.extend({ categoryId: categoryId }, gridParams));
            }
        };

        ctrl.close = function () {
            $uibModalInstance.close('close');
        };
    };

    ModalCouponProductsSelectvizrCtrl.$inject = [
        '$uibModalInstance',
        'uiGridCustomConfig',
        'uiGridConstants',
        '$http',
        '$q',
        '$translate',
        'isMobileService',
    ];

    ng.module('uiModal').controller('ModalCouponProductsSelectvizrCtrl', ModalCouponProductsSelectvizrCtrl);
})(window.angular);
