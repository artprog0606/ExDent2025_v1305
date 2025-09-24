import catProductRecommendationsTemplate from './catProductRecommendations.html';
(function (ng) {
    'use strict';

    var CatProductRecommendationsCtrl = function ($http, toaster, $translate) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.getCategories();
            ctrl.getProperties();
            ctrl.getPropertiesWithValues();
        };

        // get categories
        ctrl.getCategories = function () {
            $http
                .get('category/getRecomCategories', {
                    params: {
                        categoryId: ctrl.categoryId,
                        type: ctrl.type,
                    },
                })
                .then(function (response) {
                    ctrl.categories = response.data;
                });
        };
        ctrl.deleteCategory = function (type, relCategoryId) {
            $http
                .post('category/deleteRecomCategory', {
                    categoryId: ctrl.categoryId,
                    relcategoryId: relCategoryId,
                    type: type,
                })
                .then(function (response) {
                    ctrl.getCategories();
                    toaster.pop('success', $translate.instant('Admin.Js.Certificates.ChangesSaved'));
                });
        };
        ctrl.addCategories = function (result) {
            $http
                .post('category/addRecomCategories', {
                    categoryId: ctrl.categoryId,
                    relcategories: result.categories,
                    type: ctrl.type,
                })
                .then(function (response) {
                    ctrl.getCategories();
                    toaster.pop('success', $translate.instant('Admin.Js.Certificates.ChangesSaved'));
                });
        };

        // get properties
        ctrl.getProperties = function () {
            $http
                .get('category/getRecomProperties', {
                    params: {
                        categoryId: ctrl.categoryId,
                        type: ctrl.type,
                    },
                })
                .then(function (response) {
                    ctrl.properties = response.data;
                });
        };
        ctrl.deleteProperty = function (type, propertyId) {
            $http
                .post('category/deleteRecomProperty', {
                    categoryId: ctrl.categoryId,
                    propertyId: propertyId,
                    type: type,
                })
                .then(function (response) {
                    ctrl.getProperties();
                    toaster.pop('success', $translate.instant('Admin.Js.Certificates.ChangesSaved'));
                });
        };
        ctrl.addProperty = function (propertyId, isSame) {
            $http
                .post('category/addRecomProperty', {
                    categoryId: ctrl.categoryId,
                    propertyId: propertyId,
                    type: ctrl.type,
                    isSame: isSame,
                })
                .then(function (response) {
                    ctrl.getProperties();
                    toaster.pop('success', $translate.instant('Admin.Js.Certificates.ChangesSaved'));
                });
        };

        // get properties with values
        ctrl.getPropertiesWithValues = function () {
            $http
                .get('category/getRecomPropertiesWithValues', {
                    params: {
                        categoryId: ctrl.categoryId,
                        type: ctrl.type,
                    },
                })
                .then(function (response) {
                    ctrl.propertiesWithValues = response.data;
                });
        };
        ctrl.deletePropertyWithValue = function (type, propertyValueId) {
            $http
                .post('category/deleteRecomPropertyWithValue', {
                    categoryId: ctrl.categoryId,
                    propertyValueId: propertyValueId,
                    type: type,
                })
                .then(function (response) {
                    ctrl.getPropertiesWithValues();
                    toaster.pop('success', $translate.instant('Admin.Js.Certificates.ChangesSaved'));
                });
        };
        ctrl.addPropertyWithValue = function (propertyValueId) {
            $http
                .post('category/addRecomPropertyWithValue', {
                    categoryId: ctrl.categoryId,
                    propertyValueId: propertyValueId,
                    type: ctrl.type,
                })
                .then(function (response) {
                    ctrl.getPropertiesWithValues();
                    toaster.pop('success', $translate.instant('Admin.Js.Certificates.ChangesSaved'));
                });
        };
        ctrl.addPropertyModal = function (result) {
            if (result == null) return;
            if (result.type == '0' && result.propertyValueId != null) {
                ctrl.addPropertyWithValue(result.propertyValueId);
            } else if (result.type == '1') {
                ctrl.addProperty(result.propertyId, true);
            } else if (result.type == '2') {
                ctrl.addProperty(result.propertyId, false);
            }
        };
    };
    CatProductRecommendationsCtrl.$inject = ['$http', 'toaster', '$translate'];
    ng.module('catProductRecommendations', [])
        .controller('CatProductRecommendationsCtrl', CatProductRecommendationsCtrl)
        .component('catProductRecommendations', {
            templateUrl: catProductRecommendationsTemplate,
            controller: CatProductRecommendationsCtrl,
            controllerAs: 'ctrl',
            bindings: {
                type: '@',
                categoryId: '@',
            },
        });
})(window.angular);
