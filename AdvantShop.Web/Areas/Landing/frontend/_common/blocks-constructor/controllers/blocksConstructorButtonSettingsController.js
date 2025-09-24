(function (ng) {
    'use strict';

    /* @ngInject */
    var BlocksConstructorButtonSettingsCtrl = function (
        blocksConstructorService,
        modalService,
        toaster,
        uiGridCustomConfig,
        uiGridConstants,
        $http,
        domService,
        $translate,
        lpSettingsService,
        $q,
        $scope,
    ) {
        var ctrl = this;
        var offerCache = {};

        ctrl.$onInit = function () {
            ctrl.isMobile = window.matchMedia('(max-width: 768px)').matches;

            ctrl.buttonActions = [];

            if (ctrl.linkExclude !== true) {
                ctrl.buttonActions.push({
                    label: 'Урл адрес',
                    value: 'Url',
                });

                ctrl.buttonActions.push({
                    label: 'Переход к блоку',
                    value: 'Section',
                });
            }

            if (ctrl.commonOptions.Type === 'exit') {
                ctrl.buttonActions.push({
                    label: 'Закрытие модального окна',
                    value: 'ModalClose',
                });
            }
            if (ctrl.formExclude !== true) {
                ctrl.buttonActions.push({
                    label: 'Показ всплывающей формы',
                    value: 'Form',
                });
            }

            if (ctrl.paymentExclude !== true) {
                ctrl.buttonActions.push({
                    label: 'Переход на оплату',
                    value: 'Checkout',
                });

                ctrl.buttonActions.push({
                    label: 'Переход на оплату c Upsell',
                    value: 'CheckoutUpsell',
                });
            }

            ctrl.showShippingsMethodsList = [
                {
                    label: 'Никогда',
                    value: 'Never',
                },
                {
                    label: 'По клику',
                    value: 'ByClick',
                },
                {
                    label: 'Всегда',
                    value: 'Always',
                },
            ];
        };

        ctrl.selectBlock = function (blockSelected) {
            ctrl.buttonOptions.action_section = '#' + blockSelected.id;
        };

        ctrl.changeAction = function (action) {
            ctrl.buttonOptions.action_form = action === 'Form' ? ctrl.commonOptions.Form.Id : null;
            ctrl.buttonOptions.action_upsell_lp_id = null;
        };

        ctrl.selectOffer = function (buttonsOptions, isOne) {
            var parentData = {
                modalData: {
                    selectvizrGridOptions: ng.extend({}, uiGridCustomConfig, {
                        columnDefs: [
                            {
                                name: 'ArtNo',
                                displayName: $translate.instant('Admin.Js.OfferSelect.VendorCode'),
                                width: 100,
                                enableSorting: false,
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
                            },
                            {
                                name: 'SizeName',
                                displayName: $translate.instant('Admin.Js.OffersSelect.Size'),
                                width: 100,
                                enableSorting: false,
                            },
                            {
                                name: 'PriceFormatted',
                                displayName: $translate.instant('Admin.Js.OffersSelect.Price'),
                                width: 120,
                                enableSorting: false,
                            },
                        ],

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
                                var classes = [];

                                if (row.treeNode.children == null || row.treeNode.children.length === 0) {
                                    classes.push('ui-grid-custom-prevent-pointer');
                                }

                                if (!row.entity.Enabled) {
                                    classes.push('ui-grid-custom-disabled-item-row');
                                }

                                return classes.join(' ');

                                //return row.treeNode.children == null || row.treeNode.children.length === 0 ? 'ui-grid-custom-prevent-pointer' : '';
                            },
                        },
                    }),
                    gridOnFetch: function (grid) {
                        if (grid != null && grid.gridOptions != null && grid.gridOptions.data != null && grid.gridOptions.data.length > 0) {
                            for (var i = 0, len = grid.gridOptions.data.length; i < len; i++) {
                                if (grid.gridOptions.data[i].Main === true) {
                                    grid.gridOptions.data[i].$$treeLevel = 0;
                                }
                            }
                        }
                    },
                    onChange: function (categoryId, ids, selectMode) {
                        ctrl.data = {
                            categoryId: categoryId,
                            ids: ids,
                            selectMode: selectMode,
                        };
                    },
                    select: async function () {
                        if (ctrl.data.selectMode == 'all') {
                            await $http
                                .get('adminv3/catalog/getCatalogOfferIds', { params: ctrl.data })
                                .then(function (response) {
                                    if (response.data != null) {
                                        ctrl.data.selectMode = 'none';
                                        ctrl.data.ids = response.data.ids.filter(function (item) {
                                            return ctrl.data.ids.indexOf(item) === -1;
                                        });
                                    }
                                    buttonsOptions.use_many_offers = true;
                                    buttonsOptions.action_offer_id = null;
                                    buttonsOptions.action_offer_items = ctrl.getActionOfferItems(buttonsOptions.action_offer_items, ctrl.data.ids);
                                    return ctrl.data;
                                })
                                .then(async (data) => {
                                    await ctrl.getActionOfferItemsPrepared(buttonsOptions);
                                    modalService.close('modalSelectOffer');
                                    return data;
                                });
                        } else {
                            buttonsOptions.use_many_offers = true;
                            buttonsOptions.action_offer_id = null;
                            buttonsOptions.action_offer_items = ctrl.getActionOfferItems(buttonsOptions.action_offer_items, ctrl.data.ids);

                            if (
                                isOne != null &&
                                isOne == true &&
                                buttonsOptions.action_offer_items != null &&
                                buttonsOptions.action_offer_items.length > 0
                            ) {
                                if (ctrl.buttonOptions.show_options && buttonsOptions.action_offer_items.length > 1) {
                                    buttonsOptions.action_offer_items = [buttonsOptions.action_offer_items[1]];
                                } else {
                                    buttonsOptions.action_offer_items = [buttonsOptions.action_offer_items[0]];
                                }
                            }
                            await ctrl.getActionOfferItemsPrepared(buttonsOptions);
                            modalService.close('modalSelectOffer');
                            return ctrl.data;
                        }
                    },
                },
            };

            modalService.renderModal(
                'modalSelectOffer',
                'Выберите товар',
                '<div class="blocks-constructor-settings-col--vertical"><offers-selectvizr selectvizr-tree-url="\'adminv3/catalog/categoriestree\'" ' +
                    'selectvizr-grid-url="\'adminv3/catalog/getOffersCatalog\'" ' +
                    'selectvizr-grid-options="modalData.selectvizrGridOptions" ' +
                    'selectvizr-grid-on-fetch="modalData.gridOnFetch(grid)" ' +
                    'selectvizr-on-change="modalData.onChange(categoryId, ids, selectMode)"' +
                    '>' +
                    '</offers-selectvizr></div>',
                '<input type="button" class="blocks-constructor-btn-confirm" data-ng-click="modalData.select()" value="Применить" />' +
                    '<input type="button" class="blocks-constructor-btn-cancel blocks-constructor-btn-mar" data-modal-close="modalSelectOffer" value="Отмена" />',
                {
                    modalClass: 'blocks-constructor-modal',
                    modalOverlayClass: 'blocks-constructor-modal-floating-wrap blocks-constructor-modal--settings',
                    isFloating: true,
                    backgroundEnable: false,
                    destroyOnClose: true,
                },
                parentData,
            );

            modalService.getModal('modalSelectOffer').then(function (modal) {
                modal.modalScope.open();
            });
        };

        ctrl.getActionOfferItems = function (items, ids) {
            items = items || [];
            var res = items;
            for (var i = 0; i < ids.length; i++) {
                var offerId = ids[i];
                var finded = items.filter(function (x) {
                    return x.offerId == offerId;
                });

                if (finded == null || finded.length == 0) {
                    res.push({ offerId: offerId, offerPrice: '' });
                }
            }
            return res;
        };

        ctrl.getActionOfferItemsPrepared = async function (buttonOptions) {
            return (ctrl.action_offer_items_prepared = await blocksConstructorService.getOfferDataFromId(buttonOptions.action_offer_items || []));
        };

        ctrl.getActionOfferItemsPreparedForOne = async function (buttonOptions) {
            if (buttonOptions.action_offer_items != null && buttonOptions.action_offer_items.length > 1) {
                buttonOptions.action_offer_items = [buttonOptions.action_offer_items[0]];
            }

            await ctrl.getActionOfferItemsPrepared(buttonOptions);

            $scope.$digest();

            return ctrl.action_offer_items_prepared;
        };

        ctrl.getPreparedActionOfferItem = function (offerId) {
            if (offerCache[offerId] != null) {
                return offerCache[offerId];
            }

            if (ctrl.action_offer_items_prepared == null) {
                return null;
            }

            var items = ctrl.action_offer_items_prepared.filter(function (x) {
                return x.offerId == offerId;
            });

            if (items != null && items.length > 0) {
                offerCache[offerId] = items[0];
                return items[0];
            } else {
                return null;
            }

            //return items != null && items.length > 0 ? items[0] : {};
        };

        ctrl.deleteActionOfferItem = function (actionOfferItems, preparedItem) {
            actionOfferItems.splice(actionOfferItems.indexOf(preparedItem), 1);

            var items = ctrl.action_offer_items_prepared.filter(function (x) {
                return x.offerId === preparedItem.offerId;
            });
            if (items != null && items.length > 0) {
                ctrl.action_offer_items_prepared.splice(ctrl.action_offer_items_prepared.indexOf(items[0]), 1);
            }
        };

        ctrl.getProductNameByOfferId = function (offerId, setPrice) {
            if (offerId == null || offerId.length == 0 || offerId == 0) {
                return;
            }

            blocksConstructorService.getProductNameByOfferId({ offerId: offerId }).then(function (response) {
                var data = response.data;
                if (data != null) {
                    ctrl.productId = data.ProductId;
                    ctrl.productName = '[' + data.ArtNo + '] ' + data.Name;
                    ctrl.productEnabled = data.Enabled;
                    //if (setPrice && (ctrl.buttonOptions.action_offer_price == null || ctrl.buttonOptions.action_offer_price.length == 0)) {
                    //    ctrl.buttonOptions.action_offer_price = data.Price;
                    //}
                }
            });
        };

        ctrl.setDefaultMinAmount = function (item) {
            if (item.offerId == null || item.offerId === '' || (item.offerAmount != null && item.offerAmount !== '')) {
                if (item.offerAmount != null && item.offerAmount !== '') {
                    item.offerAmount = parseFloat(item.offerAmount);
                }
                return;
            }

            blocksConstructorService.getProductNameByOfferId(item).then(function (data) {
                if (data != null) {
                    item.offerAmount = parseFloat(data.MinAmount);
                }
            });
        };

        ctrl.deleteProduct = function (buttonsOptions) {
            buttonsOptions.action_offer_id = null;
            ctrl.productId = null;
            ctrl.productName = null;
            ctrl.productEnabled = null;
        };

        ctrl.initUseManyOffers = function (buttonOptions, isForm) {
            if (buttonOptions.use_many_offers == true) {
                buttonOptions.action_offer_id = null;
                buttonOptions.action_offer_items = buttonOptions.action_offer_items.filter(function (x) {
                    return x.offerId != null && x.offerId !== '';
                });
            } else {
                buttonOptions.use_many_offers = true;
                buttonOptions.action_offer_items = [];

                if (buttonOptions.action_offer_id != null && buttonOptions.action_offer_id != '') {
                    buttonOptions.action_offer_items.push({ offerId: buttonOptions.action_offer_id, offerPrice: '' });
                }
                buttonOptions.action_offer_id = null;
            }
        };
    };

    ng.module('blocksConstructor').controller('BlocksConstructorButtonSettingsCtrl', BlocksConstructorButtonSettingsCtrl);
})(window.angular);
