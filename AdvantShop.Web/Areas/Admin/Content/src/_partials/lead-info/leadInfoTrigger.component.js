(function (ng) {
    'use strict';

    ng.module('leadInfo').component('leadInfoTrigger', {
        controller: 'LeadInfoTriggerCtrl',
        transclude: true,
        template: '<span class="lead-info-trigger" ng-click="$ctrl.openByTrigger()" ng-transclude></span>',
        bindings: {
            leadId: '<',
            onClose: '&',
        },
    });
})(window.angular);
