(function (ng) {
    'use strict';

    var advTrackingService = function ($http) {
        var service = this;

        service.trackEvent = function (eventKey, eventKeyPostfix) {
            return $http
                .get(
                    'advantshopTracking/trackEvent?eventKey=' +
                        eventKey +
                        (eventKeyPostfix && eventKeyPostfix.length ? '&eventKeyPostfix=' + eventKeyPostfix : ''),
                )
                .then(function (response) {
                    return response.data;
                });
        };
    };

    advTrackingService.$inject = ['$http'];

    ng.module('advTracking', []).service('advTrackingService', advTrackingService);
})(window.angular);
