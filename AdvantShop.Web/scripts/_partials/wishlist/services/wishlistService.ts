import { PubSub } from '../../../_common/PubSub/PubSub.js';
import type { IHttpService, IPromise } from 'angular';
import type { WishlistCountType, WishlistCountResponseType } from '../wishlist.module';
import type { OfferIdType } from '../wishlist.module';
import type { IWishlistControlCtrl } from '../controllers/WishlistControlController';

export interface IWishlistService {
    add(offerId: OfferIdType, state: boolean): IPromise<WishlistCountResponseType>;
    remove(offerId: OfferIdType, state?: boolean): IPromise<WishlistCountResponseType>;
    getCountObj(): WishlistCountType;
    addWishlistScope(id: OfferIdType, ctrl: any): void;
    removeWishlistScope(id: OfferIdType, ctrl: any): void;
    changeWishlistControlState(id: OfferIdType, state: boolean): void;
    getCountObj(): WishlistCountType;
    getStatus(offerId: OfferIdType): IPromise<boolean>;
}

export interface IWishlistServiceStatic {
    new ($http: IHttpService): IWishlistService;
    wishlistsScopeList: Map<OfferIdType, Set<IWishlistControlCtrl>>;
}

type EndpointsNameType = 'ADD' | 'REMOVE' | 'STATUS';

export const wishlistEndpoints: Record<EndpointsNameType, string> = {
    ADD: 'wishlist/wishlistadd',
    REMOVE: 'wishlist/wishlistremove',
    STATUS: '/wishlist/getstatus',
};

export default class WishlistService implements IWishlistService {
    static wishlistsScopeList: Map<OfferIdType, Set<IWishlistControlCtrl>> = new Map();
    #countObj = <WishlistCountType>{};

    /* @ngInject */
    constructor(private readonly $http: IHttpService) {}

    add(offerId: OfferIdType, state: boolean): IPromise<WishlistCountResponseType> {
        return this.$http
            .post<WishlistCountResponseType>(wishlistEndpoints.ADD, { offerId: offerId, rnd: Math.random() })
            .then((res) => {
                const { data } = res;
                this.#countObj.count = data.Count;
                PubSub.publish(`add_to_wishlist`, offerId);
                this.changeWishlistControlState(offerId, state);
                return data;
            })
            .catch((e) => {
                throw new Error(e.data || e);
            });
    }

    remove(offerId: OfferIdType, state: boolean = false): IPromise<WishlistCountResponseType> {
        return this.$http
            .post<WishlistCountResponseType>(wishlistEndpoints.REMOVE, { offerId: offerId, rnd: Math.random() })
            .then((res) => {
                const { data } = res;
                this.#countObj.count = data.Count;
                this.changeWishlistControlState(offerId, state);
                PubSub.publish(`remove_to_wishlist`, offerId);
                return data;
            })
            .catch((e) => {
                throw new Error(e.data || e);
            });
    }

    getCountObj() {
        return this.#countObj;
    }

    getStatus(offerId: OfferIdType): IPromise<boolean> {
        return this.$http
            .get<boolean>(wishlistEndpoints.STATUS, { params: { offerId: offerId, rnd: Math.random() } })
            .then((res) => {
                return res.data;
            })
            .catch((e) => {
                throw new Error(e.data || e);
            });
    }

    addWishlistScope(id: OfferIdType, ctrl: IWishlistControlCtrl): void {
        const wishListScopes = WishlistService.wishlistsScopeList.get(id) || new Set();
        wishListScopes.add(ctrl);
        WishlistService.wishlistsScopeList.set(id, wishListScopes);
    }

    removeWishlistScope(id: OfferIdType, ctrl: IWishlistControlCtrl): void {
        const wishlistControls: Set<IWishlistControlCtrl> | undefined = WishlistService.wishlistsScopeList.get(id);
        if (wishlistControls != null) {
            for (let scope of wishlistControls) {
                if (ctrl === scope) {
                    wishlistControls.delete(ctrl);
                }
            }
        }
    }

    changeWishlistControlState(id: OfferIdType, state: boolean): void {
        const wishlistControls = WishlistService.wishlistsScopeList.get(id);
        if (wishlistControls != null) {
            for (let scope of wishlistControls) {
                scope.isAdded = state;
            }
        }
    }
}

WishlistService.$inject = ['$http'];
