(function (ng) {
    'use strict';

    var productService = function ($http) {
        var service = this;

        service.getColors = function () {
            return $http.get('product/getcolors').then(function (response) {
                return response.data;
            });
        };

        service.getSizes = function (categoryId, productId) {
            return $http.get('product/getsizes', { params: { categoryId: categoryId, productId: productId } }).then(function (response) {
                return response.data;
            });
        };

        service.getOffer = function (offerId) {
            return $http.get('product/getoffer', { params: { offerId: offerId } }).then(function (response) {
                return response.data;
            });
        };

        service.getAvailableArtNo = function (productId) {
            return $http.get('product/getavailableartno', { params: { productId: productId } }).then(function (response) {
                return response.data;
            });
        };

        service.getProductInfoForOffer = function (productId) {
            return $http.get('product/getProductInfoForOffer', { params: { productId: productId } }).then(function (response) {
                return response.data;
            });
        };

        service.addOffer = function (params) {
            return $http.post('product/addoffer', params).then(function (response) {
                return response.data;
            });
        };

        service.updateOffer = function (params) {
            return $http.post('product/updateoffer', params).then(function (response) {
                return response.data;
            });
        };

        service.getPriceRulesInfo = function () {
            return $http.get('pricerules/getInfo').then(function (response) {
                return response.data;
            });
        };

        service.getProductLastModified = function (productId) {
            return $http.get('product/getProductLastModified', { params: { productId: productId } }).then(function (response) {
                return response.data;
            });
        };

        service.getOfferStocks = function (offerId) {
            return $http.get('product/getOfferStocks', { params: { offerId: offerId } }).then(function (response) {
                return response.data;
            });
        };

        service.saveOfferStocks = function (params) {
            return $http.post('product/saveOfferStocks', params).then(function (response) {
                return response.data;
            });
        };
    };

    productService.$inject = ['$http'];

    ng.module('product').service('productService', productService);
})(window.angular);
