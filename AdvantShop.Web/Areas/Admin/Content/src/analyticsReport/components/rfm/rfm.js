import rfmTemplate from './rfm.html';
(function (ng) {
    'use strict';

    var RfmCtrl = function ($http, uiGridConstants, uiGridCustomConfig, uiGridCustomParamsConfig, uiGridCustomService, $translate) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Fio',
                    displayName: $translate.instant('Admin.Js.RFM.Fio'),
                    //width: 110
                },
                {
                    name: 'OrdersCount',
                    displayName: $translate.instant('Admin.Js.RFM.OrdersCount'),
                },
                {
                    name: 'PaidOrdersCount',
                    displayName: $translate.instant('Admin.Js.RFM.PaidOrdersCount'),
                },
                {
                    name: 'PaidOrdersSum',
                    displayName: $translate.instant('Admin.Js.RFM.PaidOrdersSum'),
                },
            ];
        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            enableSorting: false,
            columnDefs: columnDefs,
            uiGridCustom: {
                rowUrl: 'customers/view/{{row.entity.CustomerId}}',
            },
        });
        ctrl.gridOnInit = function (grid) {
            ctrl.gridTopCustomers = grid;
        };
        ctrl.$onInit = function () {
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    rfm: ctrl,
                });
            }
        };
        ctrl.recalc = function (dateFrom, dateTo) {
            ctrl.fetch(dateFrom, dateTo);
            ctrl.fetchCustomerGroups(dateFrom, dateTo);
            ctrl.fetchCommonData(dateFrom, dateTo);
            if (ctrl.gridTopCustomers != null) {
                ctrl.gridTopCustomers.setParams({
                    from: dateFrom,
                    to: dateTo,
                });
                ctrl.gridTopCustomers.fetchData();
            }
        };
        ctrl.fetchCustomerGroups = function (dateFrom, dateTo) {
            $http
                .get('analytics/getCustomerGroups', {
                    params: {
                        dateFrom: dateFrom,
                        dateTo: dateTo,
                    },
                })
                .then(function (result) {
                    ctrl.CustomerGroups = result.data;
                });
        };
        ctrl.fetchCommonData = function (dateFrom, dateTo) {
            $http
                .get('analytics/GetRfmCommonData', {
                    params: {
                        dateFrom: dateFrom,
                        dateTo: dateTo,
                    },
                })
                .then(function (result) {
                    ctrl.CommonData = result.data;
                });
        };
        ctrl.fetch = function (dateFrom, dateTo) {
            ctrl.dateFrom = dateFrom;
            ctrl.dateTo = dateTo;
            $http
                .get('analytics/getRfm', {
                    params: {
                        dateFrom: dateFrom,
                        dateTo: dateTo,
                    },
                })
                .then(function (result) {
                    ctrl.Data = result.data;
                    if (ctrl.Data != null) {
                        ctrl.Rm = ctrl.Data.Rm;
                        ctrl.Rf = ctrl.Data.Rf;
                    }
                });
        };
        ctrl.showRm = function (i, j) {
            var from = ctrl.dateFrom instanceof Date ? ctrl.dateFrom.toJSON() : ctrl.dateFrom;
            var to = ctrl.dateTo instanceof Date ? ctrl.dateTo.toJSON() : ctrl.dateTo;
            var url = 'analytics/analyticsFilter?type=rfm&group=r_m_' + (i + 1) + '_' + (j + 1) + '&from=' + from + '&to=' + to;
            var win = window.open(url, '_blank');
            win.focus();
        };
        ctrl.showRf = function (i, j) {
            var from = ctrl.dateFrom instanceof Date ? ctrl.dateFrom.toJSON() : ctrl.dateFrom;
            var to = ctrl.dateTo instanceof Date ? ctrl.dateTo.toJSON() : ctrl.dateTo;
            var url = 'analytics/analyticsFilter?type=rfm&group=r_f_' + (i + 1) + '_' + (j + 1) + '&from=' + from + '&to=' + to;
            var win = window.open(url, '_blank');
            win.focus();
        };
        ctrl.range = function (min, max) {
            var input = [];
            for (var i = min; i <= max; i += 1) {
                input.push(i);
            }
            return input;
        };
        ctrl.rangeNg = function (max, min) {
            var input = [];
            for (var i = max; i >= min; i -= 1) {
                input.push(i);
            }
            return input;
        };
    };
    RfmCtrl.$inject = ['$http', 'uiGridConstants', 'uiGridCustomConfig', 'uiGridCustomParamsConfig', 'uiGridCustomService', '$translate'];
    ng.module('analyticsReport')
        .controller('RfmCtrl', RfmCtrl)
        .component('rfm', {
            templateUrl: rfmTemplate,
            controller: RfmCtrl,
            bindings: {
                onInit: '&',
            },
        });
})(window.angular);
