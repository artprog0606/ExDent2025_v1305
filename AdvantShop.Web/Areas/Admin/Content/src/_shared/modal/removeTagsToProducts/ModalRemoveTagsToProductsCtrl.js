import './removeTagsToProducts.scss';

(function (ng) {
    'use strict';

    var ModalRemoveTagsToProductsCtrl = function ($uibModalInstance, $http, toaster, $translate, $timeout) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var resolve = ctrl.$resolve;
            ctrl.params = resolve.params;
            ctrl.getProductsTags();
            ctrl.selectedTags = [];
        };

        ctrl.getProductsTags = function () {
            $http.get('catalog/GetProductsTags', { params: ctrl.params }).then(function (response) {
                ctrl.productsTags = response.data.Tags;
            });
        };

        ctrl.removeTags = function () {
            $http.post('catalog/RemoveTagsToProducts', ng.extend(ctrl.params || {}, { removeTags: ctrl.selectedTags })).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.RemoveTagsToProducts.TagsRemovedSuccessfully'));
                    $uibModalInstance.dismiss('cancel');
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.RemoveTagsToProducts.Error'));
                    ctrl.getProductsTags();
                }
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalRemoveTagsToProductsCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate', '$timeout'];

    ng.module('uiModal').controller('ModalRemoveTagsToProductsCtrl', ModalRemoveTagsToProductsCtrl);
})(window.angular);
