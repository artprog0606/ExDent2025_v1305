import searchRequestsTemplate from './searchRequests.html';
(function (ng) {
    'use strict';

    var SearchRequestsCtrl = function ($http, uiGridConstants, uiGridCustomConfig, uiGridCustomParamsConfig, uiGridCustomService, $translate) {
        var ctrl = this;
        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'SearchTerm',
                    displayName: $translate.instant('Admin.Js.Analytics.SearchQueries'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><a target="_blank" ng-href="..{{row.entity.Request}}">{{COL_FIELD}}</a></div>',
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Analytics.SearchQueries'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'SearchTerm',
                    },
                },
                {
                    name: 'CustomerID',
                    displayName: $translate.instant('Admin.Js.Analytics.SearchQueriesCustomer'),
                    enableCellEdit: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><a ng-if="row.entity.LastName || row.entity.FirstName" target="_blank" ng-href="customers/view/{{COL_FIELD}}" ng-bind="row.entity.LastName + \' \' + row.entity.FirstName"></a></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Analytics.SearchQueriesCustomer'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'CustomerFIO',
                    },
                },
                {
                    name: 'ResultCount',
                    displayName: $translate.instant('Admin.Js.Analytics.SearchQueriesFoundCount'),
                    enableCellEdit: false,
                    width: 150,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Analytics.SearchQueriesFoundCount'),
                        type: 'range',
                        rangeOptions: {
                            from: {
                                name: 'ResultCountFrom',
                            },
                            to: {
                                name: 'ResultCountTo',
                            },
                        },
                        fetch: 'analytics/getResultCountRangeForPaging',
                    },
                },
                {
                    name: 'Date',
                    cellTemplate: '<div class="ui-grid-cell-contents"><span ng-bind="row.entity.DateFormatted"></span></div>',
                    displayName: $translate.instant('Admin.Js.Analytics.SearchQueriesDate'),
                    enableCellEdit: false,
                    width: 150,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Analytics.SearchQueriesDate'),
                        type: 'datetime',
                        term: {
                            from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                            to: new Date(),
                        },
                        datetimeOptions: {
                            from: {
                                name: 'DateFrom',
                            },
                            to: {
                                name: 'DateTo',
                            },
                        },
                    },
                },
            ],
        });
        ctrl.$onInit = function () {
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    searchRequests: ctrl,
                });
            }
        };
        ctrl.recalc = function (dateFrom, dateTo) {
            //ctrl.fetchCommonData(dateFrom, dateTo);
        };
    };
    SearchRequestsCtrl.$inject = ['$http', 'uiGridConstants', 'uiGridCustomConfig', 'uiGridCustomParamsConfig', 'uiGridCustomService', '$translate'];
    ng.module('analyticsReport')
        .controller('SearchRequestsCtrl', SearchRequestsCtrl)
        .component('searchRequests', {
            templateUrl: searchRequestsTemplate,
            controller: SearchRequestsCtrl,
            bindings: {
                onInit: '&',
            },
        });
})(window.angular);
