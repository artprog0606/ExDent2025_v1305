import moveProductInOtherCategoryTemplate from '../../src/_shared/modal/moveProductInOtherCategory/MoveProductInOtherCategory.html';
import copyProductTemplate from '../../src/product/modal/copyProduct/CopyProduct.html';
import addRemovePropertyToProductsTemplate from '../../src/_shared/modal/addRemovePropertyToProducts/addRemovePropertyToProducts.html';
import addTagsToProductsTemplate from '../../src/_shared/modal/addTagsToProducts/addTagsToProducts.html';
import removeTagsToProductsTemplate from '../../src/_shared/modal/removeTagsToProducts/removeTagsToProducts.html';

(function (ng) {
    'use strict';

    /* @ngInject */
    var CatalogCtrl = function (
        $q,
        $filter,
        $http,
        $location,
        $window,
        uiGridConstants,
        uiGridCustomConfig,
        uiGridCustomParamsConfig,
        uiGridCustomService,
        urlHelper,
        catalogService,
        SweetAlert,
        toaster,
        $translate,
        sidebarMenuService,
    ) {
        var ctrl = this,
            showMethod = urlHelper.getUrlParam('showMethod'),
            showAdvancedFilter = urlHelper.getUrlParam('showAdvancedFilter'),
            columnDefs = [
                {
                    name: '_noopColumnArtNo',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Catalog.VendorCode'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'ArtNo',
                    },
                },
                {
                    name: '_noopColumnName',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Catalog.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                },
                {
                    name: 'ProductArtNo',
                    displayName: $translate.instant('Admin.Js.Catalog.VenCode'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><a class="link-invert word-break" ng-href="product/edit/{{row.entity.ProductId}}" title="{{COL_FIELD}}">{{COL_FIELD}}</a></div>',
                    width: 80,
                },
                {
                    name: 'PhotoSrc',
                    headerCellClass: 'ui-grid-custom-header-cell-center',
                    displayName: $translate.instant('Admin.Js.Catalog.Img'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><a class="ui-grid-custom-flex-center ui-grid-custom-link-for-img" ng-href="product/edit/{{row.entity.ProductId}}"><img class="ui-grid-custom-col-img" ng-src="{{row.entity.PhotoSrc}}"></a></div>',
                    width: 80,
                    enableSorting: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Catalog.Image'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'HasPhoto',
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.Catalog.WithPhoto'),
                                value: true,
                            },
                            {
                                label: $translate.instant('Admin.Js.Catalog.WithoutPhoto'),
                                value: false,
                            },
                        ],
                    },
                },
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.Catalog.Name'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><a ng-href="product/edit/{{row.entity.ProductId}}">{{COL_FIELD}}</a></div>',
                    //enableCellEdit: true
                },
                {
                    name: 'PriceString',
                    displayName: $translate.instant('Admin.Js.Catalog.Price'),
                    enableCellEdit: true,
                    type: 'text',
                    uiGridCustomEdit: {
                        attributes: {
                            'input-ghost': '""',
                            'ng-pattern': 'uiGridEditCustom.pattern',
                        },
                        customModel: 'priceEdit',
                        onInit: function (rowEntity, colDef, newValue, uiGridEditCustom) {
                            uiGridEditCustom.priceEdit = rowEntity.PriceString;
                        },
                        onActive: function (rowEntity, colDef, newValue, uiGridEditCustom) {
                            uiGridEditCustom.priceEdit = $filter('number')(rowEntity.Price);
                            uiGridEditCustom.pattern = '^[\\d\\.,\\s]*$';
                        },
                        onDeactive: function (rowEntity, colDef, newValue, uiGridEditCustom) {
                            uiGridEditCustom.priceEdit = rowEntity.PriceString;
                            uiGridEditCustom.pattern = null;
                        },
                        onChange: function (rowEntity, colDef, newValue, uiGridEditCustom) {
                            rowEntity.Price = newValue;
                            uiGridEditCustom.pattern = null;
                        },
                    },
                    width: 100,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Catalog.Price'),
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
                    cellEditableCondition: function ($scope) {
                        return $scope.row.entity.OffersCount === 1;
                    },
                },
                {
                    name: 'Amount',
                    displayName: $translate.instant('Admin.Js.Catalog.Quant'),
                    enableCellEdit: true,
                    type: 'number',
                    width: 85,
                    cellEditableCondition: function ($scope) {
                        return (
                            $scope.row.entity.OffersCount === 1 &&
                            ($scope.row.entity.UsedWarehouses === 1 ||
                                ($scope.row.entity.UsedWarehouses === 0 && $scope.row.entity.CountWarehouses === 1))
                        );
                    },
                    uiGridCustomEdit: {
                        attributes: {
                            min: 0,
                            max: 1000000,
                        },
                    },
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Catalog.Quantity'),
                        type: 'range',
                        rangeOptions: {
                            from: {
                                name: 'AmountFrom',
                                min: 0,
                                max: 1000000,
                            },
                            to: {
                                name: 'AmountTo',
                                min: 0,
                                max: 1000000,
                            },
                        },
                        fetch: 'catalog/getamountrangeforpaging',
                    },
                },
            ];

        if (showMethod == null || showMethod.toLowerCase() === 'normal') {
            columnDefs.push({
                name: 'SortOrder',
                displayName: $translate.instant('Admin.Js.Catalog.Order'),
                type: 'number',
                width: 75,
                enableCellEdit: true,
                filter: {
                    placeholder: $translate.instant('Admin.Js.Catalog.Sorting'),
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
            });
        }

        columnDefs = columnDefs.concat([
            {
                name: 'Enabled',
                displayName: $translate.instant('Admin.Js.Catalog.Activ'),
                enableCellEdit: false,
                cellTemplate: '<ui-grid-custom-switch row="row"></ui-grid-custom-switch>',
                width: 76,
                filter: {
                    placeholder: $translate.instant('Admin.Js.Catalog.Activity'),
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: [
                        {
                            label: $translate.instant('Admin.Js.Catalog.TheyActive'),
                            value: true,
                        },
                        {
                            label: $translate.instant('Admin.Js.Catalog.Inactive'),
                            value: false,
                        },
                    ],
                },
                visible: {
                    breakpoint: 1383,
                    customFn: function (isMatchScreen) {
                        return isMatchScreen || sidebarMenuService.getState() === true;
                    },
                },
            },
            {
                visible: false,
                name: 'BrandId',
                filter: {
                    placeholder: $translate.instant('Admin.Js.Catalog.Manufacturer'),
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
                    placeholder: $translate.instant('Admin.Js.Catalog.Color'),
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
                    placeholder: $translate.instant('Admin.Js.Catalog.Size'),
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
                    placeholder: $translate.instant('Admin.Js.Catalog.Property'),
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
                    placeholder: $translate.instant('Admin.Js.Catalog.PropertyValue'),
                    type: uiGridConstants.filter.SELECT,
                    name: 'PropertyValueId',
                    fetch: 'catalog/GetPropertyValueList',
                    dynamicSearch: true,
                    dynamicSearchRelations: ['PropertyId'],
                },
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 45,
                enableSorting: false,
                useInSwipeBlock: true,
                cellTemplate: uiGridCustomService.getTemplateCellDelete('catalog/deleteproduct', '{ProductId: row.entity.ProductId}'),
            },
            {
                name: '_noopColumnTag',
                visible: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.Catalog.Tag'),
                    name: 'Tags',
                    type: 'selectMultiple',
                    fetch: 'tags/getTagsSelectOptions',
                },
            },
            {
                name: '_noopColumnDiscount',
                displayName: $translate.instant('Admin.Js.Catalog.Discount'),
                type: 'number',
                visible: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.Catalog.Discount'),
                    type: 'range',
                    rangeOptions: {
                        from: {
                            name: 'DiscountFrom',
                        },
                        to: {
                            name: 'DiscountTo',
                        },
                    },
                },
            },
            {
                name: '_noopColumnDiscountAmount',
                displayName: $translate.instant('Admin.Js.Catalog.DiscountAmount'),
                type: 'number',
                visible: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.Catalog.DiscountAmount'),
                    type: 'range',
                    rangeOptions: {
                        from: {
                            name: 'DiscountAmountFrom',
                        },
                        to: {
                            name: 'DiscountAmountTo',
                        },
                    },
                },
            },
            {
                name: '_noopColumnCreatedBy',
                visible: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.Catalog.CreatedBy'),
                    name: 'CreatedByList',
                    type: 'selectMultiple',
                    fetch: 'product/getCreatedBySelectOptions',
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
        ]);

        ctrl.categories = [];

        ctrl.catalogInit = function (isTagsVisible, isWarehouseFilterVisible) {
            ctrl.isTagsVisible = isTagsVisible;
            ctrl.isWarehouseFilterVisible = isWarehouseFilterVisible;

            if (ctrl.isWarehouseFilterVisible) {
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

            ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: columnDefs,
                uiGridCustom: {
                    rowUrl: 'product/edit/{{row.entity.ProductId}}',
                    selectionOptions: [
                        {
                            text: $translate.instant('Admin.Js.Catalog.DeleteSelected'),
                            url: 'catalog/deleteproducts',
                            field: 'ProductId',
                            before: function () {
                                return SweetAlert.confirm($translate.instant('Admin.Js.Catalog.AreYouSureDelete'), {
                                    title: $translate.instant('Admin.Js.Catalog.Deleting'),
                                }).then(function (result) {
                                    return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                                });
                            },
                        },
                        {
                            hide: showMethod === 'OnlyWithoutCategories',
                            text: $translate.instant('Admin.Js.Catalog.DeleteSelectedFromCategory'),
                            url: 'catalog/deletefromcategoryproducts',
                            field: 'ProductId',
                            before: function () {
                                return SweetAlert.confirm($translate.instant('Admin.Js.Catalog.AreYouSureDelete'), {
                                    title: $translate.instant('Admin.Js.Catalog.Deleting'),
                                }).then(function (result) {
                                    return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                                });
                            },
                        },
                        {
                            template:
                                '<ui-modal-trigger data-on-close="$ctrl.gridActionWithCallback($ctrl.clearStorage);" data-controller="\'ModalMoveProductInOtherCategoryCtrl\'" ' +
                                'data-resolve="{params: $ctrl.getSelectedParams(\'ProductId\'), removeFromCurrentCategories: true }" template-url="' +
                                moveProductInOtherCategoryTemplate +
                                '">' +
                                $translate.instant('Admin.Js.Catalog.MoveItemToAnotherCategory') +
                                '</ui-modal-trigger>',
                        },
                        {
                            template:
                                '<ui-modal-trigger data-on-close="$ctrl.gridOnAction()" data-controller="\'ModalMoveProductInOtherCategoryCtrl\'" controller-as="ctrl" ' +
                                'data-resolve="{params:$ctrl.getSelectedParams(\'ProductId\')}" template-url="' +
                                moveProductInOtherCategoryTemplate +
                                '">' +
                                $translate.instant('Admin.Js.Catalog.AddProductsAnotherCategory') +
                                '</ui-modal-trigger>',
                        },
                        //24.10.18 функционал скрыт, не доделан вывод товаров в клиентке по наличию в каналах
                        //{
                        //    template: '<ui-modal-trigger data-on-close="$ctrl.gridOnAction()" data-controller="\'ModalSalesChannelExcludedCtrl\'" controller-as="ctrl" ' +
                        //        'data-resolve=\"{data: $ctrl.getSelectedParams(\'ProductId\')}\" template-url="../areas/admin/content/src/_shared/modal/salesChannelExcluded/SalesChannelExcluded.html">' +
                        //        $translate.instant('Admin.Js.Catalog.SalesChannelEnable') + '</ui-modal-trigger>'
                        //},
                        {
                            text: $translate.instant('Admin.Js.Catalog.MakeActive'),
                            url: 'catalog/activateproducts',
                            field: 'ProductId',
                        },
                        {
                            text: $translate.instant('Admin.Js.Catalog.MakeInactive'),
                            url: 'catalog/disableproducts',
                            field: 'ProductId',
                        },
                        {
                            template:
                                '<ui-modal-trigger data-on-close="$ctrl.gridOnAction()" data-controller="\'ModalCopyProductCtrl\'" controller-as="ctrl" ' +
                                "data-resolve=\"{product:$ctrl.getSelectedParams('ProductId'), name: $ctrl.getSelectedParams('Name')}\" template-url=\"" +
                                copyProductTemplate +
                                '">' +
                                $translate.instant('Admin.Js.Catalog.CreateCopyOfProduct') +
                                '</ui-modal-trigger>',
                        },
                        {
                            template:
                                '<ui-modal-trigger data-on-close="$ctrl.gridOnAction()" data-controller="\'ModalAddRemovePropertyToProductsCtrl\'" controller-as="ctrl" ' +
                                'data-resolve="{params:$ctrl.getSelectedParams(\'ProductId\'), mode:{remove: false}}" template-url="' +
                                addRemovePropertyToProductsTemplate +
                                '" size="md">' +
                                $translate.instant('Admin.Js.Catalog.AddPropertyToProducts') +
                                '</ui-modal-trigger>',
                        },
                        {
                            template:
                                '<ui-modal-trigger data-on-close="$ctrl.gridOnAction()" data-controller="\'ModalAddRemovePropertyToProductsCtrl\'" controller-as="ctrl" ' +
                                'data-resolve="{params:$ctrl.getSelectedParams(\'ProductId\'), mode:{remove: true}}" template-url="' +
                                addRemovePropertyToProductsTemplate +
                                '" size="md">' +
                                $translate.instant('Admin.Js.Catalog.RemovePropertyFromProducts') +
                                '</ui-modal-trigger>',
                        },
                        {
                            template:
                                '<ui-modal-trigger data-on-close="$ctrl.gridOnAction()" data-controller="\'ModalAddTagsToProductsCtrl\'" controller-as="ctrl" ' +
                                'data-resolve="{params:$ctrl.getSelectedParams(\'ProductId\')}" template-url="' +
                                addTagsToProductsTemplate +
                                '" size="md">' +
                                $translate.instant('Admin.Js.Catalog.AddTagsToProducts') +
                                '</ui-modal-trigger>',
                            hide: ctrl.isTagsVisible !== true,
                        },
                        {
                            template:
                                '<ui-modal-trigger data-on-close="$ctrl.gridOnAction()" data-controller="\'ModalRemoveTagsToProductsCtrl\'" controller-as="ctrl" ' +
                                'data-resolve="{params:$ctrl.getSelectedParams(\'ProductId\')}" template-url="' +
                                removeTagsToProductsTemplate +
                                '" size="md">' +
                                $translate.instant('Admin.Js.Catalog.RemoveTagsToProducts') +
                                '</ui-modal-trigger>',
                            hide: ctrl.isTagsVisible !== true,
                        },
                    ],
                },
            });
        };

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;

            var matchMedia = $window.matchMedia('(min-width:1383px)');
            sidebarMenuService.addCallback(function (isCompact) {
                grid.toggleVisibleColumn('Enabled', matchMedia.matches === true || isCompact === true);
            });
        };

        ctrl.deleteCategory = function (categoryId) {
            SweetAlert.confirm($translate.instant('Admin.Js.Catalog.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Catalog.Deleting'),
            }).then(function (result) {
                if (result.value === true) {
                    $http.post('category/delete', { id: categoryId }).then(function (response) {
                        if (response.data.result === true) {
                            if (response.data.needRedirect) {
                                window.location = 'catalog?categoryid=' + response.data.id;
                            }
                        } else {
                            toaster.pop('error', $translate.instant('Admin.Js.Catalog.ErrorWhileDeleting'), '');
                        }
                    });
                    $q.resolve('sweetAlertConfirm');
                } else {
                    $q.reject('sweetAlertCancel');
                }
            });
        };

        ctrl.initCatalogTreeview = function (jstree) {
            ctrl.jstree = jstree;
        };

        ctrl.initCatalogLeftMenu = function (catalogLeftMenu) {
            ctrl.catalogLeftMenu = catalogLeftMenu;
        };

        ctrl.onGridDeleteItem = function () {
            if (ctrl.jstree) {
                ctrl.jstree.refresh(true, true);
                ctrl.catalogLeftMenu.updateData();
            }
        };

        ctrl.onDeleteChildCategories = function () {
            if (ctrl.jstree) {
                ctrl.jstree.refresh(true, true);
                ctrl.catalogLeftMenu.updateData();
            }
        };
    };

    ng.module('catalog', ['uiGridCustom', 'categoriesBlock', 'urlHelper', 'checklist-model', 'sidebarMenu', 'uiModal']).controller(
        'CatalogCtrl',
        CatalogCtrl,
    );
})(window.angular);
