import productGiftsTemplate from './productGifts.html';
(function (ng) {
    'use strict';

    var ProductGiftsCtrl = function ($http, SweetAlert, toaster, $translate) {
        var ctrl = this;
        ctrl.$onInit = function () {
            getGifts();
        };
        ctrl.addGifts = function (result) {
            if (result == null || result.ids == null || result.ids.length === 0) return;
            $http
                .post('product/addGifts', {
                    productId: ctrl.productId,
                    offerIds: result.ids,
                    productCount: 1,
                })
                .then(function (response) {
                    getGifts();
                    if (response.data.result) {
                        toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                    } else {
                        response.data.errors.forEach(function (err) {
                            toaster.error('', err);
                        });
                    }
                });
        };
        ctrl.deleteGift = function (giftOfferId, offerId) {
            SweetAlert.confirm($translate.instant('Admin.Js.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http
                        .post('product/deleteGift', {
                            productId: ctrl.productId,
                            giftOfferId: giftOfferId,
                            offerId: offerId,
                        })
                        .then(function (response) {
                            getGifts();
                            toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                        });
                }
            });
        };
        function getGifts() {
            $http
                .get('product/getGifts', {
                    params: {
                        productId: ctrl.productId,
                    },
                })
                .then(function (response) {
                    ctrl.products = response.data;
                });
        }
        ctrl.updateGift = function (giftOfferId, offerId, productCount) {
            $http
                .post('product/updateGift', {
                    productId: ctrl.productId,
                    giftOfferId: giftOfferId,
                    offerId: offerId,
                    productCount: productCount,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result == true) {
                        getGifts();
                        toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                    } else {
                        toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                    }
                });
        };
        ctrl.updateGiftOffer = function (giftOfferId, offerId, prevOfferId, productCount) {
            $http
                .post('product/updateGiftOffer', {
                    productId: ctrl.productId,
                    giftOfferId: giftOfferId,
                    offerId: offerId,
                    prevOfferId: prevOfferId,
                    productCount: productCount,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result == true) {
                        getGifts();
                        toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                    } else {
                        toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                    }
                });
        };
    };
    ProductGiftsCtrl.$inject = ['$http', 'SweetAlert', 'toaster', '$translate'];
    ng.module('productGifts', ['offersSelectvizr'])
        .controller('ProductGiftsCtrl', ProductGiftsCtrl)
        .component('productGifts', {
            templateUrl: productGiftsTemplate,
            controller: ProductGiftsCtrl,
            bindings: {
                productId: '@',
            },
        });
})(window.angular);
