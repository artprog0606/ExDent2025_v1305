(function (ng) {
    'use strict';

    var ModalAddCategoryListCtrl = function ($uibModalInstance, $http, $window, toaster, urlHelper, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.categoryId = ctrl.$resolve != null && ctrl.$resolve.data != null ? ctrl.$resolve.data.categoryId : null;

            if (ctrl.categoryId != null) {
                ctrl.categoryName = ctrl.$resolve.data.categoryName;
            } else {
                var categoryId = urlHelper.getUrlParam('categoryid');

                ctrl.categoryId = categoryId === null || categoryId === '' ? 0 : categoryId;

                $http.get('category/getCategoryForList', { params: { categoryId: ctrl.categoryId } }).then(function (response) {
                    if (response.data != null && response.data.category != null) {
                        ctrl.categoryId = response.data.category.CategoryId;
                        ctrl.categoryName = response.data.category.Name;
                    }
                });
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
            if (ctrl.categoryId == null) {
                toaster.pop('error', '', $translate.instant('Admin.Js.AddCategoryList.SelectACategory'));
                return;
            }

            if (ctrl.categories == null || ctrl.categories === '' || ctrl.inProgress === true) {
                return;
            }

            var categories = ctrl.categories.split('\n').filter(function (x) {
                return x.trim() !== '';
            });

            ctrl.inProgress = true;

            $http
                .post('category/addCategoryList', { categoryId: ctrl.categoryId, categories: categories })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        $window.location.assign('catalog?categoryId=' + ctrl.categoryId);
                    }
                })
                .catch(function () {
                    ctrl.inProgress = false;
                });
        };
    };

    ModalAddCategoryListCtrl.$inject = ['$uibModalInstance', '$http', '$window', 'toaster', 'urlHelper', '$translate'];

    ng.module('uiModal').controller('ModalAddCategoryListCtrl', ModalAddCategoryListCtrl);
})(window.angular);
