import bookingShedulerTemplate from './template/booking-sheduler.html';

(function (ng) {
    'use strict';

    ng.module('bookingSheduler').component('bookingSheduler', {
        templateUrl: [
            '$element',
            '$attrs',
            function (element, attrs) {
                return attrs.templatePath || bookingShedulerTemplate;
            },
        ],
        controller: 'BookingShedulerCtrl',
        bindings: {
            shedulerObj: '<',
            calendarOptions: '<',
            fetchUrl: '<?',
            fetchColumnUrl: '<?',
            shedulerOnInit: '&',
            extendCtrl: '<?',
            shedulerColumnDefs: '<?',
            shedulerOnFilterInit: '&',
            shedulerParams: '<?',
            uid: '@',
            shedulerScrollable: '<?',
            shedulerDraggable: '<?',
            shedulerColumnClasses: '<?',
            shedulerRowClasses: '<?',
            shedulerColumnWrapClasses: '<?',
            slotHeightPx: '<?',
            minSlotHeightPx: '<?',
            compactView: '<?',
            emptyText: '<?',
        },
    });
})(window.angular);
