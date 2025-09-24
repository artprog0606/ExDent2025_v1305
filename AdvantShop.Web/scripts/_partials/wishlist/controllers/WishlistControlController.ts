import type { IWishlistService } from '../services/wishlistService';
import type { OfferIdType, WishlistCountResponseType } from '../wishlist.module';
import type { IController } from 'angular';

export interface IWishlistControlCtrl extends IController {
    dirty: boolean;
    isAdded: boolean | null;
    add(offerId: OfferIdType, state: boolean): Promise<WishlistCountResponseType>;
    remove(offerId: OfferIdType, state: boolean): Promise<WishlistCountResponseType>;
    change(offerId: OfferIdType, state: boolean): Promise<void>;
    checkStatus(offerId: OfferIdType): Promise<void>;
}

export default class WishlistControlCtrl implements IWishlistControlCtrl {
    /* @ngInject */
    constructor(readonly wishlistService: IWishlistService) {}

    dirty: boolean = false;
    isAdded: boolean | null = null;

    add = async (offerId: OfferIdType, state: boolean): Promise<WishlistCountResponseType> => {
        return this.wishlistService.add(offerId, state);
    };

    remove = async (offerId: OfferIdType, state: boolean): Promise<WishlistCountResponseType> => {
        return this.wishlistService.remove(offerId, state);
    };

    change = async (offerId: OfferIdType, state: boolean) => {
        this.dirty = true;
        this.isAdded ? await this.add(offerId, state) : await this.remove(offerId, state);
    };

    checkStatus = async (offerId: OfferIdType): Promise<void> => {
        const isAdded = await this.wishlistService.getStatus(offerId);
        if (isAdded != null) {
            this.isAdded = isAdded;
        }
    };
}
