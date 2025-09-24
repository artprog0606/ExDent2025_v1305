(function (ng) {
    'use strict';

    ng.module('customerInfo').component('customerInfoTrigger', {
        controller: 'CustomerInfoTriggerCtrl',
        transclude: true,
        template: '<span class="customer-info-trigger" ng-click="$ctrl.openByTrigger()" ng-transclude></span>',
        bindings: {
            customerId: '<',
            partnerId: '<?',
            onClose: '&',
        },
    });
})(window.angular);
