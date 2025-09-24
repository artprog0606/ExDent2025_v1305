(function (ng) {
    'use strict';

    var landingsService = function ($http, $uibModal) {
        var service = this;

        service.getLandings = function (page, size, search) {
            return $http
                .get('funnels/getLandings', {
                    params: { rnd: Math.random(), page: page, itemsPerPage: size, search: search },
                })
                .then(function (response) {
                    return response.data;
                });
        };

        service.deleteLanding = function (id) {
            return $http.post('funnels/deleteSiteLanding', { id: id }).then(function (response) {
                return response.data;
            });
        };

        service.updateTitle = function (id, value) {
            return $http.post('funnels/inplace', { id: id, name: value }).then(function (response) {
                return response.data;
            });
        };

        service.copyLandingPage = function (id) {
            return $http.post('funnels/copyLandingPage', { landingPageId: id }).then(function (response) {
                return response.data;
            });
        };

        service.copyLandingSite = function (id) {
            return $http.post('funnels/copyLandingSite', { landingSiteId: id }).then(function (response) {
                return response.data;
            });
        };
    };

    landingsService.$inject = ['$http', '$uibModal'];

    ng.module('landings').service('landingsService', landingsService);
})(window.angular);
