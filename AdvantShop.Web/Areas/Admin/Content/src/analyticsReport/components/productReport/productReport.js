import productReportTemplate from './productReport.html';
(function (ng) {
    'use strict';

    var ProductReportCtrl = function ($http, uiGridCustomConfig, $translate) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.groupFormatString = 'dd';
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    productreport: ctrl,
                });
            }
        };
        ctrl.recalc = function (dateFrom, dateTo, productId, paid) {
            ctrl.dateFrom = dateFrom;
            ctrl.dateTo = dateTo;
            ctrl.productId = productId;
            ctrl.paid = paid;
            ctrl.getProductName();
            if (ctrl.productId != null) {
                ctrl.fetchSum(dateFrom, dateTo, ctrl.productId, paid);
                if (ctrl.gridProductReport != null) {
                    ctrl.gridProductReport.setParams({
                        productId: ctrl.productId,
                        dateFrom: ctrl.dateFrom,
                        dateTo: ctrl.dateTo,
                        paid: ctrl.paid,
                    });
                    ctrl.gridProductReport.fetchData(true);
                }
            }
        };
        ctrl.fetchSum = function (dateFrom, dateTo, productId, paid) {
            $http
                .get('analytics/getProductStatistics', {
                    params: {
                        type: 'sum',
                        dateFrom: dateFrom,
                        dateTo: dateTo,
                        paid: paid,
                        productId: productId,
                        groupFormatString: ctrl.groupFormatString,
                    },
                })
                .then(function (result) {
                    ctrl.SumData = result.data;
                });
        };
        ctrl.fetchCount = function (dateFrom, dateTo, productId, paid) {
            $http
                .get('analytics/getProductStatistics', {
                    params: {
                        type: 'count',
                        dateFrom: dateFrom,
                        dateTo: dateTo,
                        paid: paid,
                        productId: productId,
                        groupFormatString: ctrl.groupFormatString,
                    },
                })
                .then(function (result) {
                    ctrl.SumData = result.data;
                });
        };
        ctrl.getProductName = function () {
            ctrl.ProductName = null;
            if (ctrl.productId != null) {
                $http
                    .get('analytics/getProductStatisticsName', {
                        params: {
                            productId: ctrl.productId,
                        },
                    })
                    .then(function (result) {
                        ctrl.ProductName = result.data.name;
                    });
            }
        };
        ctrl.changeGroup = function (groupFormatString) {
            ctrl.groupFormatString = groupFormatString;
            if (ctrl.productId != null) {
                ctrl.fetchSum(ctrl.dateFrom, ctrl.dateTo, ctrl.productId, ctrl.paid);
            }
        };
        ctrl.selectProducts = function (result) {
            if (result.ids != null && result.ids.length > 0) {
                if (ctrl.onChange != null) {
                    ctrl.onChange({
                        productId: result.ids[0],
                    });
                }
                ctrl.recalc(ctrl.dateFrom, ctrl.dateTo, result.ids[0], ctrl.paid);
            }
        };
        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'Number',
                    displayName: $translate.instant('Admin.Js.ProductReport.NumberOfOrder'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"> ' +
                        '<div class="m-l-xs"><a href="orders/edit/{{row.entity.OrderId}}" target="_blank">{{row.entity.Number}}</a></div> ' +
                        '</div>',
                },
                {
                    name: 'BuyerName',
                    displayName: $translate.instant('Admin.Js.ProductReport.Customer'),
                },
                {
                    name: 'Email',
                    displayName: 'Email',
                },
                {
                    name: 'IsPaid',
                    displayName: $translate.instant('Admin.Js.ProductReport.Paid'),
                    enableCellEdit: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">{{row.entity.IsPaid ? "' +
                        $translate.instant('Admin.Js.ProductReport.Yes') +
                        '" : "' +
                        $translate.instant('Admin.Js.ProductReport.No') +
                        '"}}</div>',
                },
                {
                    name: 'OrderDateFormatted',
                    displayName: $translate.instant('Admin.Js.ProductReport.Date'),
                    enableCellEdit: false,
                },
                {
                    name: 'ProductAmount',
                    displayName: $translate.instant('Admin.Js.ProductReport.QuantityOfProducts'),
                    enableCellEdit: false,
                },
            ],
        });
        ctrl.gridOnInit = function (grid) {
            ctrl.gridProductReport = grid;
        };
    };
    ProductReportCtrl.$inject = ['$http', 'uiGridCustomConfig', '$translate'];
    ng.module('analyticsReport')
        .controller('ProductReportCtrl', ProductReportCtrl)
        .component('productReport', {
            templateUrl: productReportTemplate,
            controller: ProductReportCtrl,
            bindings: {
                onInit: '&',
                onChange: '&',
            },
        });
})(window.angular);
