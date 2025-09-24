import switcherStateTemplate from './templates/switcherState.html';
(function (ng) {
    'use strict';

    ng.module('switcherState').component('switcherState', {
        templateUrl: switcherStateTemplate,
        controller: 'SwitcherStateCtrl',
        bindings: {
            checked: '<?',
            onChange: '&',
            textOn: '@',
            textOff: '@',
            name: '@',
            invert: '<?',
            classesTriggerOn: '<?',
            classesTriggerOff: '<?',
            disabled: '<?',
        },
    });
})(window.angular);
