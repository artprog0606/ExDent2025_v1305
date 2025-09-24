(function (ng) {
    'use strict';

    var ModalChangeParentCategoryCtrl = function ($http, $uibModalInstance) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.btnChangeDisabled = true;

            var params = ctrl.$resolve;
            ctrl.treeAjaxData = { categoryIdSelected: params.selected || 0, excludeids: params.currentId || undefined };
        };

        ctrl.change = function () {
            $uibModalInstance.close({ categoryId: ctrl.categoryId, categoryName: ctrl.categoryName });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.selectCategory = function (event, data) {
            ctrl.categoryId = data.node.id;
            ctrl.categoryName = data.node.original.name;
            ctrl.btnChangeDisabled = false;
        };
    };

    ModalChangeParentCategoryCtrl.$inject = ['$http', '$uibModalInstance'];

    ng.module('uiModal').controller('ModalChangeParentCategoryCtrl', ModalChangeParentCategoryCtrl);
})(window.angular);
