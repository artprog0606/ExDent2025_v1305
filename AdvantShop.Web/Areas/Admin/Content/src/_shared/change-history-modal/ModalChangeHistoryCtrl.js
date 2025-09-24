(function (ng) {
    'use strict';

    var ModalChangeHistoryCtrl = function ($uibModalInstance) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.objId = params.objId != null ? params.objId : 0;
            ctrl.type = params.type != null ? params.type : null;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalChangeHistoryCtrl.$inject = ['$uibModalInstance'];

    ng.module('uiModal').controller('ModalChangeHistoryCtrl', ModalChangeHistoryCtrl);
})(window.angular);
