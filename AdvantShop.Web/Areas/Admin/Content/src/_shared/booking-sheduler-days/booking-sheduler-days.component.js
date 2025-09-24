(function (ng) {
    'use strict';

    ng.module('bookingShedulerDays').component('bookingShedulerDays', {
        templateUrl: [
            '$element',
            '$attrs',
            function (element, attrs) {
                return attrs.templatePath || '../areas/admin/content/src/_shared/booking-sheduler-days/template/booking-sheduler-days.html';
            },
        ],
        controller: 'BookingShedulerDaysCtrl',
        bindings: {
            shedulerObj: '<',
            fetchUrl: '<?',
            extendCtrl: '<?',
            shedulerOnInit: '&',
            shedulerParams: '<?',
            uid: '@',
            emptyText: '<?',
        },
    });
})(window.angular);
