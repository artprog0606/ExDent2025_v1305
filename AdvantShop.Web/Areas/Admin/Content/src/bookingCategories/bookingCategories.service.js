(function (ng) {
    'use strict';

    var bookingCategoriesService = function ($http) {
        var service = this;

        service.getCategories = function () {
            return $http.get('bookingCategory/getListCategories').then(function (result) {
                return result.data;
            });
        };

        service.getCategoriesRefAffiliate = function (affiliateId) {
            return $http.get('bookingCategory/getListCategoriesRefAffiliate', { params: { affiliateId: affiliateId } }).then(function (result) {
                return result.data;
            });
        };

        service.changeCategorySorting = function (categoryId, prevCategoryId, nextCategoryId) {
            return $http
                .post('bookingCategory/changeCategorySorting', {
                    categoryId: categoryId,
                    prevCategoryId: prevCategoryId,
                    nextCategoryId: nextCategoryId,
                })
                .then(function (result) {
                    return result.data;
                });
        };

        service.deleteCategory = function (categoryId) {
            return $http.post('bookingCategory/deleteCategory', { categoryId: categoryId }).then(function (response) {
                return response.data;
            });
        };
    };

    bookingCategoriesService.$inject = ['$http'];

    ng.module('bookingCategories').service('bookingCategoriesService', bookingCategoriesService);
})(window.angular);
