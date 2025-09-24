(function (ng) {
    'use strict';

    var HomeCtrl = function (uiGridCustomConfig, $translate, $window, advTrackingService) {
        var ctrl = this,
            columnDefs = [
                {
                    name: '№',
                    displayName: $translate.instant('Admin.Js.Home.Order'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><a ng-href="orders/edit/{{row.entity.OrderId}}" onclick="return advTrack(\'Core_Common_LastOrdersDashboard_ClickOrder\');">{{row.entity.Number}}</a></div>',
                    width: 110,
                },
                {
                    name: 'StatusName',
                    displayName: $translate.instant('Admin.Js.Home.Status'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><i class="fa fa-circle" style="color:#{{row.entity.StatusColor}}"></i>&nbsp;{{COL_FIELD}}</div>',
                },
                {
                    name: 'CustomerName',
                    displayName: $translate.instant('Admin.Js.Home.Customer'),
                },
                {
                    name: 'OrderDate',
                    displayName: $translate.instant('Admin.Js.Home.Date'),
                },
                {
                    name: 'Sum',
                    displayName: $translate.instant('Admin.Js.Home.Sum'),
                },
            ];

        ctrl.gridRowClick = function ($event, row) {
            advTrackingService.trackEvent('Core_Common_LastOrdersDashboard_ClickOrder').then(function () {
                $window.location.assign('orders/edit/' + row.entity.OrderId);
            });
        };

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            enableSorting: false,
            columnDefs: columnDefs,
            uiGridCustom: {
                rowClick: ctrl.gridRowClick,
            },
        });

        ctrl.gridOptionsMy = ng.extend({}, uiGridCustomConfig, {
            enableSorting: false,
            columnDefs: columnDefs,
            uiGridCustom: {
                rowClick: ctrl.gridRowClick,
            },
        });

        ctrl.gridOptionsNotMy = ng.extend({}, uiGridCustomConfig, {
            enableSorting: false,
            columnDefs: columnDefs,
            uiGridCustom: {
                rowClick: ctrl.gridRowClick,
            },
        });

        ctrl.setRangeDateOrderGraph = function (selectedDates, dateStr, instance) {
            console.log(`Temp! Date selected: ${dateStr}`);
        };
    };

    HomeCtrl.$inject = ['uiGridCustomConfig', '$translate', '$window', 'advTrackingService'];

    ng.module('home', ['uiGridCustom', 'advTracking', 'carousel', 'congratulationsDashboard', 'achievements']).controller('HomeCtrl', HomeCtrl);
})(window.angular);
