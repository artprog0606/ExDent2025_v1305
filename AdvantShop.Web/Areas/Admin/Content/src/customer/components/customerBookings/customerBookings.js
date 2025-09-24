import customerBookingsTemplate from './customerBookings.html';
(function (ng) {
    'use strict';

    var CustomerBookingsCtrl = function (uiGridCustomConfig, $translate, bookingService, isMobileService) {
        var ctrl = this;
        ctrl.$onInit = function () {
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    customerBookings: ctrl,
                });
            }
        };
        ctrl.gridBookingsOptions = ng.extend({}, uiGridCustomConfig, {
            enableSorting: false,
            enableGridMenu: !isMobileService.getValue(),
            columnDefs: [
                {
                    name: 'Id',
                    displayName: 'Номер',
                    enableCellEdit: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' +
                        '<a href=\'booking/index/{{ row.entity.AffiliateId }}?modal={{row.entity.Id}}\' ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadBooking(row.entity.Id); $event.preventDefault();">{{COL_FIELD}}</a>' +
                        '</div>',
                    width: 80,
                },
                {
                    name: 'AffiliateName',
                    displayName: 'Филиал',
                    enableCellEdit: false,
                },
                {
                    name: 'StatusName',
                    displayName: 'Статус',
                    enableCellEdit: false,
                },
                //{
                //    name: 'CustomerName',
                //    displayName: 'Контакт',
                //    enableCellEdit: false,
                //    enableSorting: false,
                //},
                {
                    name: 'ReservationResourceName',
                    displayName: 'Ресурс',
                    enableCellEdit: false,
                },
                {
                    name: 'Sum',
                    displayName: 'Сумма',
                    enableCellEdit: false,
                    width: 100,
                },
                {
                    name: 'BeginDateFormatted',
                    displayName: 'Дата начала',
                    enableCellEdit: false,
                    width: 150,
                    visible: 1441,
                },
                {
                    name: 'EndDateFormatted',
                    displayName: 'Дата окончания',
                    enableCellEdit: false,
                    width: 150,
                    visible: 1441,
                },
                {
                    name: 'DateAddedFormatted',
                    displayName: 'Дата создания',
                    enableCellEdit: false,
                    width: 150,
                    visible: 1441,
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
                    ctrl.modalClose(result);
                    return result;
                },
                function (result) {
                    ctrl.modalClose(result);
                    return result;
                },
            );
        };
        ctrl.gridOnInit = function (gridBookings) {
            ctrl.gridBookings = gridBookings;
        };
        ctrl.modalClose = function () {
            ctrl.gridBookings.fetchData();
            ctrl.onUpdate();
        };
    };
    CustomerBookingsCtrl.$inject = ['uiGridCustomConfig', '$translate', 'bookingService', 'isMobileService'];
    ng.module('customerBookings', ['uiGridCustom'])
        .controller('CustomerBookingsCtrl', CustomerBookingsCtrl)
        .component('customerBookings', {
            templateUrl: customerBookingsTemplate,
            controller: CustomerBookingsCtrl,
            bindings: {
                customerId: '<?',
                prefix: '@',
                onInit: '&',
                onUpdate: '&',
            },
        });
})(window.angular);
