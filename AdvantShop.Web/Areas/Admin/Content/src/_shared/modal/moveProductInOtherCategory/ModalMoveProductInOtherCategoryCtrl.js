(function (ng) {
    'use strict';

    var ModalMoveProductInOtherCategoryCtrl = function ($http, $uibModalInstance, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var resolve = ctrl.$resolve;
            ctrl.params = resolve.params;
            ctrl.removeFromCurrentCategories = resolve.removeFromCurrentCategories || false;
            ctrl.btnMoveDisabled = true;
            ctrl.treeCheckbox = { three_state: false };
        };

        ctrl.move = function () {
            $http
                .post(
                    'catalog/changeproductcategory',
                    ng.extend(ctrl.params || {}, {
                        newCategoryIds: ctrl.categoryId != null ? ctrl.categoryId : ctrl.categories,
                        removeFromCurrentCategories: ctrl.removeFromCurrentCategories,
                    }),
                )
                .then(function (response) {
                    if (response.data.result === true) {
                        toaster.pop(
                            'success',
                            '',
                            ctrl.removeFromCurrentCategories
                                ? $translate.instant('Admin.Js.MoveProduct.ProductsMovedToAnotherCategory')
                                : $translate.instant('Admin.Js.MoveProduct.ProductsAddedToAnotherCategory'),
                        );
                    }

                    $uibModalInstance.close({
                        categoryId: ctrl.categoryId,
                        categories: ctrl.categories,
                        removeFromCurrentCategories: ctrl.removeFromCurrentCategories,
                    });
                });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.selectCategory = function (event, data) {
            ctrl.categoryId = data.node.id;
            ctrl.btnMoveDisabled = false;
        };

        ctrl.selectCategories = function (event, data) {
            ctrl.categories = data.selected;
            ctrl.btnMoveDisabled = false;
        };
    };

    ModalMoveProductInOtherCategoryCtrl.$inject = ['$http', '$uibModalInstance', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalMoveProductInOtherCategoryCtrl', ModalMoveProductInOtherCategoryCtrl);
})(window.angular);
