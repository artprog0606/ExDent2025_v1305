(function (ng) {
    'use strict';

    var MenusCtrl = function () {
        var ctrl = this;
        ctrl.showActions;

        ctrl.menuDictionary = {};

        ctrl.menuTreeviewInit = function (jstree, menuName) {
            ctrl.menuDictionary[menuName] = jstree;
        };

        ctrl.updateMenu = function (result, menuName) {
            ctrl.menuDictionary[menuName].refresh();
        };

        ctrl.swipeRight = function () {
            alert('right');
        };

        ctrl.swipeLeft = function () {
            alert('left');
        };
    };

    MenusCtrl.$inject = [];

    ng.module('menus', ['isMobile']).controller('MenusCtrl', MenusCtrl);
})(window.angular);
