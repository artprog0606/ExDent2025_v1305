(function (ng) {
    'use strict';

    var ModalAddProductListCtrl = function ($uibModalInstance, $http, $window, toaster, urlHelper, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.categoryId = ctrl.$resolve != null && ctrl.$resolve.data != null ? ctrl.$resolve.data.categoryId : null;

            if (ctrl.categoryId != null) {
                ctrl.categoryName = ctrl.$resolve.data.categoryName;
            } else {
                var categoryId = urlHelper.getUrlParam('categoryid');

                if (categoryId !== null && categoryId !== '') {
                    ctrl.categoryId = categoryId;

                    $http.get('category/getCategoryForList', { params: { categoryId: categoryId } }).then(function (response) {
                        if (response.data != null && response.data.category != null) {
                            ctrl.categoryId = response.data.category.CategoryId;
                            ctrl.categoryName = response.data.category.Name;
                        }
                    });
                }
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.changeCategory = function (result) {
            ctrl.categoryId = result.categoryId;
            ctrl.categoryName = result.categoryName;
        };

        ctrl.save = function () {
            if (ctrl.products == null || ctrl.products === '' || ctrl.inProgress === true) return;

            if (ctrl.categoryId == null) {
                toaster.pop('error', '', $translate.instant('Admin.Js.AddProductList.SelectACategory'));
                return;
            }

            ctrl.inProgress = true;

            var products = ctrl.products.split('\n').filter(function (x) {
                return x.trim() !== '';
            });

            $http
                .post('product/addProductList', { categoryId: ctrl.categoryId, products: products })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        $window.location.assign('catalog?categoryId=' + ctrl.categoryId);
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.AddProductList.ErrorAddingProduct'));
                    }
                })
                .catch(function () {
                    ctrl.inProgress = false;
                });
        };
    };

    ModalAddProductListCtrl.$inject = ['$uibModalInstance', '$http', '$window', 'toaster', 'urlHelper', '$translate'];

    ng.module('uiModal').controller('ModalAddProductListCtrl', ModalAddProductListCtrl);
})(window.angular);
