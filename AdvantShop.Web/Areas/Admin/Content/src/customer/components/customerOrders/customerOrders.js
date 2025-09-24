import customerOrdersTemplate from './customerOrders.html';
(function (ng) {
    'use strict';

    var CustomerOrdersCtrl = function ($http, uiGridCustomConfig, $translate) {
        var ctrl = this;
        ctrl.gridOrdersOptions = ng.extend({}, uiGridCustomConfig, {
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
                },
                {
                    name: 'IsPaid',
                    displayName: $translate.instant('Admin.Js.Customer.Paid'),
                    enableCellEdit: false,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"> ' +
                        '{{row.entity.IsPaid ? "' +
                        $translate.instant('Admin.Js.Customer.Yes') +
                        '" : "' +
                        $translate.instant('Admin.Js.Customer.No') +
                        '"}}' +
                        '</div>',
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
    CustomerOrdersCtrl.$inject = ['$http', 'uiGridCustomConfig', '$translate'];
    ng.module('customerOrders', ['uiGridCustom'])
        .controller('CustomerOrdersCtrl', CustomerOrdersCtrl)
        .component('customerOrders', {
            templateUrl: customerOrdersTemplate,
            controller: CustomerOrdersCtrl,
            bindings: {
                customerId: '<?',
                prefix: '@',
            },
        });
})(window.angular);
