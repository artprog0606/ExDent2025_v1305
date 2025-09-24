(function (ng) {
    'use strict';

    ng.module('inputGhost').directive('inputGhost', function () {
        return {
            require: {
                ngModel: 'ngModel',
            },
            controller: 'InputGhostCtrl',
            controllerAs: 'inputGhost',
            bindToController: true,
            compile: function (cElement) {
                cElement[0].classList.add('input-ghost');

                return function (scope, element, attrs) {
                    scope.$watch('inputGhost.ngModel.$viewValue', function (newVal) {
                        attrs.$set('size', newVal == null || newVal.length === 1 ? 1 : newVal.length);
                    });
                };
            },
        };
    });
})(window.angular);
