(function (ng) {
    'use strict';

    var ModalReservationResourceShedulerCtrl = function (
        $uibModalInstance,
        $q,
        $http,
        toaster,
        SweetAlert,
        $translate,
        bookingService,
        $compile,
        $scope,
        $timeout,
        adminWebNotificationsEvents,
        adminWebNotificationsService,
    ) {
        var ctrl = this;

        ctrl.isInit = false;
        ctrl.reservationResourcesChanged = [];

        $scope.$on('modal.closing', function (event, result) {
            if (ctrl.mode === 'edit' && result && !result.reservationResourcesChanged) {
                event.preventDefault();
                ctrl.close();
            } else {
                if (ctrl.removeCallbackUpdateBookings) {
                    ctrl.removeCallbackUpdateBookings();
                    ctrl.removeCallbackUpdateBookings = null;
                }
            }
        });

        ctrl.calendarOptionsEdit = {
            height: 'auto',
            header: {
                left: 'title',
                center: '',
                right: 'today prev,next, prevMonth',
            },
            editable: true,
            dragScroll: true,
            selectOverlap: false,
            eventOverlap: false,
            defaultView: 'agendaWeek',
            allDaySlot: false,
            slotEventOverlap: false,
            agendaEventMinHeight: 20, //px
            slotLabelFormat: 'HH:mm',
            columnHeaderFormat: 'ddd D MMM',
            columnHeaderHtml: function (mom) {
                return '<div>' + mom.format('ddd D MMM') + '</div>';
            },
            eventRender: function (event, element, view) {
                if (event.description) {
                    element.popover({
                        //title: event.title,
                        content: event.description,
                        trigger: 'hover',
                        placement: 'top',
                        container: 'body',
                    });
                }
            },
            eventAfterAllRender: function (event) {
                $timeout(function () {
                    $compile(event.el.find('.fc-head'))($scope);
                });
            },
            eventClick: function (event) {
                ctrl.loadBooking(event.id);
            },
            dayClick: function (date, jsEvent) {
                if (!ctrl.readOnly && jsEvent.target.className.indexOf('fc-bgevent') === -1) {
                    var dateStart = bookingService.transformDate(date, false, true);
                    var dateEnd = bookingService.transformDate(moment(date).add(moment.duration(ctrl.bookingDuration)), false, true);
                    ctrl.loadBooking(undefined, ctrl.affiliateId, dateStart, dateEnd, ctrl.reservationResourceId.toString());
                }
            },
            eventResizeStart: function (event, jsEvent, ui, view) {
                ctrl.dateResizeStart = new Date();
            },
            eventResizeStop: function (event, jsEvent, ui, view) {
                ctrl.dateResizeStop = new Date();
            },
            eventResize: function (event, delta, revertFunc, jsEvent, ui, view) {
                $q.when(
                    ctrl.dateResizeStart && ctrl.dateResizeStop && ctrl.dateResizeStop - ctrl.dateResizeStart >= 100 // время перетаскивания больше 100мс
                        ? true
                        : SweetAlert.confirm('Кажется это действие выполнено случайно. Вы действительно хотите изменить продолжительность брони?', {
                              title: 'Изменение брони',
                              confirmButtonText: 'Да, изменить',
                          }),
                )
                    .then(function (result) {
                        if (result === true || result.value === true) {
                            ctrl.updateBookingAfterDrag(event.id, event.reservationResourceId, event.start, event.end, revertFunc, false);
                            return true;
                        } else {
                            return $q.reject('Exception');
                        }
                    })
                    .catch(function () {
                        if (revertFunc) {
                            revertFunc();
                        } else {
                            ctrl.fetchData();
                        }
                    });
            },
            eventDragStart: function (event, jsEvent, ui, view) {
                ctrl.dateDragStart = new Date();
            },
            eventDragStop: function (event, jsEvent, ui, view) {
                ctrl.dateDragStop = new Date();
            },
            eventDrop: function (event, delta, revertFunc, jsEvent, ui, view) {
                $q.when(
                    ctrl.dateDragStart && ctrl.dateDragStop && ctrl.dateDragStop - ctrl.dateDragStart >= 100 // время перетаскивания больше 100мс
                        ? true
                        : SweetAlert.confirm('Кажется это действие выполнено случайно. Вы действительно хотите перенести бронь?', {
                              title: 'Перенос брони',
                              confirmButtonText: 'Да, перенести',
                          }),
                )
                    .then(function (result) {
                        if (result === true || result.value === true) {
                            ctrl.updateBookingAfterDrag(event.id, event.reservationResourceId, event.start, event.end, revertFunc, false);
                            return true;
                        } else {
                            return $q.reject('Exception');
                        }
                    })
                    .catch(function () {
                        if (revertFunc) {
                            revertFunc();
                        } else {
                            ctrl.fetchData();
                        }
                    });
            },
            events: function (start, end, timezone, callback) {
                ctrl.fetchDataFullCalendar(start, end, timezone, callback);
            },
        };

        ctrl.calendarOptionsSelectFreeTime = {
            height: 'auto',
            header: {
                left: 'title',
                center: '',
                right: 'today prev,next, prevMonth',
            },
            editable: false,
            dragScroll: false,
            selectOverlap: false,
            eventOverlap: false,
            defaultView: 'agendaWeek',
            allDaySlot: false,
            slotEventOverlap: false,
            agendaEventMinHeight: 20, //px
            slotLabelFormat: 'HH:mm',
            columnHeaderFormat: 'ddd D MMM',
            columnHeaderHtml: function (mom) {
                return (
                    '<div sticky sticky-top="0" sticky-group="employeeShedulerModal" sticky-spy=".modal-body-employee-sheduler" sticky-position-type="absolute">' +
                    mom.format('ddd D MMM') +
                    '</div>'
                );
            },
            eventAfterAllRender: function (event) {
                $timeout(function () {
                    $compile(event.el.find('.fc-head'))($scope);
                });
            },
            dayClick: function (date, jsEvent) {
                if (jsEvent.target.className.indexOf('fc-bgevent') === -1) {
                    var dateStart = bookingService.transformDate(date, false, true);
                    var dateEnd = bookingService.transformDate(moment(date).add(moment.duration(ctrl.bookingDuration)), false, true);

                    $uibModalInstance.close({ beginDate: dateStart, endDate: dateEnd });
                }
            },
            events: function (start, end, timezone, callback) {
                ctrl.fetchDataFullCalendar(start, end, timezone, callback);
            },
        };

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;

            ctrl.affiliateId = params.affiliateId;
            ctrl.reservationResourceId = params.reservationResourceId;
            ctrl.date = params.date;
            ctrl.slotHeightPx = params.slotHeightPx;
            ctrl.mode = params.mode || 'edit';

            ctrl.name = params.name;
            ctrl.bookingDuration = params.bookingDuration;

            //ctrl.reservationResourceChanged(ctrl.reservationResourceId);

            ctrl.removeCallbackUpdateBookings = adminWebNotificationsService.addListener(adminWebNotificationsEvents.updateBookings, function (data) {
                if (data.affiliateId == ctrl.affiliateId && data.reservationResourceId == ctrl.reservationResourceId) {
                    ctrl.fetchData();
                }
            });

            ctrl.getReservationResourceShedulerForm().then(function (result) {
                if (result) {
                    ctrl.calendarOptionsEdit.editable = !ctrl.readOnly;
                    ctrl.isInit = true;
                }
            });
        };

        ctrl.getReservationResourceShedulerForm = function () {
            return $http
                .post('bookingResources/getReservationResourceShedulerFormData', {
                    affiliateId: ctrl.affiliateId,
                    reservationResourceId: ctrl.reservationResourceId,
                })
                .then(function (response) {
                    var data = response.data;

                    if (data.result === true) {
                        ctrl.name = data.obj.Name;
                        ctrl.bookingDuration = data.obj.BookingDuration;
                        ctrl.readOnly = !data.obj.AccessToEditing;
                        ctrl.accessToViewBooking = data.obj.AccessToViewBooking;

                        return true;
                    } else {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });

                        if (!data.errors) {
                            toaster.pop('error', 'Не удалось подгрузить дополнительные данны');
                        }

                        $uibModalInstance.dismiss('cancel');

                        return false;
                    }
                });
        };

        ctrl.fetchData = function () {
            ctrl.fullCalendar.refetchEvents();
        };

        ctrl.fetchDataFullCalendar = function (start, end, timezone, callback) {
            ctrl.shedulerProcessing = true;

            return $http
                .post('booking/getReservationResourceJournal', {
                    affiliateId: ctrl.affiliateId,
                    reservationResourceId: ctrl.reservationResourceId,
                    startDate: start.toISOString(),
                    endDate: end.toISOString(),
                })
                .then(function (response) {
                    var data = response.data;

                    if (data.result === true) {
                        ctrl.fullCalendar.option({
                            minTime: data.obj.MinTime,
                            maxTime: data.obj.MaxTime,
                        });
                        callback(data.obj.Events || []);
                    } else {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });

                        if (!data.errors) {
                            toaster.pop('error', 'Не удалось загрузить расписание');
                        }
                    }

                    ctrl.shedulerProcessing = false;
                });
        };

        ctrl.loadBooking = function (id, affiliateId, beginDate, endDate, reservationResourceId) {
            if (ctrl.accessToViewBooking) {
                var fnModalBookingClose = function (result) {
                    if (result && (result.bookingBeforeChange || result.bookingAfteChange)) {
                        if (result.bookingBeforeChange) {
                            ctrl.reservationResourceChanged(result.bookingBeforeChange.reservationResourceId);
                        }
                        if (result.bookingAfteChange) {
                            ctrl.reservationResourceChanged(result.bookingAfteChange.reservationResourceId);
                        }
                    }
                };

                bookingService.showBookingModal(id, affiliateId, beginDate, endDate, reservationResourceId).result.then(
                    function (result) {
                        ctrl.fetchData();
                        fnModalBookingClose(result);
                        return result;
                    },
                    function (result) {
                        fnModalBookingClose(result);
                        return result;
                    },
                );
            }
        };

        ctrl.updateBookingAfterDrag = function (id, reservationResourceId, beginMoment, endMoment, revertFunc, userConfirmed) {
            var beginDate = bookingService.transformDate(beginMoment);
            var endDate = bookingService.transformDate(endMoment);

            return bookingService
                .updateBookingAfterDrag(id, reservationResourceId, beginDate, endDate, userConfirmed)
                .then(function (data) {
                    if (data.result === true) {
                        if (data.obj && data.obj.UserConfirmIsRequired) {
                            SweetAlert.confirm(data.obj.ConfirmMessage, {
                                title: $translate.instant('Admin.Js.BookingJournal.SaveReservation'),
                                confirmButtonText: data.obj.ConfirmButtomText,
                            }).then(function (result) {
                                if (result === true || result.value === true) {
                                    ctrl.updateBookingAfterDrag(id, reservationResourceId, beginMoment, endMoment, revertFunc, true);
                                } else {
                                    if (revertFunc) {
                                        revertFunc();
                                    } else {
                                        ctrl.fetchData();
                                    }
                                }
                            });
                        } else {
                            toaster.pop('success', '', $translate.instant('Admin.Js.BookingJournal.ReservationSuccessfullySaved'));

                            ctrl.reservationResourceChanged(reservationResourceId);
                            ctrl.fetchData();
                        }
                    } else {
                        if (revertFunc) {
                            revertFunc();
                        } else {
                            ctrl.fetchData();
                        }

                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });

                        if (!data.errors) {
                            toaster.pop('error', $translate.instant('Admin.Js.BookingJournal.ErrorWhileSavingReservation'));
                        }
                    }
                })
                .catch(function () {
                    if (revertFunc) {
                        revertFunc();
                    } else {
                        ctrl.fetchData();
                    }

                    toaster.pop('error', $translate.instant('Admin.Js.BookingJournal.ErrorSavingReservation'));
                });
        };

        ctrl.getCalendarOptions = function () {
            var calendarOptions = null;
            switch (ctrl.mode) {
                case 'select-free-time':
                    calendarOptions = ctrl.calendarOptionsSelectFreeTime;
                    break;
                default:
                    calendarOptions = ctrl.calendarOptionsEdit;
                    break;
            }
            return ng.extend({}, calendarOptions, {
                slotDuration: ctrl.bookingDuration,
                slotLabelInterval: ctrl.bookingDuration,
                defaultDate: ctrl.date,
                //minTime: column.MinTime,
                //maxTime: column.MaxTime
            });
        };

        ctrl.fullCalendarOnInit = function (calendar) {
            ctrl.fullCalendar = calendar;
        };

        ctrl.close = function () {
            $uibModalInstance.close({ reservationResourcesChanged: ctrl.reservationResourcesChanged });
        };

        ctrl.reservationResourceChanged = function (reservationResourceId) {
            if (typeof reservationResourceId !== 'number') {
                reservationResourceId = parseInt(reservationResourceId, 10);

                if (isNaN(reservationResourceId)) {
                    throw new Error('Неправильный формат данных: ' + reservationResourceId);
                }
            }
            if (ctrl.reservationResourcesChanged.indexOf(reservationResourceId) === -1) {
                ctrl.reservationResourcesChanged.push(reservationResourceId);
            }
        };
    };

    ModalReservationResourceShedulerCtrl.$inject = [
        '$uibModalInstance',
        '$q',
        '$http',
        'toaster',
        'SweetAlert',
        '$translate',
        'bookingService',
        '$compile',
        '$scope',
        '$timeout',
        'adminWebNotificationsEvents',
        'adminWebNotificationsService',
    ];

    ng.module('uiModal').controller('ModalReservationResourceShedulerCtrl', ModalReservationResourceShedulerCtrl);
})(window.angular);
