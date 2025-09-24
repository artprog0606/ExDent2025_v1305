(function (ng) {
    'use strict';

    var FunnelBookingsCtrl = function (uiGridCustomConfig, $translate, bookingService) {
        var ctrl = this;

        ctrl.gridBookingsOptions = ng.extend({}, uiGridCustomConfig, {
            enableSorting: false,
            columnDefs: [
                {
                    name: 'Id',
                    displayName: $translate.instant('Admin.Js.LandingSite.FunnelBookings.Number'),
                    enableCellEdit: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' +
                        '<a href=\'booking/index/{{ row.entity.AffiliateId }}?modal={{row.entity.Id}}\' ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadBooking(row.entity.Id); $event.preventDefault();">{{COL_FIELD}}</a>' +
                        '</div>',
                    width: 80,
                },
                {
                    name: 'AffiliateName',
                    displayName: $translate.instant('Admin.Js.LandingSite.FunnelBookings.Affiliate'),
                    enableCellEdit: false,
                },
                {
                    name: 'StatusName',
                    displayName: $translate.instant('Admin.Js.LandingSite.FunnelBookings.Status'),
                    enableCellEdit: false,
                },
                {
                    name: 'CustomerName',
                    displayName: $translate.instant('Admin.Js.LandingSite.FunnelBookings.Contact'),
                    enableCellEdit: false,
                    enableSorting: false,
                },
                {
                    name: 'ReservationResourceName',
                    displayName: $translate.instant('Admin.Js.LandingSite.FunnelBookings.Resource'),
                    enableCellEdit: false,
                },
                {
                    name: 'Sum',
                    displayName: $translate.instant('Admin.Js.LandingSite.FunnelBookings.Sum'),
                    enableCellEdit: false,
                    width: 100,
                },
                {
                    name: 'BeginDateFormatted',
                    displayName: $translate.instant('Admin.Js.LandingSite.FunnelBookings.StartDate'),
                    enableCellEdit: false,
                    width: 150,
                },
                {
                    name: 'EndDateFormatted',
                    displayName: $translate.instant('Admin.Js.LandingSite.FunnelBookings.EndDate'),
                    enableCellEdit: false,
                    width: 150,
                },
                {
                    name: 'DateAddedFormatted',
                    displayName: $translate.instant('Admin.Js.LandingSite.FunnelBookings.CreationDate'),
                    enableCellEdit: false,
                    width: 150,
                },
            ],
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.loadBooking(row.entity.Id);
                },
            },
        });

        ctrl.loadBooking = function (id, affiliateId, beginDate, endDate, reservationResourceId) {
            bookingService.showBookingModal(id, affiliateId, beginDate, endDate, reservationResourceId).result.then(
                function (result) {
                    ctrl.gridBookings.fetchData();
                    return result;
                },
                function (result) {
                    ctrl.gridBookings.fetchData();
                    return result;
                },
            );
        };

        ctrl.gridOnInit = function (gridBookings) {
            ctrl.gridBookings = gridBookings;
        };
    };

    FunnelBookingsCtrl.$inject = ['uiGridCustomConfig', '$translate', 'bookingService'];

    ng.module('landingSite')
        .controller('FunnelBookingsCtrl', FunnelBookingsCtrl)
        .component('funnelBookings', {
            templateUrl: 'funnels/_bookings',
            controller: 'FunnelBookingsCtrl',
            transclude: true,
            bindings: {
                orderSourceId: '<',
            },
        });
})(window.angular);
