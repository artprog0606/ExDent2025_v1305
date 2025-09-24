import menuTreeviewTemplate from './templates/menuTreeview.html';
(function (ng) {
    'use strict';

    ng.module('menus').component('menuTreeview', {
        templateUrl: menuTreeviewTemplate,
        controller: 'MenuTreeviewCtrl',
        bindings: {
            selectedId: '@',
            type: '@',
            menuTreeviewOnInit: '&',
            level: '<?',
        },
    });
})(window.angular);
