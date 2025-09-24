(function (ng) {
    'use strict';

    ng.module('lpMenu').directive('lpMenuState', function () {
        return {
            controller: 'LpMenuStateCtrl',
            controllerAs: 'lpMenuState',
            scope: true,
            bindToController: true,
        };
    });

    ng.module('lpMenu').directive('lpMenuTrigger', function () {
        return {
            controller: 'LpMenuTriggerCtrl',
            controllerAs: 'lpMenuTrigger',
            scope: true,
            bindToController: true,
        };
    });
})(window.angular);
