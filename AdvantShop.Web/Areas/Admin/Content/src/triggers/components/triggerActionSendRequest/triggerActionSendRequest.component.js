import triggerActionSendRequestTemplate from './templates/triggerActionSendRequest.html';
(function (ng) {
    'use strict';

    ng.module('triggers').component('triggerActionSendRequest', {
        templateUrl: triggerActionSendRequestTemplate,
        controller: 'TriggerActionSendRequestCtrl',
        controllerAs: 'ctrl',
        bindings: {
            eventType: '=',
            action: '=',
            sendRequestParameters: '=',
        },
    });
})(window.angular);
