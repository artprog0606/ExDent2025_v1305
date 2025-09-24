import commonTemplate from './common.html';
(function (ng) {
    'use strict';

    var AnalyticsCommonCtrl = function ($http) {
        var ctrl = this;
        ctrl.$onInit = function () {
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    analyticsCommon: ctrl,
                });
            }
        };
        ctrl.recalc = function (dateFrom, dateTo, paid, status) {
            ctrl.fetch(dateFrom, dateTo, paid, status);
        };
        ctrl.fetch = function (dateFrom, dateTo, paid, status) {
            ctrl.paid = paid;
            ctrl.status = status;
            $http
                .get('bookingAnalytics/getCommon', {
                    params: {
                        dateFrom: dateFrom,
                        dateTo: dateTo,
                        isPaid: paid,
                        status: status,
                        affiliateId: ctrl.affiliateId,
                    },
                })
                .then(function (result) {
                    ctrl.Data = result.data;
                });
        };
        ctrl.showBookings = function (params) {
            if (ctrl.showBookingsFn) {
                ctrl.showBookingsFn({
                    params: params,
                });
            }
        };
        ctrl.showAllBookings = function () {
            if (ctrl.isShowAllBookings()) {
                ctrl.showBookings({
                    nostatus: '3',
                });
            }
        };
        ctrl.showPaidBookings = function () {
            if (ctrl.isShowPaidBookings()) {
                ctrl.showBookings({
                    nostatus: '3',
                    paid: true,
                });
            }
        };
        ctrl.showCancelledBookings = function () {
            if (ctrl.isShowCancelledBookings()) {
                ctrl.showBookings({
                    status: '3',
                });
            }
        };
        ctrl.showCancelledAndPaidBookings = function () {
            if (ctrl.isShowCancelledAndPaidBookings()) {
                ctrl.showBookings({
                    status: '3',
                    paid: true,
                });
            }
        };
        ctrl.isShowAllBookings = function () {
            return ctrl.status != 3;
        };
        ctrl.isShowPaidBookings = function () {
            return ctrl.status != 3 && (!ctrl.paid || ctrl.paid === 'null' || ctrl.paid == true);
        };
        ctrl.isShowCancelledBookings = function () {
            return !ctrl.status || ctrl.status === 'null' || ctrl.status == 3;
        };
        ctrl.isShowCancelledAndPaidBookings = function () {
            return (!ctrl.status || ctrl.status === 'null' || ctrl.status == 3) && (!ctrl.paid || ctrl.paid === 'null' || ctrl.paid == true);
        };
    };
    AnalyticsCommonCtrl.$inject = ['$http'];
    ng.module('bookingAnalytics')
        .controller('AnalyticsCommonCtrl', AnalyticsCommonCtrl)
        .component('bookingAnalyticsCommon', {
            templateUrl: commonTemplate,
            controller: AnalyticsCommonCtrl,
            bindings: {
                onInit: '&',
                affiliateId: '<?',
                showBookingsFn: '&',
            },
        });
})(window.angular);
