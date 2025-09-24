import triggerActionSendNotificationTemplate from './templates/triggerActionSendNotification.html';
(function (ng) {
    'use strict';

    ng.module('triggers').component('triggerActionSendNotification', {
        templateUrl: triggerActionSendNotificationTemplate,
        controller: 'TriggerActionSendNotificationCtrl',
        controllerAs: 'ctrl',
        bindings: {
            action: '=',
            allowSendNotification: '=',
            pushNotificationErrorMessage: '=',
            availableVariables: '=',
        },
    });
})(window.angular);
