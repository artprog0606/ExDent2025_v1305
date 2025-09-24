(function (ng) {
    'use strict';

    /*@ngInject*/
    /** @type{CartServiceFnType}
     *  @this{CartServiceType}
     * */
    var cartService = function ($document, $q, $http, $translate, $window, cartConfig, domService, SweetAlert, customOptionsService) {
        var service = this;
        /**
         *  @type{ICart}
         * */
        var cart = {};
        var isRequestProcess = false,
            isInitilaze = false,
            deferes = [],
            callbacks = {},
            needInfoShow = false;

        service.getData = function (cache, queryParams) {
            var defer = $q.defer();

            if (cache == null) {
                cache = true;
            }

            if (isInitilaze === true && cache === true) {
                defer.resolve(cart);
            } else {
                if (isRequestProcess === true) {
                    deferes.push(defer);
                } else {
                    isRequestProcess = true;

                    deferes.push(defer);

                    $http.post('/cart/getCart', angular.extend({ rnd: Math.random() }, queryParams || {})).then(function (response) {
                        angular.extend(cart, response.data);
                        isInitilaze = true;
                        isRequestProcess = false;

                        for (var i = deferes.length - 1; i >= 0; i--) {
                            deferes[i].resolve(cart);
                        }

                        deferes.length = 0;
                    });
                }
            }

            return defer.promise.then(function (result) {
                service.processCallback('get', result);
                return result;
            });
        };

        service.updateAmount = function (items, queryParams) {
            return $http
                .post(
                    '/cart/updateCart',
                    angular.extend(
                        {
                            items: items,
                            rnd: Math.random(),
                        },
                        queryParams || {},
                    ),
                )
                .then(function (response) {
                    service.processCallback('update', response.data);
                    return service.getData(false, queryParams);
                });
        };

        service.removeItem = function (shoppingCartItemId, queryParams) {
            return $http
                .post('/cart/removeFromCart', angular.extend({ itemId: shoppingCartItemId }, queryParams || {}))
                .then(function (response) {
                    return service.getData(false, queryParams).then(() => response.data);
                })
                .then((data) => {
                    service.processCallback('remove', data);
                    return data;
                });
        };

        service.addItem = function ({
            offerId,
            productId,
            amount,
            attributesXml,
            payment,
            mode,
            lpId,
            lpUpId,
            lpEntityId,
            lpEntityType,
            lpBlockId,
            lpButtonName,
            hideShipping,
            offerIds,
            modeFrom,
            forceHiddenPopup,
            queryParams,
        }) {
            var params = angular.extend(
                {
                    offerId,
                    productId,
                    amount,
                    attributesXml,
                    payment,
                    mode,
                    lpId,
                    lpUpId,
                    lpEntityId,
                    lpEntityType,
                    lpBlockId,
                    lpButtonName,
                    hideShipping,
                    offerIds,
                    modeFrom,
                },
                queryParams || {},
            );

            if (params.offerId == null && params.offerIds == null) return;

            return $http.post('/cart/addToCart', params).then(function (response) {
                var result = response.data,
                    defer,
                    promise;

                if (response.data.status !== 'redirect') {
                    return service.getData(false, params).then(function (data) {
                        service.processCallback('add', [result, forceHiddenPopup]);
                        return [result, data];
                    });
                } else {
                    defer = $q.defer();
                    promise = defer.promise;
                    defer.resolve([result]);
                    return promise;
                }
            });
        };

        service.addItems = function (items, queryParams) {
            if (items == null || items.length == 0) return;

            return $http.post('/cart/addCartItems', angular.extend({ items: items }, queryParams || {})).then(function (response) {
                var result = response.data;
                result.addedCount = items.length;
                if (result.status === 'success') {
                    return service.getData(false, queryParams).then(function () {
                        service.processCallback('add', result);
                        return [result];
                    });
                }
            });
        };

        service.clear = function (queryParams) {
            return $http.post('/Cart/ClearCart', queryParams || {}).then(function () {
                service.processCallback('clear');
                return service.getData(false, queryParams);
            });
        };

        service.addCallback = function (name, func, targetName) {
            callbacks[name] = callbacks[name] || [];

            if (func == null) {
                throw Error('Callback for cart equal null');
            }

            callbacks[name].push({ callback: func, targetName: targetName || null });
        };

        service.processCallback = function (name, params, targetName) {
            if (callbacks[name] == null) {
                return;
            }

            for (var i = callbacks[name].length - 1; i >= 0; i--) {
                if (targetName != null && callbacks[name][i].targetName === targetName) {
                    callbacks[name][i].callback(cart, params);
                } else {
                    callbacks[name][i].callback(cart, params);
                }
            }
        };

        service.removeCallback = function (name, targetName, cb) {
            var arrayCallbacksByName = callbacks[name],
                index;

            if (arrayCallbacksByName == null) {
                return;
            }

            if (targetName != null) {
                for (var i = 0; i < arrayCallbacksByName.length; i++) {
                    if (arrayCallbacksByName[i].targetName === targetName && (cb == null || cb === arrayCallbacksByName[i].callback)) {
                        arrayCallbacksByName.splice(i, 1);
                        i--;
                    }
                }
            } else {
                index = callbacks.indexOf(callbacks[name]);

                if (index !== -1) {
                    callbacks.splice(index, 1);
                }
            }
        };

        service.setStateInfo = function (needShow) {
            needInfoShow = needShow;
        };

        let timerShowSuccess;
        service.showInfoWithDebounce = function (data) {
            if (needInfoShow === true) {
                if (timerShowSuccess != null) {
                    clearTimeout(timerShowSuccess);
                }

                timerShowSuccess = setTimeout(function () {
                    service.showInfo(data);
                }, 700);
            }
        };

        service.showInfo = function () {
            $document[0].addEventListener('click', clickout);

            var html =
                'В корзине ' +
                cart.Count +
                ' на сумму ' +
                cart.TotalPrice +
                '<div class="flex center-xs m-t-xs m-b-xs">' +
                '<div class="col-xs-6"><a class="btn btn-small btn-expander btn-action " href="./cart">' +
                $translate.instant('Js.Cart.Cart') +
                '</a></div>' +
                (cart.ShowConfirmButtons
                    ? '<div class="col-xs-6"><a class="btn btn-small btn-expander btn-confirm" href="./' +
                      (cart.MobileIsFullCheckout ? 'checkout' : 'checkoutmobile') +
                      '">' +
                      $translate.instant('Js.Cart.Checkout') +
                      '</a></div>'
                    : '') +
                '</div>';

            SweetAlert.info(null, {
                title: null,
                html: html,
                position: 'top',
                grow: 'row',
                icon: null,
                padding: '8px 0',
                customClass: {
                    container: 'mobile-cart-popover-container',
                    popup: 'mobile-cart-popover cs-br-1',
                },
                buttonsStyling: false,
                showCloseButton: false,
                showCancelButton: false,
                showConfirmButton: false,
                timer: 5000,
                toast: true,
            })
                .then(function (result) {
                    $document[0].removeEventListener('click', clickout);

                    if (result.value === true) {
                        $window.location.assign('./cart');
                    }
                })
                .catch(function (error) {
                    throw new Error(error);
                });
        };

        function clickout(e) {
            if (domService.closest(e.target, '.swal2-popup') == null) {
                Sweetalert2.close();
            }
        }

        service.findInCart = function (productId, offerId, customOptions) {
            if (!cart.CartProducts) {
                return;
            }
            let products = cart.CartProducts.filter((x) => x.ProductId === productId);

            if (offerId === null && customOptions === null && products.length === 1) return products[0];

            if (offerId) {
                products = products.filter((x) => x.OfferId === offerId);
            }

            if (customOptions) {
                products = products.filter(
                    (product) =>
                        (product.SelectedOptions ?? []).length === customOptions.length &&
                        customOptionsService.isEqualCustomOptions(
                            product.SelectedOptions ?? [],
                            customOptionsService.customOptionItemToEvaluatedCustomOptionsMapper(customOptions),
                        ),
                );
            }
            return products[0];
        };
    };

    angular.module('cart').service('cartService', cartService);
})(angular);
