(function (ng) {
    'use strict';

    var AnalyticsFilterCtrl = function (uiGridCustomConfig, $translate) {
        var ctrl = this,
            columnDefsAbcxyz = [
                {
                    name: 'ArtNo',
                    displayName: $translate.instant('Admin.Js.AnalyticsFilter.VendorCode'),
                    width: 150,
                    enableCellEdit: false,
                    enableSorting: false,
                },
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.AnalyticsFilter.Name'),
                    enableCellEdit: false,
                    enableSorting: false,
                },
                {
                    name: 'PriceFormatted',
                    displayName: $translate.instant('Admin.Js.AnalyticsFilter.Price'),
                    width: 150,
                    enableCellEdit: false,
                    enableSorting: false,
                },
            ],
            columnDefsRfm = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.AnalyticsFilter.Name'),
                    enableCellEdit: false,
                    enableSorting: false,
                },
                {
                    name: 'Email',
                    displayName: 'Email',
                    enableCellEdit: false,
                    enableSorting: false,
                },
                {
                    name: 'Phone',
                    displayName: $translate.instant('Admin.Js.AnalyticsFilter.Phone'),
                    enableCellEdit: false,
                    enableSorting: false,
                },
                {
                    name: 'LastOrderNumber',
                    displayName: $translate.instant('Admin.Js.AnalyticsFilter.LastOrderNumber'),
                    enableCellEdit: false,
                    enableSorting: false,
                },
                {
                    name: 'LastOrderDateStr',
                    displayName: $translate.instant('Admin.Js.AnalyticsFilter.LastOrderDate'),
                    enableCellEdit: false,
                    enableSorting: false,
                },
                {
                    name: 'OrdersCount',
                    displayName: $translate.instant('Admin.Js.AnalyticsFilter.OrdersCount'),
                    enableCellEdit: false,
                    enableSorting: false,
                },
                {
                    name: 'OrdersSum',
                    displayName: $translate.instant('Admin.Js.AnalyticsFilter.OrdersSum'),
                    enableCellEdit: false,
                    enableSorting: false,
                },
            ];

        ctrl.gridOptionsAbcxyz = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefsAbcxyz,
            uiGridCustom: {
                rowUrl: 'product/edit/{{row.entity.ProductId}}',
            },
        });

        ctrl.gridOptionsRfm = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefsRfm,
            uiGridCustom: {
                rowUrl: 'customers/view/{{row.entity.CustomerId}}',
            },
        });
    };

    AnalyticsFilterCtrl.$inject = ['uiGridCustomConfig', '$translate'];

    ng.module('analyticsFilter', ['uiGridCustom']).controller('AnalyticsFilterCtrl', AnalyticsFilterCtrl);
})(window.angular);
