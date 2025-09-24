(function (ng) {
    'use strict';
    ng.module('notificationMessage').directive('notificationMessage', function () {
        return {
            controller: 'NotificationMessageCtrl',
            controllerAs: '$ctrl',
            scope: true,
            bindToController: true,
        };
    });
})(window.angular);
