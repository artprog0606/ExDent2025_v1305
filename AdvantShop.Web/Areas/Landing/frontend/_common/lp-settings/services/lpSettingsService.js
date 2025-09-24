(function (ng) {
    'use strict';

    var lpSettingsService = function ($http, $q) {
        var service = this;

        service.get = function (lpId) {
            return $http.get('landing/landingInplace/getSettings', { params: { lpId: lpId, rnd: Math.random() } }).then(function (response) {
                return response.data;
            });
        };
    };

    ng.module('lpSettings').service('lpSettingsService', lpSettingsService);

    lpSettingsService.$inject = ['$http', '$q'];
})(window.angular);
