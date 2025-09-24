(function (ng) {
    'use strict';

    var CartConfirmCtrl = function (cartService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.cartData = {};

            cartService.getData().then(function (data) {
                ctrl.cartData = data;
            });
        };
    };

    angular.module('cart').controller('CartConfirmCtrl', CartConfirmCtrl);

    CartConfirmCtrl.$inject = ['cartService'];
})(angular);
