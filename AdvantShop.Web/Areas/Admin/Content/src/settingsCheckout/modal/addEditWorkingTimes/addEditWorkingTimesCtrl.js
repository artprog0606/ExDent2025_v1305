import addEditAdditionalTimeTemplate from '../addEditAdditionalWorkingTime/addEditAdditionalWorkingTime.html';
(function (ng) {
    'use strict';

    var ModalAddEditWorkingTimesCtrl = function ($uibModalInstance, toaster, SweetAlert, $translate, $http, $uibModal) {
        var ctrl = this;
        ctrl.daysOfWeek = ['Понедельник', 'Вторник', 'Среда', 'Четверг', 'Пятница', 'Суббота', 'Воскресенье'];
        ctrl.daysOfWeekNameInHeader = ['понедельник', 'вторник', 'среду', 'четверг', 'пятницу', 'субботу', 'воскресенье'];
        ctrl.months = ['01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12'];

        ctrl.$onInit = function () {
            ctrl.getSettings();
        };

        ctrl.getSettings = function () {
            return $http.get('SettingsCheckout/GetWorkingTimeSettings').then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    if (data.obj.TimeZoneOffset.startsWith('-')) {
                        ctrl.isPlusGmt = false;
                        data.obj.TimeZoneOffset = data.obj.TimeZoneOffset.slice(1);
                    } else {
                        ctrl.isPlusGmt = true;
                    }
                    ctrl.data = data.obj;
                    ctrl.checkIntervalIsNotEmpty();
                    ctrl.form.$setPristine();
                } else {
                    if (data.errors && data.errors.length) {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });
                    } else {
                        return toaster.pop('error', '', 'Не удалось загрузить настройки');
                    }
                }
            });
        };

        ctrl.init = function (form) {
            ctrl.form = form;
            ctrl.form.$setPristine();
        };

        ctrl.deleteAllIntervals = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.WorkingTime.Settings.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.WorkingTime.Settings.DeletingIntervals'),
            }).then(function (result) {
                if (result === true || result.value) {
                    ctrl.data.WorkingTimes = {};
                    ctrl.intervalIsNotEmpty = false;
                    ctrl.form.modified = true;
                }
            });
        };

        ctrl.deleteAllIntervalsByDay = function (index) {
            SweetAlert.confirm($translate.instant('Admin.Js.WorkingTime.Settings.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.WorkingTime.Settings.DeletingIntervals') + ' на ' + ctrl.daysOfWeekNameInHeader[index],
            }).then(function (result) {
                if (result === true || result.value) {
                    var key = index == 6 ? 0 : Number(index) + 1;
                    if (ctrl.data.WorkingTimes.hasOwnProperty(key)) {
                        ctrl.data.WorkingTimes[key] = null;
                        ctrl.checkIntervalIsNotEmpty();
                        ctrl.form.modified = true;
                    }
                }
            });
        };

        ctrl.deleteInterval = function (dayIndex, index) {
            var key = dayIndex == 6 ? 0 : Number(dayIndex) + 1;
            if (ctrl.data.WorkingTimes.hasOwnProperty(key)) {
                ctrl.data.WorkingTimes[key].splice(index, 1);
                if (ctrl.data.WorkingTimes[key].length == 0) ctrl.data.WorkingTimes[key] = null;
            }
            ctrl.checkIntervalIsNotEmpty();
            ctrl.form.modified = true;
        };

        ctrl.addIntervals = function () {
            if (!ctrl.selectedDays) return toaster.pop('error', '', 'Не выбран день');
            if (ctrl.showIntervalsGenerator && (ctrl.intervalLength == null || ctrl.intervalLength <= 0))
                return toaster.pop('error', '', 'Не верно указана длина интервала');
            if (!validateTimePicker()) return;

            ctrl.selectedDays.forEach(function (selectedDay) {
                addInterval(selectedDay);
            });
            ctrl.intervalIsNotEmpty = true;
            ctrl.form.modified = true;
        };

        function addInterval(selectedDay) {
            if (!validateTimePicker()) return;
            var index = Number(selectedDay);
            var key = index == 6 ? 0 : Number(index) + 1;

            var interval = {
                TimeFrom: ctrl.timeFrom,
                TimeTo: ctrl.timeTo,
            };
            if (ctrl.data.WorkingTimes.hasOwnProperty(key)) {
                if (
                    ctrl.data.WorkingTimes[key].some(
                        (x) =>
                            (x.TimeFrom == interval.TimeFrom && x.TimeTo == interval.TimeTo) ||
                            (x.TimeFrom == '00:00' && x.TimeTo == '00:00') ||
                            (x.TimeFrom < interval.TimeTo && x.TimeTo > interval.TimeTo) ||
                            (x.TimeFrom < interval.TimeFrom && x.TimeTo > interval.TimeFrom) ||
                            (x.TimeFrom >= interval.TimeFrom && x.TimeTo <= interval.TimeTo),
                    )
                )
                    return toaster.pop('error', '', ctrl.daysOfWeek[index] + ' содержит интервал, который накладывается на указанный');
                ctrl.data.WorkingTimes[key].push(interval);
                ctrl.data.WorkingTimes[key].sort();
            } else ctrl.data.WorkingTimes[key] = [interval];
        }

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

        ctrl.getIntervals = function (index) {
            var key = index == 6 ? 0 : Number(index) + 1;
            if (ctrl.data.WorkingTimes.hasOwnProperty(key)) {
                return ctrl.data.WorkingTimes[key] || [];
            }
            return [];
        };

        ctrl.checkIntervalIsNotEmpty = function () {
            var intervalsIsNotEmpty = false;
            Object.keys(ctrl.data.WorkingTimes).forEach(function (key) {
                if (ctrl.data.WorkingTimes[key] != null && ctrl.data.WorkingTimes[key].length != 0) {
                    intervalsIsNotEmpty = true;
                    return;
                }
            });
            ctrl.intervalIsNotEmpty = intervalsIsNotEmpty;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function () {
            var workingTimes = [];
            Object.keys(ctrl.data.WorkingTimes).forEach(function (key) {
                workingTimes.push({ Key: key, Value: ctrl.data.WorkingTimes[key] });
            });
            var params = {
                TimeZoneOffset: (ctrl.isPlusGmt ? '' : '-') + ctrl.data.TimeZoneOffset,
                WorkingTimes: workingTimes,
                NotAllowCheckoutText: ctrl.data.NotAllowCheckoutText,
            };
            $http.post('SettingsCheckout/SaveWorkingTimeSettings', params).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.ChangesSaved'));
                    $uibModalInstance.close();
                } else {
                    if (data.errors && data.errors.length) {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });
                    } else {
                        return toaster.pop('error', '', 'Не удалось сохранить');
                    }
                }
            });
        };

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
                if (ctrl.data.AdditionalDate && ctrl.data.AdditionalDate.indexOf(d.getDate() + ctrl.months[d.getMonth()] + d.getFullYear()) !== -1) {
                    e[0].className += ' day-additional-time';
                }
            },
        };

        ctrl.calendarAdditionalTimeOnInit = function (calendar) {
            ctrl.calendarAdditionalTime = calendar;
        };

        ctrl.onAdditionalTimeAddUpdateDelete = function () {
            ctrl.getSettings().then(function () {
                ctrl.calendarAdditionalTime.setYear(ctrl.calendarAdditionalTime.getYear());
            });
        };

        ctrl.loadAdditionalTime = function (startDate, endDate) {
            if (startDate instanceof Date) {
                startDate = startDate.getFullYear() + '-' + (startDate.getMonth() + 1) + '-' + startDate.getDate();
            }
            if (endDate instanceof Date) {
                endDate = endDate.getFullYear() + '-' + (endDate.getMonth() + 1) + '-' + endDate.getDate();
            }

            var params = {};

            if (endDate) {
                params.startDate = startDate;
                params.endDate = endDate;
            } else {
                params.date = startDate;
            }

            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddEditAdditionalWorkingTimeCtrl',
                    controllerAs: 'ctrl',
                    size: 'xs-4',
                    backdrop: 'static',
                    templateUrl: addEditAdditionalTimeTemplate,
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
                        return result;
                    },
                );
        };

        ctrl.changeModal = function () {
            if (!ctrl.showAdditionalSettings) ctrl.showAdditionalSettings = true;
            else ctrl.showAdditionalSettings = false;
        };
    };

    ModalAddEditWorkingTimesCtrl.$inject = ['$uibModalInstance', 'toaster', 'SweetAlert', '$translate', '$http', '$uibModal'];

    ng.module('uiModal').controller('ModalAddEditWorkingTimesCtrl', ModalAddEditWorkingTimesCtrl);
})(window.angular);
