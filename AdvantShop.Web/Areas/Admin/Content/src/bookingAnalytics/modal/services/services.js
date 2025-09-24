(function (ng) {
    'use strict';

    var ModalBookingServicesAnalyticsCtrl = function ($translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.affiliateId = params.affiliateId;
            ctrl.reservationResourceId = params.reservationResourceId;
            ctrl.dateFrom = params.dateFrom;
            ctrl.dateTo = params.dateTo;
            ctrl.paid = params.paid;
            ctrl.status = params.status;
            ctrl.noStatus = params.noStatus;
        };
    };

    ModalBookingServicesAnalyticsCtrl.$inject = ['$translate'];

    ng.module('uiModal').controller('ModalBookingServicesAnalyticsCtrl', ModalBookingServicesAnalyticsCtrl);
})(window.angular);
