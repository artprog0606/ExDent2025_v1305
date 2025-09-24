import { PubSub } from '../../../_common/PubSub/PubSub.js';

(function (ng) {
    'use strict';

    var CartFullCtrl = function ($rootScope, cartService, moduleService, SweetAlert, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            cartService.getData().then(function (data) {
                ctrl.cartData = data;
            });

            ctrl.cssClassForPopoverHelpTrigger = 'cart-help-trigger-popover';
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

        ctrl.remove = function (event, shoppingCartItemId) {
            event?.preventDefault();
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

        ctrl.clear = function (event) {
            event.preventDefault();
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

        ctrl.refresh = function () {
            return cartService.getData(false).then(function (data) {
                moduleService.update('fullcartmessage');
                ctrl.cartData = data;
            });
        };
    };

    angular.module('cart').controller('CartFullCtrl', CartFullCtrl);

    CartFullCtrl.$inject = ['$rootScope', 'cartService', 'moduleService', 'SweetAlert', '$translate'];
})(window.angular);
