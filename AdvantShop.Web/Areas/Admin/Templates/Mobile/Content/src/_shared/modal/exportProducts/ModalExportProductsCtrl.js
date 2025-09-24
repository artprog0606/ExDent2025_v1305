(function (ng) {
    'use strict';

    var ModalExportProductsCtrl = function ($uibModalInstance) {
        var ctrl = this;

        ctrl.$onInit = function () {};

        ctrl.close = function (notCheck) {
            $uibModalInstance.dismiss({ notCheck: notCheck });
        };
    };

    ModalExportProductsCtrl.$inject = ['$uibModalInstance'];

    ng.module('uiModal').controller('ModalExportProductsCtrl', ModalExportProductsCtrl);
})(window.angular);
