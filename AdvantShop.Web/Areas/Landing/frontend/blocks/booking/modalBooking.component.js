(function (ng) {
    'use strict';
    ng.module('modalBooking').component('modalBooking', {
        controller: 'ModalBookingCtrl',
        controllerAs: 'modalBooking',
        bindToController: true,
        bindings: {
            blockId: '<?',
            resourceId: '<?',
            affiliateId: '<?',
            bookingByDays: '<?',
            timeFrom: '<?',
            timeEnd: '<?',
            timeEndAtNextDay: '<?',
            showServices: '<?',
            colorScheme: '@',
            yaMetrikaEventName: '@',
            gaEventCategory: '@',
            gaEventAction: '@',
            lid: '@',
            lpId: '<?',
        },
    });
})(window.angular);
