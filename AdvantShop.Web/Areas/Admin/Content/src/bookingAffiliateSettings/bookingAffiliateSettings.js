import addUpdateSmsTemplateTemplate from '../bookingAffiliate/modals/addUpdateSmsTemplate/addUpdateSmsTemplate.html';
import addUpdateAdditionalTimeTemplate from '../bookingAffiliate/modals/addUpdateAdditionalTime/addUpdateAdditionalTimeAffiliate.html';
(function (ng) {
    'use strict';

    var BookingAffiliateSettingsCtrl = function (
        $http,
        $q,
        uiGridConstants,
        uiGridCustomConfig,
        SweetAlert,
        toaster,
        $uibModal,
        bookingAffiliateService,
        $translate,
        bookingService,
        $window,
    ) {
        var ctrl = this;
        ctrl.isShowedAlertChangeBookingInterval = false;
        ctrl.YearAdditionalDate = {};

        ctrl.calendarAdditionalTimeOptions = {
            enableRangeSelection: true,
            selectRange: function (e) {
                if (e.startDate !== e.endDate) {
                    ctrl.loadAdditionalTime(e.startDate, e.endDate);
                }
            },
            clickDay: function (e) {
                ctrl.loadAdditionalTime(e.date);
            },
            customDayRenderer: function (e, d) {
                // element, date
                if (ctrl.YearAdditionalDate['y' + d.getFullYear()]) {
                    if (ctrl.YearAdditionalDate['y' + d.getFullYear()].indexOf(d.getTime()) !== -1) {
                        e[0].className += ' day-additional-time';
                    }
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
            ctrl.daysOfTheWeek = ['понедельник', 'вторник', 'среда', 'четверг', 'пятница', 'суббота', 'воскресенье'];
            ctrl.dayOfTheWeek = ctrl.daysOfTheWeek[0];
        };

        ctrl.initBookingAffiliateSettings = function (affiliateId) {
            ctrl.affiliateId = affiliateId;
            ctrl.getAdditionalData();
        };

        ctrl.getYearAdditionalDate = function (year) {
            return $http
                .get('bookingAffiliate/getYearAdditionalDate', {
                    params: { affiliateId: ctrl.affiliateId, year: year },
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
                                $translate.instant('Admin.Js.BookingAffiliateSettings.Error'),
                                $translate.instant('Admin.Js.BookingAffiliateSettings.FailedDataForYear'),
                            );
                        }
                    }
                });
        };

        ctrl.getAdditionalData = function () {
            return $http
                .get('bookingAffiliate/getSettingsAdditionalData', {
                    params: { affiliateId: ctrl.affiliateId },
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        ctrl.managers = data.obj.ManagersForAccess;
                        ctrl.managersForAnalytic = data.obj.ManagersForAnalytic;
                    } else {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });

                        if (!data.errors) {
                            toaster.pop(
                                'error',
                                $translate.instant('Admin.Js.BookingAffiliateSettings.Error'),
                                $translate.instant('Admin.Js.BookingAffiliateSettings.FailedToLoadAdditionalData'),
                            );
                        }
                    }
                });
        };

        ctrl.getTimes = function () {
            return $http
                .get('bookingAffiliate/getSettingsTimesOfBooking', {
                    params: {
                        affiliateId: ctrl.affiliateId,
                        intervalMinutes: ctrl.affiliateBookingIntervalMinutes,
                    },
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        ctrl.times = data.obj.Times;
                        ctrl.mondayTimes = data.obj.MondayTimes;
                        ctrl.tuesdayTimes = data.obj.TuesdayTimes;
                        ctrl.wednesdayTimes = data.obj.WednesdayTimes;
                        ctrl.thursdayTimes = data.obj.ThursdayTimes;
                        ctrl.fridayTimes = data.obj.FridayTimes;
                        ctrl.saturdayTimes = data.obj.SaturdayTimes;
                        ctrl.sundayTimes = data.obj.SundayTimes;
                    } else {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });

                        if (!data.errors) {
                            toaster.pop(
                                'error',
                                $translate.instant('Admin.Js.BookingAffiliateSettings.Error'),
                                $translate.instant('Admin.Js.BookingAffiliateSettings.FailedToLoadAdditionalData'),
                            );
                        }
                    }
                });
        };

        ctrl.OnChangeBookingInterval = function () {
            if (!ctrl.isShowedAlertChangeBookingInterval) {
                SweetAlert.confirm(
                    $translate.instant('Admin.Js.BookingAffiliateSettings.ChangingInterval') +
                        $translate.instant('Admin.Js.BookingAffiliateSettings.AutomaticallyRebuild') +
                        $translate.instant('Admin.Js.BookingAffiliateSettings.AreYouSureChangeInterval'),
                    {
                        title: $translate.instant('Admin.Js.BookingAffiliateSettings.BookingInterval'),
                        confirmButtonText: $translate.instant('Admin.Js.BookingAffiliateSettings.YesContinue'),
                    },
                ).then(function (result) {
                    if (result === true || result.value === true) {
                        ctrl.isShowedAlertChangeBookingInterval = true;
                        ctrl.getTimes();
                    } else {
                        ctrl.affiliateBookingIntervalMinutes = ctrl.oldAffiliateBookingIntervalMinutes;
                    }
                });
            } else {
                ctrl.getTimes();
            }
        };

        ctrl.save = function (event) {
            if (ctrl.affiliateBookingIntervalMinutes !== ctrl.oldAffiliateBookingIntervalMinutes) {
                event.preventDefault();
                SweetAlert.confirm(
                    $translate.instant('Admin.Js.BookingAffiliateSettings.BookingIntervalWasChanged') +
                        $translate.instant('Admin.Js.BookingAffiliateSettings.AreYouSureChangeInterval'),
                    {
                        title: $translate.instant('Admin.Js.BookingAffiliateSettings.BookingInterval'),
                        confirmButtonText: $translate.instant('Admin.Js.BookingAffiliateSettings.YesSave'),
                    },
                ).then(function (result) {
                    if (result === true || result.value === true) {
                        event.target.submit();
                    }
                });
            }
        };

        ctrl.loadAdditionalTime = function (startDate, endDate) {
            if (startDate instanceof Date) {
                startDate = startDate.getFullYear() + '-' + (startDate.getMonth() + 1) + '-' + startDate.getDate();
            }
            if (endDate instanceof Date) {
                endDate = endDate.getFullYear() + '-' + (endDate.getMonth() + 1) + '-' + endDate.getDate();
            }

            var params = {
                affiliateId: ctrl.affiliateId,
            };

            if (endDate) {
                params.startDate = startDate;
                params.endDate = endDate;
            } else {
                params.date = startDate;
            }

            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddUpdateBookingAffiliateAdditionalTimeCtrl',
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
                        ctrl.onAdditionalTimeAddUpdateDelete();
                        return result;
                    },
                    function (result) {
                        //ctrl.onAdditionalTimeAddUpdateDelete();
                        return result;
                    },
                );
        };

        ctrl.onAdditionalTimeAddUpdateDelete = function () {
            ctrl.YearAdditionalDate = {};
            ctrl.calendarAdditionalTime.setYear(ctrl.calendarAdditionalTime.getYear());
        };

        ctrl.calendarAdditionalTimeOnInit = function (calendar) {
            ctrl.calendarAdditionalTime = calendar;
        };

        ctrl.selectionStop = function (selected, model) {
            bookingService.selectableTimeEventStop(selected, model);
        };

        ctrl.changeAssignedManager = function (managerId, single) {
            if (single) {
                ctrl.managerIds = [managerId];
            }
        };

        ctrl.changeAssignedAnalyticManager = function (managerId, single) {
            if (single) {
                ctrl.analyticManagerIds = [managerId];
            }
        };

        ctrl.deleteAffiliate = function () {
            SweetAlert.confirm('Вы уверены, что хотите удалить филиал и все связанные с ним данные?', {
                title: 'Удаление',
            }).then(function (result) {
                if (result === true || result.value === true) {
                    bookingAffiliateService.delete(ctrl.affiliateId).then(function (data) {
                        if (data.result === true) {
                            $window.location.assign('booking');
                        } else {
                            data.errors.forEach(function (error) {
                                toaster.pop('error', error);
                            });
                        }
                    });
                }
            });
        };

        // sms notification

        ctrl.onChangeSendSmsBeforeStartBooiking = function () {
            if (ctrl.noSendSmsBeforeStartBooiking) {
                ctrl.oldForHowManyMinutesToSendSms = ctrl.forHowManyMinutesToSendSms;
                ctrl.forHowManyMinutesToSendSms = null;
            } else {
                ctrl.forHowManyMinutesToSendSms = ctrl.oldForHowManyMinutesToSendSms || 10;
            }
        };

        ctrl.gridBookingSmsTemplatesInited = false;

        var columnDefsSmsTemplates = [
            {
                name: 'StatusName',
                displayName: 'Статус',
                enableCellEdit: false,
                filter: {
                    placeholder: 'Статус',
                    type: uiGridConstants.filter.SELECT,
                    name: 'Status',
                    fetch: 'booking/getStatusesList',
                },
            },
            {
                name: 'Text',
                displayName: 'Шаблон Смс',
                enableCellEdit: false,
            },
            {
                name: 'Enabled',
                displayName: 'Активен',
                cellTemplate: '<ui-grid-custom-switch row="row"></ui-grid-custom-switch>',
                width: 100,
                filter: {
                    name: 'Enabled',
                    placeholder: 'Активен',
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: [
                        { label: 'Активные', value: true },
                        { label: 'Неактивные', value: false },
                    ],
                },
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 80,
                enableSorting: false,
                useInSwipeBlock: true,
                cellTemplate:
                    '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                    '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadSmsTemplate(row.entity.Id)" aria-label="Редактировать"></button> ' +
                    '<ui-grid-custom-delete url="bookingAffiliate/deleteSmsTemplate" params="{\'Id\': row.entity.Id}"></ui-grid-custom-delete>' +
                    '</div></div>' +
                    '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="bookingAffiliate/deleteSmsTemplate" params="{\'Id\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
            },
        ];

        ctrl.gridBookingSmsTemplatesOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefsSmsTemplates,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.loadSmsTemplate(row.entity.Id);
                },
                selectionOptions: [
                    {
                        text: 'Удалить выделенные',
                        url: 'bookingAffiliate/deleteSmsTemplates',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm('Вы уверены, что хотите удалить?', { title: 'Удаление' }).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                ],
            },
        });

        ctrl.gridBookingSmsTemplatesOnInit = function (grid) {
            ctrl.gridBookingSmsTemplates = grid;
            ctrl.gridBookingSmsTemplatesInited = true;
        };

        ctrl.loadSmsTemplate = function (id) {
            var params = {
                id: id,
            };

            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddUpdateSmsTemplateCtrl',
                    controllerAs: 'ctrl',
                    size: 'lg',
                    backdrop: 'static',
                    templateUrl: addUpdateSmsTemplateTemplate,
                    resolve: {
                        params: params,
                    },
                })
                .result.then(
                    function (result) {
                        ctrl.gridBookingSmsTemplates.fetchData();
                        return result;
                    },
                    function (result) {
                        return result;
                    },
                );
        };

        // sms notification end

        // Cancel Unpaid

        ctrl.onChangeCancelUnpaidBooiking = function () {
            if (ctrl.noCancelUnpaid) {
                ctrl.oldCancelUnpaidViaMinutes = ctrl.cancelUnpaidViaMinutes;
                ctrl.cancelUnpaidViaMinutes = null;
            } else {
                ctrl.cancelUnpaidViaMinutes = ctrl.oldCancelUnpaidViaMinutes || 30;
            }
        };

        ctrl.formSubmitFromMobileHeader = function () {
            let btnSubmit = document.createElement('button');
            let form = document.querySelector('.bookingAffiliateSettingsForm');
            btnSubmit.setAttribute('type', 'submit');
            btnSubmit.setAttribute('data-button-validation', '');
            btnSubmit.style.display = 'none';
            form.appendChild(btnSubmit);
            btnSubmit.click();
        };

        // Cancel Unpaid end
    };

    BookingAffiliateSettingsCtrl.$inject = [
        '$http',
        '$q',
        'uiGridConstants',
        'uiGridCustomConfig',
        'SweetAlert',
        'toaster',
        '$uibModal',
        'bookingAffiliateService',
        '$translate',
        'bookingService',
        '$window',
    ];

    ng.module('bookingAffiliateSettings', []).controller('BookingAffiliateSettingsCtrl', BookingAffiliateSettingsCtrl);
})(window.angular);
