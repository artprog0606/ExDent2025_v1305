(function (ng) {
    'use strict';

    var ModalExportOrdersCtrl = function ($uibModalInstance) {
        var ctrl = this;

        ctrl.$onInit = function () {};

        ctrl.close = function (notCheck) {
            $uibModalInstance.dismiss({ notCheck: notCheck });
        };
    };

    ModalExportOrdersCtrl.$inject = ['$uibModalInstance'];

    ng.module('uiModal').controller('ModalExportOrdersCtrl', ModalExportOrdersCtrl);
})(window.angular);
