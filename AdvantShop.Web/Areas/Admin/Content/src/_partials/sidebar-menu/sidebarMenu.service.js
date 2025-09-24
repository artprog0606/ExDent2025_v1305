(function (ng) {
    'use strict';

    var CSS_ANIMATION_DELAY = 300;

    var sidebarMenuService = function ($cookies, $timeout) {
        var service = this;
        var KEY = 'adminSidebarMenu';
        var callbacks = [];
        var callbacksMenuStates = [];

        service.getState = function () {
            return $cookies.getObject(KEY) || false;
        };

        service.setState = function (value) {
            $cookies.putObject(KEY, value);
            return value;
        };

        service.toggle = function () {
            var state = service.getState();
            var stateNew = !state;
            service.setState(stateNew);

            service.processMenuStatesCallback(stateNew);

            $timeout(function () {
                service.processCallback(stateNew);
            }, CSS_ANIMATION_DELAY);
        };

        service.addCallback = function (fn) {
            callbacks.push(fn);
        };

        service.addCallbackForMenuStates = function (fn) {
            callbacksMenuStates.push(fn);
        };

        service.processCallback = function (value) {
            callbacks.forEach(function (fn) {
                fn(value);
            });
        };

        service.processMenuStatesCallback = function (value) {
            callbacksMenuStates.forEach(function (fn) {
                fn(value);
            });
        };
    };

    sidebarMenuService.$inject = ['$cookies', '$timeout'];

    ng.module('sidebarMenu').service('sidebarMenuService', sidebarMenuService);
})(window.angular);
