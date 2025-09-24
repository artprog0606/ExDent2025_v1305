(function (ng) {
    'use strict';

    var CookiesPolicyCtrl = function ($cookies, $translate) {
        var ctrl = this;

        ctrl.accept = function () {
            ctrl.accepted = true;
            $cookies.put(ctrl.cookieName, 'true');
        };
    };

    angular.module('cookiesPolicy').controller('CookiesPolicyCtrl', CookiesPolicyCtrl);

    CookiesPolicyCtrl.$inject = ['$cookies', '$translate'];
})(window.angular);
