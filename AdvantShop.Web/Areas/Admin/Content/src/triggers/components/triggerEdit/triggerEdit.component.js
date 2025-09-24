import triggerEditTemplate from './templates/triggerEdit.html';
(function (ng) {
    'use strict';

    ng.module('triggers').component('triggerEdit', {
        templateUrl: triggerEditTemplate,
        controller: 'TriggerEditCtrl',
        controllerAs: 'ctrl',
        bindings: {
            id: '<?',
            eventType: '<?',
            onInit: '&',
        },
    });
})(window.angular);
