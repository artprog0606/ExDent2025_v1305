(function (ng) {
    'use strict';
    ng.module('modalBookingServices').component('modalBookingServices', {
        controller: 'ModalBookingServicesCtrl',
        controllerAs: 'modalBookingServices',
        bindToController: true,
        bindings: {
            blockId: '<',
            resourceId: '<',
            affiliateId: '<',
            showPrice: '<?',
            colorScheme: '@',
        },
    });
})(window.angular);
