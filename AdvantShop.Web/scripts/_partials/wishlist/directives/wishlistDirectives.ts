import type { IWishlistService } from '../services/wishlistService';
import type { IDirective, IParseService } from 'angular';

angular.module('wishlist').directive('wishlistControl', [
    'wishlistService',
    '$parse',
    (wishlistService: IWishlistService, $parse: IParseService): IDirective => ({
        restrict: 'A',
        scope: true,
        controller: 'WishlistControlCtrl',
        controllerAs: 'wishlistControl',
        bindToController: true,
        link(scope, element, attrs, ctrl) {
            if (attrs.wishlistControl !== null && ctrl) {
                const offerId: number = $parse(attrs.wishlistControl)(scope);
                ctrl.offerId = offerId;
                scope.$on('$destroy', () => {
                    wishlistService.removeWishlistScope(offerId, ctrl);
                });
                if (ctrl.offerId !== null) {
                    wishlistService.addWishlistScope(offerId, ctrl);
                }
            }
        },
    }),
]);

angular.module('wishlist').directive(
    'wishlistCount',
    (): IDirective => ({
        restrict: 'A',
        scope: true,
        controller: 'WishlistCountCtrl',
        controllerAs: 'wishlistCount',
        bindToController: true,
        link(scope, element, attrs, ctrl) {
            if (ctrl) {
                ctrl.countObj.count = parseInt(attrs.startCount, 10);
            }
        },
    }),
);

angular.module('wishlist').directive('wishlistWrapper', [
    'wishlistService',
    'domService',
    (wishlistService: IWishlistService, domService): IDirective => ({
        restrict: 'A',
        scope: true,
        link(scope, element, attrs) {
            const items = element[0].querySelectorAll<HTMLElement>(attrs.wishlistWrapper);

            if (items.length > 0) {
                const dirRemove = document.createElement('a');
                // eslint-disable-next-line no-script-url
                dirRemove.href = 'javascript:void(0);';
                dirRemove.setAttribute('role', 'link');
                dirRemove.className = 'js-wishlist-remove wishlist-remove icon-cancel-circled-before link-text-decoration-none cs-l-5';

                // eslint-disable-next-line no-plusplus
                for (let i = items.length - 1; i >= 0; i--) {
                    items[i].appendChild(dirRemove.cloneNode());

                    items[i].addEventListener('click', (event: Event) => {
                        const item = items[i],
                            target = event.target as Element;
                        if (target.classList.contains('js-wishlist-remove')) {
                            const offerId = Number(item.getAttribute('data-offer-id'));
                            if (!Number.isNaN(offerId)) {
                                wishlistService.remove(Number(offerId)).then(() => {
                                    const blockForDelete = domService.closest(item, '.js-products-view-block');
                                    blockForDelete.parentNode.removeChild(blockForDelete);
                                });
                            }
                        }
                    });
                }
            }
        },
    }),
]);
