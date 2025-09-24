import customerLeadsTemplate from './customerLeads.html';
(function (ng) {
    'use strict';

    var CustomerLeadsCtrl = function ($http, uiGridCustomConfig, $translate) {
        var ctrl = this;
        ctrl.$onInit = function () {
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    customerLeads: ctrl,
                });
            }
        };
        ctrl.gridLeadsOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'Id',
                    displayName: $translate.instant('Admin.Js.Customer.NumberLead'),
                    enableCellEdit: false,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"> ' +
                        '<div class="m-l-xs"><lead-info-trigger lead-id="row.entity.Id"><a href="">{{row.entity.Id}}</a></lead-info-trigger></div> ' +
                        '</div>',
                },
                {
                    name: 'DealStatusName',
                    displayName: $translate.instant('Admin.Js.Customer.Status'),
                    enableCellEdit: false,
                    enableSorting: false,
                },
                {
                    name: 'FullName',
                    displayName: $translate.instant('Admin.Js.Customer.CustomerFullName'),
                    enableCellEdit: false,
                    enableSorting: false,
                },
                {
                    name: 'Sum',
                    displayName: $translate.instant('Admin.Js.Customer.Cost'),
                    enableCellEdit: false,
                    enableSorting: false,
                },
                {
                    name: 'CreatedDateFormatted',
                    displayName: $translate.instant('Admin.Js.Customer.TimeOfCreation'),
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
        ctrl.gridOnInit = function (gridLeads) {
            ctrl.gridLeads = gridLeads;
        };
        ctrl.modalClose = function () {
            ctrl.gridLeads.fetchData();
            ctrl.onUpdate();
        };
    };
    CustomerLeadsCtrl.$inject = ['$http', 'uiGridCustomConfig', '$translate'];
    ng.module('customerLeads', ['uiGridCustom'])
        .controller('CustomerOrdersCtrl', CustomerLeadsCtrl)
        .component('customerLeads', {
            templateUrl: customerLeadsTemplate,
            controller: CustomerLeadsCtrl,
            bindings: {
                customerId: '<?',
                prefix: '@',
                onInit: '&',
                onUpdate: '&',
            },
        });
})(window.angular);
