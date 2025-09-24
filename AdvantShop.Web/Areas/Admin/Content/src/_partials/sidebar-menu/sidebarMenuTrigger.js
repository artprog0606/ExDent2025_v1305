(function (ng) {
    'use strict';

    var SidebarMenuTriggerCtrl = function ($rootScope, sidebarMenuService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            sidebarMenuService.addCallback(function () {
                $rootScope.$broadcast('uiGridCustomAutoResize');
            });
        };

        ctrl.toggle = function () {
            sidebarMenuService.toggle();
        };
    };

    SidebarMenuTriggerCtrl.$inject = ['$rootScope', 'sidebarMenuService'];

    ng.module('sidebarMenu').controller('SidebarMenuTriggerCtrl', SidebarMenuTriggerCtrl);
})(window.angular);
