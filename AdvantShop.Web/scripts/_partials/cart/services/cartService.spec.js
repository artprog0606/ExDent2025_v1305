/**
 * @jest-environment jsdom
 */

import angular from 'angular';
import 'angular-mocks';
import { fn, spyOn } from 'jest-mock';

import '../../../../vendors/sweetalert/sweetalert2.default.js';
import '../../../../vendors/ng-sweet-alert/ng-sweet-alert.js';
import '../../../_common/dom/dom.module.js';
import '../../../_common/module/module.module.js';
import '../../../_common/popover/popover.module.js';
import '../../../_common/urlHelper/urlHelperService.module.js';
import '../cart.module.js';
import '../../../../node_modules/angular-translate/dist/angular-translate.js';
import { PubSub } from '../../../_common/PubSub/PubSub.js';
import '../../../_common/PubSub/__mocks__/PubSub.js';
import {
    cartData,
    addItemServiceResultRedirect,
    addItemServiceResultSucess,
    cartAddAttrsAll,
    cartAddAttrs,
    cartAddNeedParams,
    addToCartResultSucess,
    addToCartResultRedirect,
    updateCartParams,
    cartDataWithCustomOptions,
    evaluatedCustomOptionsToCustomOptionItemMapper,
} from '../__mocks__/cart.js';

