import './intervalsByRange.html';

(function (ng) {
    'use strict';

    var ModalGenerateIntervalsCtrl = function ($uibModalInstance, toaster) {
        var ctrl = this;
        ctrl.daysOfWeek = ['Понедельник', 'Вторник', 'Среда', 'Четверг', 'Пятница', 'Суббота', 'Воскресенье'];

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.generate = function () {
            if (!validateTimePicker()) return;
            var data = {
                timeFrom: ctrl.timeFrom,
                timeTo: ctrl.timeTo,
                selectedDays: ctrl.selectedDays,
                intervalLength: ctrl.intervalLength,
                creationStep: ctrl.creationStep,
            };
            $uibModalInstance.close(data);
        };
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
    };

    ModalGenerateIntervalsCtrl.$inject = ['$uibModalInstance', 'toaster'];

    ng.module('uiModal').controller('ModalGenerateIntervalsCtrl', ModalGenerateIntervalsCtrl);
})(window.angular);
