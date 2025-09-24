(function (ng) {
    'use strict';

    var triggersService = function ($http) {
        var service = this;

        service.getTrigger = function (id) {
            return $http.get('triggers/getTrigger', { params: { id: id } }).then(function (response) {
                return response.data;
            });
        };

        service.getTriggerFormData = function (eventType, objectTypes) {
            return $http.get('triggers/getTriggerFormData', { params: { eventType: eventType, objectTypes: objectTypes } }).then(function (response) {
                return response.data;
            });
        };

        service.deleteTrigger = function (id) {
            return $http.post('triggers/deleteTrigger', { id: id }).then(function (response) {
                return response.data;
            });
        };

        service.saveName = function (id, name) {
            return $http.post('triggers/saveName', { id: id, name: name }).then(function (response) {
                return response.data;
            });
        };

        service.deleteTriggers = function () {
            return $http.post('salesChannels/delete', { type: 'Triggers' }).then(function (response) {
                return response.data;
            });
        };
    };

    triggersService.$inject = ['$http'];

    ng.module('triggers').service('triggersService', triggersService);
})(window.angular);
