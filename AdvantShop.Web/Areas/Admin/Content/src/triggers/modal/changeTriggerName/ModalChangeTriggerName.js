(function (ng) {
    'use strict';

    var ModalChangeTriggerNameCtrl = function ($uibModalInstance, toaster, $translate, triggersService) {
        var ctrl = this;

        ctrl.triggerCtrl = ctrl.$resolve.triggerCtrl;
        ctrl.triggerName = ctrl.triggerCtrl.name;

        ctrl.saveNameTrigger = function () {
            console.log(ctrl.triggerName);
            triggersService
                .saveName(ctrl.triggerCtrl.id, ctrl.triggerName)
                .then(function () {
                    toaster.pop('success', $translate.instant('Admin.Js.Triggers.SaveNameComplete'));
                    $uibModalInstance.close(ctrl.triggerName);
                })
                .catch(function () {
                    toaster.pop('error', $translate.instant('Admin.Js.Triggers.SaveNameError'));
                });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalChangeTriggerNameCtrl.$inject = ['$uibModalInstance', 'toaster', '$translate', 'triggersService'];

    ng.module('uiModal').controller('ModalChangeTriggerNameCtrl', ModalChangeTriggerNameCtrl);
})(window.angular);
