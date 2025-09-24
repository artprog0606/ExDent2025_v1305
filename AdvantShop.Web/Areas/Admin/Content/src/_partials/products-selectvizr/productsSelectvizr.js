import excludedExportProductsTemplate from './../../_shared/modal/excludedExportProducts/excludedExportProducts.html';
(function (ng) {
    'use strict';

    var ProductsSelectvizrCtrl = /* @ngInject*/ function ($q, $http, $uibModal, $scope, $transclude, $attrs) {
        var ctrl = this;
        var deferGrid;
        ctrl.ids = [];
        ctrl.$onInit = function () {
            ctrl.property = ctrl.selectvizrProperty || 'ProductId';
            ctrl.customTemplates = {};
            ctrl.selectvizrGridParams = ctrl.selectvizrGridParams || {};
            ctrl.selectvizrGridParams.categoryId = ctrl.selectvizrGridParams.categoryId || 0;
            ctrl.selectvizrGridParams.showMethod = ctrl.selectvizrGridParams.showMethod || 'AllProducts';
            ctrl.selectvizrTreeSearch = ctrl.selectvizrTreeSearch || {
                ajax: {
                    url: 'catalog/categoriesTreeBySearchRequest',
                },
            };
            $transclude($scope, function (nodes) {
                let templateName;
                Array.from(nodes).forEach(function (el) {
                    if (el instanceof Element && el.matches('script[type="text/ng-template"]')) {
                        templateName = el.getAttribute('data-template-name');
                        if (templateName == null || templateName.length === 0) {
                            throw new Error('Not set attribute "data-template-name" for tag script inside component "products-selectvizr"');
                        }
                        ctrl.customTemplates[templateName] = el.innerHTML;
                    }
                });
            });
        };
        ctrl.treeCallbacks = {
            //onLoadedJstree: function () {
            //    if (ctrl.selectvizrTreeItemsSelected != null) {
            //        ctrl.a = ctrl.jstree.select_node(ctrl.selectvizrTreeItemsSelected);
            //    }
            //},

            select_node: function (event, data) {
                data.instance.open_node(data.node);
                ctrl.selectvizrGridParams.categoryId = data.node.id;
                ctrl.selectvizrGridParams.Page = 1;
                if (ctrl.selectvizrGridParams.categoryId == 0) {
                    ctrl.selectvizrGridParams.showMethod = 'AllProducts';
                } else {
                    ctrl.selectvizrGridParams.showMethod = null;
                }
                ctrl.selectvizrGridParams.Page = 1;
                ctrl.getGrid().then(function () {
                    ctrl.grid.setParams(ctrl.selectvizrGridParams);
                    ctrl.grid.fetchData();
                    if (ctrl.selectvizrOnChange != null) {
                        ctrl.selectvizrOnChange({
                            categoryId: ctrl.selectvizrGridParams.categoryId,
                            ids: ctrl.selectionCustom.getSelectedParams(ctrl.property).ids,
                        });
                    }
                });
            },
        };
        ctrl.jstreeOnInit = function (jstree) {
            ctrl.jstree = jstree;
        };
        ctrl.getGrid = function () {
            if (ctrl.grid != null) {
                return $q.when(ctrl.grid);
            } else {
                deferGrid = $q.defer();
                return deferGrid.promise;
            }
        };
        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
            if (ctrl.selectvizrOnInit != null) {
                ctrl.selectvizrOnInit({
                    grid: grid,
                });
            }
            if (deferGrid != null) {
                deferGrid.resolve(grid);
            }
        };
        ctrl.gridSelectionOnInit = function (selectionCustom) {
            ctrl.selectionCustom = selectionCustom;
            ctrl.selectvizrOnChange(
                ng.extend(
                    {
                        categoryId: ctrl.selectvizrGridParams.categoryId,
                    },
                    {
                        gridParams: ctrl.selectionCustom.getSelectedParams(ctrl.property),
                    },
                ),
            );
        };
        ctrl.gridSelectionOnChange = function (rows) {
            if (ctrl.selectvizrOnChange != null && ctrl.selectionCustom != null) {
                ctrl.selectvizrOnChange(
                    ng.extend(
                        {
                            categoryId: ctrl.selectvizrGridParams.categoryId /*ids: ctrl.ids*/,
                        },
                        {
                            gridParams: ctrl.selectionCustom.getSelectedParams(ctrl.property),
                        },
                        {
                            rows: rows,
                        },
                    ),
                );
            }
        };
        ctrl.changeExcludeListProducts = function (isExclude) {
            if (!ctrl.listArt) return;
            var listArtNo = ctrl.listArt.split('\n').filter(Boolean);
            if (listArtNo == null) return;
            var url = isExclude ? 'exportFeeds/excludeListProducts' : 'exportFeeds/includeListProducts';
            ctrl.isProgress = true;
            $http
                .post(url, {
                    exportId: ctrl.selectvizrGridParams.exportFeedId,
                    listArtNo: listArtNo,
                })
                .then(function (response) {
                    var result = response.data.obj;
                    ctrl.openModalExcludedExportProducts(result, listArtNo.length - result.length, isExclude);
                    ctrl.showTextArea = false;
                    ctrl.isProgress = false;
                    ctrl.grid.fetchData();
                });
        };
        ctrl.gridItemsSelectedFilterFn = function (rowEntity) {
            var result = false;
            if (ctrl.selectvizrGridItemsSelected != null && ctrl.selectvizrGridItemsSelected.length > 0) {
                for (var i = 0, len = ctrl.selectvizrGridItemsSelected.length; i < len; i++) {
                    if (rowEntity.ProductId === ctrl.selectvizrGridItemsSelected[i]) {
                        ctrl.selectvizrGridItemsSelected.splice(i, 1);
                        result = true;
                        break;
                    }
                }
            }
            return result;
        };
        ctrl.openModalExcludedExportProducts = function (listMissingArtNo, countExcluded, isExclude) {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalExcludedExportProductsCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: excludedExportProductsTemplate,
                    resolve: {
                        params: {
                            listMissingArtNo: listMissingArtNo,
                            countExcludedArtNo: countExcluded,
                            isExclude: isExclude,
                        },
                    },
                    size: 'xs-6',
                })
                .result.then(
                    function (result) {
                        ctrl.grid.fetchData();
                        return result;
                    },
                    function (result) {
                        return result;
                    },
                );
        };
        ctrl.onSelectCategory = function (result) {
            if (result != null) {
                var category = result.categoryIds[0];
                updateOnSelectCategory(category.categoryId);
                ctrl.categoryNameSelected = category.name;
            }
        };
        function updateOnSelectCategory(categoryId) {
            ctrl.selectvizrGridParams.categoryId = categoryId;
            if (ctrl.selectvizrGridParams.categoryId == 0) {
                ctrl.selectvizrGridParams.showMethod = 'AllProducts';
            } else {
                ctrl.selectvizrGridParams.showMethod = null;
            }
            ctrl.selectvizrGridParams.Page = 1;
            if (ctrl.grid != null) {
                ctrl.grid.setParams(ctrl.selectvizrGridParams);
                ctrl.grid.fetchData();
            }
            if (ctrl.selectvizrOnChange != null) {
                ctrl.selectvizrOnChange({
                    categoryId: ctrl.selectvizrGridParams.categoryId,
                    ids: [],
                });
            }
        }
        ctrl.gridSelectionItemsSelectedFnWrap = function (rowEntity) {
            if ($attrs.selectvizrGridSelectionItemsSelectedFn != null) {
                return ctrl.selectvizrGridSelectionItemsSelectedFn({
                    rowEntity,
                });
            } else {
                return ctrl.gridItemsSelectedFilterFn(rowEntity);
            }
        };
    };
    ng.module('productsSelectvizr', ['uiGridCustom', 'ui.grid']).controller('ProductsSelectvizrCtrl', ProductsSelectvizrCtrl);
})(window.angular);
