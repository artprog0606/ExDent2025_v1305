(function (ng) {
    'use strict';

    var ProductsByCategoryCtrl = function ($http, $httpParamSerializer, $location, urlHelper) {
        var ctrl = this;
        var pageYOffset;

        ctrl.$onInit = function () {
            ctrl.page = 1;
            ctrl.categoriesData = {};
            ctrl.isShowNotFilterItems = true;
        };

        ctrl.$postLink = function () {
            ctrl.urlParams = $location.search();
            ctrl.urlHash = $location.hash();
            var urlParamsFromHash = ctrl.urlHash.split('?');

            ctrl.urlParamsObject = urlHelper.getUrlParamsAsObject(ctrl.urlParams);
            ctrl.urlHashObject = urlHelper.getUrlParamsAsObject(urlParamsFromHash.join('&'));

            if (ctrl.urlParamsObject != null) {
                ctrl.categoryIdFromUrl = parseFloat(ctrl.urlParamsObject.categoryId);
            }

            if (isNaN(ctrl.categoryIdFromUrl) === true && ctrl.urlHashObject != null) {
                ctrl.categoryIdFromUrl = parseFloat(ctrl.urlHashObject.categoryId);
            }

            if (isNaN(ctrl.categoryIdFromUrl) === false) {
                ctrl.selectCategory(ctrl.categoryIdFromUrl);
            }
        };

        ctrl.catalogFilterInit = function (catalogFilter) {
            ctrl.catalogFilter = catalogFilter;
        };

        ctrl.getFilteredItems = function (isFiltered, page, data) {
            pageYOffset = window.pageYOffset;
            var currentPage =
                page != null
                    ? page
                    : ctrl.categoriesData[ctrl.categoryIdSelected] != null &&
                        ctrl.categoriesData[ctrl.categoryIdSelected].Pager != null &&
                        ctrl.categoriesData[ctrl.categoryIdSelected].Pager.CurrentPage != null
                      ? ctrl.categoriesData[ctrl.categoryIdSelected].Pager.CurrentPage + 1
                      : 2;

            if (isFiltered == null) {
                isFiltered = ctrl.isFiltered || false;
            }

            if (isFiltered) {
                ctrl.isShowNotFilterItems = false;
            }

            var params = {
                categoryId: ctrl.categoryIdSelected,
                countPerPage: ctrl.countPerPage,
                page: currentPage,
            };
            params = ng.extend(params, data);

            $http.post('landing/landing/getProductsByCategory', params).then(function (response) {
                if (ctrl.categoriesData[ctrl.categoryIdSelected] == null || ctrl.categoriesData[ctrl.categoryIdSelected].ProductsModel == null) {
                    ctrl.categoriesData[ctrl.categoryIdSelected] = response.data;
                } else {
                    ctrl.categoriesData[ctrl.categoryIdSelected].Pager = response.data.Pager;
                    ctrl.categoriesData[ctrl.categoryIdSelected].ProductsModel.Products = isFiltered
                        ? response.data.ProductsModel.Products
                        : ctrl.categoriesData[ctrl.categoryIdSelected].ProductsModel.Products.concat(response.data.ProductsModel.Products);
                }

                ctrl.isFiltered = isFiltered;
            });
        };

        ctrl.filter = function () {
            ctrl.getFilteredItems(true, 0, ctrl.catalogFilter.filterSelectedData);
        };

        ctrl.selectCategory = function (categoryId) {
            ctrl.categoriesData[ctrl.categoryIdSelected] = null;

            ctrl.categoryIdSelected = categoryId;
            ctrl.isFiltered = false;
            ctrl.isShowNotFilterItems = true;
            if (ctrl.catalogFilter != null) {
                ctrl.catalogFilter.init();
            }
        };

        // старый метод, для совместимости
        ctrl.getItems = function (categoryId, countPerPage, page) {
            pageYOffset = window.pageYOffset;
            $http
                .get('landing/landing/getProductsByCategory', {
                    params: {
                        categoryId: categoryId,
                        countPerPage: countPerPage,
                        page: page,
                    },
                })
                .then(function (response) {
                    if (ctrl.categoriesData[categoryId] == null) {
                        ctrl.categoriesData[categoryId] = response.data;
                    } else {
                        ctrl.categoriesData[categoryId].Pager = response.data.Pager;
                        ctrl.categoriesData[categoryId].ProductsModel.Products = ctrl.categoriesData[categoryId].ProductsModel.Products.concat(
                            response.data.ProductsModel.Products,
                        );
                    }
                });
        };

        ctrl.getLabels = function (product) {
            var params = {
                recommended: product.Recomend,
                sales: product.Sales,
                best: product.Bestseller,
                news: product.New,
                discountPercent: product.TotalDiscount.Percent,
                discountAmount: product.TotalDiscount.Amount,
                discountType: product.TotalDiscount.Type,
                discountHasValue: product.TotalDiscount.HasValue,
            };

            return 'landing/landing/getProductLabels?' + $httpParamSerializer(params);
        };

        ctrl.setScrollAfterShowMore = function () {
            window.scroll(window.pageXOffset, pageYOffset === window.pageYOffset ? pageYOffset + window.pageYOffset - pageYOffset : pageYOffset);
        };
    };

    ng.module('productsByCategory').controller('ProductsByCategoryCtrl', ProductsByCategoryCtrl);

    ProductsByCategoryCtrl.$inject = ['$http', '$httpParamSerializer', '$location', 'urlHelper'];
})(window.angular);
