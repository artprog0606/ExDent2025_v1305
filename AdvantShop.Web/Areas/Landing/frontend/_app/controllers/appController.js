(function (ng) {
    'use strict';

    var AppCtrl = function ($window) {};

    AppCtrl.$inject = ['$window'];

    var module = ng.module('app');

    module.controller('AppCtrl', AppCtrl);
})(window.angular);
