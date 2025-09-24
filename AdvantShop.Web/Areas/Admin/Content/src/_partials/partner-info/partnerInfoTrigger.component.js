(function (ng) {
    'use strict';

    ng.module('partnerInfo').component('partnerInfoTrigger', {
        controller: 'PartnerInfoTriggerCtrl',
        transclude: true,
        template: '<span class="partner-info-trigger" ng-click="$ctrl.openByTrigger()" ng-transclude></span>',
        bindings: {
            partnerId: '<',
            onClose: '&',
        },
    });
})(window.angular);
