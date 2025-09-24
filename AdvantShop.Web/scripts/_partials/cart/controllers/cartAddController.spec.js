/**
 * @jest-environment jsdom
 */

import angular from 'angular';
import 'angular-mocks';
import {fn, spyOn} from 'jest-mock';

import '../../../../vendors/sweetalert/sweetalert2.default.js';
import '../../../../vendors/ng-sweet-alert/ng-sweet-alert.js';
import 'angularjs-toaster';
import '../../../_common/dom/dom.module.js';
import '../../../_common/module/module.module.js';
import '../../../_common/popover/popover.module.js';
import '../../../_common/urlHelper/urlHelperService.module.js';
import '../cart.module.js';
import '../../../../node_modules/angular-translate/dist/angular-translate.js';
import {PubSub} from '../../../_common/PubSub/PubSub.js';
import '../../../_common/PubSub/__mocks__/PubSub.js';
import {cartData, addItemServiceResultRedirect, addItemServiceResultSucess, cartAddAttrsAll, updateCartResponseSuccess} from '../__mocks__/cart.js';
import {cartAddConfigDefault} from '../cartConfigDefault.ts';

describe('CartAddCtrl', function () {
    let $controller,
        $rootScope,
        $document,
        $attrs,
        $parse,
        $q,
        $timeout,
        $window,
        cartConfig,
        cartService,
        moduleService,
        popoverService,
        SweetAlert,
        $translate,
        domService,
        ctrl,
        $scope,
        $compile,
        customOptionsService,
        $flushPendingTasks,
        toaster,
        $httpBackend;

    const rnd = 0.123456789;
    spyOn(global.Math, 'random').mockReturnValue(rnd);

    const targetElement = document.createElement('button');
    targetElement.textContent = 'Click me';
    document.body.appendChild(targetElement);

    const mouseEventClick = new MouseEvent('click', {
        bubbles: true,
        cancelable: true,
    });

    Object.defineProperty(mouseEventClick, 'target', {value: targetElement});

    document.head.innerHTML = `<base href="http://example.net/" />`;

    beforeEach(() => {
        angular.mock.module('popover');
        angular.mock.module('ng-sweet-alert');
        angular.mock.module('pascalprecht.translate');
        angular.mock.module('dom');
        angular.mock.module('module');
        angular.mock.module('toaster');
        angular.mock.module('urlHelper');
        angular.mock.module('cart');
        angular.mock.inject(($injector) => {
            $parse = $injector.get('$parse');
            $compile = $injector.get('$compile');
            $q = $injector.get('$q');
            $timeout = $injector.get('$timeout');
            $window = $injector.get('$window');
            cartConfig = $injector.get('cartConfig');
            cartService = $injector.get('cartService');
            customOptionsService = $injector.get('customOptionsService');
            moduleService = $injector.get('moduleService');
            popoverService = $injector.get('popoverService');
            SweetAlert = $injector.get('SweetAlert');
            domService = $injector.get('domService');
            $controller = $injector.get('$controller');
            $rootScope = $injector.get('$rootScope');
            $document = $injector.get('$document');
            $flushPendingTasks = $injector.get('$flushPendingTasks');
            toaster = $injector.get('toaster');
            $httpBackend = $injector.get('$httpBackend');
        });

        $scope = $rootScope.$new();

        ctrl = $controller('CartAddCtrl', {
            $document,
            $scope,
            $attrs: {},
            $parse,
            $q,
            $timeout,
            $window,
            cartConfig,
            cartService,
            moduleService,
            popoverService,
            SweetAlert,
            $translate,
            domService,
            toaster,
        });
    });

    describe('$onInit', function () {
        it('should correct init ctrl', async function () {
            ctrl.refresh = fn();
            ctrl.parseAttributes = fn(() => {
            });
            ctrl.data = {};
            ctrl.$onInit();

            expect(ctrl.refresh).toHaveBeenCalled();
            expect(ctrl.parseAttributes).toHaveBeenCalled();
        });

        it('should correct init ctrl if source is mobile', async function () {
            cartService.setStateInfo = fn();
            ctrl.refresh = fn();
            ctrl.parseAttributes = fn(() => {
                ctrl.data = {
                    source: 'mobile',
                };
            });

            ctrl.$onInit();

            expect(cartService.setStateInfo).toHaveBeenCalledWith(true);
            expect(ctrl.refresh).toHaveBeenCalled();
            expect(ctrl.parseAttributes).toHaveBeenCalled();
        });

        it('should correct subscribe to events', function () {
            cartService.setStateInfo = fn();
            ctrl.refresh = fn();
            ctrl.parseAttributes = fn(() => {
            });
            ctrl.data = {
                offerId: 123,
                productId: 1,
            };

            customOptionsService.getSelectedOptions = fn();
            cartService.findInCart = fn();

            ctrl.$onInit();

            expect(ctrl.refresh).toHaveBeenCalledTimes(1);
            expect(customOptionsService.getSelectedOptions).not.toHaveBeenCalled();
            expect(cartService.findInCart).not.toHaveBeenCalled();

            PubSub.publish('cart.updateAmount');
            expect(ctrl.refresh).toHaveBeenCalledTimes(2);
            PubSub.publish('cart.remove', 123);
            expect(ctrl.refresh).toHaveBeenCalledTimes(3);
            PubSub.publish('cart.clear');
            expect(ctrl.refresh).toHaveBeenCalledTimes(4);
            PubSub.publish('product.customOptions.change', {productId: 1, offerId: 123, items: []});
            expect(customOptionsService.getSelectedOptions).toHaveBeenCalled();
            expect(cartService.findInCart).toHaveBeenCalled();
        });
        it('should not call callback in subscribe if not this product', async function () {
            cartService.setStateInfo = fn();
            ctrl.refresh = fn();
            ctrl.parseAttributes = fn(() => {
            });
            ctrl.data = {
                offerId: 123,
                productId: 1,
            };

            customOptionsService.getSelectedOptions = fn();
            cartService.findInCart = fn();

            ctrl.$onInit();

            expect(ctrl.refresh).toHaveBeenCalledTimes(1);
            expect(customOptionsService.getSelectedOptions).not.toHaveBeenCalled();
            expect(cartService.findInCart).not.toHaveBeenCalled();

            PubSub.publish('cart.remove', 444);
            expect(ctrl.refresh).toHaveBeenCalledTimes(1);
            PubSub.publish('product.customOptions.change', {productId: 2, offerId: 123, items: []});
            expect(customOptionsService.getSelectedOptions).not.toHaveBeenCalled();
            expect(cartService.findInCart).not.toHaveBeenCalled();
        });
    });
    describe('parseAttributes', function () {
        it.each([
            {key: 'forceHiddenPopup', value: cartAddAttrsAll.forceHiddenPopup},
            {key: 'hideShipping', value: cartAddAttrsAll.hideShipping},
            {key: 'lpId', value: cartAddAttrsAll.lpId.toString()},
            {key: 'lpUpId', value: cartAddAttrsAll.lpUpId.toString()},
            {key: 'lpEntityId', value: cartAddAttrsAll.lpEntityId.toString()},
            {key: 'lpEntityType', value: cartAddAttrsAll.lpEntityType},
            {key: 'lpBlockId', value: cartAddAttrsAll.lpBlockId.toString()},
            {key: 'lpButtonName', value: cartAddAttrsAll.lpButtonName},
            {key: 'maxStepSpinbox', value: cartAddAttrsAll.maxStepSpinbox.toString()},
            {key: 'minStepSpinbox', value: cartAddAttrsAll.minStepSpinbox.toString()},
            {key: 'stepSpinbox', value: cartAddAttrsAll.stepSpinbox.toString()},
            {key: 'mode', value: cartAddAttrsAll.mode},
            {key: 'modeFrom', value: cartAddAttrsAll.modeFrom},
            {key: 'source', value: cartAddAttrsAll.source},
        ])('should correct parse static attr "$key"', ({key, value}) => {
            $httpBackend.whenPOST('/cart/getCart', {rnd}).respond(200, cartData);

            const newScope = $rootScope.$new();

            const anchorElement = document.createElement('a');
            anchorElement.dataset.cartAdd = '';
            anchorElement.dataset[key] = value;

            const element = $compile(anchorElement)(newScope);
            const elementScope = element.scope();
            newScope.$digest();

            expect(elementScope.cartAdd.data[key]).toEqual(value);
        });

        it.each([
            {key: 'offerId', value: cartAddAttrsAll.offerId, newValue: 2},
            {key: 'productId', value: cartAddAttrsAll.productId, newValue: 2},
            {key: 'amount', value: cartAddAttrsAll.amount, newValue: 123},
            {
                key: 'attributesXml',
                value: cartAddAttrsAll.attributesXml,
                newValue: {
                    newProp: 'newPropValue',
                },
            },
            {key: 'payment', value: cartAddAttrsAll.payment, newValue: 'mypayment'},
            {key: 'cartAddType', value: cartAddAttrsAll.cartAddType, newValue: 1},
            {key: 'offerIds', value: cartAddAttrsAll.offerIds, newValue: [22, 33]},
        ])('should correct parse watched attr "$key" and update after change data', ({key, value, newValue}) => {
            $httpBackend.whenPOST('/cart/getCart', {rnd}).respond(200, cartData);

            const newScope = $rootScope.$new();

            $rootScope[key] = cartAddAttrsAll[key];

            const anchorElement = document.createElement('a');
            anchorElement.dataset.cartAdd = '';
            anchorElement.dataset[key] = key;

            const element = $compile(anchorElement)(newScope);
            const elementScope = element.scope();
            newScope.$digest();

            expect(elementScope.cartAdd.data[key]).toEqual(value);

            $rootScope[key] = newValue;
            elementScope.$digest();

            expect(elementScope.cartAdd.data[key]).toEqual(newValue);
        });

        it('should correct parse watched attr "href" and update after change ng-href', () => {
            $httpBackend.whenPOST('/cart/getCart', {rnd}).respond(200, cartData);

            const newScope = $rootScope.$new();

            $rootScope.newHref = 'initHref';

            const anchorElement = document.createElement('a');
            anchorElement.dataset.cartAdd = '';
            anchorElement.dataset.ngHref = '{{newHref}}';
            anchorElement.href = 'initHref';

            const element = $compile(anchorElement)(newScope);
            const elementScope = element.scope();

            newScope.$digest();

            expect(elementScope.cartAdd.data.href).toEqual('initHref');

            $rootScope.newHref = cartAddAttrsAll.href;
            elementScope.$digest();

            expect(elementScope.cartAdd.data.href).toEqual(cartAddAttrsAll.href);
        });
    });

    describe('refresh', function () {
        it('should get from cache when params is true', async function () {
            $httpBackend.expectPOST('/cart/getCart', {rnd}).respond(200, cartData);

            ctrl.data = {
                productId: cartData.CartProducts[0].ProductId,
            };

            ctrl.refresh(true);
            $httpBackend.flush();
            expect(ctrl.needAdd).toBeFalsy();
            expect(ctrl.productCartData).toEqual(cartData.CartProducts[0]);

            cartService.getData = fn(() => Promise.resolve(cartData));
            await ctrl.refresh(true);
            expect(cartService.getData).toHaveBeenCalledWith(true);
        });
        it('should not get from cache when params is false', async function () {
            $httpBackend.expectPOST('/cart/getCart', {rnd}).respond(200, cartData);

            ctrl.data = {
                productId: cartData.CartProducts[0].ProductId,
            };

            ctrl.refresh();
            $httpBackend.flush();

            expect(ctrl.needAdd).toBeFalsy();
            expect(ctrl.productCartData).toEqual(cartData.CartProducts[0]);

            cartService.getData = fn(() => Promise.resolve(cartData));
            await ctrl.refresh(false);
            expect(cartService.getData).toHaveBeenCalledWith(false);
        });

        it('should correct refresh cart data without data and request return empty array', function () {
            $httpBackend.expectPOST('/cart/getCart', {rnd}).respond(200, {
                CartProducts: [],
            });

            ctrl.data = {
                productId: cartData.CartProducts[0].ProductId,
            };

            ctrl.refresh();
            $httpBackend.flush();

            expect(ctrl.productCartData).toEqual(null);
            expect(ctrl.needAdd).toBeTruthy();
        });

        it('should correct set productCartData and needAdd property, when found in cart', function () {
            $httpBackend.expectPOST('/cart/getCart', {rnd}).respond(200, cartData);

            ctrl.cartData = cartData;

            ctrl.data = {
                productId: cartData.CartProducts[0].ProductId,
            };
            ctrl.needAdd = true;

            ctrl.refresh();

            $httpBackend.flush();
            expect(ctrl.needAdd).toBeFalsy();
            expect(ctrl.productCartData).toEqual(cartData.CartProducts[0]);
        });

        it('should set productCartData null and needAdd property, when not found in cart', async function () {
            $httpBackend.expectPOST('/cart/getCart', {rnd}).respond(200, cartData);

            ctrl.cartData = cartData;

            ctrl.needAdd = true;

            ctrl.data = {
                productId: '30',
            };

            ctrl.refresh();
            $httpBackend.flush();

            expect(ctrl.needAdd).toBeTruthy();
            expect(ctrl.productCartData).toEqual(null);
        });
    });
    describe('updateAmount', function () {
        it('should not call if state is loading', function () {
            cartService.removeItem = fn(() => Promise.resolve());

            cartService.updateAmount = fn(() => Promise.resolve());
            ctrl.addItem = fn(() => Promise.resolve());

            ctrl.state = cartAddConfigDefault.cartStateButton.loading;
            ctrl.productCartData = {
                Amount: 0,
            };

            ctrl.updateAmount(cartData.CartProducts[0].ProductId, 10);

            $scope.$apply();

            expect(cartService.removeItem).not.toHaveBeenCalled();
            expect(cartService.updateAmount).not.toHaveBeenCalled();
            expect(ctrl.addItem).not.toHaveBeenCalled();
        });

        it('should correct remove item, when amount == 0', async function () {
            const newCartData = {
                CartProducts: [
                    {
                        ProductId: 2,
                    },
                    {
                        ProductId: 3,
                    },
                ],
            };
            $httpBackend.expectPOST('/cart/getCart', {rnd}).respond(200, newCartData);

            cartService.removeItem = fn(() => $q.resolve());

            cartService.updateAmount = fn();

            moduleService.update = fn();

            ctrl.isLoading = false;
            ctrl.needAdd = false;

            const productCartData = {
                Amount: 0,
                ShoppingCartItemId: 22,
                productId: '1',
            };
            ctrl.productCartData = productCartData;

            ctrl.data = {
                productId: '1',
            };

            ctrl.updateAmount(1, 10);

            expect(ctrl.isLoading).toBeTruthy();

            expect(cartService.removeItem).toHaveBeenCalledWith(productCartData.ShoppingCartItemId);
            expect(cartService.updateAmount).not.toHaveBeenCalled();

            $httpBackend.flush();

            expect(moduleService.update).toHaveBeenCalledWith('fullcartmessage');
            expect(ctrl.productCartData).toEqual(null);

            expect(ctrl.needAdd).toBeTruthy();
            expect(ctrl.isLoading).toBeFalsy();
        });

        it('should correct update item', async function () {

            $httpBackend.expectPOST('/cart/getCart', {rnd}).respond(200, cartData);

            cartService.removeItem = fn();

            cartService.updateAmount = fn(() => $q.resolve());
            ctrl.addItem = fn();

            moduleService.update = fn();

            ctrl.isLoading = false;
            ctrl.needAdd = false;
            const productCartData = {
                Amount: 2,
                ShoppingCartItemId: 22,
            };
            ctrl.productCartData = productCartData;

            ctrl.data = {
                productId: cartData.CartProducts[0].ProductId,
            };

            ctrl.updateAmount(cartData.CartProducts[0].ProductId, 10);

            expect(ctrl.isLoading).toBeTruthy();

            expect(cartService.updateAmount).toHaveBeenCalledWith([
                {
                    Key: 10,
                    Value: cartData.CartProducts[0].ProductId,
                },
            ]);

            expect(cartService.removeItem).not.toHaveBeenCalled();

            $httpBackend.flush()

            expect(moduleService.update).toHaveBeenCalledWith('minicartmessage');
            expect(ctrl.needAdd).toBeFalsy();
        });

        it('should correct add item when fast quickly called several times', async function () {

            $httpBackend.expectPOST('/cart/getCart', {rnd}).respond(200, cartData);

            cartService.removeItem = fn();

            cartService.updateAmount = fn(() => $q.resolve());

            moduleService.update = fn(() => $q.resolve());

            ctrl.isLoading = false;

            const productCartData = {
                Amount: 2,
                ShoppingCartItemId: 22,
            };
            ctrl.productCartData = productCartData;

            ctrl.data = {
                productId: '1',
            };

            ctrl.updateAmount(1, 10);
            ctrl.updateAmount(1, 10);

            expect(ctrl.isLoading).toBeTruthy();
            expect(cartService.updateAmount).toHaveBeenCalled();
            expect(cartService.removeItem).not.toHaveBeenCalled();

            $httpBackend.flush();

            expect(ctrl.isLoadingAdd).toBeFalsy();
            expect(ctrl.isLoading).toBeFalsy();
        });

        it('should not update amount if addItem is not resolved', async function () {
            cartService.removeItem = fn();
            cartService.addItem = fn(() => Promise.resolve(addItemServiceResultSucess));

            cartService.updateAmount = fn(() => Promise.resolve());

            moduleService.update = fn(() => Promise.resolve());

            ctrl.isLoading = false;

            const productCartData = {
                Amount: 2,
                ShoppingCartItemId: 22,
            };
            ctrl.productCartData = productCartData;

            ctrl.data = {
                productId: '1',
            };

            const result = ctrl.addItem(mouseEventClick);
            ctrl.updateAmount(1, 10);

            expect(ctrl.isLoadingAdd).toBeTruthy();
            expect(ctrl.isLoading).toBeFalsy();
            expect(cartService.updateAmount).not.toHaveBeenCalled();
            expect(cartService.removeItem).not.toHaveBeenCalled();

            await result;

            expect(ctrl.isLoadingAdd).toBeFalsy();
            expect(ctrl.isLoading).toBeFalsy();
        });
    });
    describe('addItem', function () {
        let addToCartFn = fn();
        let cartAddFn = fn();
        let cartAddFnv2 = fn();

        beforeEach(() => {
            addToCartFn = fn();
            cartAddFn = fn();
            cartAddFnv2 = fn();

            PubSub.subscribe('add_to_cart', addToCartFn);
            PubSub.subscribe('cart.add', cartAddFn);
            PubSub.subscribe('cart.addv2', cartAddFnv2);

            moduleService.update = fn(() => Promise.resolve());
        });

        afterEach(() => {
            PubSub.clear();
        });

        it('should the correct add item, when valid function is null', async function () {
            cartService.addItem = fn(() => Promise.resolve(addItemServiceResultSucess));

            ctrl.data = {
                cartAddValid: null,
            };

            const result = await ctrl.addItem(mouseEventClick);

            expect(cartService.addItem).toHaveBeenCalled();
            expect(result).toEqual(addItemServiceResultSucess);
        });

        it.each([
            {
                cartAddValidResult: false,
                countCall: 0,
                expected: undefined,
            },
            {
                cartAddValidResult: true,
                countCall: 1,
                expected: addItemServiceResultSucess,
            },
        ])('should the correct add item, when valid function return $cartAddValidResult', async ({cartAddValidResult, countCall, expected}) => {
            cartService.addItem = fn(() => Promise.resolve(addItemServiceResultSucess));

            ctrl.data = {
                cartAddValid: fn(() => cartAddValidResult),
            };

            const result = await ctrl.addItem(mouseEventClick);

            expect(ctrl.data.cartAddValid).toHaveBeenCalled();
            expect(cartService.addItem).toHaveBeenCalledTimes(countCall);
            expect(result).toEqual(expected);
        });

        it('should the correct add item and return data', async function () {
            cartService.addItem = fn(() => Promise.resolve(addItemServiceResultSucess));

            ctrl.data = cartAddAttrsAll;

            const result = await ctrl.addItem(mouseEventClick);

            expect(cartService.addItem).toHaveBeenCalledWith(cartAddAttrsAll);

            expect(result).toEqual(addItemServiceResultSucess);
        });

        it('should call refresh', async function () {
            ctrl.refresh = fn();
            cartService.addItem = fn(() => Promise.resolve(addItemServiceResultSucess));

            ctrl.data = cartAddAttrsAll;

            const result = await ctrl.addItem(mouseEventClick);

            expect(cartService.addItem).toHaveBeenCalledWith(cartAddAttrsAll);
            expect(ctrl.refresh).toHaveBeenCalled();

            expect(result).toEqual(addItemServiceResultSucess);
        });

        it('should the correct publish to PubSub, when success status', async function () {
            cartService.addItem = fn(() => Promise.resolve(addItemServiceResultSucess));

            ctrl.data = cartAddAttrsAll;

            const result = await ctrl.addItem(mouseEventClick);
            expect(addToCartFn).toHaveBeenCalledWith(cartAddAttrsAll.href);
            expect(cartAddFn).toHaveBeenCalledWith(
                cartAddAttrsAll.offerId,
                cartAddAttrsAll.productId,
                cartAddAttrsAll.amount,
                cartAddAttrsAll.attributesXml,
                addItemServiceResultSucess[0].cartId,
                targetElement,
            );
            expect(cartAddFnv2).toHaveBeenCalledWith(
                cartAddAttrsAll.productId,
                addItemServiceResultSucess[0].cartId,
                addItemServiceResultSucess[0].CartItem,
                targetElement,
            );
        });

        it('should the correct update module "minicartmessage" and  "fullcartmessage", when success status', async function () {
            cartService.addItem = fn(() => Promise.resolve(addItemServiceResultSucess));

            ctrl.data = cartAddAttrsAll;

            const result = await ctrl.addItem(mouseEventClick);
            expect(moduleService.update).toHaveBeenCalledWith(['minicartmessage', 'fullcartmessage']);
        });

        it('should the correct change location, when redirect status', async function () {
            cartService.addItem = fn(() => Promise.resolve(addItemServiceResultRedirect));
            Object.defineProperty($window, 'location', {
                value: {assign: fn()},
            });

            ctrl.data = cartAddAttrsAll;

            const result = await ctrl.addItem(mouseEventClick);

            expect($window.location.assign).toHaveBeenCalledWith(ctrl.data.href);
        });

        it('should the correct change location, when redirect status and has url redirect', async function () {
            const addItemServiceResultRedirectWithUrl = [
                {
                    ...addItemServiceResultRedirect[0],
                    url: 'example',
                },
                addItemServiceResultRedirect[1],
            ];

            cartService.addItem = fn(() => Promise.resolve(addItemServiceResultRedirectWithUrl));
            Object.defineProperty($window, 'location', {
                value: {assign: fn()},
            });

            ctrl.data = cartAddAttrsAll;

            const result = await ctrl.addItem(mouseEventClick);

            expect($window.location.assign).toHaveBeenCalledWith('example');
        });

        it('should the correct call showInfoWithDebounce, when success status and source is mobile', async function () {
            cartService.addItem = fn(() => Promise.resolve(addItemServiceResultSucess));
            cartService.showInfoWithDebounce = fn();
            ctrl.data = {...cartAddAttrsAll, source: 'mobile'};

            const result = await ctrl.addItem(mouseEventClick);

            expect(cartService.showInfoWithDebounce).toHaveBeenCalled();
        });
        it('should not call showInfoWithDebounce, when redirect status', async function () {
            cartService.addItem = fn(() => Promise.resolve(addItemServiceResultRedirect));

            cartService.showInfoWithDebounce = fn();
            ctrl.source = 'mobile';

            Object.defineProperty($window, 'location', {
                value: {assign: fn()},
            });

            ctrl.data = cartAddAttrsAll;

            const result = await ctrl.addItem(mouseEventClick);

            expect(cartService.showInfoWithDebounce).not.toHaveBeenCalled();
        });
        it('should the correct add item, when fast quickly called several times ', async function () {
            cartService.addItem = fn(() => Promise.resolve(addItemServiceResultSucess));

            ctrl.data = cartAddAttrsAll;

            const result = ctrl.addItem(mouseEventClick);

            expect(cartService.addItem).toHaveBeenCalledTimes(1);
            expect(ctrl.isLoadingAdd).toBeTruthy();

            const result2 = ctrl.addItem(mouseEventClick);

            try {
                $flushPendingTasks();
            } catch (e) {
                console.log('catch flush error');
            }
            expect(cartService.addItem).toHaveBeenCalledTimes(1);
            expect(ctrl.isLoadingAdd).toBeTruthy();
            expect(result2.$$state.value).toEqual(null);
        });
    });
    describe('popoverModule', function () {
        it('should not show, when getModule return null', async function () {
            moduleService.getModule = fn();
            popoverService.getPopoverScope = fn(() =>
                Promise.resolve({
                    active: fn(),
                    updatePosition: fn(),
                    deactive: fn(),
                }),
            );

            expect(popoverService.getPopoverScope).not.toHaveBeenCalled();

            ctrl.popoverModule(['exampleContent']);

            try {
                await $flushPendingTasks();
            } catch (e) {
                console.log('catch flush error');
            }

            expect(popoverService.getPopoverScope).not.toHaveBeenCalled();
        });
        it('should not show, when content empty', async function () {
            moduleService.getModule = fn(() => {
                return {prop: 1};
            });
            popoverService.getPopoverScope = fn(() =>
                Promise.resolve({
                    active: fn(),
                    updatePosition: fn(),
                    deactive: fn(),
                }),
            );

            expect(popoverService.getPopoverScope).not.toHaveBeenCalled();

            ctrl.popoverModule(['']);

            try {
                await $flushPendingTasks();
            } catch (e) {
                console.log('catch flush error');
            }

            expect(popoverService.getPopoverScope).not.toHaveBeenCalled();
        });
        it('should correct active popover', async function () {
            const popoverScope = {
                active: fn(),
                updatePosition: fn(),
                deactive: fn(),
            };
            moduleService.getModule = fn(() => 'any');
            popoverService.getPopoverScope = fn(() => Promise.resolve(popoverScope));

            expect(popoverService.getPopoverScope).not.toHaveBeenCalled();

            await ctrl.popoverModule(['exampleContent']);

            await $flushPendingTasks();

            expect(popoverService.getPopoverScope).toHaveBeenCalled();
            expect(popoverScope.active).toHaveBeenCalled();
            expect(popoverScope.updatePosition).toHaveBeenCalled();
            expect(popoverScope.deactive).not.toHaveBeenCalled();

            await ctrl.popoverModule(['exampleContent']);

            await $flushPendingTasks();

            expect(popoverScope.deactive).toHaveBeenCalled();
        });
    });

    describe('checkSizeAndColor', function () {
        it('should not call, when offer is null', function () {
            cartService.setStateInfo = fn();
            ctrl.refresh = fn();
            ctrl.parseAttributes = fn(() => {
            });
            ctrl.data = {
                offerId: 123,
                productId: 1,
            };

            customOptionsService.getSelectedOptions = fn();
            cartService.findInCart = fn();

            ctrl.checkSizeAndColor({}, 'color', {});

            expect(cartService.findInCart).not.toHaveBeenCalled();
        });

        it('should not call, when productId not equal', function () {
            cartService.setStateInfo = fn();
            ctrl.refresh = fn();
            ctrl.parseAttributes = fn(() => {
            });
            ctrl.data = {
                offerId: 123,
                productId: 1,
            };

            customOptionsService.getSelectedOptions = fn();
            cartService.findInCart = fn();

            ctrl.checkSizeAndColor({}, 'color', {
                offer: {
                    ProductId: 2,
                },
            });

            expect(cartService.findInCart).not.toHaveBeenCalled();
        });

        it('should find product in cart by offer', function () {
            cartService.setStateInfo = fn();
            ctrl.refresh = fn();
            ctrl.parseAttributes = fn(() => {
            });
            ctrl.data = {
                offerId: 123,
                productId: 1,
            };

            customOptionsService.getSelectedOptions = fn();
            const resultFind = {
                OfferId: 134,
                productId: 1,
            };
            cartService.findInCart = fn(() => {
                return resultFind;
            });

            ctrl.checkSizeAndColor({}, 'color', {
                offer: {
                    ProductId: 1,
                },
            });

            expect(cartService.findInCart).toHaveBeenCalled();
            expect(ctrl.productCartData).toEqual(resultFind);
        });
    });

    describe('getStateButton', function () {
        it('initial state "add"', function () {
            ctrl.refresh = fn();
            ctrl.parseAttributes = fn(() => {
            });
            ctrl.data = {};

            ctrl.$onInit();
            expect(ctrl.getStateButton()).toEqual(cartAddConfigDefault.cartStateButton.add);
        });

        it('get "add" state, after  remove item', async function () {
            const newCartData = {
                CartProducts: [
                    {
                        ProductId: 2,
                    },
                    {
                        ProductId: 3,
                    },
                ],
            };
            $httpBackend.expectPOST('/cart/getCart', {rnd}).respond(200, newCartData);

            cartService.removeItem = fn(() => Promise.resolve());

            ctrl.isLoading = false;
            ctrl.needAdd = false;
            ctrl.refresh = fn();

            const productCartData = {
                Amount: 0,
                ShoppingCartItemId: 22,
                productId: '1',
            };
            ctrl.productCartData = productCartData;

            ctrl.data = {
                productId: '1',
            };

            await ctrl.updateAmount(1, 10);

            expect(ctrl.getStateButton()).toEqual(cartAddConfigDefault.cartStateButton.add);
        });

        it('get "loading" state, after send request and before complete', async function () {
            const newCartData = {
                CartProducts: [
                    {
                        ProductId: 2,
                    },
                    {
                        ProductId: 3,
                    },
                ],
            };
            $httpBackend.expectPOST('/cart/getCart', {rnd}).respond(200, newCartData);

            cartService.removeItem = fn(() => Promise.resolve());

            ctrl.isLoading = false;
            ctrl.needAdd = false;
            ctrl.refresh = fn();

            const productCartData = {
                Amount: 0,
                ShoppingCartItemId: 22,
                productId: '1',
            };
            ctrl.productCartData = productCartData;

            ctrl.data = {
                productId: '1',
            };

            const promise = ctrl.updateAmount(1, 10);

            expect(ctrl.getStateButton()).toEqual(cartAddConfigDefault.cartStateButton.loading);

            await promise;

            expect(ctrl.getStateButton()).toEqual(cartAddConfigDefault.cartStateButton.add);
        });

        it('get "update" state, after add item ', async function () {
            cartService.addItem = fn(() => Promise.resolve(addItemServiceResultSucess));
            $httpBackend.whenPOST('/cart/getCart', {rnd}).respond(200, cartData);

            ctrl.data = {
                productId: cartData.CartProducts[0].ProductId,
                cartAddType: cartAddConfigDefault.cartAddType.WithSpinbox,
            };

            await ctrl.addItem(mouseEventClick);
            $httpBackend.flush();

            expect(ctrl.getStateButton()).toEqual(cartAddConfigDefault.cartStateButton.update);
        });

        it('get "add" state, when cart add type classic ', async function () {
            cartService.addItem = fn(() => Promise.resolve(addItemServiceResultSucess));
            $httpBackend.whenPOST('/cart/getCart', {rnd}).respond(200, cartData);

            ctrl.data = {
                productId: cartData.CartProducts[0].ProductId,
                cartAddType: cartAddConfigDefault.cartAddType.Classic,
            };

            await ctrl.addItem(mouseEventClick);
            $httpBackend.flush();

            expect(ctrl.getStateButton()).toEqual(cartAddConfigDefault.cartStateButton.add);
        });
    });
});
