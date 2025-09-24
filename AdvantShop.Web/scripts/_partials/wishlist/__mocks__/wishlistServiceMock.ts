import type { IWishlistService } from '../services/wishlistService';
import type { WishlistCountResponseType, WishlistCountType, OfferIdType } from '../wishlist.module';
import { getWishlistData, getWishlistResponseData, getWishlistResponseStatus } from './wishlist';
import type { IQService, IPromise } from 'angular';
import { IWishlistControlCtrl } from '../controllers/WishlistControlController';

export const WishlistService = ($q: IQService): IWishlistService => ({
    add(offerId: OfferIdType, state: boolean) {
        return $q<WishlistCountResponseType>((resolve) => resolve(getWishlistResponseData()));
    },
    remove(offerId: OfferIdType, state: boolean): IPromise<WishlistCountResponseType> {
        return $q<WishlistCountResponseType>((resolve) => resolve(getWishlistResponseData()));
    },
    removeWishlistScope: (id: OfferIdType, ctrl: IWishlistControlCtrl) => {},
    getCountObj(): WishlistCountType {
        return getWishlistData();
    },
    getStatus(offerId: OfferIdType): IPromise<boolean> {
        return $q<boolean>((resolve) => resolve(getWishlistResponseStatus()));
    },
    addWishlistScope(id: OfferIdType, ctrl: IWishlistControlCtrl) {},
    changeWishlistControlState(id: OfferIdType, state: boolean) {},
});
