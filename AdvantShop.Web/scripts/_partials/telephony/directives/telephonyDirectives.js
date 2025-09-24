(function (ng) {
    'use strict';

    angular.module('telephony').directive('telephonyForm', function () {
        return {
            restrict: 'A',
            scope: true,
            controller: 'TelephonyFormCtrl',
            controllerAs: 'telephonyForm',
            bindToController: true,
        };
    });
})(window.angular);
