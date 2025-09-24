(function (ng) {
    'use strict';

    ng.module('sidebarMenu')
        .directive('sidebarMenuState', function () {
            return {
                restrict: 'A',
                scope: true,
                controller: 'SidebarMenuStateCtrl',
                controllerAs: 'sidebarMenuState',
                bindToController: true,
            };
        })
        .directive('sidebarMenuTrigger', function () {
            return {
                restrict: 'A',
                scope: true,
                controller: 'SidebarMenuTriggerCtrl',
                controllerAs: 'sidebarMenuTrigger',
                bindToController: true,
            };
        });
})(window.angular);
