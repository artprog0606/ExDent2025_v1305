import ordersAnalysisTemplate from './ordersAnalysis.html';
(function (ng) {
    'use strict';

    var OrdersAnalysisCtrl = function ($http) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.chartOptions = {
                legend: {
                    position: 'top',
                    alight: 'start',
                    labels: {
                        fontColor: 'red',
                    },
                },
            };
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    orders: ctrl,
                });
            }
            ctrl.ShippingsGroupByName = false;
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
            ctrl.fetchPayments(dateFrom, dateTo, paid, orderStatus);
            ctrl.fetchShippings(dateFrom, dateTo, paid, orderStatus);
            ctrl.fetchStatuses(dateFrom, dateTo, paid, orderStatus);
            ctrl.fetchSources(dateFrom, dateTo, paid, orderStatus);
            ctrl.fetchRepeatOrders(dateFrom, dateTo, paid, orderStatus);
            ctrl.getOrdersData(dateFrom, dateTo, paid, orderStatus);
        };
        ctrl.mapLabels = function (item) {
            var result = [];
            var value = item;
            while (value !== '') {
                result.push(value.substring(0, 30));
                value = value.substring(30);
            }
            return result;
        };
        ctrl.fetchPayments = function (dateFrom, dateTo, paid, orderStatus) {
            $http
                .get('analytics/getOrders', {
                    params: {
                        type: 'payments',
                        dateFrom: dateFrom,
                        dateTo: dateTo,
                        paid: paid,
                        orderStatus: orderStatus,
                    },
                })
                .then(function (result) {
                    ctrl.Payments = result.data;
                    ctrl.Payments.Labels = result.data.Labels.map(ctrl.mapLabels);
                });
        };
        ctrl.fetchShippings = function (dateFrom, dateTo, paid, orderStatus) {
            $http
                .get('analytics/getOrders', {
                    params: {
                        type: ctrl.ShippingsGroupByName ? 'shippingsGroupedByName' : 'shippings',
                        dateFrom: dateFrom,
                        dateTo: dateTo,
                        paid: paid,
                        orderStatus: orderStatus,
                        deliveryDateFrom: ctrl.deliveryDateFrom,
                        deliveryDateTo: ctrl.deliveryDateTo,
                    },
                })
                .then(function (result) {
                    ctrl.Shippings = result.data;
                    ctrl.Shippings.Labels = result.data.Labels.map(ctrl.mapLabels);
                });
        };
        ctrl.onChangeShippingsGroupByNameOnOff = function (checked) {
            ctrl.ShippingsGroupByName = checked;
            ctrl.fetchShippings(ctrl.dateFrom, ctrl.dateTo, ctrl.paid, ctrl.orderStatus);
        };
        ctrl.onChangeShippingsDate = function () {
            ctrl.fetchShippings(ctrl.dateFrom, ctrl.dateTo, ctrl.paid, ctrl.orderStatus);
        };
        ctrl.fetchStatuses = function (dateFrom, dateTo, paid, orderStatus) {
            $http
                .get('analytics/getOrders', {
                    params: {
                        type: 'statuses',
                        dateFrom: dateFrom,
                        dateTo: dateTo,
                        paid: paid,
                        orderStatus: orderStatus,
                    },
                })
                .then(function (result) {
                    ctrl.Statuses = result.data;
                });
        };
        ctrl.fetchSources = function (dateFrom, dateTo, paid, orderStatus) {
            $http
                .get('analytics/getOrders', {
                    params: {
                        type: 'sources',
                        dateFrom: dateFrom,
                        dateTo: dateTo,
                        paid: paid,
                        orderStatus: orderStatus,
                    },
                })
                .then(function (result) {
                    ctrl.Sources = result.data;
                });
        };
        ctrl.fetchRepeatOrders = function (dateFrom, dateTo, paid, orderStatus) {
            $http
                .get('analytics/getOrders', {
                    params: {
                        type: 'repeatorders',
                        dateFrom: dateFrom,
                        dateTo: dateTo,
                        paid: paid,
                        orderStatus: orderStatus,
                    },
                })
                .then(function (result) {
                    ctrl.RepeatOrders = result.data;
                });
        };
        ctrl.getOrdersData = function (dateFrom, dateTo, paid, orderStatus) {
            $http
                .get('analytics/getOrdersData', {
                    params: {
                        from: dateFrom,
                        to: dateTo,
                        paid: paid,
                        orderStatus: orderStatus,
                    },
                })
                .then(function (result) {
                    ctrl.Data = result.data;
                });
        };
    };
    OrdersAnalysisCtrl.$inject = ['$http'];
    ng.module('analyticsReport')
        .controller('OrdersAnalysisCtrl', OrdersAnalysisCtrl)
        .component('ordersAnalysis', {
            templateUrl: ordersAnalysisTemplate,
            controller: OrdersAnalysisCtrl,
            bindings: {
                onInit: '&',
                isVisible: '<?',
                onDestroy: '&',
            },
        });
})(window.angular);
