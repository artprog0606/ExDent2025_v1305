import bookingsViewGridTemplate from './../bookingJournal/modal/bookingsViewGrid/bookingsViewGrid.html';
(function (ng) {
    'use strict';

    var BookingAnalyticsCtrl = function ($uibModal, $q, $timeout) {
        var ctrl = this;
        ctrl.components = [];
        ctrl.bookingComponentsPromise = [];
        ctrl.$onInit = function () {
            ctrl.showTab('booking');
        };
        ctrl.showTab = function (tab) {
            if (ctrl.selectedTab !== tab) {
                ctrl.selectedTab = tab;
            }
            switch (tab) {
                case 'booking':
                    ctrl.getComponent('analyticsCommon').then(function () {
                        ctrl.components['analyticsCommon'].recalc(ctrl.dateFrom, ctrl.dateTo, ctrl.paid, ctrl.status);
                    });
                    ctrl.getComponent('turnover').then(function () {
                        ctrl.components['turnover'].recalc(ctrl.dateFrom, ctrl.dateTo, ctrl.paid, ctrl.status);
                    });
                    ctrl.getComponent('analyticsSources').then(function () {
                        ctrl.components['analyticsSources'].recalc(ctrl.dateFrom, ctrl.dateTo, ctrl.paid, ctrl.status);
                    });
                    ctrl.getComponent('analyticsPaymentMethods').then(function () {
                        ctrl.components['analyticsPaymentMethods'].recalc(ctrl.dateFrom, ctrl.dateTo, ctrl.paid, ctrl.status);
                    });
                    break;
                case 'reservationResources':
                    ctrl.getComponent('reservationResources').then(function () {
                        ctrl.components['reservationResources'].recalc(ctrl.dateFrom, ctrl.dateTo, ctrl.paid, ctrl.status);
                    });
                    break;
                case 'services':
                    $timeout(function () {
                        ctrl.getComponent('servicesAnalytics').then(function () {
                            ctrl.components['servicesAnalytics'].fetch();
                        });
                    }, 50);
                    break;
            }
        };
        ctrl.updateData = function () {
            ctrl.showTab(ctrl.selectedTab);
        };
        ctrl.servicesGroupByReservationResourceChange = function (checked) {
            ctrl.servicesGroupByReservationResource = checked;
            $timeout(function () {
                ctrl.getComponent('servicesAnalytics').then(function () {
                    ctrl.components['servicesAnalytics'].fetch();
                });
            }, 50);
        };
        ctrl.getServicesDateTo = function () {
            var dateTo = new Date(ctrl.dateTo);
            dateTo.setDate(dateTo.getDate() + 1); // add one day
            dateTo = dateTo.getFullYear() + '-' + ('0' + (dateTo.getMonth() + 1)).slice(-2) + '-' + ('0' + dateTo.getDate()).slice(-2);
            return dateTo;
        };
        ctrl.onInitCommon = function (analyticsCommon) {
            ctrl.components['analyticsCommon'] = analyticsCommon;
            if (ctrl.bookingComponentsPromise['analyticsCommon']) {
                ctrl.bookingComponentsPromise['analyticsCommon'].resolve();
            }
        };
        ctrl.onInitTurnover = function (turnover) {
            ctrl.components['turnover'] = turnover;
            if (ctrl.bookingComponentsPromise['turnover']) {
                ctrl.bookingComponentsPromise['turnover'].resolve();
            }
        };
        ctrl.onInitSources = function (analyticsSources) {
            ctrl.components['analyticsSources'] = analyticsSources;
            if (ctrl.bookingComponentsPromise['analyticsSources']) {
                ctrl.bookingComponentsPromise['analyticsSources'].resolve();
            }
        };
        ctrl.onInitPaymentMethods = function (analyticsPaymentMethods) {
            ctrl.components['analyticsPaymentMethods'] = analyticsPaymentMethods;
            if (ctrl.bookingComponentsPromise['analyticsPaymentMethods']) {
                ctrl.bookingComponentsPromise['analyticsPaymentMethods'].resolve();
            }
        };
        ctrl.onInitReservationResources = function (reservationResources) {
            ctrl.components['reservationResources'] = reservationResources;
            if (ctrl.bookingComponentsPromise['reservationResources']) {
                ctrl.bookingComponentsPromise['reservationResources'].resolve();
            }
        };
        ctrl.onInitServicesAnalytics = function (servicesAnalytics) {
            ctrl.components['servicesAnalytics'] = servicesAnalytics;
            if (ctrl.bookingComponentsPromise['servicesAnalytics']) {
                ctrl.bookingComponentsPromise['servicesAnalytics'].resolve();
            }
        };
        ctrl.getComponent = function (name) {
            if (ctrl.components[name]) {
                return $q.resolve();
            } else {
                ctrl.bookingComponentsPromise[name] = ctrl.bookingComponentsPromise[name] ? ctrl.bookingComponentsPromise[name] : $q.defer();
                return ctrl.bookingComponentsPromise[name].promise;
            }
        };
        ctrl.showBookings = function (params) {
            var fnModalBookingsViewClose = function (result) {
                if (result && (result.reservationResourcesChanged || result.bookingsChanged)) {
                    ctrl.showTab(ctrl.selectedTab);
                }
            };

            // грид фильтрует ДО даты, не ПО дату
            var dateTo = new Date(ctrl.dateTo);
            dateTo.setDate(dateTo.getDate() + 1); // add one day
            dateTo = dateTo.getFullYear() + '-' + ('0' + (dateTo.getMonth() + 1)).slice(-2) + '-' + ('0' + dateTo.getDate()).slice(-2);
            var baseParams = {
                affiliateId: ctrl.affiliateId,
                beginDateFrom: ctrl.dateFrom,
                endDateTo: dateTo,
                paid: ctrl.paid === 'null' ? undefined : ctrl.paid,
                status: ctrl.status === 'null' ? undefined : ctrl.status,
            };
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalBookingsViewGridCtrl',
                    controllerAs: 'ctrl',
                    size: 'xs-11',
                    backdrop: 'static',
                    templateUrl: bookingsViewGridTemplate,
                    resolve: {
                        params: ng.extend(baseParams, params || {}),
                    },
                })
                .result.then(
                    function (result) {
                        fnModalBookingsViewClose(result);
                        return result;
                    },
                    function (result) {
                        fnModalBookingsViewClose(result);
                        return result;
                    },
                );
        };
    };
    BookingAnalyticsCtrl.$inject = ['$uibModal', '$q', '$timeout'];
    ng.module('bookingAnalytics', []).controller('BookingAnalyticsCtrl', BookingAnalyticsCtrl);
})(window.angular);
