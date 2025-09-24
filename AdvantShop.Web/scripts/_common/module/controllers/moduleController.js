(function (ng) {
    'use strict';

    var ModuleCtrl = function () {
        var ctrl = this;
    };

    angular.module('module').controller('ModuleCtrl', ModuleCtrl);

    ModuleCtrl.$inject = [];
})(window.angular);
