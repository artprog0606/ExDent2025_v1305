(function (ng) {
    'use strict';

    var ModalAddUpdateReservationResourceAdditionalTimeCtrl = function ($uibModalInstance, $http, toaster, SweetAlert, $translate, bookingService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;

            ctrl.date = params.date;
            ctrl.affiliateId = params.affiliateId;
            ctrl.reservationResourceId = params.reservationResourceId;

            ctrl.mode = ctrl.date ? 'selected-day' : 'selecting-day';

            ctrl.canBeEditing = false;

            if (ctrl.mode === 'selected-day') {
                ctrl.getAdditionalTimeForm().then(function () {
                    ctrl.getAdditionalTime(ctrl.affiliateId, ctrl.reservationResourceId, ctrl.date);
                });
            } else {
                ctrl.getAdditionalTimeForm();
                ctrl.canBeEditing = true;
            }
        };

        ctrl.getAdditionalTimeForm = function () {
            if (!ctrl.date) {
                var now = new Date();
                ctrl.date = now.getFullYear() + '-' + (now.getMonth() + 1) + '-' + now.getDate();
            }

            return $http
                .get('bookingResources/getAdditionalTimeFrom', {
                    params: {
                        affiliateId: ctrl.affiliateId,
                        reservationResourceId: ctrl.reservationResourceId,
                        date: ctrl.date,
                    },
                })
                .then(function (response) {
                    var data = response.data;

                    if (data.result === true) {
                        ctrl.times = data.obj.Times;
                        ctrl.workTimes = data.obj.WorkTimes;
                        ctrl.canBeEditing = data.obj.CanBeEditing;
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
                            toaster.pop('error', $translate.instant('Admin.Js.BookingUsers.FailedLoadData'));
                        }
                    }
                });
        };

        ctrl.getAdditionalTime = function (affiliateId, reservationResourceId, date) {
            return $http
                .get('bookingResources/getAdditionalTime', {
                    params: { affiliateId: affiliateId, reservationResourceId: reservationResourceId, date: date },
                })
                .then(function (response) {
                    var data = response.data;

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
                            toaster.pop('error', $translate.instant('Admin.Js.BookingUsers.FailedLoadData'));
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

        ctrl.addUpdateAdditionalTime = function (userConfirmed) {
            var url = 'bookingResources/updateAdditionalTime'; // ctrl.mode === 'selecting-day' ? 'bookingResources/addAdditionalTime' : 'bookingResources/updateAdditionalTime';

            var workTimes = !ctrl.freeDay
                ? ctrl.workTimes.filter(function (value) {
                      return ctrl.times.indexOf(value) !== -1;
                  })
                : [];

            var params = {
                affiliateId: ctrl.affiliateId,
                reservationResourceId: ctrl.reservationResourceId,
                date: ctrl.date,
                times: workTimes,
                userConfirmed: userConfirmed,
            };

            $http.post(url, params).then(function (result) {
                var data = result.data;
                if (data.result === true) {
                    if (data.obj && data.obj.UserConfirmIsRequired) {
                        SweetAlert.confirm(data.obj.ConfirmMessage, {
                            title: $translate.instant('Admin.Js.BookingUsers.SavingTime'),
                            confirmButtonText: data.obj.ConfirmButtomText,
                        }).then(
                            function (result) {
                                if (result === true || result.value === true) {
                                    ctrl.addUpdateAdditionalTime(true);
                                } else {
                                    ctrl.btnLoading = false;
                                }
                            },
                            function () {
                                ctrl.btnLoading = false;
                            },
                        );
                    } else {
                        toaster.pop('success', '', $translate.instant('Admin.Js.BookingUsers.TimeSaved'));
                        $uibModalInstance.close();
                    }
                } else {
                    ctrl.btnLoading = false;

                    if (data.errors && data.errors.length) {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });
                    } else {
                        toaster.pop('error', $translate.instant('Admin.Js.BookingUsers.FailedToSave'));
                    }
                }
            });
        };

        ctrl.selectionStop = function (selected, model) {
            bookingService.selectableTimeEventStop(selected, model);
        };

        ctrl.deleteAdditionalTime = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.BookingUsers.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.BookingUsers.Delete'),
            }).then(
                function (result) {
                    if (result === true || result.value === true) {
                        $http
                            .post('bookingResources/deleteAdditionalTime', {
                                date: ctrl.date,
                                affiliateId: ctrl.affiliateId,
                                reservationResourceId: ctrl.reservationResourceId,
                            })
                            .then(function (response) {
                                var data = response.data;
                                if (data.result === true) {
                                    toaster.pop('success', '', $translate.instant('Admin.Js.BookingUsers.TimeDeleted'));
                                    $uibModalInstance.close();
                                } else {
                                    ctrl.btnDeleteLoading = false;

                                    if (data.errors && data.errors.length) {
                                        data.errors.forEach(function (error) {
                                            toaster.pop('error', error);
                                        });
                                    } else {
                                        toaster.pop('error', $translate.instant('Admin.Js.BookingUsers.FailedToDelete'));
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

    ModalAddUpdateReservationResourceAdditionalTimeCtrl.$inject = [
        '$uibModalInstance',
        '$http',
        'toaster',
        'SweetAlert',
        '$translate',
        'bookingService',
    ];

    ng.module('uiModal').controller('ModalAddUpdateReservationResourceAdditionalTimeCtrl', ModalAddUpdateReservationResourceAdditionalTimeCtrl);
})(window.angular);
