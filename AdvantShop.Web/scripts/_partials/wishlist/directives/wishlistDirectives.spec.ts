import { createTestApp } from 'angularjs-jest';
import compareModule from '../wishlist.module';
import { WishlistService as WishlistServiceMock } from '../__mocks__/wishlistServiceMock';
import type { IQService } from 'angular';
import { getOfferId } from '../__mocks__/wishlist';
import { spyOn } from 'jest-mock';

describe('wishlistControl', () => {
    const getTestApp = () =>
        createTestApp({
            modules: [compareModule],
            mocks: { wishlistService: ($q: IQService) => WishlistServiceMock($q) },
            access: [],
        });
    it('should render compareControl', async () => {
        const app = getTestApp();
        const offerId = getOfferId();
        const element = app.render(`<div wishlist-control="${offerId}"></div>`, app.$scope);
        const removeWishlistScopeMock = spyOn(app.wishlistService, 'removeWishlistScope');
        app.wishlistService.removeWishlistScopeMock = removeWishlistScopeMock;
        element.scope().$destroy();
        expect(removeWishlistScopeMock).toHaveBeenCalled();
    });
});
