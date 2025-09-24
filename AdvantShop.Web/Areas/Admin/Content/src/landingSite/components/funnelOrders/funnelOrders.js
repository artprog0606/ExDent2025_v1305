(function (ng) {
    'use strict';

    var FunnelOrdersCtrl = function (uiGridCustomConfig, $translate) {
        var ctrl = this;

        ctrl.gridOrdersOptions = ng.extend({}, uiGridCustomConfig, {
            uiGridCustom: {
                rowUrl: 'orders/edit/{{row.entity.OrderId}}',
            },
            columnDefs: [
                {
                    name: 'Number',
                    displayName: $translate.instant('Admin.Js.Customer.OrderNumber'),
                    enableCellEdit: false,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"> ' +
                        '<div class="m-l-xs"><a href="orders/edit/{{row.entity.OrderId}}" target="_blank">{{row.entity.Number}}</a></div> ' +
                        '</div>',
                },
                {
                    name: 'StatusName',
                    displayName: $translate.instant('Admin.Js.Customer.Status'),
                    enableCellEdit: false,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><i class="fa fa-circle m-r-xs" style="color:#{{row.entity.Color}}"></i> {{row.entity.StatusName}}</div>',
                },
                {
                    name: 'BuyerName',
                    displayName: $translate.instant('Admin.Js.Orders.Customer'),
                    enableCellEdit: false,
                    enableSorting: false,
                },
                {
                    name: 'IsPaid',
                    displayName: $translate.instant('Admin.Js.Orders.Payment'),
                    enableCellEdit: false,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div class="adv-checkbox-label">' +
                        '<input type="checkbox" ng-model="row.entity.IsPaid" readonly class="adv-checkbox-input control-checkbox pointer-events-none" data-e2e="switchOnOffSelect" />' +
                        '<span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span>' +
                        '</div></div>',
                    width: 65,
                },
                {
                    name: 'SumFormatted',
                    displayName: $translate.instant('Admin.Js.Customer.Sum'),
                    enableCellEdit: false,
                    enableSorting: false,
                },
                {
                    name: 'OrderDateFormatted',
                    displayName: $translate.instant('Admin.Js.Customer.Date'),
                    enableCellEdit: false,
                    enableSorting: false,
                },
                {
                    name: 'ManagerName',
                    displayName: $translate.instant('Admin.Js.Customer.Manager'),
                    enableCellEdit: false,
                    enableSorting: false,
                },
            ],
        });
    };

    FunnelOrdersCtrl.$inject = ['uiGridCustomConfig', '$translate'];

    ng.module('landingSite')
        .controller('FunnelOrdersCtrl', FunnelOrdersCtrl)
        .component('funnelOrders', {
            templateUrl: 'funnels/_orders',
            controller: 'FunnelOrdersCtrl',
            transclude: true,
            bindings: {
                orderSourceId: '<',
            },
        });
})(window.angular);
