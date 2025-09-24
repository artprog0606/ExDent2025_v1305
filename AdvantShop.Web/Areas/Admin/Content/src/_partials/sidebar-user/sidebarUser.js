(function (ng) {
    'use strict';

    var SidebarUserCtrl = function () {
        var ctrl = this;

        ctrl.$onInit = function () {
            if (ctrl.resolve != null && ctrl.resolve.params != null && ctrl.resolve.params.user != null) {
                ctrl.user = ctrl.resolve.params.user;
            }
        };

        ctrl.close = function () {
            ctrl.dismiss({ $value: ctrl });
        };
    };

    ng.module('sidebarUser', []).controller('SidebarUserCtrl', SidebarUserCtrl);
})(window.angular);
