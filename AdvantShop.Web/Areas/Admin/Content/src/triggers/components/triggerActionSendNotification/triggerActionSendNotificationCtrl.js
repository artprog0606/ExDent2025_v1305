(function (ng) {
    'use strict';

    var TriggerActionSendNotificationCtrl = function () {
        var ctrl = this;

        ctrl.addParam = function () {
            if (!ctrl.action.NotificationRequestParams) ctrl.action.NotificationRequestParams = [];

            ctrl.action.NotificationRequestParams.push({ Key: ctrl.newParamKey, Value: ctrl.newParamValue });

            ctrl.newParamKey = null;
            ctrl.newParamValue = null;
            ctrl.showAddingNewParam = false;
        };

        ctrl.removeParam = function (key) {
            ctrl.action.NotificationRequestParams = ctrl.action.NotificationRequestParams.filter(function (x) {
                return x.Key != key;
            });
        };
    };

    ng.module('triggers').controller('TriggerActionSendNotificationCtrl', TriggerActionSendNotificationCtrl);
})(window.angular);