describe('CartService', function () {
    let $q, $timeout, $window, cartService, cartConfig, SweetAlert, domService, $rootScope, $document, $flushPendingTasks, $httpBackend;

    const rnd = 0.123456789;
    spyOn(global.Math, 'random').mockReturnValue(rnd);
    document.head.innerHTML = `<base href="http://example.net/" />`;

    beforeEach(() => {
        angular.mock.module('ng-sweet-alert');
        angular.mock.module('pascalprecht.translate');

        angular.mock.module('dom');
        angular.mock.module('urlHelper');
        angular.mock.module('cart');
        angular.mock.inject(($injector) => {
            $q = $injector.get('$q');
            $timeout = $injector.get('$timeout');
            $window = $injector.get('$window');
            cartConfig = $injector.get('cartConfig');
            SweetAlert = $injector.get('SweetAlert');
            domService = $injector.get('domService');
            cartService = $injector.get('cartService');
            $rootScope = $injector.get('$rootScope');
            $document = $injector.get('$document');
            $flushPendingTasks = $injector.get('$flushPendingTasks');
            $httpBackend = $injector.get('$httpBackend');
        });
    });

    describe('getData', function () {
        let fn1;

        beforeEach(() => {
            fn1 = fn();
            cartService.addCallback(cartConfig.callbackNames.get, fn1);
        });

        it('should correctly call getCart', function () {
            $httpBackend.expectPOST('/cart/getCart', { rnd, param1: 'test' }).respond(200, cartData);
            let result = null;

            cartService.getData(null, { param1: 'test' }).then((response) => {
                result = response;
            });

            $httpBackend.flush();

            expect(result).toEqual(cartData);

            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });

        it('should correctly call callback after get cart', function () {
            $httpBackend.expectPOST('/cart/getCart', { rnd, param1: 'test' }).respond(200, cartData);
            let result = null;

            cartService.getData(null, { param1: 'test' }).then((response) => {
                result = response;
            });

            $httpBackend.flush();

            expect(fn1).toHaveBeenCalledWith(cartData, cartData);

            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });

        it('should correctly return the cart when called from multiple locations', function () {
            let callCount = 0;
            $httpBackend.expectPOST('/cart/getCart', { rnd, param1: 'test' }).respond(() => {
                ++callCount;
                return [200, cartData];
            });

            let result1 = null;
            let result2 = null;
            let result3 = null;

            cartService.getData(null, { param1: 'test' }).then((response) => {
                result1 = response;
            });
            cartService.getData(null, { param1: 'test' }).then((response) => {
                result2 = response;
            });
            cartService.getData(null, { param1: 'test' }).then((response) => {
                result3 = response;
            });

            $httpBackend.flush();
            expect(result1).toEqual(cartData);
            expect(result2).toEqual(cartData);
            expect(result3).toEqual(cartData);
            expect(callCount).toEqual(1);
            expect(fn1).toHaveBeenCalledWith(cartData, cartData);

            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });

        it('should correctly get cart data from cache, when cache is true and cart service is initilaze', function () {
            let callCount = 0;
            $httpBackend.expectPOST('/cart/getCart', { rnd, param1: 'test' }).respond(() => {
                ++callCount;
                return [200, cartData];
            });

            let result1 = null;
            let result2 = null;

            cartService.getData(null, { param1: 'test' }).then((response) => {
                result1 = response;
            });
            $httpBackend.flush();
            expect(result1).toEqual(cartData);
            expect(callCount).toEqual(1);
            expect(fn1).toHaveBeenCalledTimes(1);

            $httpBackend.resetExpectations();

            cartService.getData(null, { param1: 'test' }).then((response) => {
                result2 = response;
            });

            try {
                $flushPendingTasks();
            } catch (e) {
                console.log('catch flush error');
            }

            expect(result2).toEqual(cartData);
            expect(callCount).toEqual(1);
            expect(fn1).toHaveBeenCalledTimes(2);
            expect(fn1).toHaveBeenCalledWith(cartData, cartData);

            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });
    });

    describe('updateAmount', function () {
        let fn1;

        beforeEach(() => {
            fn1 = fn();
            cartService.addCallback(cartConfig.callbackNames.update, fn1);
        });

        it('should correctly call updateCart', function () {
            const addToCartResponse = { data: addToCartResultSucess };
            const request = updateCartParams;

            $httpBackend.expectPOST('/cart/updateCart', { items: request, rnd }).respond(200, addToCartResponse);
            $httpBackend.expectPOST('/cart/getCart', { rnd }).respond(200, cartData);
            let result = null;

            cartService.updateAmount(request).then((response) => {
                result = response;
            });

            $httpBackend.flush();

            expect(result).toEqual(cartData);

            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });

        it('should correctly call callback after update', function () {
            const addToCartResponse = { data: addToCartResultSucess };
            const request = updateCartParams;

            $httpBackend.expectPOST('/cart/updateCart', { items: request, rnd }).respond(200, addToCartResponse);
            $httpBackend.expectPOST('/cart/getCart', { rnd }).respond(200, cartData);
            let result = null;

            cartService.updateAmount(request).then((response) => {
                result = response;
            });

            $httpBackend.flush();

            expect(fn1).toHaveBeenCalledWith(cartData, addToCartResponse);

            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });
    });
    describe('removeItem', function () {
        let fn1;

        beforeEach(() => {
            fn1 = fn();
            cartService.addCallback(cartConfig.callbackNames.remove, fn1);
        });

        it('should correctly call removeFromCart', function () {
            const removeFromCartResponse = { data: addToCartResultSucess };
            const request = 1;

            $httpBackend.expectPOST('/cart/removeFromCart', { itemId: 1 }).respond(200, removeFromCartResponse);
            $httpBackend.expectPOST('/cart/getCart', { rnd }).respond(200, cartData);
            let result = null;

            cartService.removeItem(request).then((response) => {
                result = response;
            });

            $httpBackend.flush();

            expect(result).toEqual(removeFromCartResponse);
            expect(fn1).toHaveBeenCalledWith(cartData, removeFromCartResponse);

            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });

        it('should correctly call callback after remove', function () {
            const removeFromCartResponse = { data: addToCartResultSucess };
            const request = 1;

            $httpBackend.expectPOST('/cart/removeFromCart', { itemId: 1 }).respond(200, removeFromCartResponse);
            $httpBackend.expectPOST('/cart/getCart', { rnd }).respond(200, cartData);
            let result = null;

            cartService.removeItem(request).then((response) => {
                result = response;
            });

            $httpBackend.flush();

            expect(fn1).toHaveBeenCalledWith(cartData, removeFromCartResponse);

            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });
    });

    describe('addItem', function () {
        let fn1;

        beforeEach(() => {
            fn1 = fn();
            cartService.addCallback(cartConfig.callbackNames.add, fn1);
        });

        it('should correctly call addToCart, when response is successful', function () {
            const addToCartResponse = addToCartResultSucess;
            const request = cartAddAttrsAll;

            $httpBackend.expectPOST('/cart/addToCart', cartAddNeedParams).respond(200, addToCartResponse);
            $httpBackend.expectPOST('/cart/getCart', { rnd, ...cartAddNeedParams }).respond(200, cartData);
            let result = null;

            cartService.addItem(request).then((response) => {
                result = response;
            });

            $httpBackend.flush();

            expect(result).toEqual(addItemServiceResultSucess);
            expect(fn1).toHaveBeenCalledWith(cartData, [addToCartResponse, cartAddAttrsAll.forceHiddenPopup]);

            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });

        it('should correctly call callback after addItem', function () {
            const addToCartResponse = addToCartResultSucess;
            const request = cartAddAttrsAll;

            $httpBackend.expectPOST('/cart/addToCart', cartAddNeedParams).respond(200, addToCartResponse);
            $httpBackend.expectPOST('/cart/getCart', { rnd, ...cartAddNeedParams }).respond(200, cartData);
            let result = null;

            cartService.addItem(request).then((response) => {
                result = response;
            });

            $httpBackend.flush();

            expect(fn1).toHaveBeenCalledWith(cartData, [addToCartResponse, cartAddAttrsAll.forceHiddenPopup]);

            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });

        it('should correctly call addToCart, when response is redirect', function () {
            const addToCartResponse = addToCartResultRedirect;
            const request = cartAddAttrsAll;

            $httpBackend.expectPOST('/cart/addToCart', cartAddNeedParams).respond(200, addToCartResponse);
            let result = null;

            cartService.addItem(request).then((response) => {
                result = response;
            });

            $httpBackend.flush();

            expect(result).toEqual([addToCartResponse]);
            expect(fn1).not.toHaveBeenCalled();

            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });

        it('should not call callback, when response is redirect', function () {
            const addToCartResponse = addToCartResultRedirect;
            const request = cartAddAttrsAll;

            $httpBackend.expectPOST('/cart/addToCart', cartAddNeedParams).respond(200, addToCartResponse);
            let result = null;

            cartService.addItem(request).then((response) => {
                result = response;
            });

            $httpBackend.flush();

            expect(fn1).not.toHaveBeenCalled();

            $httpBackend.verifyNoOutstandingExpectation();
            $httpBackend.verifyNoOutstandingRequest();
        });

        it('should not call addToCart when offerId and offerIds is null', function () {
            const addToCartResponse = addToCartResultRedirect;
            const request = { ...cartAddAttrsAll, offerId: null, offerIds: null };

            $httpBackend.expectPOST('/cart/addToCart').respond(200, addToCartResponse);

            const result = cartService.addItem(request);

            try {
                $httpBackend.flush();
            } catch (e) {
                console.log('catch flush error');
            }

            expect($httpBackend.verifyNoOutstandingExpectation).toThrow();
        });
    });
    describe('findInCart', function () {
        it('should return first founded by productId', function () {
            $httpBackend.expectPOST('/cart/getCart', { rnd, param1: 'test' }).respond(200, cartData);
            cartService.getData(null, { param1: 'test' });

            $httpBackend.flush();

            expect(cartService.findInCart(1753)).toEqual(cartData.CartProducts[0]);
        });

        it('should return first founded by productId and offerId', function () {
            $httpBackend.expectPOST('/cart/getCart', { rnd, param1: 'test' }).respond(200, cartDataWithCustomOptions);
            const targetOffer = 11111;
            let targetProduct = cartDataWithCustomOptions.CartProducts.find((x) => x.OfferId === targetOffer);

            cartService.getData(null, { param1: 'test' });

            $httpBackend.flush();

            expect(cartService.findInCart(targetProduct.ProductId, targetOffer)).toEqual(targetProduct);
        });

        it('should return first founded by productId and customOptions', function () {
            $httpBackend.expectPOST('/cart/getCart', { rnd, param1: 'test' }).respond(200, cartDataWithCustomOptions);
            let targetProduct = cartDataWithCustomOptions.CartProducts[0];

            cartService.getData(null, { param1: 'test' });

            $httpBackend.flush();

            expect(
                cartService.findInCart(targetProduct.ProductId, null, evaluatedCustomOptionsToCustomOptionItemMapper(targetProduct.SelectedOptions)),
            ).toEqual(targetProduct);
        });

        it('should not found product if has customOptions and in cart product without options', function () {
            $httpBackend.expectPOST('/cart/getCart', { rnd, param1: 'test' }).respond(200, cartDataWithCustomOptions);
            let targetProduct = cartData.CartProducts[0];

            cartService.getData(null, { param1: 'test' });
            $httpBackend.flush();

            expect(
                cartService.findInCart(
                    targetProduct.ProductId,
                    null,
                    evaluatedCustomOptionsToCustomOptionItemMapper(cartDataWithCustomOptions.CartProducts[0].SelectedOptions),
                ),
            ).toBeFalsy();
        });
    });
});
