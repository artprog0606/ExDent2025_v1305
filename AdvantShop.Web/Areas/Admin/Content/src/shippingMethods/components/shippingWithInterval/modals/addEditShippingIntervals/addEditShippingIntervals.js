import './addEditShippingIntervals.html';

(function (ng) {
    'use strict';

    var ModalAddEditShippingIntervalsCtrl = function ($uibModalInstance, toaster, SweetAlert, $translate) {
        var ctrl = this;
        ctrl.daysOfWeek = ['Понедельник', 'Вторник', 'Среда', 'Четверг', 'Пятница', 'Суббота', 'Воскресенье'];
        ctrl.daysOfWeekNameInHeader = ['понедельник', 'вторник', 'среду', 'четверг', 'пятницу', 'субботу', 'воскресенье'];

        ctrl.$onInit = function () {
            if (ctrl.$resolve && ctrl.$resolve.data) {
                ctrl.data = ng.copy(ctrl.$resolve.data);
                ctrl.checkDeliveryIntervalIsNotEmpty();
            }
        };

        ctrl.init = function (form) {
            ctrl.form = form;
            ctrl.form.$setPristine();
        };

        ctrl.deleteAllIntervals = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.ShippingWithInterval.Settings.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.ShippingWithInterval.Settings.DeletingIntervals'),
            }).then(function (result) {
                if (result === true || result.value) {
                    ctrl.data.deliveryIntervals = {};
                    ctrl.deliveryIntervalIsNotEmpty = false;
                    ctrl.form.modified = true;
                }
            });
        };

        ctrl.deleteAllIntervalsByDay = function (index) {
            SweetAlert.confirm($translate.instant('Admin.Js.ShippingWithInterval.Settings.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.ShippingWithInterval.Settings.DeletingIntervals') + ' на ' + ctrl.daysOfWeekNameInHeader[index],
            }).then(function (result) {
                if (result === true || result.value) {
                    var key = index == 6 ? 0 : Number(index) + 1;
                    if (ctrl.data.deliveryIntervals.hasOwnProperty(key)) {
                        ctrl.data.deliveryIntervals[key] = [];
                        ctrl.checkDeliveryIntervalIsNotEmpty();
                        ctrl.form.modified = true;
                    }
                }
            });
        };

        ctrl.deleteInterval = function (dayIndex, index) {
            var key = dayIndex == 6 ? 0 : Number(dayIndex) + 1;
            if (ctrl.data.deliveryIntervals.hasOwnProperty(key)) ctrl.data.deliveryIntervals[key].splice(index, 1);
            ctrl.checkDeliveryIntervalIsNotEmpty();
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
            ctrl.deliveryIntervalIsNotEmpty = true;
            ctrl.form.modified = true;
        };

        function addInterval(selectedDay) {
            if (!validateTimePicker()) return;
            var index = Number(selectedDay);
            var key = index == 6 ? 0 : Number(index) + 1;

            var interval = ctrl.timeFrom + '-' + ctrl.timeTo;
            if (ctrl.data.deliveryIntervals.hasOwnProperty(key)) {
                if (ctrl.data.deliveryIntervals[key].indexOf(interval) != -1)
                    return toaster.pop('error', '', ctrl.daysOfWeek[index] + ' уже содержит указанный интервал');
                ctrl.data.deliveryIntervals[key].push(interval);
                ctrl.data.deliveryIntervals[key].sort();
            } else ctrl.data.deliveryIntervals[key] = [interval];
        }

        ctrl.generateIntervals = function (data) {
            if (!data) return;
            data.selectedDays.forEach(function (selectedDay) {
                var index = Number(selectedDay);
                var key = index == 6 ? 0 : Number(index) + 1;
                var timeArr = data.timeFrom.split(':');
                var totalFromMinutes = Number(timeArr[0]) * 60 + Number(timeArr[1]);
                timeArr = data.timeTo.split(':');
                var totalToMinutes = Number(timeArr[0]) * 60 + Number(timeArr[1]);
                var timeToMinutes = totalFromMinutes + data.intervalLength;
                if (!ctrl.data.deliveryIntervals[key]) ctrl.data.deliveryIntervals[key] = [];
                var needSort = ctrl.data.deliveryIntervals[key].length > 0;
                for (; timeToMinutes <= totalToMinutes; totalFromMinutes += data.creationStep) {
                    var timeFrom = getTimeStrFromTotalMinutes(totalFromMinutes);
                    var interval = timeFrom + '-' + getTimeStrFromTotalMinutes(timeToMinutes);

                    if (!needSort || ctrl.data.deliveryIntervals[key].indexOf(interval) == -1) {
                        var i = 0;
                        for (; i < ctrl.data.deliveryIntervals[key].length; i++) {
                            if (ctrl.data.deliveryIntervals[key][i].startsWith(timeFrom)) ctrl.data.deliveryIntervals[key].splice(i, 1);
                        }
                        ctrl.data.deliveryIntervals[key].push(interval);
                    }
                    timeToMinutes += data.creationStep;
                }
                if (needSort) ctrl.data.deliveryIntervals[key].sort();
            });
            ctrl.deliveryIntervalIsNotEmpty = true;
            ctrl.form.modified = true;
        };

        function getTimeStrFromTotalMinutes(totalMinutes) {
            var totalHours = totalMinutes / 60;
            var minutes = Math.round((totalHours % 1) * 60);
            var hours = Math.trunc(totalHours);
            return (hours < 10 ? '0' + hours : hours) + ':' + (minutes < 10 ? '0' + minutes : minutes);
        }

        function validateTimePicker() {
            if (!ctrl.timeFrom || !ctrl.timeTo) {
                toaster.pop('error', '', 'Не заполнен интервал');
                return false;
            }

            if (ctrl.timeFrom == ctrl.timeTo) {
                toaster.pop('error', '', 'Одинаковое время в интервале');
                return false;
            }

            if (ctrl.timeFrom > ctrl.timeTo) {
                toaster.pop('error', '', 'Время начала больше конца');
                return false;
            }
            return true;
        }

        ctrl.getIntervals = function (index) {
            var key = index == 6 ? 0 : Number(index) + 1;
            if (ctrl.data.deliveryIntervals.hasOwnProperty(key)) {
                return ctrl.data.deliveryIntervals[key];
            }
            return [];
        };

        ctrl.checkDeliveryIntervalIsNotEmpty = function () {
            var intervalsIsNotEmpty = false;
            Object.keys(ctrl.data.deliveryIntervals).forEach(function (key) {
                if (ctrl.data.deliveryIntervals[key] != null && ctrl.data.deliveryIntervals[key].length != 0) {
                    intervalsIsNotEmpty = true;
                    return;
                }
            });
            ctrl.deliveryIntervalIsNotEmpty = intervalsIsNotEmpty;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function () {
            $uibModalInstance.close(ctrl.data);
        };
    };

    ModalAddEditShippingIntervalsCtrl.$inject = ['$uibModalInstance', 'toaster', 'SweetAlert', '$translate'];

    ng.module('uiModal').controller('ModalAddEditShippingIntervalsCtrl', ModalAddEditShippingIntervalsCtrl);
})(window.angular);
