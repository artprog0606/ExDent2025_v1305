(function (ng) {
    'use strict';

    var ModalChooseLandingFunnelCtrl = function ($uibModalInstance) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.btnChangeDisabled = true;
        };

        ctrl.choose = function () {
            $uibModalInstance.close({ id: ctrl.id });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.selectItem = function (event, data) {
            ctrl.id = data.node.id;
            ctrl.btnChangeDisabled = false;
        };
    };

    ModalChooseLandingFunnelCtrl.$inject = ['$uibModalInstance'];

    ng.module('uiModal').controller('ModalChooseLandingFunnelCtrl', ModalChooseLandingFunnelCtrl);
})(window.angular);
