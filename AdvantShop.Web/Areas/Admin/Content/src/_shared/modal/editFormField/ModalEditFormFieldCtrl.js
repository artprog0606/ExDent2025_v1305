import './editFormField.html';

(function (ng) {
    'use strict';
    var increment = 1;
    var ModalEditFormFieldCtrl = function ($uibModalInstance) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.params = ctrl.$resolve != null ? ctrl.$resolve.params : null;
            //deep clone object
            if (ctrl.params != null) {
                try {
                    ctrl.params = JSON.parse(JSON.stringify(ctrl.params));
                } catch (e) {
                    console.error(e);
                }
            }

            ctrl.formName = 'modalEditFormField' + increment;
            increment++;
        };

        ctrl.save = function (ctrl) {
            var form = ctrl.form;
            $uibModalInstance.close(form);
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalEditFormFieldCtrl.$inject = ['$uibModalInstance'];

    ng.module('uiModal').controller('ModalEditFormFieldCtrl', ModalEditFormFieldCtrl);
})(window.angular);
