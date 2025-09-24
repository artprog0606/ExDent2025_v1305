(function (ng) {
    'use strict';

    var ModalAddUpdateReservationResourceCtrl = function ($uibModalInstance, $http, toaster, SweetAlert, $translate, bookingService, Upload) {
        var ctrl = this;

        ctrl.isShowedAlertChangeBookingInterval = false;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;

            ctrl.id = params.id;
            ctrl.affiliateId = params.affiliateId;

            ctrl.mode = ctrl.id ? 'edit' : 'add';
            ctrl.canBeEditing = false;

            ctrl.defaultBookingInterval = true;
            ctrl.getReservationResourceForm(true).then(function () {
                if (ctrl.mode === 'add') {
                    ctrl.managerId = 0;
                    ctrl.active = true;
                    ctrl.canBeEditing = true;
                    ctrl.isInit = true;
                    ctrl.sortOrder = 0;
                } else {
                    ctrl.getReservationResource(ctrl.id, ctrl.affiliateId).then(function () {
                        ctrl.isInit = true;
                    });
                }
            });

            ctrl.daysOfTheWeek = ['понедельник', 'вторник', 'среда', 'четверг', 'пятница', 'суббота', 'воскресенье'];
            ctrl.dayOfTheWeek = ctrl.daysOfTheWeek[0];
        };

        ctrl.getReservationResourceForm = function (fistLoad) {
            return $http
                .post('bookingResources/getReservationResourceFormData', {
                    id: ctrl.id,
                    affiliateId: ctrl.affiliateId,
                    bookingIntervalMinutes: ctrl.bookingIntervalMinutes,
                    fistLoad: !!fistLoad,
                })
                .then(function (response) {
                    var data = response.data;

                    if (data.result === true) {
                        ctrl.managers = data.obj.Managers;
                        ctrl.managers.forEach(function (x) {
                            if (x.value === null) {
                                x.value = '0';
                            }
                        });

                        ctrl.affiliateBookingIntervalMinutes = data.obj.AffiliateBookingIntervalMinutes;
                        if (fistLoad) {
                            ctrl.oldBookingIntervalMinutes = ctrl.bookingIntervalMinutes = ctrl.affiliateBookingIntervalMinutes;
                        }
                        ctrl.bookingIntervals = data.obj.BookingIntervals;

                        ctrl.times = data.obj.Times;

                        ctrl.mondayWorkTimes = data.obj.MondayWorkTimes;
                        ctrl.tuesdayWorkTimes = data.obj.TuesdayWorkTimes;
                        ctrl.wednesdayWorkTimes = data.obj.WednesdayWorkTimes;
                        ctrl.thursdayWorkTimes = data.obj.ThursdayWorkTimes;
                        ctrl.fridayWorkTimes = data.obj.FridayWorkTimes;
                        ctrl.saturdayWorkTimes = data.obj.SaturdayWorkTimes;
                        ctrl.sundayWorkTimes = data.obj.SundayWorkTimes;

                        ctrl.mondayTimes = data.obj.MondayTimes;
                        ctrl.tuesdayTimes = data.obj.TuesdayTimes;
                        ctrl.wednesdayTimes = data.obj.WednesdayTimes;
                        ctrl.thursdayTimes = data.obj.ThursdayTimes;
                        ctrl.fridayTimes = data.obj.FridayTimes;
                        ctrl.saturdayTimes = data.obj.SaturdayTimes;
                        ctrl.sundayTimes = data.obj.SundayTimes;

                        ctrl.tags = data.obj.Tags;
                    } else {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });

                        if (!data.errors) {
                            toaster.pop(
                                'error',
                                $translate.instant('Admin.Js.BookingUsers.Error'),
                                $translate.instant('Admin.Js.BookingUsers.FailedLoadAddData'),
                            );
                        }
                    }
                });
        };

        ctrl.getReservationResource = function (id, affiliateId) {
            return $http.get('bookingResources/getReservationResource', { params: { id: id, affiliateId: affiliateId } }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    ctrl.name = data.obj.Name;
                    ctrl.managerId = data.obj.ManagerId || 0;
                    ctrl.description = data.obj.Description;
                    ctrl.active = data.obj.Enabled;
                    ctrl.sortOrder = data.obj.SortOrder;
                    ctrl.image = data.obj.Image;
                    ctrl.photoSrc = data.obj.PhotoSrc;
                    ctrl.selectedTags = data.obj.Tags;
                    ctrl.canBeEditing = data.obj.CanBeEditing;

                    ctrl.defaultBookingInterval = data.obj.BookingIntervalMinutes == null;
                    if (data.obj.BookingIntervalMinutes != null) {
                        ctrl.oldBookingIntervalMinutes = ctrl.bookingIntervalMinutes = data.obj.BookingIntervalMinutes;
                    }
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });

                    if (!data.errors) {
                        toaster.pop('error', $translate.instant('Admin.Js.BookingUsers.ErrorLoadingEmployeeData'));
                    }
                    ctrl.close();
                }
            });
        };

        ctrl.updateImage = function (params) {
            if (params != null) {
                ctrl.image = params.fileName;
                ctrl.photoSrc = ctrl.photoEncoded = params.base64String;
                ctrl.photoFile = params.file;
            }
        };

        ctrl.deleteImage = function () {
            if (ctrl.photoEncoded) {
                ctrl.image = null;
                ctrl.photoSrc = ctrl.photoEncoded = null;
                ctrl.photoFile = null;
            } else {
                $http.post('bookingResources/deleteReservationResourceImage', { id: ctrl.id }).then(function (response) {
                    var data = response.data;

                    if (data.result === true) {
                        ctrl.image = null;
                    } else {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });

                        if (!data.errors) {
                            toaster.pop('error', 'Ошибка', 'Ошибка при удалении изображения');
                        }
                    }
                });
            }
        };

        ctrl.addUpdateReservationResource = function () {
            var url = ctrl.mode === 'add' ? 'bookingResources/add' : 'bookingResources/update';

            if (ctrl.mondayTimes) {
                ctrl.mondayTimes = ctrl.mondayTimes.filter(function (value) {
                    return ctrl.mondayWorkTimes.indexOf(value) !== -1;
                });
            }
            if (ctrl.tuesdayTimes) {
                ctrl.tuesdayTimes = ctrl.tuesdayTimes.filter(function (value) {
                    return ctrl.tuesdayWorkTimes.indexOf(value) !== -1;
                });
            }
            if (ctrl.wednesdayTimes) {
                ctrl.wednesdayTimes = ctrl.wednesdayTimes.filter(function (value) {
                    return ctrl.wednesdayWorkTimes.indexOf(value) !== -1;
                });
            }
            if (ctrl.thursdayTimes) {
                ctrl.thursdayTimes = ctrl.thursdayTimes.filter(function (value) {
                    return ctrl.thursdayWorkTimes.indexOf(value) !== -1;
                });
            }
            if (ctrl.fridayTimes) {
                ctrl.fridayTimes = ctrl.fridayTimes.filter(function (value) {
                    return ctrl.fridayWorkTimes.indexOf(value) !== -1;
                });
            }
            if (ctrl.saturdayTimes) {
                ctrl.saturdayTimes = ctrl.saturdayTimes.filter(function (value) {
                    return ctrl.saturdayWorkTimes.indexOf(value) !== -1;
                });
            }
            if (ctrl.sundayTimes) {
                ctrl.sundayTimes = ctrl.sundayTimes.filter(function (value) {
                    return ctrl.sundayWorkTimes.indexOf(value) !== -1;
                });
            }
            var params = {
                id: ctrl.id,
                affiliateId: ctrl.affiliateId,
                managerId: ctrl.managerId || '',
                name: ctrl.name,
                description: ctrl.description,
                bookingIntervalMinutes: ctrl.defaultBookingInterval ? '' : ctrl.bookingIntervalMinutes,
                sortOrder: ctrl.sortOrder ?? '',
                enabled: ctrl.active,
                image: ctrl.image,

                MondayTimes: ctrl.mondayTimes,
                TuesdayTimes: ctrl.tuesdayTimes,
                WednesdayTimes: ctrl.wednesdayTimes,
                ThursdayTimes: ctrl.thursdayTimes,
                FridayTimes: ctrl.fridayTimes,
                SaturdayTimes: ctrl.saturdayTimes,
                SundayTimes: ctrl.sundayTimes,

                Tags: ctrl.selectedTags,
                PhotoFile: ctrl.photoFile,
            };

            Upload.upload({
                url: url,
                data: params,
            }).then(function (result) {
                var data = result.data;
                if (data.result === true) {
                    toaster.pop(
                        'success',
                        '',
                        ctrl.mode === 'add' ? $translate.instant('Admin.Js.BookingUsers.EmployeeSuccessfullyAdded') : 'Ресурс успешно сохранен',
                    );
                    $uibModalInstance.close();
                } else {
                    ctrl.btnLoading = false;
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });
                }
            });
        };

        ctrl.selectionStop = function (selected, model) {
            bookingService.selectableTimeEventStop(selected, model);
        };

        ctrl.onChangeBookingInterval = function () {
            if (!ctrl.isShowedAlertChangeBookingInterval) {
                SweetAlert.confirm(
                    // локализацию добавлять одной строкой (здесь разбира строка только для удобочтения кода). после локализации удалить коммент. спасибо :)
                    'Изменение интервала бронирования приведет к перестроению графика бронирования. ' +
                        'Вы уверены, что хотите изменить интервал бронирования?',
                    { title: 'Интервал бронирования', confirmButtonText: 'Да, продолжить' },
                ).then(function (result) {
                    if (result === true || result.value === true) {
                        ctrl.isShowedAlertChangeBookingInterval = true;
                        ctrl.getReservationResourceForm();
                    } else {
                        ctrl.bookingIntervalMinutes = ctrl.oldBookingIntervalMinutes;
                    }
                });
            } else {
                ctrl.getReservationResourceForm();
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getComponentCtrl = function (item) {
            ctrl.listOfReservationResourceServicesCtrl = item;
        };
    };

    ModalAddUpdateReservationResourceCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', 'SweetAlert', '$translate', 'bookingService', 'Upload'];

    ng.module('uiModal').controller('ModalAddUpdateReservationResourceCtrl', ModalAddUpdateReservationResourceCtrl);
})(window.angular);
