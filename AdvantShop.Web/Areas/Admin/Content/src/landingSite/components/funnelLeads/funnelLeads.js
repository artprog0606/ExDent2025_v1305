(function (ng) {
    'use strict';

    var FunnelLeadsCtrl = function (uiGridCustomConfig, $translate) {
        var ctrl = this;

        ctrl.gridLeadsOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'Id',
                    displayName: $translate.instant('Admin.Js.Leads.Number'),
                    enableCellEdit: false,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                        '<lead-info-trigger lead-id="row.entity.Id" on-close="grid.appScope.$ctrl.fetchData()">' +
                        '<a href="">{{row.entity.Id}}</a>' +
                        '</lead-info-trigger>' +
                        '</div></div>',
                    width: 90,
                },
                {
                    name: 'SalesFunnelName',
                    displayName: $translate.instant('Admin.Js.Leads.SalesFunnel'),
                    enableCellEdit: false,
                    enableSorting: false,
                },
                {
                    name: 'DealStatusName',
                    displayName: $translate.instant('Admin.Js.Leads.DealStage'),
                    enableCellEdit: false,
                    enableSorting: false,
                },
                {
                    name: 'FullName',
                    displayName: $translate.instant('Admin.Js.Leads.Contact'),
                    enableCellEdit: false,
                    enableSorting: false,
                },
                {
                    name: 'Sum',
                    displayName: $translate.instant('Admin.Js.Leads.Budget'),
                    enableCellEdit: false,
                    enableSorting: false,
                },
                {
                    name: 'CreatedDateFormatted',
                    displayName: $translate.instant('Admin.Js.Leads.DateOfCreation'),
                    enableCellEdit: false,
                    enableSorting: false,
                },
                {
                    name: 'ManagerName',
                    displayName: $translate.instant('Admin.Js.Leads.Manager'),
                    enableCellEdit: false,
                    enableSorting: false,
                },
            ],
        });

        ctrl.gridOnInit = function (gridLeads) {
            ctrl.gridLeads = gridLeads;
        };
    };

    FunnelLeadsCtrl.$inject = ['uiGridCustomConfig', '$translate'];

    ng.module('landingSite')
        .controller('FunnelLeadsCtrl', FunnelLeadsCtrl)
        .component('funnelLeads', {
            templateUrl: 'funnels/_leads',
            controller: 'FunnelLeadsCtrl',
            transclude: true,
            bindings: {
                orderSourceId: '<',
            },
        });
})(window.angular);
