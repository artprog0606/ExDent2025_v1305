(function (ng) {
    'use strict';

    var ModalBookingsViewGridCtrl = function (
        $scope,
        $uibModalInstance,
        $translate,
        uiGridConstants,
        uiGridCustomConfig,
        bookingService,
        adminWebNotificationsEvents,
        adminWebNotificationsService,
    ) {
        var ctrl = this;
        ctrl.gridInited = false;
        ctrl.reservationResourcesChanged = [];
        ctrl.bookingsChanged = [];

        $scope.$on('modal.closing', function (event, result) {
            if (result && !result.reservationResourcesChanged && !result.bookingsChanged) {
                event.preventDefault();
                ctrl.close();
            } else {
                if (ctrl.removeCallbackUpdateBookings) {
                    ctrl.removeCallbackUpdateBookings();
                    ctrl.removeCallbackUpdateBookings = null;
                }
            }
        });

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
                filter: {
                    placeholder: 'Статус',
                    type: uiGridConstants.filter.SELECT,
                    name: 'Status',
                    fetch: 'booking/getStatusesList',
                },
            },
            {
                name: '_noopColumnNoStatus',
                visible: false,
                filter: {
                    placeholder: 'Исключая статус',
                    type: uiGridConstants.filter.SELECT,
                    name: 'NoStatus',
                    fetch: 'booking/getStatusesList',
                },
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
                        { label: $translate.instant('Admin.Js.Orders.Yes'), value: true },
                        { label: $translate.instant('Admin.Js.Orders.No'), value: false },
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
                filter: {
                    placeholder: 'Дата начала',
                    type: 'datetime',
                    //term: {
                    //    from: new Date((new Date()).setMonth((new Date()).getMonth() - 1)),
                    //    to: new Date()
                    //},
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
                width: 150,
                filter: {
                    placeholder: 'Дата окончания',
                    type: 'datetime',
                    //term: {
                    //    from: new Date((new Date()).setMonth((new Date()).getMonth() - 1)),
                    //    to: new Date()
                    //},
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
                width: 150,
                filter: {
                    placeholder: 'Дата создания',
                    type: 'datetime',
                    //term: {
                    //    from: new Date((new Date()).setMonth((new Date()).getMonth() - 1)),
                    //    to: new Date()
                    //},
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

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.affiliateId = params.affiliateId;
            ctrl.reservationResourceId = params.reservationResourceId;

            ctrl.gridParams = {
                BookingId: params.bookingId,
                affiliateFilterId: ctrl.affiliateId,
                Status: params.status,
                NoStatus: params.nostatus,
                ReservationResourceId: ctrl.reservationResourceId,
                IsPaid: params.paid,
                PaymentMethodId: params.paymentMethodId,
                CustomerId: params.customerId,
                SumFrom: params.sumFrom,
                SumTo: params.sumTo,
                BeginDateFrom: ctrl.convertToDate(params.beginDateFrom),
                BeginDateTo: ctrl.convertToDate(params.beginDateTo),
                EndDateFrom: ctrl.convertToDate(params.endDateFrom),
                EndDateTo: ctrl.convertToDate(params.endDateTo),
                DateAddedFrom: ctrl.convertToDate(params.dateAddedFrom),
                DateAddedTo: ctrl.convertToDate(params.dateAddedTo),
            };
            for (var key in ctrl.gridParams) {
                if (Object.prototype.hasOwnProperty.call(ctrl.gridParams, key) && ctrl.gridParams[key] === undefined) {
                    delete ctrl.gridParams[key];
                }
            }

            ctrl.removeCallbackUpdateBookings = adminWebNotificationsService.addListener(adminWebNotificationsEvents.updateBookings, function (data) {
                if (
                    data.affiliateId == ctrl.affiliateId &&
                    (!ctrl.reservationResourceId || data.reservationResourceId == ctrl.reservationResourceId)
                ) {
                    ctrl.fetchData();
                }
            });
        };

        ctrl.gridBookingOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefsJournal,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.loadBooking(row.entity.Id);
                    $event.preventDefault();
                },
            },
        });

        ctrl.gridBookingOnInit = function (grid) {
            ctrl.gridJournal = grid;
            ctrl.gridInited = true;
        };

        ctrl.fetchData = function (ignoreHistory) {
            ctrl.gridJournal.fetchData(ignoreHistory);
        };

        ctrl.loadBooking = function (id, affiliateId, beginDate, endDate, reservationResourceId) {
            var fnModalBookingClose = function (result) {
                if (result && (result.bookingBeforeChange || result.bookingAfteChange)) {
                    if (result.bookingBeforeChange) {
                        ctrl.reservationResourceChanged(result.bookingBeforeChange.reservationResourceId);
                        ctrl.bookingChanged(result.bookingBeforeChange.id);
                    }
                    if (result.bookingAfteChange) {
                        ctrl.reservationResourceChanged(result.bookingAfteChange.reservationResourceId);
                        ctrl.bookingChanged(result.bookingAfteChange.id);
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
        };

        ctrl.close = function () {
            $uibModalInstance.close({
                reservationResourcesChanged: ctrl.reservationResourcesChanged,
                bookingsChanged: ctrl.bookingsChanged,
            });
        };

        ctrl.reservationResourceChanged = function (reservationResourceId) {
            ctrl.pushInt(ctrl.reservationResourcesChanged, reservationResourceId);
        };

        ctrl.bookingChanged = function (bookingId) {
            ctrl.pushInt(ctrl.bookingsChanged, bookingId);
        };

        ctrl.pushInt = function (arr, val) {
            if (typeof val !== 'number') {
                val = parseInt(val, 10);

                if (isNaN(val)) {
                    throw new Error('Неправильный формат данных: ' + val);
                }
            }
            if (arr.indexOf(val) === -1) {
                arr.push(val);
            }
        };

        ctrl.convertToDate = function (date) {
            if (date && typeof date === 'string') {
                date = new Date(new Date(date).setHours(0, 0, 0, 0));
            }
            if (date && typeof date === 'number') {
                date = new Date(new Date(date));
            }
            return date;
        };
    };

    ModalBookingsViewGridCtrl.$inject = [
        '$scope',
        '$uibModalInstance',
        '$translate',
        'uiGridConstants',
        'uiGridCustomConfig',
        'bookingService',
        'adminWebNotificationsEvents',
        'adminWebNotificationsService',
    ];

    ng.module('uiModal').controller('ModalBookingsViewGridCtrl', ModalBookingsViewGridCtrl);
})(window.angular);
