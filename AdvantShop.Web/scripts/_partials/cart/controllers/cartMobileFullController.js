import { PubSub } from '../../../_common/PubSub/PubSub.js';

(function (ng) {
    'use strict';

    var CartMobileFullCtrl = function ($rootScope, cartService, moduleService, SweetAlert, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            cartService.getData().then(function (data) {
                ctrl.cartData = data;
            });
        };

        ctrl.updateAmount = function (value, itemId) {
            var item = {
                Key: itemId,
                Value: value,
            };

            cartService.updateAmount([item]).then(function () {
                moduleService.update('fullcartmessage');
                PubSub.publish('cart.updateAmount');
            });
        };

        ctrl.remove = function (shoppingCartItemId) {
            SweetAlert.confirm($translate.instant('Js.Cart.Removing.AreYouSureDelete'), {
                title: $translate.instant('Js.Cart.Removing'),
            }).then(function (result) {
                if (result.isConfirmed) {
                    cartService.removeItem(shoppingCartItemId).then(function (result) {
                        moduleService.update('fullcartmessage');
                        PubSub.publish('cart.remove', result.offerId);
                    });
                }
            });
        };

        ctrl.clear = function () {
            SweetAlert.confirm($translate.instant('Js.Cart.Clear.AreYouSureClear'), {
                title: $translate.instant('Js.Cart.Warning.Clear.Title'),
            }).then(function (result) {
                if (result.isConfirmed) {
                    cartService.clear().then(function () {
                        moduleService.update('fullcartmessage');
                        PubSub.publish('cart.clear');
                    });
                }
            });
        };

        ctrl.getOptions = function (min, step, amount, max) {
            var tempArr = [];

            var start = Math.ceil(min / step) * step;

            if (amount > max) {
                //if you need limit options number by availability
                for (let i = start; i <= amount; i = +(i + step).toFixed(4)) {
                    tempArr.push(i);
                }
            } else {
                for (let i = start; i <= max; i = +(i + step).toFixed(4)) {
                    tempArr.push(i);
                }
            }

            return tempArr;
        };

        ctrl.refresh = function () {
            return cartService.getData(false).then(function (data) {
                ctrl.cartData = data;
            });
        };
    };

    angular.module('cart').controller('CartMobileFullCtrl', CartMobileFullCtrl);

    CartMobileFullCtrl.$inject = ['$rootScope', 'cartService', 'moduleService', 'SweetAlert', '$translate'];
})(window.angular);
