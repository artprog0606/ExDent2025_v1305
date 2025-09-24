(function (ng) {
    'use strict';

    var LeadInfoTriggerCtrl = function (leadInfoService) {
        var ctrl = this;

        ctrl.openByTrigger = function () {
            leadInfoService.addInstance({ leadId: ctrl.leadId }, { onClose: ctrl.onClose });
        };
    };

    LeadInfoTriggerCtrl.$inject = ['leadInfoService'];

    ng.module('leadInfo').controller('LeadInfoTriggerCtrl', LeadInfoTriggerCtrl);
})(window.angular);
