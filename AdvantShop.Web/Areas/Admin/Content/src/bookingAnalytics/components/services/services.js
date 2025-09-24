import servicesTemplate from './servicesBookingAnalytics.html';
(function (ng) {
    'use strict';

    var ServicesAnalyticsCtrl = function ($translate, $http, uiGridCustomConfig, adminWebNotificationsEvents, adminWebNotificationsService) {
        var ctrl = this;
        ctrl.gridInited = false;
        ctrl.gridOnInit = function (grid) {
            ctrl.gridServices = grid;
            ctrl.gridServices.gridOptions.data = ctrl.ServicesAnalyticsData;
            ctrl.gridInited = true;
        };
        ctrl.$onInit = function () {
            ctrl.gridServicesUniqueId = 'gridAnaliticsServices' + Math.floor(Math.random() * 1000);
            if (ctrl.autoFetchByNewBooking) {
                ctrl.removeCallbackUpdateBookings = adminWebNotificationsService.addListener(
                    adminWebNotificationsEvents.updateBookings,
                    function (data) {
                        if (
                            (!ctrl.affiliateId || data.affiliateId == ctrl.affiliateId) &&
                            (!ctrl.reservationResourceId || data.reservationResourceId == ctrl.reservationResourceId)
                        ) {
                            ctrl.fetch();
                        }
                    },
                );
            }
            if (!ctrl.noFetchByOnInit) {
                ctrl.fetch();
            }
            ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
                useExternalSorting: false,
                enableSorting: true,
                uiGridCustom: {
                    groupByField: ctrl.groupByReservationResource ? 'ReservationResourceId' : null,
                    groupByFieldTitle: ctrl.groupByReservationResource ? 'ReservationResourceName' : null,
                },
                onRegisterApi: function (gridApi) {
                    ctrl.gridServicesApi = gridApi;
                },
                columnDefs: [
                    {
                        name: 'ArtNo',
                        displayName: $translate.instant('Admin.Js.BookingServices.ArtNo'),
                        //cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{COL_FIELD}}</span></div>',
                        width: 80,
                    },
                    {
                        name: 'Name',
                        displayName: $translate.instant('Admin.Js.BookingServices.Name'),
                        //cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{COL_FIELD}}</span></div>',
                    },
                    {
                        name: 'Count',
                        displayName: 'Кол-во',
                        width: 100,
                        sortingAlgorithm: function (a, b, rowA, rowB, direction) {
                            return ctrl.sortAlg(a, b, rowA, rowB, direction);
                        },
                    },
                    {
                        name: 'Sum',
                        displayName: 'Сумма',
                        width: 100,
                        sortingAlgorithm: function (a, b, rowA, rowB, direction) {
                            return ctrl.sortAlg(a, b, rowA, rowB, direction);
                        },
                    },
                ],
            });
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    servicesAnalytics: ctrl,
                });
            }
        };
        ctrl.fetch = function () {
            $http
                .get('bookingAnalytics/getServices', {
                    params: {
                        affiliateId: ctrl.affiliateId,
                        reservationResourceId: ctrl.reservationResourceId,
                        status: ctrl.status,
                        noStatus: ctrl.noStatus,
                        isPaid: ctrl.paid,
                        dateFrom: ctrl.dateFrom,
                        dateTo: ctrl.dateTo,
                        groupByReservationResource: ctrl.groupByReservationResource,
                    },
                })
                .then(function (result) {
                    ctrl.ServicesAnalyticsData = result.data;
                    if (ctrl.gridServices != null) {
                        ctrl.gridServices.gridOptions.uiGridCustom.groupByField = ctrl.groupByReservationResource ? 'ReservationResourceId' : null;
                        ctrl.gridServices.gridOptions.uiGridCustom.groupByFieldTitle = ctrl.groupByReservationResource
                            ? 'ReservationResourceName'
                            : null;
                        ctrl.gridServices.gridOptions.data = ctrl.ServicesAnalyticsData;
                    }
                });
        };
        ctrl.sortAlg = function (a, b, rowA, rowB, direction) {
            var nulls = ctrl.gridServicesApi.core.sortHandleNulls(a, b);
            if (nulls !== null) {
                if (nulls === 1) {
                    return -1;
                }
                if (nulls === -1) {
                    return 1;
                }
                return nulls;
            } else {
                if (a > b) {
                    return 1;
                }
                if (a < b) {
                    return -1;
                }
                // a должно быть равным b
                return 0;
            }
        };
        ctrl.$onDestroy = function () {
            if (ctrl.removeCallbackUpdateBookings) {
                ctrl.removeCallbackUpdateBookings();
                ctrl.removeCallbackUpdateBookings = null;
            }
        };
    };
    ServicesAnalyticsCtrl.$inject = ['$translate', '$http', 'uiGridCustomConfig', 'adminWebNotificationsEvents', 'adminWebNotificationsService'];
    ng.module('bookingAnalytics')
        .controller('ServicesAnalyticsCtrl', ServicesAnalyticsCtrl)
        .component('servicesAnalytics', {
            templateUrl: servicesTemplate,
            controller: ServicesAnalyticsCtrl,
            bindings: {
                onInit: '&',
                noFetchByOnInit: '<?',
                groupByReservationResource: '<?',
                autoFetchByNewBooking: '<?',
                affiliateId: '<?',
                reservationResourceId: '<?',
                dateFrom: '<?',
                dateTo: '<?',
                paid: '<?',
                status: '<?',
                noStatus: '<?',
            },
        });
})(window.angular);
