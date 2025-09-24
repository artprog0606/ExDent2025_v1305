(function (ng) {
    'use strict';

    var ModalAddEditAdditionalWorkingTimeCtrl = function ($uibModalInstance, $http, toaster, SweetAlert, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;

            ctrl.startDate = params.startDate;
            ctrl.endDate = params.endDate;
            ctrl.date = params.date;
            ctrl.affiliateId = params.affiliateId;

            ctrl.mode = ctrl.startDate && ctrl.endDate ? 'range' : ctrl.date ? 'one-day' : 'select-day';
            ctrl.getAdditionalTimes();
        };

        ctrl.init = function (form) {
            ctrl.form = form;
            ctrl.form.$setPristine();
        };

        ctrl.getAdditionalTimes = function () {
            if (ctrl.mode === 'select-day' && !ctrl.date) {
                var now = new Date();
                ctrl.date = now.getFullYear() + '-' + (now.getMonth() + 1) + '-' + now.getDate();
            }

            var params = {};

            if (ctrl.startDate && ctrl.endDate) {
                params.dateFrom = ctrl.startDate;
                params.dateTo = ctrl.endDate;
            } else {
                params.date = ctrl.date;
            }

            return $http.get('settingsCheckout/getAdditionalWorkingTime', { params: params }).then(function (response) {
                var data = response.data;

                if (data.result === true) {
                    ctrl.workingTimes = data.obj.WorkingTimes;
                    ctrl.isWork = data.obj.IsWork;
                    if (typeof ctrl.isWork === 'undefined') {
                        ctrl.isWork = !data.obj.WorkTimes || !data.obj.WorkTimes.length;
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
                if (params.date) {
                    ctrl.form.$setPristine();
                }
            });
        };

        ctrl.save = function () {
            var params = {
                IsWork: ctrl.isWork,
                DateStart: ctrl.startDate || ctrl.date,
                DateEnd: ctrl.endDate || ctrl.date,
                WorkingTimes: ctrl.workingTimes,
            };
            return $http.post('settingsCheckout/SaveAdditionalWorkingTimeSettings', params).then(function (response) {
                if (response.data.result) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.ChangesSaved'));
                    $uibModalInstance.close();
                } else if (response.data.errors && response.data.errors.length) {
                    response.data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });
                } else {
                    return toaster.pop('error', '', 'Не удалось сохранить');
                }
            });
        };

        ctrl.delete = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.WorkingTime.Settings.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.WorkingTime.Settings.DeletingIntervals'),
            }).then(function (result) {
                if (result === true || result.value) {
                    var params = {
                        dateStart: ctrl.startDate || ctrl.date,
                        dateEnd: ctrl.endDate || ctrl.date,
                    };
                    $http.post('settingsCheckout/DeleteAdditionalWorkingTimes', params).then(function (response) {
                        if (response.data.result) $uibModalInstance.close();
                        else return toaster.pop('error', '', 'Не удалось удалить');
                    });
                }
            });
        };

        ctrl.addInterval = function () {
            if (!validateTimePicker()) return;
            var interval = {
                TimeFrom: ctrl.timeFrom,
                TimeTo: ctrl.timeTo,
            };
            if (ctrl.workingTimes) {
                if (
                    ctrl.workingTimes.some(
                        (x) =>
                            (x.TimeFrom == interval.TimeFrom && x.TimeTo == interval.TimeTo) ||
                            (x.TimeFrom == '00:00' && x.TimeTo == '00:00') ||
                            (x.TimeFrom < interval.TimeTo && x.TimeTo > interval.TimeTo) ||
                            (x.TimeFrom < interval.TimeFrom && x.TimeTo > interval.TimeFrom),
                    )
                )
                    return toaster.pop('error', '', 'Уже есть интервал, который накладывается на указанный');
                ctrl.workingTimes.push(interval);
                ctrl.workingTimes.sort((a, b) => a.TimeFrom.split(':')[0] - b.TimeFrom.split(':')[0]);
            } else ctrl.workingTimes = [interval];
            ctrl.form.modified = true;
        };

        function validateTimePicker() {
            if (!ctrl.timeFrom || !ctrl.timeTo) {
                toaster.pop('error', '', 'Не заполнен интервал');
                return false;
            }

            if (ctrl.timeFrom == ctrl.timeTo && ctrl.timeTo != '00:00') {
                toaster.pop('error', '', 'Одинаковое время в интервале');
                return false;
            }

            if (ctrl.timeTo != '00:00' && ctrl.timeFrom > ctrl.timeTo) {
                toaster.pop('error', '', 'Время начала больше конца');
                return false;
            }
            return true;
        }

        ctrl.deleteInterval = function (index) {
            ctrl.workingTimes.splice(index, 1);
            ctrl.form.modified = true;
        };

        ctrl.toLocaleDateString = function (date) {
            if (date instanceof Date) {
                return date.toLocaleDateString();
            }

            return new Date(date).toLocaleDateString();
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalAddEditAdditionalWorkingTimeCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', 'SweetAlert', '$translate'];

    ng.module('uiModal').controller('ModalAddEditAdditionalWorkingTimeCtrl', ModalAddEditAdditionalWorkingTimeCtrl);
})(window.angular);
