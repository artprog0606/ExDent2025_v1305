(function (ng) {
    'use strict';

    var ModalTBankRegistrationCtrl = function ($uibModalInstance, $sce) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.advId = ctrl.$resolve ? ctrl.$resolve.params.advId : null;
            var scriptPath = ctrl.$resolve ? ctrl.$resolve.params.scriptPath : null;
            ctrl.scriptPath = $sce.trustAsResourceUrl(scriptPath);
            const scriptTag = document.createElement('script');
            scriptTag.type = 'module';
            scriptTag.src = ctrl.scriptPath;
            document.body.appendChild(scriptTag);
        };

        ctrl.close = function () {
            if (ctrl.shopId !== undefined) {
                $uibModalInstance.close();
            } else {
                $uibModalInstance.dismiss('cancel');
            }
        };
    };

    ModalTBankRegistrationCtrl.$inject = ['$uibModalInstance', '$sce'];

    ng.module('uiModal').controller('ModalTBankRegistrationCtrl', ModalTBankRegistrationCtrl);
})(window.angular);
