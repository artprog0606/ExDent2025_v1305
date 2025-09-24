import avgcheckTemplate from './avgcheck.html';
(function (ng) {
    'use strict';

    var AvgcheckCtrl = function ($http) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.groupFormatString = 'dd';
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    avgcheck: ctrl,
                });
            }
        };
        ctrl.$onChanges = function (changes) {
            if (changes.isVisible && changes.isVisible.currentValue) {
                ctrl.recalc(ctrl.dateFrom, ctrl.dateTo, ctrl.paid, ctrl.orderStatus);
            }
        };
        ctrl.$onDestroy = function () {
            if (ctrl.onDestroy != null) {
                ctrl.onDestroy({});
            }
        };
        ctrl.recalc = function (dateFrom, dateTo, paid, orderStatus) {
            if (ctrl.isVisible === false) return;
            if (dateFrom === undefined || dateTo === undefined) {
                return;
            }

            ctrl.dateFrom = dateFrom;
            ctrl.dateTo = dateTo;
            ctrl.paid = paid;
            ctrl.orderStatus = orderStatus;
            ctrl.fetchAvgCheck();
            ctrl.fetchByCity();
        };
        ctrl.changeGroup = function (groupFormatString) {
            ctrl.groupFormatString = groupFormatString;
            ctrl.recalc(ctrl.dateFrom, ctrl.dateTo, ctrl.paid, ctrl.orderStatus);
        };
        ctrl.fetchAvgCheck = function () {
            $http
                .get('analytics/getAvgCheck', {
                    params: {
                        type: 'avg',
                        dateFrom: ctrl.dateFrom,
                        dateTo: ctrl.dateTo,
                        paid: ctrl.paid,
                        orderStatus: ctrl.orderStatus,
                        groupFormatString: ctrl.groupFormatString,
                    },
                })
                .then(function (result) {
                    ctrl.AvgData = result.data;
                });
        };
        ctrl.fetchByCity = function () {
            $http
                .get('analytics/getAvgCheck', {
                    params: {
                        type: 'city',
                        dateFrom: ctrl.dateFrom,
                        dateTo: ctrl.dateTo,
                        paid: ctrl.paid,
                        orderStatus: ctrl.orderStatus,
                    },
                })
                .then(function (result) {
                    ctrl.AvgCityData = result.data;
                });
        };
    };
    AvgcheckCtrl.$inject = ['$http'];
    ng.module('analyticsReport')
        .controller('AvgcheckCtrl', AvgcheckCtrl)
        .component('avgcheck', {
            templateUrl: avgcheckTemplate,
            controller: AvgcheckCtrl,
            bindings: {
                onInit: '&',
                isVisible: '<?',
                onDestroy: '&',
            },
        });
})(window.angular);
