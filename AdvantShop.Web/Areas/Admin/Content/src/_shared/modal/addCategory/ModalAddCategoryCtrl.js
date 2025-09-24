(function (ng) {
    'use strict';

    var ModalAddCategoryCtrl = function ($uibModalInstance) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.btnChangeDisabled = true;
            var params = ctrl.$resolve;

            ctrl.treeCheckbox = { three_state: false };

            ctrl.treeAjaxData = { categoryIdSelected: params.selected || 0, excludeids: params.currentId || undefined };
        };

        ctrl.choose = function () {
            $uibModalInstance.close({ categories: ctrl.categories });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.selectCategories = function (event, data) {
            ctrl.categories = data.selected;
            ctrl.btnChangeDisabled = false;
        };
    };

    ModalAddCategoryCtrl.$inject = ['$uibModalInstance'];

    ng.module('uiModal').controller('ModalAddCategoryCtrl', ModalAddCategoryCtrl);
})(window.angular);
