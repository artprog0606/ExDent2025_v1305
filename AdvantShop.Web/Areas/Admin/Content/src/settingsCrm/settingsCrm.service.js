(function (ng) {
    'use strict';

    var settingsCrmService = function ($http) {
        var service = this;

        service.setCrmActive = function (active) {
            return $http.post('settingsCrm/setCrmActive', { active: active });
        };

        service.getSalesFunnels = function () {
            return $http.get('salesFunnels/getSalesFunnels');
        };

        service.getOrderStatuses = function () {
            return $http.get('settingsCrm/getOrderStatuses');
        };

        service.saveDefaultSalesFunnelId = function (defaultSalesFunnelId) {
            return $http.post('settingsCrm/saveDefaultSalesFunnelId', { id: defaultSalesFunnelId });
        };

        service.saveOrderStatusIdFromLead = function (orderStatusIdFromLead) {
            return $http.post('settingsCrm/saveOrderStatusIdFromLead', { id: orderStatusIdFromLead });
        };
    };

    settingsCrmService.$inject = ['$http'];
    ng.module('settingsCrm').service('settingsCrmService', settingsCrmService);
})(window.angular);
