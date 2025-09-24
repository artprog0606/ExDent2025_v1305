import profitTemplate from './profit.html';
(function (ng) {
    'use strict';

    var ProfitCtrl = function ($http) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.useShippingCost = false;
            ctrl.groupFormatString = 'dd';
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    profit: ctrl,
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
            ctrl.fetchSum();
            ctrl.fetchCount();
        };
        ctrl.changeGroup = function (groupFormatString) {
            ctrl.groupFormatString = groupFormatString;
            ctrl.recalc(ctrl.dateFrom, ctrl.dateTo, ctrl.paid, ctrl.orderStatus);
        };
        ctrl.fetchSum = function () {
            $http
                .get('analytics/getProfit', {
                    params: {
                        type: 'sum',
                        dateFrom: ctrl.dateFrom,
                        dateTo: ctrl.dateTo,
                        useShippingCost: ctrl.useShippingCost,
                        paid: ctrl.paid,
                        orderStatus: ctrl.orderStatus,
                        groupFormatString: ctrl.groupFormatString,
                    },
                })
                .then(function (result) {
                    ctrl.ProfitSum = result.data;
                });
        };
        ctrl.fetchCount = function () {
            $http
                .get('analytics/getProfit', {
                    params: {
                        type: 'count',
                        dateFrom: ctrl.dateFrom,
                        dateTo: ctrl.dateTo,
                        useShippingCost: ctrl.useShippingCost,
                        paid: ctrl.paid,
                        orderStatus: ctrl.orderStatus,
                        groupFormatString: ctrl.groupFormatString,
                    },
                })
                .then(function (result) {
                    ctrl.ProfitCount = result.data;
                });
        };
    };
    ProfitCtrl.$inject = ['$http'];
    ng.module('analyticsReport')
        .controller('ProfitCtrl', ProfitCtrl)
        .component('profit', {
            templateUrl: profitTemplate,
            controller: ProfitCtrl,
            bindings: {
                onInit: '&',
                isVisible: '<?',
                onDestroy: '&',
            },
        });
})(window.angular);
