/*@ngInject*/
function HeaderCtrl(sidebarsContainerService) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.searchHeaderActive = false;
    };

    ctrl.toggleMenu = function ($event) {
        sidebarsContainerService.toggle({ contentId: 'sidebarMenu', isStatic: true });
    };

    ctrl.searchHide = function () {
        ctrl.searchHeaderActive = false;
    };

    ctrl.searchShow = function () {
        ctrl.searchHeaderActive = true;
        sidebarsContainerService.close({ contentId: 'sidebarMenu', isStatic: true });
    };

    ctrl.toggleCart = function ($event) {
        if ($event != null) {
            $event.preventDefault();
        }

        sidebarsContainerService.toggle({
            contentId: 'sidebarCart',
            title: 'Корзина',
            template:
                '<div data-cart-mini=""><div class="cart-mini-list-mobile" data-is-show-remove="true" data-cart-mini-list data-is-mobile="true" data-cart-data="cartMini.cartData"></div></div>',
            hideFooter: true,
        });
    };

    ctrl.togglePhonesList = (templateUrl) => {
        sidebarsContainerService.toggle({
            contentId: 'phonesList',
            templateUrl: templateUrl,
            hideFooter: true,
            hideHeader: true,
        });
    };

    ctrl.searchFocus = function () {
        sidebarsContainerService.close({ contentId: 'sidebarMenu', isStatic: true });
    };
}

export default HeaderCtrl;
