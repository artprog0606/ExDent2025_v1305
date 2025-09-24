(function (ng) {
    'use strict';

    var ModalExportCustomersCtrl = function ($uibModalInstance) {
        var ctrl = this;

        ctrl.$onInit = function () {};

        ctrl.close = function (notCheck) {
            $uibModalInstance.dismiss({ notCheck: notCheck });
        };
    };

    ModalExportCustomersCtrl.$inject = ['$uibModalInstance'];

    ng.module('uiModal').controller('ModalExportCustomersCtrl', ModalExportCustomersCtrl);
})(window.angular);
