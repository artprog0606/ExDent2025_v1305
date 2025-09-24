(function (ng) {
    'use strict';

    var ModalAddUpdateBookingAffiliateAdditionalTimeCtrl = function (
        $uibModalInstance,
        $http,
        toaster,
        bookingAffiliateService,
        SweetAlert,
        $translate,
        bookingService,
    ) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;

            ctrl.startDate = params.startDate;
            ctrl.endDate = params.endDate;
            ctrl.date = params.date;
            ctrl.affiliateId = params.affiliateId;

            ctrl.mode = ctrl.startDate && ctrl.endDate ? 'range' : ctrl.date ? 'one-day' : 'select-day';

            if (ctrl.mode === 'one-day') {
                ctrl.getAdditionalTimeForm().then(function () {
                    ctrl.getAdditionalTime(ctrl.affiliateId, ctrl.date);
                });
            } else {
                ctrl.getAdditionalTimeForm();
            }
        };

        ctrl.getAdditionalTimeForm = function () {
            if (ctrl.mode === 'select-day' && !ctrl.date) {
                var now = new Date();
                ctrl.date = now.getFullYear() + '-' + (now.getMonth() + 1) + '-' + now.getDate();
            }

            var params = { affiliateId: ctrl.affiliateId };

            if (ctrl.startDate && ctrl.endDate) {
                params.startDate = ctrl.startDate;
                params.endDate = ctrl.endDate;
            } else {
                params.date = ctrl.date;
            }

            return $http.get('bookingAffiliate/getAdditionalTimeFrom', { params: params }).then(function (response) {
                var data = response.data;

                if (data.result === true) {
                    ctrl.times = data.obj.Times;
                    ctrl.workTimes = data.obj.WorkTimes;
                    ctrl.existAdditionalTimes = data.obj.ExistAdditionalTimes;
                    if (typeof ctrl.freeDay === 'undefined') {
                        ctrl.freeDay = !data.obj.WorkTimes || !data.obj.WorkTimes.length;
                    }
                } else {
                    if (data.errors && data.errors.length) {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });
                    } else {
                        toaster.pop('error', $translate.instant('Admin.Js.BookingAffiliate.FailedToLoadDataForForm'));
                    }
                }
            });
        };

        ctrl.getAdditionalTime = function (affiliateId, date) {
            return bookingAffiliateService.getAdditionalTime(affiliateId, date).then(function (data) {
                if (data.result === true) {
                    if (ctrl.existAdditionalTimes || (data.obj.Times && data.obj.Times.length)) {
                        ctrl.workTimes = data.obj.Times;
                        ctrl.freeDay = !data.obj.Times || !data.obj.Times.length;
                    }
                } else {
                    if (data.errors && data.errors.length) {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });
                    } else {
                        toaster.pop('error', $translate.instant('Admin.Js.BookingAffiliate.FailedToLoadDataForForm'));
                    }
                }
            });
        };

        ctrl.changeDate = function () {
            ctrl.getAdditionalTimeForm();
        };

        ctrl.changeFreeDay = function () {
            ctrl.freeDay = !ctrl.freeDay;
        };

        ctrl.toLocaleDateString = function (date) {
            if (date instanceof Date) {
                return date.toLocaleDateString();
            }

            return new Date(date).toLocaleDateString();
        };

        ctrl.addUpdateAdditionalTime = function () {
            var action = null;

            var workTimes = !ctrl.freeDay
                ? ctrl.workTimes.filter(function (value) {
                      return ctrl.times.indexOf(value) !== -1;
                  })
                : [];

            if (ctrl.mode === 'range') {
                action = bookingAffiliateService.updateAdditionalTimes(ctrl.affiliateId, ctrl.startDate, ctrl.endDate, workTimes);
            } else if (ctrl.mode === 'select-day') {
                action = bookingAffiliateService.addAdditionalTime(ctrl.affiliateId, ctrl.date, workTimes);
            } else {
                action = bookingAffiliateService.updateAdditionalTime(ctrl.affiliateId, ctrl.date, workTimes);
            }

            action.then(function (data) {
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.BookingAffiliate.TimeSuccessfullySaved'));
                    $uibModalInstance.close();
                } else {
                    ctrl.btnLoading = false;

                    if (data.errors && data.errors.length) {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });
                    } else {
                        toaster.pop('error', $translate.instant('Admin.Js.BookingAffiliate.FailedToSave'));
                    }
                }
            });
        };

        ctrl.selectionStop = function (selected, model) {
            bookingService.selectableTimeEventStop(selected, model);
        };

        ctrl.deleteAdditionalTime = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.BookingAffiliate.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.BookingAffiliate.Deleting'),
            }).then(
                function (result) {
                    if (result === true || result.value === true) {
                        var action = null;

                        if (ctrl.mode === 'range') {
                            action = bookingAffiliateService.deleteAdditionalTimes(ctrl.affiliateId, ctrl.startDate, ctrl.endDate);
                        } else {
                            action = bookingAffiliateService.deleteAdditionalTime(ctrl.affiliateId, ctrl.date);
                        }

                        action.then(function (data) {
                            if (data.result === true) {
                                toaster.pop('success', '', $translate.instant('Admin.Js.BookingAffiliate.TimeSuccessfullyDeleted'));
                                $uibModalInstance.close();
                            } else {
                                ctrl.btnDeleteLoading = false;

                                if (data.errors && data.errors.length) {
                                    data.errors.forEach(function (error) {
                                        toaster.pop('error', error);
                                    });
                                } else {
                                    toaster.pop('error', $translate.instant('Admin.Js.BookingAffiliate.FailedToDelete'));
                                }
                            }
                        });
                    }
                },
                function () {
                    ctrl.btnDeleteLoading = false;
                },
            );
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalAddUpdateBookingAffiliateAdditionalTimeCtrl.$inject = [
        '$uibModalInstance',
        '$http',
        'toaster',
        'bookingAffiliateService',
        'SweetAlert',
        '$translate',
        'bookingService',
    ];

    ng.module('uiModal').controller('ModalAddUpdateBookingAffiliateAdditionalTimeCtrl', ModalAddUpdateBookingAffiliateAdditionalTimeCtrl);
})(window.angular);
