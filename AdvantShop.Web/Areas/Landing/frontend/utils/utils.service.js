(function (ng) {
    'use strict';

    var utilsService = function () {
        var service = this;

        service.debounce = function (func, wait, immediate) {
            var timeout;
            return function executedFunction() {
                var context = this;
                var args = arguments;

                var later = function () {
                    timeout = null;
                    if (!immediate) func.apply(context, args);
                };

                var callNow = immediate && !timeout;

                clearTimeout(timeout);

                timeout = setTimeout(later, wait);

                if (callNow) func.apply(context, args);
            };
        };

        service.throttle = function (func, time) {
            return function (args) {
                var previousCall = this.lastCall;
                this.lastCall = Date.now();
                if (previousCall === undefined || this.lastCall - previousCall > time) {
                    func(args);
                }
            };
        };
    };

    ng.module('utils', []).service('utilsService', utilsService);
})(window.angular);
