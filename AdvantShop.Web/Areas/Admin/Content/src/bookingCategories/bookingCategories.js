(function (ng) {
    'use strict';

    var BookingCategoriesCtrl = function () {
        var ctrl = this;

        ctrl.getListBookingData = function (data) {
            ctrl.listBookingData = data;
        };
    };

    BookingCategoriesCtrl.$inject = [];

    ng.module('bookingCategories', ['listBookingCategories']).controller('BookingCategoriesCtrl', BookingCategoriesCtrl);
})(window.angular);
