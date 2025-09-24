import bookingCategoriesTreeviewTemplate from './templates/bookingCategoriesTreeview.html';
(function (ng) {
    'use strict';

    ng.module('bookingCategoriesTreeview').component('bookingCategoriesTreeview', {
        templateUrl: bookingCategoriesTreeviewTemplate,
        controller: 'BookingCategoriesTreeviewCtrl',
        bindings: {
            categoryIdSelected: '@',
            affiliateId: '@',
            onInit: '&',
        },
    });
})(window.angular);
