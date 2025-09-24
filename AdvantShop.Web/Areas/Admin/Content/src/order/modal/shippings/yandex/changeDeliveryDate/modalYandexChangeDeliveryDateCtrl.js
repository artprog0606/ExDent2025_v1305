(function (ng) {
    'use strict';

    /* @ngInject */
    var ModalYandexChangeDeliveryDateCtrl = function ($uibModalInstance) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.datesOfDelivery = params.datesOfDelivery;
            ctrl.timesOfDelivery = params.timesOfDelivery;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function () {
            var data = {
                selectedInterval: ctrl.selectedInterval,
                dateOfDelivery: ctrl.dateOfDelivery,
                timeOfDelivery: ctrl.timeOfDelivery,
            };
            $uibModalInstance.close(data);
        };

        ctrl.changeInterval = function () {
            ctrl.timeOfDelivery = ctrl.getFormattedInerval(ctrl.selectedInterval.From, ctrl.selectedInterval.To);
        };

        ctrl.getTimesOfDelivery = function (dateOfDelivery) {
            return ctrl.timesOfDelivery[dateOfDelivery];
        };

        ctrl.getFormattedInerval = function (from, to) {
            const _from = new Date(from);
            const _to = new Date(to);
            var fromHours = _from.getHours(),
                fromMinutes = _from.getMinutes(),
                fromTimeFormatted = (fromHours < 10 ? '0' + fromHours : fromHours) + ':' + (fromMinutes < 10 ? '0' + fromMinutes : fromMinutes);
            var toHours = _to.getHours(),
                toMinutes = _to.getMinutes(),
                toTimeFormatted = (toHours < 10 ? '0' + toHours : toHours) + ':' + (toMinutes < 10 ? '0' + toMinutes : toMinutes);

            var offset = _from.getTimezoneOffset() / -60;
            var offsetRemains = offset % 1;
            var offsetStr = offset > 0 ? '+' + Math.trunc(offset) : Math.trunc(offset);
            offsetStr = offsetRemains == 0 ? offsetStr + ':00' : offsetStr + ':' + offsetRemains * 60;
            return fromTimeFormatted + '-' + toTimeFormatted + ' (GMT ' + offsetStr + ')';
        };
    };

    ng.module('uiModal').controller('ModalYandexChangeDeliveryDateCtrl', ModalYandexChangeDeliveryDateCtrl);
})(window.angular);
