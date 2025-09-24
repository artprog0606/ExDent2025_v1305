/* @ngInject */
function CartPageCtrl($http, toaster, $translate, cartService, $scope) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.refreshCart();
        cartService.addCallback('remove', ctrl.refreshCart);
        cartService.addCallback('add', ctrl.refreshCart);
        cartService.addCallback('clear', ctrl.refreshCart);

        $scope.$on('$destroy', function () {
            cartService.removeCallback('remove', ctrl.refreshCart);
            cartService.removeCallback('add', ctrl.refreshCart);
            cartService.removeCallback('clear', ctrl.refreshCart);
        });
    };

    ctrl.getUrlShareCart = function () {
        $http.get('cart/getUrlShareCart').then(function (response) {
            if (response.data) {
                ctrl.urlShareCart = response.data;
            } else {
                toaster.pop('error', $translate.instant('Js.Cart.CouldNotCopyLink'));
            }
        });
    };

    ctrl.copyToClipboard = function () {
        let input = document.getElementById('urlShareCart');
        input.select();

        if (document.execCommand('copy')) {
            toaster.pop('success', '', $translate.instant('Js.Cart.LinkCopied'));
        } else {
            toaster.pop('error', '', $translate.instant('Js.Cart.CouldNotCopyLink'));
        }
    };

    ctrl.refreshCart = function () {
        cartService.getData().then(function (data) {
            ctrl.cartData = data;
        });
    };
}

export default CartPageCtrl;
