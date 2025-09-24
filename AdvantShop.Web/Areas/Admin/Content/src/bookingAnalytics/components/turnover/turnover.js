import turnoverTemplate from './turnover.html';
(function (ng) {
    'use strict';

    var BookingTurnoverCtrl = function ($http) {
        var ctrl = this;
        ctrl.colors = ['#ff9900', '#cc6600', '#008000', '#0066cc', '#cccccc'];
        ctrl.datasetOverride = [
            {
                type: 'line',
                fill: false,
            },
        ];
        ctrl.chartOptions = {
            title: {
                display: true,
            },
            scales: {
                x: {
                    display: true,
                },
                y: {
                    display: true,
                },
                // xAxes: [{
                //     stacked: true
                // }],
                // yAxes: [{
                //     stacked: true
                // }]
            },
        };
        ctrl.$onInit = function () {
            ctrl.groupFormatString = 'dd';
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    turnover: ctrl,
                });
            }
        };
        ctrl.recalc = function (dateFrom, dateTo, paid, status) {
            ctrl.dateFrom = dateFrom;
            ctrl.dateTo = dateTo;
            ctrl.paid = paid;
            ctrl.status = status;
            ctrl.fetchSum();
            ctrl.fetchCount();
        };
        ctrl.changeGroup = function (groupFormatString) {
            ctrl.groupFormatString = groupFormatString;
            ctrl.recalc(ctrl.dateFrom, ctrl.dateTo, ctrl.paid, ctrl.status);
        };
        ctrl.fetchSum = function () {
            $http
                .get('bookingAnalytics/getTurnover', {
                    params: {
                        type: 'sum',
                        dateFrom: ctrl.dateFrom,
                        dateTo: ctrl.dateTo,
                        isPaid: ctrl.paid,
                        status: ctrl.status,
                        groupFormatString: ctrl.groupFormatString,
                        affiliateId: ctrl.affiliateId,
                    },
                })
                .then(function (result) {
                    ctrl.TurnoverSum = result.data;
                });
        };
        ctrl.fetchCount = function () {
            $http
                .get('bookingAnalytics/getTurnover', {
                    params: {
                        type: 'count',
                        dateFrom: ctrl.dateFrom,
                        dateTo: ctrl.dateTo,
                        isPaid: ctrl.paid,
                        status: ctrl.status,
                        groupFormatString: ctrl.groupFormatString,
                        affiliateId: ctrl.affiliateId,
                    },
                })
                .then(function (result) {
                    ctrl.TurnoverCount = result.data;
                });
        };
    };
    BookingTurnoverCtrl.$inject = ['$http'];
    ng.module('bookingAnalytics')
        .controller('BookingTurnoverCtrl', BookingTurnoverCtrl)
        .component('bookingTurnover', {
            templateUrl: turnoverTemplate,
            controller: BookingTurnoverCtrl,
            bindings: {
                onInit: '&',
                affiliateId: '<?',
            },
        });
})(window.angular);
