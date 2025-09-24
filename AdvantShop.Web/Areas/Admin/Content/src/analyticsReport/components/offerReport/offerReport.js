import offerReportTemplate from './offerReport.html';
(function (ng) {
    'use strict';

    var OfferReportCtrl = function ($http, uiGridCustomConfig, $translate) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.groupFormatString = 'dd';
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    offerreport: ctrl,
                });
            }
        };
        ctrl.recalc = function (dateFrom, dateTo, offerId, paid) {
            ctrl.dateFrom = dateFrom;
            ctrl.dateTo = dateTo;
            ctrl.offerId = offerId;
            ctrl.paid = paid;
            ctrl.getOfferName();
            if (ctrl.offerId != null) {
                ctrl.fetchSum(dateFrom, dateTo, ctrl.offerId, paid);
                if (ctrl.gridOfferReport != null) {
                    ctrl.gridOfferReport.setParams({
                        offerId: ctrl.offerId,
                        dateFrom: ctrl.dateFrom,
                        dateTo: ctrl.dateTo,
                        paid: ctrl.paid,
                    });
                    ctrl.gridOfferReport.fetchData(true);
                }
            }
        };
        ctrl.fetchSum = function (dateFrom, dateTo, offerId, paid) {
            $http
                .get('analytics/getOfferStatistics', {
                    params: {
                        type: 'sum',
                        dateFrom: dateFrom,
                        dateTo: dateTo,
                        paid: paid,
                        offerId: offerId,
                        groupFormatString: ctrl.groupFormatString,
                    },
                })
                .then(function (result) {
                    ctrl.SumData = result.data;
                });
        };
        ctrl.fetchCount = function (dateFrom, dateTo, offerId, paid) {
            $http
                .get('analytics/getOfferStatistics', {
                    params: {
                        type: 'count',
                        dateFrom: dateFrom,
                        dateTo: dateTo,
                        paid: paid,
                        offerId: offerId,
                        groupFormatString: ctrl.groupFormatString,
                    },
                })
                .then(function (result) {
                    ctrl.SumData = result.data;
                });
        };
        ctrl.getOfferName = function () {
            ctrl.OfferName = null;
            if (ctrl.offerId != null) {
                $http
                    .get('analytics/getOfferStatisticsName', {
                        params: {
                            offerId: ctrl.offerId,
                        },
                    })
                    .then(function (result) {
                        ctrl.OfferName = result.data.name;
                    });
            }
        };
        ctrl.changeGroup = function (groupFormatString) {
            ctrl.groupFormatString = groupFormatString;
            if (ctrl.offerId != null) {
                ctrl.fetchSum(ctrl.dateFrom, ctrl.dateTo, ctrl.offerId, ctrl.paid);
            }
        };
        ctrl.selectOffer = function (result) {
            if (result.ids && result.ids.length > 0) {
                if (ctrl.onChange != null) {
                    ctrl.onChange({
                        offerId: result.ids[0],
                    });
                }
                ctrl.recalc(ctrl.dateFrom, ctrl.dateTo, result.ids[0], ctrl.paid);
            }
        };
        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'Number',
                    displayName: $translate.instant('Admin.Js.OfferReport.NumberOfOrder'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"> ' +
                        '<div class="m-l-xs"><a href="orders/edit/{{row.entity.OrderId}}" target="_blank">{{row.entity.Number}}</a></div> ' +
                        '</div>',
                },
                {
                    name: 'BuyerName',
                    displayName: $translate.instant('Admin.Js.OfferReport.Customer'),
                },
                {
                    name: 'Email',
                    displayName: 'Email',
                },
                {
                    name: 'IsPaid',
                    displayName: $translate.instant('Admin.Js.OfferReport.Paid'),
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
                    displayName: $translate.instant('Admin.Js.OfferReport.Date'),
                    enableCellEdit: false,
                },
                {
                    name: 'OfferAmount',
                    displayName: $translate.instant('Admin.Js.OfferReport.QuantityOfOffer'),
                    enableCellEdit: false,
                },
            ],
        });
        ctrl.gridOnInit = function (grid) {
            ctrl.gridOfferReport = grid;
        };
    };
    OfferReportCtrl.$inject = ['$http', 'uiGridCustomConfig', '$translate'];
    ng.module('analyticsReport')
        .controller('OfferReportCtrl', OfferReportCtrl)
        .component('offerReport', {
            templateUrl: offerReportTemplate,
            controller: OfferReportCtrl,
            bindings: {
                onInit: '&',
                onChange: '&',
            },
        });
})(window.angular);
