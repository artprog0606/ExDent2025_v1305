(function (ng) {
    'use strict';

    var SubBlockCountdownViewConstructorController = function () {
        var ctrl = this;

        ctrl.$onInit = function () {
            if (ctrl.date != null && ctrl.date.end_date == null) {
                ctrl.date.end_date = ctrl.date.selectType.value === 'default' ? flatpickr.formatDate(new Date(), 'd.m.Y H:i') : 30;
            } else if (ctrl.date != null && ctrl.date.selectType.value === 'default' && ctrl.date.end_date != null) {
                ctrl.date.end_date = flatpickr.formatDate(flatpickr.parseDate(ctrl.date.end_date, 'd.m.Y H:i'), 'd.m.Y H:i');
            }
        };

        ctrl.changeDate = function () {
            if (ctrl.onChange != null) {
                ctrl.onChange({ date: ctrl.date });
            }
        };

        ctrl.resetEndDate = function (selectType) {
            ctrl.date.end_date = selectType.value === 'default' ? flatpickr.formatDate(new Date(), 'd.m.Y H:i') : 30;

            if (ctrl.onChange != null) {
                ctrl.onChange({ date: ctrl.date });
            }
        };
    };

    ng.module('blocksConstructor').controller('SubBlockCountdownViewConstructorController', SubBlockCountdownViewConstructorController);

    SubBlockCountdownViewConstructorController.$inject = [];
})(window.angular);
