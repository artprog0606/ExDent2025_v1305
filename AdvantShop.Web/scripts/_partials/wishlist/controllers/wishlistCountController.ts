import { IWishlistService } from '../services/wishlistService';
import type { WishlistCountType } from '../wishlist.module';
import type { IController } from 'angular';

export default class WishlistCountCtrl implements IController {
    countObj: WishlistCountType | null = null;
    /*@ngInject*/
    constructor(private readonly wishlistService: IWishlistService) {
        this.countObj = this.wishlistService.getCountObj();
    }
}
