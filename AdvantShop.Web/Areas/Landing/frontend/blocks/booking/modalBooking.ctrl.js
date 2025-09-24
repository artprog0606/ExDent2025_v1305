(function (ng) {
    'use strict';

    var ModalBookingCtrl = function (
        $element,
        $http,
        $translate,
        $q,
        $filter,
        modalService,
        toaster,
        $window,
        trackingService,
        bookingCartService,
        Upload,
    ) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.modalId = 'modalBooking_' + ctrl.blockId + '_' + ctrl.resourceId + '_' + ctrl.affiliateId;
        };

        ctrl.$postLink = function () {
            $element[0].addEventListener('click', ctrl.showModal);
        };

        ctrl.showModal = function () {
            if (modalService.hasModal(ctrl.modalId) === false) {
                modalService.renderModal(
                    ctrl.modalId,
                    null,
                    '<div data-ng-include="\'areas/landing/frontend/blocks/booking/templates/modalBooking.html\'"></div>',
                    null,
                    {
                        modalClass: 'lp-modal-booking ' + (ctrl.colorScheme || 'color-scheme--light'),
                        callbackOpen: 'modalBooking.modalOpen()',
                        callbackClose: 'modalBooking.modalClose()',
                    },
                    {
                        modalBooking: ctrl,
                    },
                );
            }

            modalService.getModal(ctrl.modalId).then(function (modal) {
                modal.modalScope.open();
            });
        };

        ctrl.modalOpen = function () {
            ctrl.view = ctrl.showServices ? 'services' : 'date';
            if (ctrl.ngForm) {
                ctrl.ngForm.$setPristine();
            }

            ctrl.getFormData().then(function (data) {
                ctrl.fieldsTpl =
                    'landing/landing/formFields?id=' + ctrl.formData.Id + '&ngModel=modalBooking.form&isVertical=true&objId=' + ctrl.modalId;
            });
        };

        ctrl.modalClose = function () {
            ctrl.resultData = null;
            ctrl.showForm = false;
        };

        ctrl.initModalBooking = function (ngForm) {
            ctrl.ngForm = ngForm;
            ctrl.MonthFreeDays = [];
            ctrl.MonthFreeDaysStart = [];
            ctrl.MonthFreeDaysEnd = [];
            ctrl.selectedServices = [];
        };

        ctrl.showBookingForm = function () {
            ctrl.view = 'form';
        };

        ctrl.showDates = function () {
            ctrl.view = 'date';
        };

        ctrl.showServicesView = function () {
            ctrl.view = 'services';
        };

        ctrl.getMonthFreeDaysKey = function (date) {
            return 'y' + date.getFullYear() + 'm' + date.getMonth();
        };

        // region "booking by time"
        ctrl.setTime = function (time) {
            ctrl.time = time;
            if (!ctrl.shoppingCart) {
                ctrl.showBookingForm();
            }
        };

        ctrl.fpOptionsByTime = {
            inline: true,
            minDate: 'today',
            startDateFormat: 'd.m.Y',
            dateFormat: 'd.m.Y',
            defaultDate: new Date(),
            onMonthChange: function (selectedDates, dateStr, instance) {
                ctrl.calendarByTimeMonthYearChange(instance);
            },
            onYearChange: function (selectedDates, dateStr, instance) {
                ctrl.calendarByTimeMonthYearChange(instance);
            },
            disable: [
                function (date) {
                    var key = ctrl.getMonthFreeDaysKey(date);
                    return ctrl.MonthFreeDays[key] ? ctrl.MonthFreeDays[key].indexOf(date.getDate()) !== -1 : false;
                },
            ],
        };

        ctrl.onFlatpickrByTimeInit = function (flatpickrByTime) {
            ctrl.flatpickrByTime = flatpickrByTime;

            var promise;
            if (!ctrl.selectedDate) {
                promise = ctrl.getFreeDayByTime().then(function (date) {
                    ctrl.selectedDate = new Date(date);
                    ctrl.modelDate = $filter('ngFlatpickr')(ctrl.selectedDate, 'd.m.Y');
                    ctrl.flatpickrByTime.setDate(ctrl.selectedDate);
                    ctrl.flatpickrByTime.jumpToDate(ctrl.selectedDate);
                });
            } else {
                promise = $q.resolve();
            }

            promise.then(function () {
                ctrl.getMonthFreeDaysByTime(ctrl.flatpickrByTime.currentYear, ctrl.flatpickrByTime.currentMonth).then(function () {
                    ctrl.time = null;
                    ctrl.getBookingTimes();
                });
            });
        };

        ctrl.changeDateByTime = function () {
            ctrl.selectedDate = ctrl.flatpickrByTime.selectedDates[0];
            ctrl.time = null;
            ctrl.getBookingTimes();
        };

        ctrl.calendarByTimeMonthYearChange = function (instance) {
            ctrl.getMonthFreeDaysByTime(instance.currentYear, instance.currentMonth);
        };

        ctrl.getFreeDayByTime = function () {
            if (ctrl.selectedDate) {
                return $q.resolve();
            }

            return $http
                .get('landing/landing/GetFreeDayByTime', {
                    params: {
                        affiliateId: ctrl.affiliateId,
                        resourceId: ctrl.resourceId,
                        selectedServices: ctrl.selectedServices,
                    },
                })
                .then(function (response) {
                    var data = response.data;

                    if (data.result === true) {
                        return data.obj;
                    } else {
                        if (!data.errors) {
                            toaster.error('', 'Не удалось загрузить дни записи');
                        } else {
                            data.errors.forEach(function (error) {
                                toaster.error('', error);
                            });
                        }
                    }
                    return null;
                });
        };

        ctrl.getMonthFreeDaysByTime = function (year, month) {
            var key = ctrl.getMonthFreeDaysKey(new Date(year, month, 1));
            var nextKey = ctrl.getMonthFreeDaysKey(new Date(year, month + 1, 1));
            var prevKey = ctrl.getMonthFreeDaysKey(new Date(year, month - 1, 1));

            if (ctrl.MonthFreeDays[key] && ctrl.MonthFreeDays[prevKey] && ctrl.MonthFreeDays[nextKey]) {
                return $q.resolve();
            }

            return $http
                .get('landing/landing/GetBookingByTimeMonthFreeDays', {
                    params: {
                        affiliateId: ctrl.affiliateId,
                        resourceId: ctrl.resourceId,
                        year: year,
                        month: month + 1,
                        loadCurrentMonth: !ctrl.MonthFreeDays[key],
                        loadPrevMonth: !ctrl.MonthFreeDays[prevKey],
                        loadNextMonth: !ctrl.MonthFreeDays[nextKey],
                        selectedServices: ctrl.selectedServices,
                    },
                })
                .then(function (response) {
                    var data = response.data;

                    if (data.result === true) {
                        if (!data.obj) return;

                        if (data.obj.CurrentMonth) {
                            ctrl.MonthFreeDays[key] = data.obj.CurrentMonth;
                        }
                        if (data.obj.PrevMonth) {
                            ctrl.MonthFreeDays[prevKey] = data.obj.PrevMonth;
                        }
                        if (data.obj.NextMonth) {
                            ctrl.MonthFreeDays[nextKey] = data.obj.NextMonth;
                        }

                        ctrl.flatpickrByTime.redraw();
                    } else {
                        if (!data.errors) {
                            toaster.error('', 'Не удалось загрузить дни записи');
                        } else {
                            data.errors.forEach(function (error) {
                                toaster.error('', error);
                            });
                        }
                    }
                });
        };

        ctrl.getBookingTimes = function () {
            return $http
                .get('landing/landing/getBookingTimes', {
                    params: {
                        resourceId: ctrl.resourceId,
                        affiliateId: ctrl.affiliateId,
                        selectedDate: ctrl.flatpickrByTime.formatDate(ctrl.selectedDate, 'Y-m-dTH:i'),
                        selectedServices: ctrl.selectedServices,
                    },
                })
                .then(function (response) {
                    var data = response.data;

                    if (data.result === true) {
                        ctrl.times = data.obj.Times.slice(0);
                        ctrl.hasFreeTime = data.obj.HasFreeTime;
                    } else {
                        if (!data.errors) {
                            toaster.error('', 'Не удалось загрузить время записи');
                        } else {
                            data.errors.forEach(function (error) {
                                toaster.error('', error);
                            });
                        }
                    }
                });
        };
        // region end "booking by time"

        // region "booking by days"

        ctrl.fpStartOptions = {
            wrap: true,
            dateFormat: 'd.m.Y',
            startDateFormat: 'd.m.Y',
            minDate: 'today',
            defaultDate: new Date(),
            onMonthChange: function (selectedDates, dateStr, instance) {
                ctrl.calendarStartMonthYearChange(instance);
            },
            onYearChange: function (selectedDates, dateStr, instance) {
                ctrl.calendarStartMonthYearChange(instance);
            },
            disable: [
                function (date) {
                    var key = ctrl.getMonthFreeDaysKey(date);
                    return ctrl.MonthFreeDaysStart[key] ? ctrl.MonthFreeDaysStart[key].indexOf(date.getDate()) === -1 : false;
                },
            ],
        };

        ctrl.fpEndOptions = {
            wrap: true,
            dateFormat: 'd.m.Y',
            startDateFormat: 'd.m.Y',
            minDate: 'today',
            defaultDate: new Date(),
            onMonthChange: function (selectedDates, dateStr, instance) {
                ctrl.calendarEndMonthYearChange(instance);
            },
            onYearChange: function (selectedDates, dateStr, instance) {
                ctrl.calendarEndMonthYearChange(instance);
            },
            disable: [
                function (date) {
                    if (ctrl.startDateObj && date < ctrl.startDateObj) return true;
                    var key = ctrl.getMonthFreeDaysKey(date);
                    return ctrl.MonthFreeDaysEnd[key] ? ctrl.MonthFreeDaysEnd[key].indexOf(date.getDate()) === -1 : false;
                },
            ],
        };

        ctrl.onFlatpickrStartInit = function (flatpickrStart) {
            ctrl.flatpickrStart = flatpickrStart;

            var promise;
            if (!ctrl.startDateObj) {
                promise = ctrl.getStartDayByDays().then(function (date) {
                    if (date) {
                        ctrl.startDateObj = new Date(new Date(date).setHours(0, 0, 0, 0));
                        ctrl.flatpickrStart.setDate(ctrl.startDateObj);
                        ctrl.flatpickrStart.jumpToDate(ctrl.startDateObj);
                        ctrl.startDate = ctrl.flatpickrStart.formatDate(ctrl.startDateObj, ctrl.fpStartOptions.startDateFormat);
                        ctrl.changeStartDate(true);
                    }
                });
            } else {
                promise = $q.resolve();
            }

            promise.then(function () {
                ctrl.getMonthDaysForStart(ctrl.flatpickrStart.currentYear, ctrl.flatpickrStart.currentMonth).then(function () {});
            });
        };

        ctrl.onFlatpickrEndInit = function (flatpickrEnd) {
            ctrl.flatpickrEnd = flatpickrEnd;
            if (ctrl.startDateObj) {
                ctrl.getMonthDaysForEnd(ctrl.flatpickrEnd.currentYear, ctrl.flatpickrEnd.currentMonth).then(function () {
                    //ctrl.flatpickrEnd.setDate(ctrl.endDateObj);
                    //ctrl.flatpickrEnd.jumpToDate(ctrl.endDateObj);
                });
            }
        };

        ctrl.changeStartDate = function (forceUpdate) {
            var startDate = ctrl.flatpickrStart.selectedDates[0];
            var selectedDate;
            if (startDate != null) {
                selectedDate = new Date(ctrl.flatpickrStart.selectedDates[0].setHours(0, 0, 0, 0));

                if (forceUpdate || ctrl.startDateObj.getTime() != selectedDate.getTime()) {
                    ctrl.startDateObj = selectedDate;
                    if (ctrl.endDateObj < ctrl.startDateObj) {
                        ctrl.endDate = null;
                        ctrl.endDateObj = null;
                    }
                    ctrl.flatpickrEnd.jumpToDate(ctrl.startDateObj);
                    ctrl.MonthFreeDaysEnd = [];
                    if (ctrl.flatpickrEnd) {
                        ctrl.calendarEndMonthYearChange(ctrl.flatpickrEnd).then(function () {
                            if (ctrl.endDateObj) {
                                var key = ctrl.getMonthFreeDaysKey(ctrl.endDateObj);
                                if (ctrl.MonthFreeDaysEnd[key].indexOf(ctrl.endDateObj.getDate()) === -1) {
                                    ctrl.endDate = null;
                                    ctrl.endDateObj = null;
                                }
                            }
                        });
                    }
                }
            }
        };

        ctrl.changeEndDate = function () {
            var endDate = ctrl.flatpickrEnd.selectedDates[0];

            if (endDate != null) {
                ctrl.endDateObj = new Date(endDate.setHours(0, 0, 0, 0));
            }
        };

        ctrl.calendarStartMonthYearChange = function (instance) {
            return ctrl.getMonthDaysForStart(instance.currentYear, instance.currentMonth);
        };

        ctrl.calendarEndMonthYearChange = function (instance) {
            return ctrl.getMonthDaysForEnd(instance.currentYear, instance.currentMonth);
        };

        ctrl.getStartDayByDays = function () {
            if (ctrl.startDateObj) {
                return $q.resolve();
            }

            return $http
                .get('landing/landing/GetStartDayByDays', {
                    params: {
                        affiliateId: ctrl.affiliateId,
                        resourceId: ctrl.resourceId,
                        timeFrom: ctrl.timeFrom,
                        timeEnd: ctrl.timeEnd,
                        timeEndAtNextDay: ctrl.timeEndAtNextDay,
                        selectedServices: ctrl.selectedServices,
                    },
                })
                .then(function (response) {
                    var data = response.data;

                    if (data.result === true) {
                        return data.obj;
                    } else {
                        if (!data.errors) {
                            toaster.error('', 'Не удалось загрузить дни записи');
                        } else {
                            data.errors.forEach(function (error) {
                                toaster.error('', error);
                            });
                        }
                    }
                    return null;
                });
        };

        ctrl.getMonthDaysForStart = function (year, month) {
            return ctrl.getMonthDaysByDay('start', year, month);
        };

        ctrl.getMonthDaysForEnd = function (year, month) {
            return ctrl.getMonthDaysByDay('end', year, month);
        };

        ctrl.getMonthDaysByDay = function (typeDate, year, month) {
            var key = ctrl.getMonthFreeDaysKey(new Date(year, month, 1));
            var nextKey = ctrl.getMonthFreeDaysKey(new Date(year, month + 1, 1));
            var prevKey = ctrl.getMonthFreeDaysKey(new Date(year, month - 1, 1));

            var flatpickr = typeDate === 'start' ? ctrl.flatpickrStart : ctrl.flatpickrEnd;
            var monthFreeDays = typeDate === 'start' ? ctrl.MonthFreeDaysStart : ctrl.MonthFreeDaysEnd;
            var url = typeDate === 'start' ? 'landing/landing/GetBookingByDaysMonthStartDays' : 'landing/landing/GetBookingByDaysMonthEndDays';

            if ((typeDate === 'end' || ctrl.startDateObj) && monthFreeDays[key] && monthFreeDays[prevKey] && monthFreeDays[nextKey]) {
                return $q.resolve();
            }

            return $http
                .get(url, {
                    params: {
                        affiliateId: ctrl.affiliateId,
                        resourceId: ctrl.resourceId,
                        year: year,
                        month: month + 1,
                        loadCurrentMonth: !monthFreeDays[key],
                        loadPrevMonth: !monthFreeDays[prevKey],
                        loadNextMonth: !monthFreeDays[nextKey],
                        startDate: typeDate === 'start' ? null : ctrl.flatpickrStart.formatDate(new Date(ctrl.startDateObj), 'Y-m-d'),
                        timeFrom: ctrl.timeFrom,
                        timeEnd: ctrl.timeEnd,
                        timeEndAtNextDay: ctrl.timeEndAtNextDay,
                        selectedServices: ctrl.selectedServices,
                    },
                })
                .then(function (response) {
                    var data = response.data;

                    if (data.result === true) {
                        if (!data.obj) return;

                        if (data.obj.CurrentMonth) {
                            monthFreeDays[key] = data.obj.CurrentMonth;
                        }
                        if (data.obj.PrevMonth) {
                            monthFreeDays[prevKey] = data.obj.PrevMonth;
                        }
                        if (data.obj.NextMonth) {
                            monthFreeDays[nextKey] = data.obj.NextMonth;
                        }

                        flatpickr.redraw();
                    } else {
                        if (!data.errors) {
                            toaster.error('', 'Не удалось загрузить доступные дни');
                        } else {
                            data.errors.forEach(function (error) {
                                toaster.error('', error);
                            });
                        }
                    }
                });
        };
        // region end "booking by days"

        ctrl.getFormData = function () {
            return $http
                .get('landing/landing/getBookingFormData', {
                    params: {
                        resourceId: ctrl.resourceId,
                        affiliateId: ctrl.affiliateId,
                        blockId: ctrl.blockId,
                        loadServices: ctrl.showServices,
                        lpId: ctrl.lpId,
                    },
                })
                .then(function (response) {
                    var data = response.data;

                    if (data.result === true) {
                        ctrl.formData = data.obj.form;
                        ctrl.categories = data.obj.categories;
                        if (ctrl.categories && ctrl.categories.length === 1) {
                            ctrl.categories[0].expanded = true;
                        }
                        ctrl.shoppingCart = data.obj.shoppingCart;
                    } else {
                        if (!data.errors) {
                            toaster.error('', 'Не удалось загрузить форму');
                        } else {
                            data.errors.forEach(function (error) {
                                toaster.error('', error);
                            });
                        }
                    }
                });
        };

        ctrl.submit = function () {
            ctrl.formSubmitInProcess = true;

            var beginDate, endDate;
            if (!ctrl.bookingByDays) {
                var dateStr = ctrl.flatpickrByTime.formatDate(ctrl.flatpickrByTime.selectedDates[0], 'Y-m-d');
                beginDate = dateStr + 'T' + ctrl.time.From;
                endDate = dateStr + 'T' + ctrl.time.To;
            } else {
                beginDate = ctrl.flatpickrStart.formatDate(ctrl.flatpickrStart.selectedDates[0], 'Y-m-d') + 'T' + ctrl.timeFrom;
                endDate = ctrl.flatpickrEnd.formatDate(ctrl.flatpickrEnd.selectedDates[0], 'Y-m-d') + 'T' + ctrl.timeEnd;
            }

            var promise;

            if (!ctrl.shoppingCart) {
                promise = Upload.upload({
                    url: 'landing/landing/addBooking',
                    data: {
                        form: ng.extend(ctrl.form, { id: ctrl.formData.Id, blockId: ctrl.blockId }),
                        beginDate: beginDate,
                        endDate: endDate,
                        affiliateId: ctrl.affiliateId,
                        resourceId: ctrl.resourceId,
                        selectedServices: ctrl.selectedServices,
                        lid: ctrl.lid,
                        lpId: ctrl.lpId,
                    },
                }).then(function (response) {
                    var data = response.data;
                    if (data.result) {
                        ctrl.resultData = data.obj;

                        if (
                            data.obj.PostAction === 'RedrectToUrl' ||
                            data.obj.PostAction === 'RedrectToUrlAndEmail' ||
                            data.obj.PostAction === 'RedirectToCheckout'
                        ) {
                            $window.location.assign(data.obj.RedirectUrl);
                        }
                    } else {
                        if (!data.errors) {
                            toaster.error('', 'Ошибка при бронировании');
                        } else {
                            data.errors.forEach(function (error) {
                                toaster.error('', error);
                            });
                        }
                    }

                    return data;
                });
            } else {
                promise = bookingCartService
                    .addItem(beginDate, endDate, ctrl.affiliateId, ctrl.resourceId, ctrl.selectedServices)
                    .then(function (result) {
                        var data = result[0];

                        if (data.result) {
                            modalService.close(ctrl.modalId);
                        } else {
                            if (!data.errors) {
                                toaster.error('', 'Ошибка при бронировании');
                            } else {
                                data.errors.forEach(function (error) {
                                    toaster.error('', error);
                                });
                            }
                        }
                        return data;
                    });
            }

            promise.then(function (data) {
                if (data.result) {
                    ctrl.form = null;
                    ctrl.selectedDate = null;
                    ctrl.selectedServices = [];
                    ctrl.modelDate = null;
                    ctrl.startDate = null;
                    ctrl.endDate = null;
                    ctrl.time = null;
                }
            });

            promise.finally(function () {
                ctrl.formSubmitInProcess = false;

                if (ctrl.yaMetrikaEventName != null && ctrl.yaMetrikaEventName.length > 0) {
                    trackingService.trackYaEvent(ctrl.yaMetrikaEventName);
                }

                if (ctrl.gaEventAction != null && ctrl.gaEventAction.length > 0) {
                    trackingService.trackGaEvent(ctrl.gaEventCategory, ctrl.gaEventAction);
                }
            });

            return promise;
        };
    };

    ng.module('modalBooking').controller('ModalBookingCtrl', ModalBookingCtrl);

    ModalBookingCtrl.$inject = [
        '$element',
        '$http',
        '$translate',
        '$q',
        '$filter',
        'modalService',
        'toaster',
        '$window',
        'trackingService',
        'bookingCartService',
        'Upload',
    ];
})(window.angular);
