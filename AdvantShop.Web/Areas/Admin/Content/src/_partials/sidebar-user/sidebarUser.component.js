import sidebarUserTemplate from './templates/sidebar-user.html';
(function (ng) {
    'use strict';

    ng.module('sidebarUser').component('sidebarUser', {
        templateUrl: sidebarUserTemplate,
        controller: 'SidebarUserCtrl',
        bindings: {
            close: '&',
            dismiss: '&',
            resolve: '<',
        },
    });
})(window.angular);
