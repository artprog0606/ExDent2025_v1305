import getBillingLinkTemplate from './../../../order/modal/getBillingLink/getBillingLink.html';
(function (ng) {
    'use strict';

    var ModalAddUpdateBookingCtrl = function (
        $uibModalInstance,
        $http,
        $httpParamSerializer,
        $q,
        toaster,
        uiGridCustomConfig,
        SweetAlert,
        $translate,
        bookingService,
        $window,
        $scope,
        adminWebNotificationsEvents,
        adminWebNotificationsService,
        $uibModal,
        $timeout,
    ) {
        var ctrl = this;
        ctrl.items = [];
        ctrl.customer = {};
        ctrl.bookingInfo = {};
        ctrl.summary = {};
        $scope.$on('modal.closing', function (event, result) {
            if (ctrl.removeCallbackUpdateBookings) {
                ctrl.removeCallbackUpdateBookings();
                ctrl.removeCallbackUpdateBookings = null;
            }
        });
        ctrl.$onInit = function () {
            ctrl.params = ctrl.$resolve.params;
            ctrl.bookingInfo.id = ctrl.params.id;
            ctrl.redirectToBooking = ctrl.params.redirectToBooking;
            ctrl.mode = ctrl.bookingInfo.id ? 'edit' : 'add';
            ctrl.canBeEditing = ctrl.mode === 'add';
            ctrl.bookingInfo.affiliateId = ctrl.params.affiliateId;
            ctrl.bookingInfo.reservationResourceId = ctrl.params.reservationResourceId;
            ctrl.gridUniqueId =
                ctrl.mode === 'add' ? 'gridBookingItemsAdd' + Math.floor(Math.random() * 1000) : 'gridBookingItems' + ctrl.bookingInfo.id;
            if (ctrl.mode !== 'add') {
                ctrl.loadBooking();
                ctrl.getAttachmentHelpText();
                ctrl.removeCallbackUpdateBookings = adminWebNotificationsService.addListener(
                    adminWebNotificationsEvents.updateBookings,
                    ctrl.callbackUpdateBookings,
                );
            } else {
                ctrl.updateSummary().then(function () {
                    ctrl.getBookingItemsSummaryComponent().then(function () {
                        ctrl.bookingItemsSummaryComponent.callToggleSelectCurrencyLabel();
                    });
                });
            }
        };
        ctrl.isLoaded = function () {
            return ctrl.bookingIsLoaded && ctrl.bookingInfoIsCompleted && ctrl.bookingCustomerIsCompleted;
        };
        ctrl.gridBookingItemsOptions = ng.extend({}, uiGridCustomConfig, {
            useExternalSorting: false,
            enableCellEdit: false,
            columnDefs: [
                {
                    name: 'ImageSrc',
                    displayName: '',
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><span class="ui-grid-custom-flex-center ui-grid-custom-link-for-img"><img class="ui-grid-custom-col-img" ng-src="{{COL_FIELD}}"></span></div>',
                    width: 100,
                    enableSorting: false,
                },
                {
                    name: 'ArtNo',
                    displayName: $translate.instant('Admin.Js.BookingJournal.ArtNo'),
                    width: 75,
                },
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.BookingJournal.Name'),
                },
                {
                    name: 'Price',
                    displayName: $translate.instant('Admin.Js.BookingJournal.Price'),
                    enableCellEdit: true,
                    width: 100,
                },
                {
                    name: 'Amount',
                    displayName: $translate.instant('Admin.Js.BookingJournal.Amount'),
                    enableCellEdit: true,
                    width: 75,
                },
                {
                    name: 'Cost',
                    displayName: $translate.instant('Admin.Js.BookingJournal.Cost'),
                    width: 100,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents ui-grid-cell-contents-order-items">' +
                        '<div><span ng-if="row.entity.Currency.IsCodeBefore">{{row.entity.Currency.CurrencySymbol}}</span>{{row.entity.Price * row.entity.Amount}} <span ng-if="!row.entity.Currency.IsCodeBefore">{{row.entity.Currency.CurrencySymbol}}</span></div> ' +
                        '</div>',
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 35,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteBookingItem(row.entity)" ' +
                        'class="ui-grid-custom-service-icon fa fa-times link-invert"></a>' +
                        '</div></div>' +
                        '<a ng-if="grid.appScope.$ctrl.isMobile" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteBookingItem(row.entity)" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</a>',
                },
            ],
        });
        ctrl.callbackUpdateBookings = function (data) {
            if (!ctrl.btnLoading && data.bookingId == ctrl.bookingInfo.id) {
                ctrl.isDeletedBooking = data.typeChange === 'deleted';
                var message = ctrl.isDeletedBooking
                    ? 'Данная бронь была удалена другим пользователем. Закрыть окно с бронью?'
                    : 'Данная бронь была отредактирована другим пользователем. Для предотвращения затирания предыдущих изменений вам необходимо заново открыть данную бронь. Открыть?';
                SweetAlert.confirm(message, {
                    title: ctrl.isDeletedBooking ? 'Бронь удалена' : 'Бронь изменена',
                    confirmButtonText: ctrl.isDeletedBooking ? 'Да, закрыть' : 'Да, открыть',
                }).then(function (result) {
                    if (result === true || result.value) {
                        if (ctrl.isDeletedBooking) {
                            ctrl.close();
                        } else {
                            ctrl.loadBooking();
                        }
                    } else {
                        if (ctrl.isDeletedBooking) {
                            ctrl.canBeEditing = false;
                            ctrl.canBeDeleted = false;
                        }
                        ctrl.isOldBooking = true;
                    }
                });
            }
        };
        ctrl.getAttachmentHelpText = function () {
            return $http.get('booking/getAttachmentHelpText', {}).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    ctrl.attachmentHelpText = data.obj.AttachmentHelpText;
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });

                    //if (!data.errors) {
                    //    toaster.pop('error', '');
                    //}
                }
            });
        };
        ctrl.loadBooking = function () {
            ctrl.bookingIsLoaded = false;
            ctrl.bookingInfoIsCompleted = false;
            ctrl.bookingCustomerIsCompleted = false;
            ctrl.getBooking(ctrl.bookingInfo.id).then(function (data) {
                if (data.result === true) {
                    ctrl.bookingIsLoaded = true;
                    if (!ctrl.canBeEditing) {
                        ctrl.gridBookingItemsOptions.columnDefs.filter(function (item) {
                            return item.name === '_serviceColumn';
                        })[0].visible = false;
                        ctrl.gridBookingItemsOptions.columnDefs.forEach(function (item) {
                            item.enableCellEdit = false;
                        });
                    }
                    ctrl.getBookingCustomerComponent().then(function () {
                        ctrl.bookingCustomerComponent.callGetgCustomerFields().then(function () {
                            ctrl.bookingCustomerIsCompleted = true;
                        });
                    });
                    ctrl.getBookingInfoComponent().then(function () {
                        ctrl.bookingInfoComponent.callGetBookingForm().then(function (dataBookingForm) {
                            ctrl.bookingInfoIsCompleted = dataBookingForm.result === true;
                        });
                    });
                    ctrl.getTemplateDocx();
                    ctrl.getAttachments();
                }
            });
        };
        ctrl.getBooking = function (id) {
            return bookingService.getBooking(id).then(function (data) {
                if (data.result === true) {
                    data.obj.BeginDate = new Date(data.obj.BeginDate);
                    data.obj.EndDate = new Date(data.obj.EndDate);
                    ctrl.bookingInfo.affiliateId = data.obj.AffiliateId;
                    ctrl.bookingInfo.reservationResourceId = data.obj.ReservationResourceId.toString();
                    ctrl.bookingInfo.beginDate = data.obj.BeginDate;
                    ctrl.bookingInfo.endDate = data.obj.EndDate;
                    ctrl.bookingInfo.adminComment = data.obj.AdminComment;
                    ctrl.customer.customerId = data.obj.CustomerId;
                    ctrl.customer.firstName = data.obj.FirstName;
                    ctrl.customer.lastName = data.obj.LastName;
                    ctrl.customer.patronymic = data.obj.Patronymic;
                    ctrl.customer.organization = data.obj.Organization;
                    ctrl.customer.phone = data.obj.Phone;
                    ctrl.customer.standardPhone = data.obj.StandardPhone;
                    ctrl.customer.birthday = data.obj.BirthDay;
                    ctrl.customer.email = data.obj.EMail;
                    ctrl.customer.social = data.obj.Social;
                    ctrl.customer.customerType = data.obj.CustomerType;
                    ctrl.bookingInfo.status = data.obj.StatusName;
                    ctrl.bookingInfo.managerId = data.obj.ManagerId || 0;
                    ctrl.bookingInfo.orderSourceId = data.obj.OrderSourceId;
                    ctrl.bookingInfo.payed = data.obj.Payed;
                    ctrl.bookingInfo.orderId = data.obj.OrderId;
                    ctrl.isPaied = data.obj.Payed;
                    ctrl.canBeDeleted = data.obj.CanBeDeleted;
                    ctrl.canBeEditing = data.obj.CanBeEditing;
                    ctrl.getBookingInfoComponent().then(function () {
                        ctrl.bookingInfoComponent.selectDateAndTime(data.obj.BeginDate, data.obj.EndDate);
                    });
                    ctrl.items = data.obj.Items;
                    if (ctrl.gridBookingItems != null) {
                        ctrl.gridBookingItems.gridOptions.data = ctrl.items;
                    }
                    ctrl.summary = ng.extend(ctrl.summary, data.obj.Summary);
                    ctrl.getBookingItemsSummaryComponent().then(function () {
                        ctrl.bookingItemsSummaryComponent.callToggleSelectCurrencyLabel();
                    });
                    ctrl.bookingBeforeChange = ctrl.getCurrentStatusModel();
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });
                    if (!data.errors) {
                        toaster.pop('error', $translate.instant('Admin.Js.BookingJournal.ErrorLoadingBookingData'));
                    }
                }
                return data;
            });
        };
        ctrl.addUpdateBooking = function (userConfirmed) {
            if (ctrl.isOldBooking) {
                if (!ctrl.isDeletedBooking) {
                    SweetAlert.confirm(
                        'Данная бронь была отредактирована другим пользователем. Сохранение приведет к затиранию предыдущих изменений. Продолжить?',
                        {
                            title: $translate.instant('Admin.Js.BookingJournal.SaveReservation'),
                            confirmButtonText: 'Да, продолжить',
                        },
                    ).then(function (result) {
                        if (result === true || result.value === true) {
                            ctrl.isOldBooking = false;
                            ctrl.addUpdateBooking();
                        } else {
                            ctrl.btnLoading = false;
                        }
                    });
                } else {
                    SweetAlert.alert('Данная бронь была удалена другим пользователем.', {
                        title: $translate.instant('Admin.Js.BookingJournal.SaveReservation'),
                    }).then(function (result) {
                        ctrl.btnLoading = false;
                    });
                }
                return;
            }
            var url = ctrl.mode === 'add' ? 'booking/add' : 'booking/update';
            var beginDate, endDate;
            if (ctrl.bookingInfo.dateTimeMode === 'select') {
                var times = ctrl.bookingInfo.time.split('-');
                if (ctrl.bookingInfo.date instanceof Date) {
                    ctrl.bookingInfo.date =
                        ctrl.bookingInfo.date.getFullYear() + '-' + (ctrl.bookingInfo.date.getMonth() + 1) + '-' + ctrl.bookingInfo.date.getDate();
                }
                if (typeof ctrl.bookingInfo.date === 'string') {
                    // задолбался под этот fp подстраиваться!!!!
                    ctrl.bookingInfo.date = new Date(ctrl.bookingInfo.date);
                    ctrl.bookingInfo.date =
                        ctrl.bookingInfo.date.getFullYear() + '-' + (ctrl.bookingInfo.date.getMonth() + 1) + '-' + ctrl.bookingInfo.date.getDate();
                }
                beginDate = ctrl.bookingInfo.date + 'T' + times[0];
                endDate = ctrl.bookingInfo.date + 'T' + times[1];
            } else {
                var tempDate = typeof ctrl.bookingInfo.beginDate === 'string' ? new Date(ctrl.bookingInfo.beginDate) : ctrl.bookingInfo.beginDate;
                beginDate =
                    tempDate.getFullYear() +
                    '-' +
                    ('0' + (tempDate.getMonth() + 1)).slice(-2) +
                    '-' +
                    ('0' + tempDate.getDate()).slice(-2) +
                    'T' +
                    ('0' + tempDate.getHours()).slice(-2) +
                    ':' +
                    ('0' + tempDate.getMinutes()).slice(-2);
                tempDate = typeof ctrl.bookingInfo.endDate === 'string' ? new Date(ctrl.bookingInfo.endDate) : ctrl.bookingInfo.endDate;
                endDate =
                    tempDate.getFullYear() +
                    '-' +
                    ('0' + (tempDate.getMonth() + 1)).slice(-2) +
                    '-' +
                    ('0' + tempDate.getDate()).slice(-2) +
                    'T' +
                    ('0' + tempDate.getHours()).slice(-2) +
                    ':' +
                    ('0' + tempDate.getMinutes()).slice(-2);
            }
            var params = {
                id: ctrl.bookingInfo.id,
                affiliateId: ctrl.bookingInfo.affiliateId,
                status: ctrl.bookingInfo.status,
                payed: ctrl.bookingInfo.payed,
                reservationResourceId: ctrl.bookingInfo.reservationResourceId,
                managerId: ctrl.bookingInfo.managerId || null,
                orderSourceId: ctrl.bookingInfo.orderSourceId,
                beginDate: beginDate,
                endDate: endDate,
                customerId: ctrl.customer.customerId,
                firstName: ctrl.customer.firstName,
                lastName: ctrl.customer.lastName,
                patronymic: ctrl.customer.patronymic,
                organization: ctrl.customer.organization,
                phone: ctrl.customer.phone,
                standardPhone: null,
                email: ctrl.customer.email,
                birthday: ctrl.customer.birthday,
                customerFields: ctrl.customer.customerFields,
                items: ctrl.gridBookingItems.gridOptions.data,
                summary: ctrl.summary,
                adminComment: ctrl.bookingInfo.adminComment,
                userConfirmed: userConfirmed,
                customerType: ctrl.customer.customerType.value,
            };
            $http.post(url, params).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    if (data.obj && data.obj.UserConfirmIsRequired) {
                        SweetAlert.confirm(data.obj.ConfirmMessage, {
                            title: $translate.instant('Admin.Js.BookingJournal.SaveReservation'),
                            confirmButtonText: data.obj.ConfirmButtomText,
                        }).then(function (result) {
                            if (result === true || result.value === true) {
                                ctrl.addUpdateBooking(true);
                            } else {
                                ctrl.btnLoading = false;
                            }
                        });
                    } else {
                        toaster.pop(
                            'success',
                            '',
                            ctrl.mode === 'add' ? $translate.instant('Admin.Js.BookingJournal.BookingSuccessfullyAdded') : 'Изменения сохранены',
                        );
                        if (ctrl.redirectToBooking && $window.location.pathname.split('/').pop() != 'booking') {
                            $window.location.assign('booking');
                        }
                        $uibModalInstance.close({
                            bookingBeforeChange: ctrl.bookingBeforeChange,
                            bookingAfteChange: ctrl.getCurrentStatusModel(),
                        });
                    }
                } else {
                    ctrl.btnLoading = false;
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });
                    if (!data.errors) {
                        toaster.pop('error', $translate.instant('Admin.Js.BookingJournal.ErrorSavingReservation'));
                    }
                }
            });
        };
        ctrl.getCurrentStatusModel = function () {
            return {
                id: ctrl.bookingInfo.id,
                affiliateId: ctrl.bookingInfo.affiliateId,
                reservationResourceId: ctrl.bookingInfo.reservationResourceId,
                beginDate: ctrl.bookingInfo.beginDate,
                endDate: ctrl.bookingInfo.endDate,
                customerId: ctrl.customer.customerId,
                firstName: ctrl.customer.firstName,
                lastName: ctrl.customer.lastName,
                patronymic: ctrl.customer.patronymic,
                organization: ctrl.customer.organization,
                phone: ctrl.customer.phone,
                standardPhone: ctrl.customer.standardPhone,
                email: ctrl.customer.email,
                birthday: ctrl.customer.birthday,
                customerFields: ctrl.customer.customerFields,
                status: ctrl.bookingInfo.status,
                date: ctrl.bookingInfo.date,
                time: ctrl.bookingInfo.time,
                items: ctrl.items,
            };
        };
        ctrl.deleteBooking = function () {
            SweetAlert.confirm('Вы уверены, что хотите удалить бронь?', {
                title: 'Удаление',
            })
                .then(function (result) {
                    if ((result === true && !result.isDismissed) || (result.value && !result.isDismissed)) {
                        bookingService.delete(ctrl.bookingInfo.id).then(function (data) {
                            if (data.result === false) {
                                data.errors.forEach(function (error) {
                                    toaster.pop('error', error);
                                });
                                ctrl.btnLoading = false;
                            } else {
                                toaster.pop('success', '', 'Бронь удалена');
                                $uibModalInstance.close({
                                    bookingBeforeChange: ctrl.bookingBeforeChange,
                                });
                            }
                        });
                    }
                })
                .finally(function () {
                    ctrl.btnLoading = false;
                });
        };
        ctrl.updateSummary = function (params) {
            return $http
                .post('booking/updateSummary', {
                    bookingId: ctrl.bookingInfo.id,
                    currentItems: ctrl.gridBookingItems ? ctrl.gridBookingItems.gridOptions.data : null,
                    summary: ctrl.summary,
                    updateStatusBillingLink: (params || {}).updateStatusBillingLink,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        ctrl.summary = ng.extend(ctrl.summary, data.obj.NewSummary);
                    } else {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });
                        if (!data.errors) {
                            toaster.pop('error', 'Не удалось обновить информацию');
                        }
                    }
                });
        };
        ctrl.changePayment = function (payment) {
            return $http
                .post('booking/changePayment', {
                    payment: payment,
                    summary: ctrl.summary,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        ctrl.summary = ng.extend(ctrl.summary, data.obj.NewSummary);
                    } else {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });
                        if (!data.errors) {
                            toaster.pop('error', 'Не удалось обновить информацию');
                        }
                    }
                });
        };
        ctrl.changePay = function (pay) {
            ctrl.bookingInfo.payed = pay;
        };
        ctrl.createPaymentLink = function () {
            SweetAlert.confirm(
                $translate.instant(
                    'Вы действительно хотите создать ссылку на оплату? При этом будет создан заказ и покупатель получит уведомление о заказе',
                ),
                {
                    title: '',
                },
            ).then(function (result) {
                if (result === true || result.value) {
                    bookingService.createOrder(ctrl.bookingInfo.id).then(function (data) {
                        if (data.result === true && data.orderId != null) {
                            ctrl.bookingInfo.orderId = data.orderId;
                            ctrl.summary.OrderId = data.orderId;
                            $uibModal.open({
                                bindToController: true,
                                controller: 'ModalGetBillingLinkCtrl',
                                controllerAs: 'ctrl',
                                templateUrl: getBillingLinkTemplate,
                                resolve: {
                                    params: {
                                        orderId: ctrl.bookingInfo.orderId,
                                    },
                                },
                            });
                            ctrl.updateSummary({
                                updateStatusBillingLink: true,
                            });
                            ctrl.updateEvents();
                        }
                    });
                }
            });
        };
        ctrl.createOrder = function () {
            SweetAlert.confirm($translate.instant('Вы действительно хотите создать заказ? При этом покупатель получит уведомление о заказе'), {
                title: '',
            }).then(function (result) {
                if (result === true || result.value) {
                    bookingService
                        .createOrder(ctrl.bookingInfo.id)
                        .then(function (data) {
                            if (data.result === true && data.orderId != null) {
                                ctrl.bookingInfo.orderId = data.orderId;
                                ctrl.summary.OrderId = data.orderId;
                                ctrl.updateSummary({
                                    updateStatusBillingLink: true,
                                });
                                ctrl.updateEvents();
                                toaster.pop('success', '', 'Заказ успешно создан');
                                return data.orderId;
                            } else {
                                toaster.pop('error', '', 'Ошибка при создании заказа');
                                return false;
                            }
                        })
                        .then(function (orderId) {
                            if (orderId) {
                                $timeout(function () {
                                    $window.location.assign('orders/edit/' + orderId);
                                }, 1000);
                            }
                        });
                }
            });
        };
        ctrl.gridBookingItemsOnInit = function (grid) {
            ctrl.gridBookingItems = grid;
            ctrl.gridBookingItems.gridOptions.data = ctrl.items;
        };
        ctrl.gridOnInplaceBeforeApply = function () {
            return ctrl.checkStopEdit();
        };
        ctrl.bookingItemsSummaryOnInit = function (bookingItemsSummary) {
            ctrl.bookingItemsSummaryComponent = bookingItemsSummary;
            if (ctrl.bookingItemsSummaryComponentPromise) {
                ctrl.bookingItemsSummaryComponentPromise.resolve();
            }
        };
        ctrl.getBookingItemsSummaryComponent = function () {
            if (ctrl.bookingItemsSummaryComponent) {
                return $q.resolve();
            } else {
                ctrl.bookingItemsSummaryComponentPromise = ctrl.bookingItemsSummaryComponentPromise
                    ? ctrl.bookingItemsSummaryComponentPromise
                    : $q.defer();
                return ctrl.bookingItemsSummaryComponentPromise.promise;
            }
        };
        ctrl.bookingInfoOnInit = function (bookingInfo) {
            ctrl.bookingInfoComponent = bookingInfo;
            if (ctrl.bookingInfoComponentPromise) {
                ctrl.bookingInfoComponentPromise.resolve();
            }
        };
        ctrl.getBookingInfoComponent = function () {
            if (ctrl.bookingInfoComponent) {
                return $q.resolve();
            } else {
                ctrl.bookingInfoComponentPromise = ctrl.bookingInfoComponentPromise ? ctrl.bookingInfoComponentPromise : $q.defer();
                return ctrl.bookingInfoComponentPromise.promise;
            }
        };

        /* customer */

        ctrl.bookingCustomerOnInit = function (bookingCustomer) {
            ctrl.bookingCustomerComponent = bookingCustomer;
            if (ctrl.bookingCustomerComponentPromise) {
                ctrl.bookingCustomerComponentPromise.resolve();
            }
        };
        ctrl.getBookingCustomerComponent = function () {
            if (ctrl.bookingCustomerComponent) {
                return $q.resolve();
            } else {
                ctrl.bookingCustomerComponentPromise = ctrl.bookingCustomerComponentPromise ? ctrl.bookingCustomerComponentPromise : $q.defer();
                return ctrl.bookingCustomerComponentPromise.promise;
            }
        };
        ctrl.bookingEventsOnInit = function (bookingEvents) {
            ctrl.bookingEvents = bookingEvents;
        };
        ctrl.updateEvents = function () {
            //$timeout(function () {
            if (ctrl.bookingEvents) {
                ctrl.bookingEvents.getLeadEvents();
            }
            //});
        };
        /* end customer */

        /* attachments */
        ctrl.getAttachments = function () {
            bookingService.getAttachments(ctrl.bookingInfo.id).then(function (data) {
                ctrl.attachments = data;
            });
        };
        ctrl.uploadAttachment = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            ctrl.loadingFiles = true;
            if (($event.type === 'change' || $event.type === 'drop') && $files != null && $files.length > 0) {
                bookingService.uploadAttachment(ctrl.bookingInfo.id, $files).then(function (data) {
                    ctrl.uploadAttachmentResult(data);
                    ctrl.loadingFiles = false;
                });
            } else if ($invalidFiles.length > 0) {
                toaster.pop('error', 'Ошибка при загрузке', 'Файл не соответствует требованиям');
                ctrl.loadingFiles = false;
            } else {
                ctrl.loadingFiles = false;
            }
        };
        ctrl.uploadAttachmentResult = function (data) {
            for (var i in data) {
                if (data[i].Result === true) {
                    ctrl.attachments.push(data[i].Attachment);
                    toaster.pop('success', '', 'Файл "' + data[i].Attachment.OriginFileName + '" добавлен');
                } else {
                    toaster.pop(
                        'error',
                        'Ошибка при загрузке',
                        (data[i].Attachment != null ? data[i].Attachment.OriginFileName + ': ' : '') + data[i].Error,
                    );
                }
            }
        };
        ctrl.deleteAttachment = function (id) {
            SweetAlert.confirm('Вы уверены, что хотите удалить файл?', {
                title: 'Удаление',
            }).then(function (result) {
                if (result === true || result.value) {
                    bookingService.deleteAttachment(ctrl.bookingInfo.id, id).then(function (data) {
                        if (data.result === true) {
                            ctrl.getAttachments();
                        } else {
                            data.errors.forEach(function (error) {
                                toaster.pop('error', error);
                            });
                            if (!data.errors) {
                                toaster.pop('error', 'Не удалось удалить файл');
                            }
                        }
                    });
                }
            });
        };

        /* end attachments */

        /* templateDocx */

        ctrl.getTemplateDocx = function () {
            $http.get('booking/getTemplatesByType', {}).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    ctrl.templatesDocx = data.obj.Templates;
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });
                    if (!data.errors) {
                        toaster.pop('error', 'Ошибка', 'Ошибка при загрузке шаблонов документов');
                    }
                }
            });
        };
        ctrl.createDocx = function () {
            var params = {
                bookingId: ctrl.bookingInfo.id,
                templatesDocx: ctrl.selectedTemplatesDocx.slice(),
                attach: ctrl.templatesDocxAttach,
            };
            var url = 'booking/generateTemplates';
            if (ctrl.templatesDocxAttach) {
                ctrl.creatingDocxs = true;
                $http.post(url, params).then(function (response) {
                    var data = response.data;
                    if (data.result === false) {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });
                        if (!data.errors) {
                            toaster.pop('error', 'Ошибка', 'Ошибка при генерации документов');
                        }
                    } else {
                        ctrl.uploadAttachmentResult(data);
                    }
                    ctrl.creatingDocxs = false;
                    ctrl.selectedTemplatesDocx = [];
                    ctrl.templatesDocxAttach = false;
                    ctrl.getAttachments();
                });
            } else {
                $window.location.href = url + '?' + $httpParamSerializer(params);
            }
        };

        /* end templateDocx */

        ctrl.addServicesModal = function (result) {
            if (result == null || result.servicesIds == null || result.servicesIds.length === 0) return;
            if (ctrl.checkStopEdit()) {
                $http
                    .post('booking/addUpdateBookingItems', {
                        bookingId: ctrl.bookingInfo.id,
                        currentItems: ctrl.gridBookingItems.gridOptions.data,
                        newServiceIds: result.servicesIds,
                    })
                    .then(function (response) {
                        var data = response.data;
                        if (data.result === true) {
                            ctrl.gridBookingItems.gridOptions.data = data.obj.NewItems;
                            ctrl.updateSummary();
                        } else {
                            data.errors.forEach(function (error) {
                                toaster.pop('error', error);
                            });
                            if (!data.errors) {
                                toaster.pop('error', $translate.instant('Admin.Js.BookingJournal.FailedToAddItems'));
                            }
                        }
                    });
            }
        };
        ctrl.deleteBookingItem = function (item) {
            SweetAlert.confirm('Вы уверены, что хотите удалить услугу?', {
                title: 'Удаление',
            }).then(function (result) {
                if (result === true || result.value) {
                    if (ctrl.checkStopEdit()) {
                        var indexItem = ctrl.gridBookingItems.gridOptions.data.indexOf(item);
                        if (indexItem > -1) {
                            ctrl.gridBookingItems.gridOptions.data.splice(indexItem, 1);
                            ctrl.updateSummary();
                        }
                    }
                }
            });
        };
        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
        ctrl.checkStopEdit = function () {
            var result = true;
            if (ctrl.isPaied === true) {
                SweetAlert.alert('Оплаченная бронь не может быть изменена', {
                    title: 'Изменение брони',
                });
                result = false;
            }
            return result;
        };
    };
    ModalAddUpdateBookingCtrl.$inject = [
        '$uibModalInstance',
        '$http',
        '$httpParamSerializer',
        '$q',
        'toaster',
        'uiGridCustomConfig',
        'SweetAlert',
        '$translate',
        'bookingService',
        '$window',
        '$scope',
        'adminWebNotificationsEvents',
        'adminWebNotificationsService',
        '$uibModal',
        '$timeout',
    ];
    ng.module('uiModal').controller('ModalAddUpdateBookingCtrl', ModalAddUpdateBookingCtrl);
})(window.angular);
