import personAvatarTemplate from './templates/person-avatar.html';
(function (ng) {
    'use strict';

    ng.module('personAvatar').component('personAvatar', {
        templateUrl: personAvatarTemplate,
        controller: 'PersonAvatarCtrl',
        transclude: true,
        bindings: {
            startValue: '@',
            noAvatarSrc: '@',
            customerId: '@',
            showLogout: '<',
            link: '@',
        },
    });
})(window.angular);
