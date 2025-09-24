import angular from 'angular';
import 'angular-mocks';
import '../../../_common/PubSub/__mocks__/PubSub.js';
import '../wishlist.module';
import { mockRandom, resetMockRandom } from 'jest-mock-random';
import WishlistService, { wishlistEndpoints, type IWishlistService, type IWishlistServiceStatic } from './wishlistService';
import {
    getWishlistResponseData,
    getWishlistResponseErrorText,
    getWishlistResponseStatus,
    getWishlistScope,
    getOfferId,
} from '../__mocks__/wishlist';
import type { WishlistCountResponseType } from '../wishlist.module';
import { IWishlistControlCtrl } from '../controllers/WishlistControlController';
import { PubSub } from '../../../_common/PubSub/PubSub.js';

describe('WishlistService', () => {
    let $httpBackend, wishlistService: IWishlistService & IWishlistServiceStatic, offerId: number;

    beforeEach(() => {
        offerId = getOfferId();
        mockRandom(0.2);
        angular.mock.module('wishlist');
        angular.mock.inject(($injector) => {
            $httpBackend = $injector.get('$httpBackend');
            wishlistService = $injector.get('wishlistService');
        });
    });

    afterEach(() => {
        resetMockRandom();
        PubSub.clear();
        WishlistService.wishlistsScopeList.clear();
    });

    describe('add', () => {
        it('should correctly call', function () {
            const wishlistData: WishlistCountResponseType = getWishlistResponseData();
            let result = {};

            $httpBackend.expectPOST(wishlistEndpoints.ADD, { rnd: Math.random(), offerId }).respond(200, wishlistData);
            wishlistService.add(offerId, true).then((res) => {
                result = res;
            });
            $httpBackend.flush();
            expect(result).toEqual(wishlistData);
            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });

        it('should correctly handle exception', function () {
            const error = getWishlistResponseErrorText();
            let result: Error = new Error();
            $httpBackend.expectPOST(wishlistEndpoints.ADD, { rnd: Math.random(), offerId }).respond(500, error);
            wishlistService.add(offerId, true).catch((e: Error) => {
                result = e;
            });
            $httpBackend.flush();
            expect(result.message).toEqual(error);
            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });
    });

    describe('remove', () => {
        it('should correctly call', function () {
            const wishlistData: WishlistCountResponseType = getWishlistResponseData();
            let result = {};

            $httpBackend.expectPOST(wishlistEndpoints.REMOVE, { rnd: Math.random(), offerId }).respond(200, wishlistData);
            wishlistService.remove(offerId, false).then((res) => {
                result = res;
            });
            $httpBackend.flush();
            expect(result).toEqual(wishlistData);
            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });

        it('should correctly handle exception', function () {
            const error = getWishlistResponseErrorText();
            let result: Error = new Error();
            $httpBackend.expectPOST(wishlistEndpoints.REMOVE, { rnd: Math.random(), offerId }).respond(500, error);
            wishlistService.remove(offerId, false).catch((e: Error) => {
                result = e;
            });
            $httpBackend.flush();
            expect(result.message).toEqual(error);
            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });
    });

    describe('getCountObj', () => {
        it('should correctly return data', function () {
            let wishlistData: WishlistCountResponseType = getWishlistResponseData();

            $httpBackend.expectPOST(wishlistEndpoints.ADD, { rnd: Math.random(), offerId }).respond(200, wishlistData);
            wishlistService.add(offerId, true);
            $httpBackend.flush();
            expect(wishlistService.getCountObj().count).toBe(wishlistData.Count);

            wishlistData = getWishlistResponseData();
            $httpBackend.expectPOST(wishlistEndpoints.REMOVE, { rnd: Math.random(), offerId }).respond(200, wishlistData);
            wishlistService.remove(offerId, false);
            $httpBackend.flush();
            expect(wishlistService.getCountObj().count).toEqual(wishlistData.Count);

            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });
    });

    describe('getStatus', () => {
        it('should correctly call', () => {
            const wishlistResponseData: boolean = getWishlistResponseStatus();
            let result;

            $httpBackend.expectGET(`${wishlistEndpoints.STATUS}?offerId=${offerId}&rnd=${Math.random()}`).respond(200, wishlistResponseData);
            wishlistService.getStatus(offerId).then((res) => {
                result = res;
            });
            $httpBackend.flush();
            expect(result).toEqual(wishlistResponseData);
            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });

        it('should correctly handle exception', () => {
            const wishlistErrorData: string = getWishlistResponseErrorText();
            let result;

            $httpBackend.expectGET(`${wishlistEndpoints.STATUS}?offerId=${offerId}&rnd=${Math.random()}`).respond(500, wishlistErrorData);
            wishlistService.getStatus(offerId).catch((e) => {
                result = e.message;
            });
            $httpBackend.flush();
            expect(result).toEqual(wishlistErrorData);
            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });
    });

    describe('addWishlistScope', () => {
        it('should correctly call', () => {
            const wishlistScope: IWishlistControlCtrl = getWishlistScope();
            wishlistService.addWishlistScope(offerId, wishlistScope);
            const wishlistScopeList = WishlistService.wishlistsScopeList.get(offerId);
            expect(wishlistScopeList?.has(wishlistScope)).toBe(true);
        });
    });

    describe('removeWishlistScope', () => {
        it('should correctly call', () => {
            const wishlistScope: IWishlistControlCtrl = getWishlistScope();
            wishlistService.addWishlistScope(offerId, wishlistScope);
            const wishlistScopeList = WishlistService.wishlistsScopeList.get(offerId);
            expect(wishlistScopeList?.has(wishlistScope)).toBe(true);
            wishlistService.removeWishlistScope(offerId, wishlistScope);
            expect(wishlistScopeList?.has(wishlistScope)).toBe(false);
        });
    });

    describe('changeWishlistControlState', () => {
        it('should correctly call', () => {
            const wishlistScope: IWishlistControlCtrl = getWishlistScope();
            wishlistService.addWishlistScope(offerId, wishlistScope);
            const wishlistScopeList = WishlistService.wishlistsScopeList.get(offerId);
            expect(wishlistScopeList?.has(wishlistScope)).toBeTruthy();
            wishlistService.changeWishlistControlState(offerId, true);
            expect(wishlistScope.isAdded).toBe(true);
            wishlistService.changeWishlistControlState(offerId, false);
            expect(wishlistScope.isAdded).toBe(false);
        });
    });
});
