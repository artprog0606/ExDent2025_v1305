(function (ng) {
    'use strict';

    ng.module('inplaceLanding').directive('inplaceLandingSwitch', function () {
        return {
            restrict: 'A',
            scope: true,
            controller: 'InplaceLandingSwitchCtrl',
            controllerAs: 'inplaceLandingSwitch',
            bindToController: true,
        };
    });

    //#endregion
})(window.angular);
