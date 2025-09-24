(function (ng) {
    'use strict';

    var productPropertiesService = function ($http) {
        var service = this;

        service.addPropertyWithValue = function (params) {
            return $http.post('product/AddPropertyWithValue', params).then(function (response) {
                return response.data;
            });
        };

        service.addPropertyValue = function (params) {
            return $http.post('product/addPropertyValue', params).then(function (response) {
                return response.data;
            });
        };

        service.removePropertyValue = function (params) {
            return $http.post('product/deletePropertyValue', params).then(function (response) {
                return response.data;
            });
        };

        service.findPropertyValue = function (propertyId, search) {
            return $http.get('product/getPropertyValues', { params: { propertyId: propertyId, search: search } }).then(function (response) {
                return response.data;
            });
        };

        service.getCurrentProperties = function (productId) {
            return $http.get('product/getProperties', { params: { productId: productId } }).then(function (response) {
                return response.data;
            });
        };

        service.getAllProperties = function (page, count, q, productId) {
            return $http
                .get('product/getAllProperties', { params: { page: page, count: count, q: q, productId: productId } })
                .then(function (response) {
                    return response.data;
                });
        };

        service.getAllPropertyValues = function (propertyId, page, count, q) {
            return $http
                .get('product/getAllPropertyValues', {
                    params: { propertyId: propertyId, page: page, count: count, q: q },
                })
                .then(function (response) {
                    return response.data;
                });
        };
    };

    productPropertiesService.$inject = ['$http'];

    ng.module('productProperties').service('productPropertiesService', productPropertiesService);
})(window.angular);
