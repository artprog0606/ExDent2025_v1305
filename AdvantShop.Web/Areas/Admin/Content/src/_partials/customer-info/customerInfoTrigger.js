(function (ng) {
    'use strict';

    var CustomerInfoTriggerCtrl = function (customerInfoService) {
        var ctrl = this;

        ctrl.openByTrigger = function () {
            customerInfoService.addInstance({ customerId: ctrl.customerId, partnerId: ctrl.partnerId }, { onClose: ctrl.onClose });
        };
    };

    CustomerInfoTriggerCtrl.$inject = ['customerInfoService'];

    ng.module('customerInfo').controller('CustomerInfoTriggerCtrl', CustomerInfoTriggerCtrl);
})(window.angular);
