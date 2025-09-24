(function (ng) {
    'use strict';

    var CartMiniCtrl = function (cartService, cartConfig) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.cartData = {};

            cartService.getData().then(function (data) {
                ctrl.cartData = data;
            });
        };

        ctrl.addMinicartList = function (miniCartList) {
            ctrl.list = miniCartList;
        };

        ctrl.triggerClick = function (event) {
            if (event != null) {
                event.preventDefault();
            }

            if (ctrl.list != null) {
                ctrl.list.cartToggle(true);
            }

            cartService.processCallback(cartConfig.callbackNames.open);
        };
    };

    angular.module('cart').controller('CartMiniCtrl', CartMiniCtrl);

    CartMiniCtrl.$inject = ['cartService', 'cartConfig'];
})(angular);
