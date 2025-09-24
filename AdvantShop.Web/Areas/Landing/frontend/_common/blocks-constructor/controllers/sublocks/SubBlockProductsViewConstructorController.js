(function (ng) {
    'use strict';

    var SubBlockProductsViewCtrl = function (blocksConstructorService, uiGridConstants, uiGridCustomConfig, $q, $http, modalService, toaster) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.selectvizrTreeUrl = '/adminv3/catalog/categoriestree';
            ctrl.selectvizrGridUrl = '/adminv3/catalog/getcatalog';
            ctrl.selectvizrTreeSearch = {
                ajax: {
                    url: './adminv3/catalog/categoriesTreeBySearchRequest',
                },
            };
            //ctrl.selectvizrGridUrl = '/landinginplace/productsforgrid';
            ctrl.data = [];
            ctrl.defer = $q.defer();
            ctrl.unSelectedItems = [];

            ctrl.sortableOptions = {
                //containment: '#photosSortable',
                //scrollableContainer: '#photosSortable',
                //containerPositioning: 'relative',
                orderChanged: function (event) {
                    if (ctrl.onOrderChanged) {
                        ctrl.onOrderChanged(event);
                    }
                },
            };

            ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
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
                        visible: false,
                        name: 'BrandId',
                        filter: {
                            placeholder: 'Производитель',
                            type: uiGridConstants.filter.SELECT,
                            name: 'BrandId',
                            fetch: '/adminv3/catalog/getBrandList',
                        },
                    },
                    {
                        visible: false,
                        name: 'Price',
                        filter: {
                            placeholder: 'Цена',
                            type: 'range',
                            rangeOptions: {
                                from: {
                                    name: 'PriceFrom',
                                },
                                to: {
                                    name: 'PriceTo',
                                },
                            },
                            fetch: '/adminv3/catalog/getpricerangeforpaging',
                        },
                    },
                    {
                        visible: false,
                        name: 'Amount',
                        filter: {
                            placeholder: 'Количество',
                            type: 'range',
                            rangeOptions: {
                                from: {
                                    name: 'AmountFrom',
                                },
                                to: {
                                    name: 'AmountTo',
                                },
                            },
                            fetch: '/adminv3/catalog/getamountrangeforpaging',
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
            });

            //if (ctrl.multiSelect === false) {
            //    ng.extend(ctrl.gridOptions, {
            //        multiSelect: false,
            //        modifierKeysToMultiSelect: false,
            //        enableRowSelection: true,
            //        enableRowHeaderSelection: false
            //    });
            //}
        };

        ctrl.onOrderChanged = function (event) {
            var sourceIndex = event.source.index,
                destIndex = event.dest.index,
                elMoved = ctrl.itemsSelected[sourceIndex];

            ctrl.itemsSelected.splice(sourceIndex, 1);
            ctrl.itemsSelected.splice(destIndex, 0, elMoved);
        };

        ctrl.openProductsModal = function (data) {
            var modalId = 'modalProducts_' + data.blockId;
            if (ctrl.itemsSelected == null) {
                ctrl.itemsSelected = data.settings.product_ids;
            }

            //if (ctrl.itemsSelected.length > 0) {
            //    ctrl.getCategoryIdsByProductId(ctrl.itemsSelected).then(function (data) {
            //        console.log(data);
            //    });
            //}
            ctrl.itemsSelectedCopy = ng.copy(ctrl.itemsSelected);
            modalService.renderModal(
                modalId,
                'Выберите товары',
                '<div class="blocks-constructor-settings-col--vertical"><products-selectvizr selectvizr-tree-url="selectvizrTreeUrl"' +
                    'selectvizr-grid-url="selectvizrGridUrl"' +
                    'selectvizr-grid-options="gridOptions"' +
                    'selectvizr-tree-items-selected="categoryIds"' +
                    'selectvizr-on-init="onInitSelectvizr(grid)"' +
                    'selectvizr-tree-search="selectvizrTreeSearch"' +
                    'selectvizr-grid-items-selected="itemsSelectedCopy"' +
                    'selectvizr-on-change="onChange(categoryId, gridParams, rows)">' +
                    '</products-selectvizr></div>',
                '<button type="button" class="blocks-constructor-btn-confirm" data-ng-click="select(modalId)">Выбрать</button><button type="button" class="blocks-constructor-btn-cancel blocks-constructor-btn-mar" modal-close>Отмена</button>',
                {
                    callbackClose: 'onModalClose()',
                    modalClass: 'blocks-constructor-modal',
                    modalOverlayClass: 'blocks-constructor-modal-floating-wrap blocks-constructor-modal--settings',
                    isFloating: true,
                    backgroundEnable: false,
                    destroyOnClose: true,
                },
                {
                    selectvizrTreeUrl: ctrl.selectvizrTreeUrl,
                    selectvizrGridUrl: ctrl.selectvizrGridUrl,
                    gridOptions: ctrl.gridOptions,
                    itemsSelectedCopy: ctrl.itemsSelectedCopy,
                    itemsSelected: ctrl.itemsSelected,
                    selectvizrTreeItemsSelected: ctrl.itemsSelectedCopy,
                    onChange: ctrl.onChange,
                    products: ctrl.data,
                    modalData: data,
                    modalId: modalId,
                    onModalClose: ctrl.onModalClose,
                    onInitSelectvizr: ctrl.onInitSelectvizr,
                    select: ctrl.select,
                    selectvizrTreeSearch: ctrl.selectvizrTreeSearch,
                },
            );

            modalService.getModal(modalId).then(function (modal) {
                modal.modalScope.open();
            });
        };

        ctrl.onInitSelectvizr = function (grid) {
            ctrl.gridCustom = grid;
        };

        ctrl.onModalClose = function () {
            ctrl.data.ids = ctrl.itemsSelected;
            ctrl.gridCustom.gridApi.grid.appScope.$destroy();
        };

        ctrl.getUniqueIds = function (ids) {
            var uniqueItems = [];

            ids.forEach(function (item) {
                uniqueItems.indexOf(item) === -1 ? uniqueItems.push(item) : null;
            });

            return uniqueItems;
        };

        ctrl.deleteProduct = function (id, scopeCtrl) {
            //ctrl.itemsSelected = ctrl.itemsSelected.concat(ctrl.data.ids || []);

            var uniqueItems = ctrl.getUniqueIds(ctrl.itemsSelected);

            ctrl.itemsSelected = uniqueItems.filter(function (productid) {
                return productid !== id;
            });
            ctrl.getProductsByIds(ctrl.itemsSelected);

            //ctrl.deleteVideoByProductId(id, scopeCtrl);
        };

        ctrl.getProductsByIds = function (productIds) {
            if (productIds != null && productIds.length === 0) {
                ctrl.products = [];
                return;
            }
            var promise = $http.post('/landinginplace/GetProductsByIds', { productIds: productIds }).then(function (response) {
                if (response.data != null && response.data.Products.length > 0) {
                    ctrl.products = response.data.Products;
                }
            });
        };

        ctrl.onChange = function (categoryId, gridParams, rows) {
            if (gridParams == null) {
                return;
            }

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
                    } else {
                        var index = ctrl.unSelectedItems.indexOf(item.entity.ProductId);

                        if (index !== -1) {
                            ctrl.unSelectedItems[index] = undefined;
                        }
                    }
                });

                ctrl.unSelectedItems = ctrl.unSelectedItems.filter(function (item) {
                    return item !== undefined;
                });
            }

            if (itemIndex != null) {
                ctrl.data[itemIndex].rows = rows;
                ng.extend(ctrl.data[itemIndex], gridParams);
                //ctrl.data[itemIndex].ids = gridParams.ids;
                //ctrl.data[itemIndex].selectMode = gridParams.selectMode;
            } else {
                ctrl.data.push(
                    ng.extend({ categoryId: categoryId, rows: rows }, gridParams),
                    //{ categoryId: categoryId, ids: gridParams.ids, rows: rows, selectMode: gridParams.selectMode }
                );
            }
        };

        ctrl.onApplySettings = function (modalData) {
            modalData.Settings.product_ids = ctrl.itemsSelected;

            return blocksConstructorService
                .saveProductsIds(modalData.BlockId, ctrl.itemsSelected.toString())
                .then(function (response) {
                    if (response.result !== true) {
                        throw new Error('Ошибка при добавлении товара!');
                    }
                })
                .catch(function (e) {
                    toaster.pop('error', 'Ошибка при добавлении товара!');
                });
        };

        ctrl.closeModal = function (modalId) {
            modalService.close(modalId);
        };

        ctrl.select = function (modalId) {
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

                    //return uniqueItems;
                })
                .then(function (data) {
                    if (data.length === 0) {
                        alert('Выберите хотя бы один элемент');
                        return;
                    } else {
                        ctrl.itemsSelected = data;
                        ctrl.getProductsByIds(data);
                        ctrl.closeModal(modalId);
                    }
                });
        };

        ctrl.getProductsByCategory = function (categoryId) {
            return $http.get('landinginplace/ProductsForGrid', { params: { categoryId: categoryId } }).then(function (response) {
                return response.data;
            });
        };
    };

    ng.module('blocksConstructor').controller('SubBlockProductsViewCtrl', SubBlockProductsViewCtrl);

    SubBlockProductsViewCtrl.$inject = [
        'blocksConstructorService',
        'uiGridConstants',
        'uiGridCustomConfig',
        '$q',
        '$http',
        'modalService',
        'toaster',
    ];
})(window.angular);
