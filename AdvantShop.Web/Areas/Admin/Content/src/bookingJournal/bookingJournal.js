(function (ng) {
    'use strict';

    var BookingJournalCtrl = function (
        $cookies,
        $location,
        $window,
        $q,
        $translate,
        uiGridConstants,
        uiGridCustomConfig,
        SweetAlert,
        toaster,
        bookingService,
        adminWebNotificationsEvents,
        adminWebNotificationsService,
        isMobileService,
    ) {
        var ctrl = this;
        ctrl.containerJournalDataInited = false;
        ctrl.viewModes = {
            sheduler: 'sheduler',
            shedulerCompact: 'sheduler-short',
            grid: 'grid',
            shedulerMultiDays: 'sheduler-days',
        };
        ctrl.views = [
            { name: 'Планировщик', value: ctrl.viewModes.sheduler },
            {
                name: 'Компактный планировщик',
                value: ctrl.viewModes.shedulerCompact,
            },
            { name: 'Таблица', value: ctrl.viewModes.grid },
            { name: 'По дням', value: ctrl.viewModes.shedulerMultiDays },
        ];

        ctrl.$onInit = function () {
            var locationSearch = $location.search();

            if (locationSearch != null) {
                if (locationSearch.modal != null) {
                    ctrl.loadBooking(locationSearch.modal);
                }
            }

            adminWebNotificationsService.addListener(adminWebNotificationsEvents.updateBookings, ctrl.callbackUpdateBookings);
        };

        ctrl.callbackUpdateBookings = function (data) {
            if (!data.affiliateId || !ctrl.affiliateId || data.affiliateId == ctrl.affiliateId) {
                if (ctrl.sheduler == null || data.reservationResourceId == null) {
                    ctrl.fetchData(true);
                } else {
                    var columnIndex = null;

                    for (var index = 0; index < ctrl.sheduler.shedulerObj.Columns.length; index++) {
                        if (ctrl.sheduler.shedulerObj.Columns[index].Id == data.reservationResourceId) {
                            columnIndex = index;
                            ctrl.fetchData(true, columnIndex);
                            break;
                        }
                    }

                    if (columnIndex == null) {
                        ctrl.fetchData(true);
                    }
                }
            }
        };

        ctrl.init = function (viewMode, readOnly, accessToViewBooking) {
            ctrl.readOnly = readOnly;
            ctrl.accessToViewBooking = accessToViewBooking;
            ctrl.viewMode = ctrl.oldViewMode = viewMode;
            ctrl.gridParams = { affiliateFilterId: ctrl.affiliateId };

            if (ctrl.viewMode === ctrl.viewModes.grid) {
                columnDefsJournal.filter(function (item) {
                    return item.name === 'ReservationResourceName';
                })[0].filter.fetch += '?affiliateId=' + ctrl.affiliateId;
            }

            if (ctrl.readOnly) {
                ctrl.shedulerCalendarOptions.editable = false;
            }
        };

        ctrl.loadBooking = function (id, affiliateId, beginDate, endDate, reservationResourceId) {
            if (ctrl.accessToViewBooking) {
                bookingService.showBookingModal(id, affiliateId, beginDate, endDate, reservationResourceId).result.then(
                    function (result) {
                        ctrl.modalBookingClose(result);
                        return result;
                    },
                    function (result) {
                        ctrl.modalBookingDismiss(result);
                        return result;
                    },
                );

                if (id) {
                    $location.search('modal', id);
                }
            }
        };

        ctrl.deleteBooking = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.BookingJournal.AreYouSureWantDelete'), {
                title: $translate.instant('Admin.Js.BookingJournal.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    bookingService.delete(id).then(function (data) {
                        if (data.result === false) {
                            data.errors.forEach(function (error) {
                                toaster.pop('error', error);
                            });
                        }
                        ctrl.fetchData();
                    });
                }
            });
        };

        ctrl.changeView = function () {
            ctrl.setCookie('bookingjournal_viewmode', ctrl.viewMode);

            if (
                (ctrl.viewMode === ctrl.viewModes.sheduler || ctrl.viewMode === ctrl.viewModes.shedulerCompact) &&
                (ctrl.oldViewMode === ctrl.viewModes.sheduler || ctrl.oldViewMode === ctrl.viewModes.shedulerCompact)
            ) {
                ctrl.sheduler.changeCompactView(ctrl.viewMode === ctrl.viewModes.shedulerCompact);
                ctrl.oldViewMode = ctrl.viewMode;
            } else {
                ctrl.reload('');
            }
        };

        ctrl.setCookie = function (name, value) {
            var date = new Date();
            date.setFullYear(date.getFullYear() + 1);
            $cookies.put(name, value, { expires: date });
        };

        ctrl.reload = function (query) {
            var loc = $window.location.href.split('#')[0];
            $window.location.href = loc.split('?')[0] + (query || '') + (query ? '&' : '?') + ('rnd=' + +Math.random());
        };

        ctrl.modalBookingDismiss = function (result, refresh) {
            $location.search('modal', null);
        };

        ctrl.modalBookingClose = function (result, refresh) {
            if (ctrl.viewMode === ctrl.viewModes.sheduler || ctrl.viewMode === ctrl.viewModes.shedulerCompact) {
                if (result && (result.bookingBeforeChange || result.bookingAfteChange)) {
                    var columnIndexBefore = null,
                        columnIndexAfte = null,
                        col = null;

                    for (var index = 0; index < ctrl.sheduler.shedulerObj.Columns.length; index++) {
                        col = ctrl.sheduler.shedulerObj.Columns[index];

                        if (result.bookingBeforeChange && col.Id === result.bookingBeforeChange.reservationResourceId) {
                            columnIndexBefore = index;
                        }
                        if (result.bookingAfteChange && col.Id == result.bookingAfteChange.reservationResourceId) {
                            columnIndexAfte = index;
                        }

                        if (
                            (!result.bookingBeforeChange || (columnIndexBefore != null && columnIndexBefore >= 0)) &&
                            (!result.bookingAfteChange || (columnIndexAfte != null && columnIndexAfte >= 0))
                        ) {
                            break;
                        }
                    }

                    if (columnIndexBefore != null && columnIndexAfte != null && columnIndexBefore === columnIndexAfte) {
                        ctrl.fetchColumnData(columnIndexBefore);
                    } else {
                        if (columnIndexBefore != null) {
                            ctrl.fetchColumnData(columnIndexBefore);
                        }
                        if (columnIndexAfte != null) {
                            ctrl.fetchColumnData(columnIndexAfte);
                        }
                    }
                } else {
                    ctrl.sheduler.fetchData();
                }
            } else {
                ctrl.fetchData();
            }
            $location.search('modal', null);
        };

        // #region sheduler

        ctrl.shedulerCalendarOptions = {
            height: 'auto',
            header: false,
            editable: true,
            dragScroll: false,
            selectOverlap: false,
            eventOverlap: false,
            allDaySlot: false,
            slotEventOverlap: false,
            agendaEventMinHeight: 20, //px
            slotLabelFormat: 'HH:mm',
            columnHeader: false,
            eventClick: function (event) {
                ctrl.loadBooking(event.id);
            },
            dayClick: function (date, jsEvent) {
                if (!ctrl.readOnly && jsEvent.target.className.indexOf('fc-bgevent') === -1) {
                    //var colIndex = parseInt(domService.closest(jsEvent.target, '[data-columnindex]').getAttribute('data-columnindex'), 10);
                    var colIndex = parseInt($(jsEvent.target).closest('[data-columnindex]').attr('data-columnindex'), 10);
                    var column = ctrl.sheduler.shedulerObj.Columns[colIndex];

                    var dateStart = bookingService.transformDate(date, true, true);
                    var dateEnd = bookingService.transformDate(moment(date).add(moment.duration(column.BookingDuration)), true, true);

                    ctrl.loadBooking(undefined, ctrl.affiliateId, dateStart, dateEnd, column.Obj ? column.Obj.Id.toString() : undefined);
                }
            },
        };

        ctrl.shedulerOnInit = function (sheduler) {
            ctrl.sheduler = sheduler;
            ctrl.containerJournalDataInited = true;
        };

        // #endregion sheduler

        // #region shedulerDays

        ctrl.shedulerDaysOnInit = function (sheduler) {
            ctrl.shedulerDays = sheduler;
            ctrl.containerJournalDataInited = true;
        };

        // #endregion sheduler

        // #region grid

        var columnDefsJournal = [
            {
                name: 'Id',
                displayName: 'Номер',
                enableCellEdit: false,
                cellTemplate:
                    '<div class="ui-grid-cell-contents">' +
                    '<a href=\'booking/index/{{ row.entity.AffiliateId }}?modal={{row.entity.Id}}\' ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadBooking(row.entity.Id); $event.preventDefault();">{{COL_FIELD}}</a>' +
                    '</div>',
                width: 80,
                filter: {
                    placeholder: 'Номер',
                    type: uiGridConstants.filter.INPUT,
                    name: 'BookingId',
                },
            },
            {
                name: 'StatusName',
                displayName: 'Статус',
                enableCellEdit: false,
            },
            {
                name: 'CustomerName',
                displayName: 'Контакт',
                enableCellEdit: false,
            },
            {
                name: 'ReservationResourceName',
                displayName: 'Ресурс',
                enableCellEdit: false,
                filter: {
                    placeholder: 'Ресурс',
                    type: uiGridConstants.filter.SELECT,
                    name: 'ReservationResourceId',
                    fetch: 'bookingResources/getResourcesList',
                },
            },
            {
                name: '_noopColumnPayments',
                enableHiding: false,
                visible: false,
                filter: {
                    placeholder: 'Метод оплаты',
                    type: uiGridConstants.filter.SELECT,
                    name: 'PaymentMethodId',
                    fetch: 'booking/getPaymentMethods',
                },
            },
            {
                name: 'IsPaid',
                displayName: 'Оплата',
                enableCellEdit: false,
                cellTemplate:
                    '<div class="ui-grid-cell-contents"><label class="adv-checkbox-label">' +
                    '<input type="checkbox" disabled ng-model="row.entity.IsPaid" class="adv-checkbox-input control-checkbox" />' +
                    '<span class="adv-checkbox-emul"></span>' +
                    '</label></div>',
                filter: {
                    placeholder: 'Оплата',
                    type: uiGridConstants.filter.SELECT,
                    name: 'IsPaid',
                    selectOptions: [
                        {
                            label: $translate.instant('Admin.Js.Orders.Yes'),
                            value: true,
                        },
                        {
                            label: $translate.instant('Admin.Js.Orders.No'),
                            value: false,
                        },
                    ],
                },
                width: 65,
            },
            {
                name: 'SumFormatted',
                displayName: 'Сумма',
                enableCellEdit: false,
                filter: {
                    placeholder: 'Сумма',
                    type: 'range',
                    rangeOptions: {
                        from: {
                            name: 'SumFrom',
                        },
                        to: {
                            name: 'SumTo',
                        },
                    },
                },
                width: 100,
            },
            {
                name: 'BeginDateFormatted',
                displayName: 'Дата начала',
                width: 150,
                visible: 1441,
                filter: {
                    placeholder: 'Дата начала',
                    type: 'datetime',
                    term: {
                        from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                        to: new Date(),
                    },
                    datetimeOptions: {
                        from: {
                            name: 'BeginDateFrom',
                        },
                        to: {
                            name: 'BeginDateTo',
                        },
                    },
                },
            },
            {
                name: 'EndDateFormatted',
                displayName: 'Дата окончания',
                visible: 1441,
                width: 150,
                filter: {
                    placeholder: 'Дата окончания',
                    type: 'datetime',
                    term: {
                        from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                        to: new Date(),
                    },
                    datetimeOptions: {
                        from: {
                            name: 'EndDateFrom',
                        },
                        to: {
                            name: 'EndDateTo',
                        },
                    },
                },
            },
            {
                name: 'DateAddedFormatted',
                displayName: 'Дата создания',
                visible: 1441,
                width: 150,
                filter: {
                    placeholder: 'Дата создания',
                    type: 'datetime',
                    term: {
                        from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                        to: new Date(),
                    },
                    datetimeOptions: {
                        from: {
                            name: 'DateAddedFrom',
                        },
                        to: {
                            name: 'DateAddedTo',
                        },
                    },
                },
            },
            {
                name: '_serviceColumn',
                enableHiding: false,
                displayName: '',
                width: 80,
                enableSorting: false,
                cellTemplate:
                    '<div class="ui-grid-cell-contents"><div>' +
                    '<button type="button" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadBooking(row.entity.Id); $event.preventDefault();" class="btn-icon ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></button>' +
                    //'<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteBooking(row.entity.Id)" ng-if="!grid.appScope.$ctrl.gridExtendCtrl.readOnly" class="ui-grid-custom-service-icon fa fa-times link-invert"></a>' +
                    '</div></div>',
            },
        ];

        ctrl.gridBookingOptions = ng.extend({}, uiGridCustomConfig, {
            enableGridMenu: !isMobileService.getValue(),
            columnDefs: columnDefsJournal,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.loadBooking(row.entity.Id);
                    $event.preventDefault();
                },
                /*selectionOptions: [
                    {
                        text: 'Удалить выделенные',
                        url: 'booking/deleteBookings',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm('Вы уверены, что хотите удалить?', { title: 'Удаление' }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    }
                ]*/
            },
        });

        ctrl.gridBookingOnInit = function (grid) {
            ctrl.gridJournal = grid;
            ctrl.containerJournalDataInited = true;
        };

        ctrl.changeStatusParam = function (statusId) {
            ctrl.gridParams['status'] = statusId;
            ctrl.gridJournal.setParams(ctrl.gridParams);
            ctrl.gridJournal.fetchData();
        };

        // #endregion grid

        ctrl.fetchColumnData = function (colIndex) {
            return ctrl.sheduler.fetchColumnData(colIndex);
        };

        ctrl.fetchData = function (ignoreHistory, colIndex) {
            if (ctrl.viewMode === ctrl.viewModes.sheduler || ctrl.viewMode === ctrl.viewModes.shedulerCompact) {
                if (colIndex == null) {
                    ctrl.sheduler.fetchData();
                } else {
                    ctrl.sheduler.fetchColumnData(colIndex);
                }
            } else if (ctrl.viewMode === ctrl.viewModes.grid) {
                ctrl.gridJournal.fetchData(ignoreHistory);
            } else {
                ctrl.shedulerDays.fetchData();
            }
        };
    };

    BookingJournalCtrl.$inject = [
        '$cookies',
        '$location',
        '$window',
        '$q',
        '$translate',
        'uiGridConstants',
        'uiGridCustomConfig',
        'SweetAlert',
        'toaster',
        'bookingService',
        'adminWebNotificationsEvents',
        'adminWebNotificationsService',
        'isMobileService',
    ];

    ng.module('bookingJournal', ['uiGridCustom']).controller('BookingJournalCtrl', BookingJournalCtrl);
})(window.angular);
