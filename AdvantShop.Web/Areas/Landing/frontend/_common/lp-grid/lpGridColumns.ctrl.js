(function (ng) {
    'use strict';

    var LpGridColumnsCtrl = function ($transclude, lpGridTypes) {
        var ctrl = this;

        ctrl.$onInit = function () {};
    };

    ng.module('lpGrid').controller('LpGridColumnsCtrl', LpGridColumnsCtrl);

    LpGridColumnsCtrl.$inject = ['$transclude', 'lpGridTypes'];
})(window.angular);
