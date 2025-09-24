(function (ng) {
    'use strict';

    var LpSettingsCtrl = function (
        $window,
        $http,
        toaster,
        blocksConstructorService,
        lpSettingsService,
        modalService,
        uiGridCustomConfig,
        uiGridConstants,
        $q,
        urlHelper,
    ) {
        var ctrl = this;

        ctrl.initLp = function (lpId) {
            ctrl.lpId = lpId;
            ctrl.colorSchemeList = blocksConstructorService.getColorSchemeList();
            ctrl.getSettings().then(function (settings) {
                ctrl.colorScheme = settings.ColorSchemes[0];
            });
        };

        ctrl.getSettings = function () {
            return lpSettingsService.get(ctrl.lpId).then(function (data) {
                return (ctrl.settings = data);
            });
        };

        ctrl.saveSettings = function () {
            ctrl.pictureLoaderFavicon.save(function (result) {
                if (result != null) {
                    ctrl.settings.Favicon = result.picture;
                }

                $http.post('landing/landingInplace/saveSettings', { lpId: ctrl.lpId, settings: ctrl.settings }).then(function (response) {
                    if (response.data != null) {
                        urlHelper.setLocationQueryParams('tab', null);
                        $window.location.href = response.data;
                    } else {
                        toaster.pop('error', '', 'Настройки не сохранены');
                    }
                });
            });
        };

        ctrl.onFaviconDeleted = function () {
            ctrl.getSettings();
        };

        ctrl.pictureLoaderFaviconOnInit = function (pictureLoader) {
            ctrl.pictureLoaderFavicon = pictureLoader;
        };

        ctrl.defer = $q.defer();
        ctrl.unSelectedItems = [];

        ctrl.selectProduct = function (buttonsOptions) {
            var parentData = {
                modalData: {
                    selectvizrGridOptions: ng.extend({}, uiGridCustomConfig, {
                        columnDefs: [
                            {
                                name: 'ProductArtNo',
                                displayName: 'Артикул',
                                width: 100,
                                filter: {
                                    placeholder: 'Артикул',
                                    type: uiGridConstants.filter.INPUT,
                                    name: 'ArtNo',
                                },
                            },
                            {
                                name: 'Name',
                                displayName: 'Название',
                                filter: {
                                    placeholder: 'Название',
                                    type: uiGridConstants.filter.INPUT,
                                    name: 'Name',
                                },
                            },
                            {
                                visible: true,
                                name: 'Enabled',
                                displayName: 'Активность',
                                enableCellEdit: false,
                                cellTemplate:
                                    '<div class="ui-grid-cell-contents"><label class="adv-checkbox-label">' +
                                    '<input type="checkbox" disabled ng-model="row.entity.Enabled" class="adv-checkbox-input control-checkbox" data-e2e="switchOnOffSelect" />' +
                                    '<span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span>' +
                                    '</label></div>',
                            },
                        ],
                        enableFullRowSelection: true,
                    }),
                    onChange: function (categoryId, gridParams, rows) {
                        if (gridParams == null) {
                            return;
                        }

                        ctrl.data = ctrl.data || [];

                        var itemIndex,
                            willDeleteIds = [];

                        for (var i = 0, len = ctrl.data.length; i < len; i++) {
                            if (ctrl.data[i].categoryId === categoryId) {
                                itemIndex = i;
                                break;
                            }
                        }

                        if (rows != null) {
                            rows.forEach(function (item) {
                                if (!item.isSelected) {
                                    ctrl.unSelectedItems.push(item.entity.ProductId);
                                }
                            });
                        }

                        if (itemIndex != null) {
                            ctrl.data[itemIndex].rows = rows;
                            ng.extend(ctrl.data[itemIndex], gridParams);
                        } else {
                            ctrl.data.push(ng.extend({ categoryId: categoryId, rows: rows }, gridParams));
                        }
                    },
                    select: function (modalId) {
                        var promiseArray;

                        if (ctrl.data.length === 0) return;
                        ctrl.data.forEach(function (dataItem) {
                            if (dataItem.selectMode == 'all') {
                                if (dataItem.rows != null) {
                                    delete dataItem.rows;
                                }
                                var promise = $http.get('/adminv3/catalog/getCatalogIds', { params: dataItem }).then(function (response) {
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

                        $q.all(promiseArray || ctrl.data)
                            .then(function (data) {
                                var allIds = data.reduce(function (previousValue, currentValue) {
                                    return previousValue.concat(currentValue.ids);
                                }, []);
                                allIds = (ctrl.itemsSelected || []).concat(allIds);
                                var uniqueItems = ctrl.getUniqueIds(allIds);

                                allIds.forEach(function (item) {
                                    uniqueItems.indexOf(item) === -1 ? uniqueItems.push(item) : null;
                                });

                                if (ctrl.unSelectedItems.length > 0) {
                                    var newIdsAfterDelete = uniqueItems.filter(function (item) {
                                        return ctrl.unSelectedItems.indexOf(item) === -1;
                                    });
                                    ctrl.unSelectedItems.length = 0;
                                    return newIdsAfterDelete;
                                }

                                return uniqueItems;
                            })
                            .then(function (data) {
                                if (data.length === 0) {
                                    alert('Выберите хотя бы один элемент');
                                    return;
                                } else {
                                    ctrl.itemsSelected = data;
                                    ctrl.getProductsByIds(data);
                                    modalService.close(modalId);
                                }
                            });
                    },
                },
            };

            modalService.renderModal(
                'modalSelectProduct',
                'Выберите товар',
                '<div class="blocks-constructor-settings-col--vertical"><products-selectvizr selectvizr-tree-url="\'adminv3/catalog/categoriestree\'" ' +
                    'selectvizr-grid-url="\'adminv3/catalog/getCatalog\'" ' +
                    'selectvizr-grid-options="modalData.selectvizrGridOptions" ' +
                    'selectvizr-grid-on-fetch="modalData.gridOnFetch(grid)" ' +
                    'selectvizr-on-change="modalData.onChange(categoryId, gridParams, rows)"' +
                    '>' +
                    '</products-selectvizr></div>',
                '<input type="button" class="blocks-constructor-btn-confirm" data-ng-click="modalData.select(\'modalSelectProduct\')" value="Применить" />' +
                    '<input type="button" class="blocks-constructor-btn-cancel blocks-constructor-btn-mar" data-modal-close="modalSelectProduct" value="Отмена" />',
                {
                    modalClass: 'blocks-constructor-modal',
                    modalOverlayClass: 'blocks-constructor-modal-floating-wrap blocks-constructor-modal--settings',
                    isFloating: true,
                    backgroundEnable: false,
                    destroyOnClose: true,
                },
                parentData,
            );

            modalService.getModal('modalSelectProduct').then(function (modal) {
                modal.modalScope.open();
            });
        };

        ctrl.getProductsByIds = function (productIds) {
            if (productIds != null && productIds.length === 0) {
                ctrl.products = [];
                return;
            }
            var promise = $http.post('/landinginplace/getProductsByIds', { productIds: productIds }).then(function (response) {
                if (response.data != null && response.data.Products.length > 0) {
                    ctrl.AuthOrderProducts = response.data.Products != null ? response.data.Products : null;
                    ctrl.settings.AuthOrderProductIds =
                        ctrl.AuthOrderProducts != null
                            ? ctrl.AuthOrderProducts.map(function (x) {
                                  return x.ProductId;
                              })
                            : null;
                }
            });
        };

        ctrl.getUniqueIds = function (ids) {
            var uniqueItems = [];

            ids.forEach(function (item) {
                uniqueItems.indexOf(item) === -1 ? uniqueItems.push(item) : null;
            });

            return uniqueItems;
        };

        ctrl.getDealStatuses = function (salesFunnelId) {
            if (salesFunnelId == null || salesFunnelId == '') return;

            $http.get('/adminv3/salesFunnels/getDealStatuses', { params: { salesFunnelId: salesFunnelId } }).then(function (response) {
                ctrl.DealStatuses = response.data;
            });
        };

        ctrl.getAuthOrderProducts = function (productIds) {
            ctrl.getProductsByIds(productIds);
        };

        ctrl.deleteAuthOrderProduct = function (item) {
            ctrl.AuthOrderProducts.splice(ctrl.AuthOrderProducts.indexOf(item), 1);
            ctrl.settings.AuthOrderProductIds.splice(ctrl.settings.AuthOrderProductIds.indexOf(item.ProductId), 1);
        };

        ctrl.callbackClose = function () {
            urlHelper.setLocationQueryParams('tab', null);
        };
    };

    ng.module('lpSettings').controller('LpSettingsCtrl', LpSettingsCtrl);

    LpSettingsCtrl.$inject = [
        '$window',
        '$http',
        'toaster',
        'blocksConstructorService',
        'lpSettingsService',
        'modalService',
        'uiGridCustomConfig',
        'uiGridConstants',
        '$q',
        'urlHelper',
    ];
})(window.angular);
