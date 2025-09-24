import sourcesTemplate from './sources.html';
(function (ng) {
    'use strict';

    var AnalyticsSourcesCtrl = function ($http) {
        var ctrl = this;
        ctrl.$onInit = function () {
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    analyticsSources: ctrl,
                });
            }
        };
        ctrl.recalc = function (dateFrom, dateTo, paid, status) {
            ctrl.fetch(dateFrom, dateTo, paid, status);
        };
        ctrl.fetch = function (dateFrom, dateTo, paid, status) {
            $http
                .get('bookingAnalytics/getBookingSources', {
                    params: {
                        dateFrom: dateFrom,
                        dateTo: dateTo,
                        isPaid: paid,
                        status: status,
                        affiliateId: ctrl.affiliateId,
                    },
                })
                .then(function (result) {
                    ctrl.BookingSources = result.data;
                });
        };
    };
    AnalyticsSourcesCtrl.$inject = ['$http'];
    ng.module('bookingAnalytics')
        .controller('AnalyticsSourcesCtrl', AnalyticsSourcesCtrl)
        .component('bookingAnalyticsSources', {
            templateUrl: sourcesTemplate,
            controller: AnalyticsSourcesCtrl,
            bindings: {
                onInit: '&',
                affiliateId: '<?',
            },
        });
})(window.angular);
