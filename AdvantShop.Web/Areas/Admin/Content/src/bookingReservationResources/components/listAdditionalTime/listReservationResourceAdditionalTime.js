import listReservationResourceAdditionalTimeTemplate from './listReservationResourceAdditionalTime.html';
import addUpdateAdditionalTimeTemplate from '../../modal/addUpdateAdditionalTime/addUpdateAdditionalTimeReservationResources.html';
(function (ng) {
    'use strict';

    var ListReservationResourceAdditionalTimeCtrl = function ($http, SweetAlert, toaster, $uibModal, $translate) {
        var ctrl = this;

        ctrl.YearAdditionalDate = {};

        ctrl.calendarAdditionalTimeOptions = {
            clickDay: function (e) {
                if (!ctrl.readonly || e.element[0].childNodes[0].className.indexOf('day-additional-time') !== -1) {
                    ctrl.loadAdditionalTime(e.date);
                }
            },
            customDayRenderer: function (e, d) {
                // element, date
                if (ctrl.YearAdditionalDate['y' + d.getFullYear()]) {
                    if (ctrl.YearAdditionalDate['y' + d.getFullYear()].indexOf(d.getTime()) !== -1) {
                        e[0].className += ' day-additional-time';
                    } /* else if (ctrl.readonly) {
                        e[0].parentNode.className += " disabled";
                    }*/
                }
            },
            renderEnd: function (e) {
                if (!ctrl.YearAdditionalDate['y' + e.currentYear]) {
                    var $calendar = ctrl.calendarAdditionalTime ? $(ctrl.calendarAdditionalTime.element) : $(e.target);
                    $calendar.addClass('calendar-processing');
                    ctrl.getYearAdditionalDate(e.currentYear).then(function () {
                        ctrl.calendarAdditionalTime.setYear(e.currentYear);
                        $calendar.removeClass('calendar-processing');
                    });
                }
            },
        };

        ctrl.$onInit = function () {
            if (ctrl.onInit != null) {
                ctrl.onInit();
            }
        };

        ctrl.getYearAdditionalDate = function (year) {
            return $http
                .get('bookingResources/getYearAdditionalDate', {
                    params: {
                        affiliateId: ctrl.affiliateId,
                        reservationResourceId: ctrl.reservationResourceId,
                        year: year,
                    },
                })
                .then(function (response) {
                    var data = response.data;

                    if (data.result === true) {
                        ctrl.YearAdditionalDate['y' + year] = data.obj.map(function (d) {
                            return new Date(d).getTime();
                        });
                    } else {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });

                        if (!data.errors) {
                            toaster.pop(
                                'error',
                                $translate.instant('Admin.Js.BookingUsers.Error'),
                                $translate.instant('Admin.Js.BookingUsers.FailedToLoadDataForYear'),
                            );
                        }
                    }
                });
        };

        ctrl.calendarAdditionalTimeOnInit = function (calendar) {
            ctrl.calendarAdditionalTime = calendar;
        };

        ctrl.loadAdditionalTime = function (date) {
            if (date instanceof Date) {
                date = date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
            }

            var params = {
                affiliateId: ctrl.affiliateId,
                reservationResourceId: ctrl.reservationResourceId,
                date: date,
            };

            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddUpdateReservationResourceAdditionalTimeCtrl',
                    controllerAs: 'ctrl',
                    size: 'lg',
                    backdrop: 'static',
                    templateUrl: addUpdateAdditionalTimeTemplate,
                    resolve: {
                        params: params,
                    },
                })
                .result.then(
                    function (result) {
                        ctrl.onAdditionalTimeAddUpdate();
                        return result;
                    },
                    function (result) {
                        //ctrl.onAdditionalTimeAddUpdate();
                        return result;
                    },
                );
        };

        ctrl.onAdditionalTimeAddUpdate = function () {
            ctrl.YearAdditionalDate = {};
            ctrl.calendarAdditionalTime.setYear(ctrl.calendarAdditionalTime.getYear());
        };
    };

    ListReservationResourceAdditionalTimeCtrl.$inject = ['$http', 'SweetAlert', 'toaster', '$uibModal', '$translate'];

    ng.module('listReservationResourceAdditionalTime', [])
        .controller('ListReservationResourceAdditionalTimeCtrl', ListReservationResourceAdditionalTimeCtrl)
        .component('listReservationResourceAdditionalTime', {
            templateUrl: listReservationResourceAdditionalTimeTemplate,
            controller: 'ListReservationResourceAdditionalTimeCtrl',
            transclude: true,
            bindings: {
                onInit: '&',
                affiliateId: '<',
                reservationResourceId: '<',
                readonly: '<?',
            },
        });
})(window.angular);
