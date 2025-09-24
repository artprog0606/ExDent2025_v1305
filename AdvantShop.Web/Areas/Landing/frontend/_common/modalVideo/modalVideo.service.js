(function (ng) {
    'use strict';

    var modalVideoService = function ($window) {
        var service = this;
        var onChangeMQCallbackList = [];
        var isSetMq = false;

        service.setMQ = function (mqString) {
            if (!isSetMq) {
                isSetMq = true;
                service.mqState = $window.matchMedia(mqString);
                service.mqState.addListener(function (event) {
                    onChangeMQCallbackList.forEach(function (callback) {
                        callback(service.mqState.matches);
                    });
                });
            }
        };

        service.addCallbackOnChangeMQ = function (callback) {
            onChangeMQCallbackList.push(callback);
        };

        service.getMQState = function () {
            return service.mqState.matches;
        };
    };

    ng.module('modalVideo').service('modalVideoService', modalVideoService);

    modalVideoService.$inject = ['$window'];
})(window.angular);
