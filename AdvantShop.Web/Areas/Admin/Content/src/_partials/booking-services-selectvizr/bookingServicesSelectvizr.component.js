import bookingServicesSelectvizrTemplate from './templates/booking-services-selectvizr.html';
(function (ng) {
    'use strict';

    ng.module('bookingServicesSelectvizr').component('bookingServicesSelectvizr', {
        templateUrl: bookingServicesSelectvizrTemplate,
        controller: 'BookingServicesSelectvizrCtrl',
        transclude: true,
        bindings: {
            selectvizrTreeUrl: '<',
            selectvizrGridUrl: '<',
            selectvizrGridOptions: '<',
            selectvizrGridParams: '<?',
            selectvizrGridItemsSelected: '<?',
            selectvizrOnChange: '&',
        },
    });
})(window.angular);
