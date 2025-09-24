(function (ng) {
    'use strict';

    var LeadsListSourcesCtrl = function (uiGridCustomConfig, $http, $translate) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.LeadsListSources.Name'),
                    enableCellEdit: false,
                },
                {
                    name: 'LeadsCount',
                    displayName: $translate.instant('Admin.Js.LeadsListSources.LeadsCount'),
                    enableCellEdit: false,
                    width: 90,
                    headerCellClass: 'ui-grid-text-center',
                    cellClass: 'ui-grid-text-center',
                },
                {
                    name: 'PercentLeads',
                    displayName: $translate.instant('Admin.Js.LeadsListSources.PercentLeads'),
                    enableCellEdit: false,
                    width: 90,
                    headerCellClass: 'ui-grid-text-center',
                    cellClass: 'ui-grid-text-center',
                },
            ];
        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {},
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };
    };

    LeadsListSourcesCtrl.$inject = ['uiGridCustomConfig', '$http', '$translate'];

    ng.module('leads').controller('LeadsListSourcesCtrl', LeadsListSourcesCtrl);
})(window.angular);
