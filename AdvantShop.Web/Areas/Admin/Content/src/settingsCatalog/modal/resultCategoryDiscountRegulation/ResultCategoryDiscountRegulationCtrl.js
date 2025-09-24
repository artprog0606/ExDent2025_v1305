(function (ng) {
    'use strict';

    var ResultCategoryDiscountRegulationCtrl = function ($uibModalInstance) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;

            ctrl.title = params.title;
            ctrl.msg = params.msg;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ResultCategoryDiscountRegulationCtrl.$inject = ['$uibModalInstance'];

    ng.module('uiModal').controller('ResultCategoryDiscountRegulationCtrl', ResultCategoryDiscountRegulationCtrl);
})(window.angular);
