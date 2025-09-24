(function (ng) {
    'use strict';

    angular.module('cookiesPolicy').directive('cookiesPolicyModal', function () {
        return {
            restrict: 'A',
            scope: true,
            controller: 'CookiesPolicyCtrl',
            controllerAs: 'cookiesPolicy',
            bindToController: true,
            link: function (scope, elem, attrs, ctrl) {
                ctrl.cookieName = attrs.cookieName;
            },
        };
    });
})(window.angular);
