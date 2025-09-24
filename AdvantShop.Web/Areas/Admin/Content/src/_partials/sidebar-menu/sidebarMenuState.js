(function (ng) {
    'use strict';

    var SidebarMenuStateCtrl = function (sidebarMenuService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.isCompact = sidebarMenuService.getState();

            sidebarMenuService.addCallbackForMenuStates(function (state) {
                ctrl.onChangeState(state);
            });
        };

        ctrl.onChangeState = function (state) {
            ctrl.isCompact = state;
        };

        ctrl.addCallback = function (callback) {
            sidebarMenuService.addCallback(callback);
        };
    };

    SidebarMenuStateCtrl.$inject = ['sidebarMenuService'];

    ng.module('sidebarMenu', []).controller('SidebarMenuStateCtrl', SidebarMenuStateCtrl);
})(window.angular);
