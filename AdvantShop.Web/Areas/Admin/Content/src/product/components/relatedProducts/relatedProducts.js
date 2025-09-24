import relatedProductsTemplate from './relatedProducts.html';
(function (ng) {
    'use strict';

    var RelatedProductsCtrl = function ($http, SweetAlert, toaster, $translate) {
        var ctrl = this;
        ctrl.$onInit = function () {
            getRelatedProducts();
        };
        ctrl.addProductsModal = function (result) {
            if (result == null || result.ids == null || result.ids.length === 0) return;
            $http
                .post('product/addRelatedProduct', {
                    productId: ctrl.productId,
                    type: ctrl.type,
                    ids: result.ids,
                })
                .then(function (response) {
                    getRelatedProducts();
                    toaster.success('', $translate.instant('Admin.Js.Product.ChangesSaved'));
                });
        };
        ctrl.deleteRelatedProduct = function (relatedProductId) {
            SweetAlert.confirm($translate.instant('Admin.Js.Product.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Product.Deleting'),
            }).then(function (result) {
                if (result === true || result.value === true) {
                    $http
                        .post('product/deleteRelatedProduct', {
                            productId: ctrl.productId,
                            type: ctrl.type,
                            relatedProductId: relatedProductId,
                        })
                        .then(function (response) {
                            getRelatedProducts();
                            toaster.success('', $translate.instant('Admin.Js.Product.ChangesSaved'));
                        });
                }
            });
        };
        function getRelatedProducts() {
            $http
                .get('product/getRelatedProducts', {
                    params: {
                        productId: ctrl.productId,
                        type: ctrl.type,
                    },
                })
                .then(function (response) {
                    ctrl.products = response.data;
                });
        }
        ctrl.sortableOptions = {
            orderChanged: function (event) {
                var id = event.source.itemScope.item.Id,
                    prev = ctrl.products[event.dest.index - 1],
                    next = ctrl.products[event.dest.index + 1];
                $http
                    .post('product/changeRelatedProductsSorting', {
                        id: id,
                        prevId: prev != null ? prev.Id : null,
                        nextId: next != null ? next.Id : null,
                    })
                    .then(function (response) {
                        toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                    });
            },
        };
    };
    RelatedProductsCtrl.$inject = ['$http', 'SweetAlert', 'toaster', '$translate'];
    ng.module('relatedProducts', [])
        .controller('RelatedProductsCtrl', RelatedProductsCtrl)
        .component('relatedProducts', {
            templateUrl: relatedProductsTemplate,
            controller: RelatedProductsCtrl,
            bindings: {
                productId: '=',
                type: '@',
            },
        });
})(window.angular);
