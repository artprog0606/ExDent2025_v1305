import lpCartPopupTemplate from './lp-cart-popup.html';
(function (ng) {
    'use strict';

    ng.module('lpCart', [])
        .controller('LpCartPopupCtrl', [
            '$compile',
            '$element',
            '$scope',
            '$timeout',
            'cartService',
            'cartConfig',
            'bookingCartService',
            'bookingCartConfig',
            'modalService',
            function ($compile, $element, $scope, $timeout, cartService, cartConfig, bookingCartService, bookingCartConfig, modalService) {
                var ctrl = this;
                ctrl.$onInit = function () {
                    cartService.addCallback(cartConfig.callbackNames.add, ctrl.add);
                    bookingCartService.addCallback(bookingCartConfig.callbackNames.add, ctrl.add);
                };
                ctrl.$postLink = function () {
                    if (ctrl.type === 'goods') {
                        cartService
                            .getData(false, {
                                lpId: ctrl.lpId,
                            })
                            .then(function (data) {
                                ctrl.cartData = data;
                            });
                    }
                    if (ctrl.type === 'booking') {
                        bookingCartService
                            .getData(false, {
                                lpId: ctrl.lpId,
                            })
                            .then(function (data) {
                                ctrl.bookingCartData = data;
                            });
                    }
                };
                ctrl.updateAmount = function (value, itemId) {
                    var item = {
                        Key: itemId,
                        Value: value,
                    };
                    cartService
                        .updateAmount([item], {
                            lpId: ctrl.lpId,
                        })
                        .then(function () {});
                };
                ctrl.removeCartItem = function (shoppingCartItemId) {
                    cartService
                        .removeItem(shoppingCartItemId, {
                            lpId: ctrl.lpId,
                        })
                        .then(function (result) {});
                };
                ctrl.removeBookingItem = function (shoppingCartItemId) {
                    bookingCartService
                        .removeItem(shoppingCartItemId, {
                            lpId: ctrl.lpId,
                        })
                        .then(function (result) {});
                };
                ctrl.refreshCart = function () {
                    return cartService
                        .getData(false, {
                            lpId: ctrl.lpId,
                        })
                        .then(function (data) {
                            ctrl.cartData = data;
                        });
                };
                ctrl.refreshBooking = function () {
                    return bookingCartService
                        .getData(false, {
                            lpId: ctrl.lpId,
                        })
                        .then(function (data) {
                            ctrl.bookingCartData = data;
                        });
                };
                ctrl.add = function (result, params) {
                    const forceHiddenPopup = params[1];
                    if (forceHiddenPopup) return;

                    modalService.open('modalLpCartPopup');
                };
            },
        ])
        .component('lpCartPopup', {
            controller: 'LpCartPopupCtrl',
            bindings: {
                lpId: '<?',
                type: '@',
                hideShipping: '<?',
            },
            templateUrl: lpCartPopupTemplate,
        });
})(window.angular);
