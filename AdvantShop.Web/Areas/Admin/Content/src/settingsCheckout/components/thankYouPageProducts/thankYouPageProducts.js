import thankYouPageProductsTemplate from './thankYouPageProducts.html';
(function (ng) {
    'use strict';

    var ThankYouPageProductsCtrl = function ($filter, $http, SweetAlert, toaster, $translate) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.getProducts();
        };
        ctrl.addProductsModal = function (result) {
            if (result == null || result.ids == null || result.ids.length === 0) return;
            $http
                .post('settingsCheckout/addThankYouPageProducts', {
                    ids: result.ids,
                })
                .then(function (response) {
                    ctrl.getProducts();
                    toaster.success($translate.instant('Admin.Js.Common.ChangesSaved'));
                });
        };
        ctrl.deleteProduct = function (productId) {
            SweetAlert.confirm($translate.instant('Admin.Js.Common.Deleting'), {
                title: $translate.instant('Admin.Js.Common.AreYouSureDelete'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http
                        .post('settingsCheckout/deleteThankYouPageProduct', {
                            productId: productId,
                        })
                        .then(function (response) {
                            ctrl.getProducts();
                            toaster.success($translate.instant('Admin.Js.Common.ChangesSaved'));
                        });
                }
            });
        };
        ctrl.getProducts = function () {
            $http.get('settingsCheckout/getThankYouPageProducts').then(function (response) {
                ctrl.products = response.data.products;
                ctrl.productIds = response.data.productIds;
            });
        };
    };
    ThankYouPageProductsCtrl.$inject = ['$filter', '$http', 'SweetAlert', 'toaster', '$translate'];
    ng.module('thankYouPageProducts', []).controller('ThankYouPageProductsCtrl', ThankYouPageProductsCtrl).component('thankYouPageProducts', {
        templateUrl: thankYouPageProductsTemplate,
        controller: ThankYouPageProductsCtrl,
    });
})(window.angular);
