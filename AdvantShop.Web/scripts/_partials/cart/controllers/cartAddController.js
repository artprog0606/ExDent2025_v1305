import {PubSub} from '../../../_common/PubSub/PubSub.js';

(function (ng) {
    'use strict';

    var cartToolbar, timerPopoverHide;
    /*@ngInject*/
    /** @type{CartAddControllerFnType}
     *  @this{CartAddController}
     * */
    var CartAddCtrl = function (
        $document,
        $scope,
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
        toaster,
        customOptionsService,
    ) {
        var ctrl = this;
        ctrl.parseAttributes = function () {
            ctrl.data = {
                cartAddValid: $attrs.cartAddValid != null ? $parse($attrs.cartAddValid) : null,
                maxStepSpinbox: $attrs.maxStepSpinbox,
                minStepSpinbox: $attrs.minStepSpinbox,
                stepSpinbox: $attrs.stepSpinbox,
                href: $attrs.href,
                mode: $attrs.mode,
                lpId: $attrs.lpId,
                lpUpId: $attrs.lpUpId,
                lpEntityId: $attrs.lpEntityId,
                lpEntityType: $attrs.lpEntityType,
                lpBlockId: $attrs.lpBlockId,
                lpButtonName: $attrs.lpButtonName,
                hideShipping: $attrs.hideShipping,
                source: $attrs.source,
                modeFrom: $attrs.modeFrom,
                offerId: $attrs.offerId != null ? $parse($attrs.offerId)($scope) : null,
                productId: $attrs.productId != null ? $parse($attrs.productId)($scope) : null,
                amount: $attrs.amount != null ? $parse($attrs.amount)($scope) : null,
                attributesXml: $attrs.attributesXml != null ? $parse($attrs.attributesXml)($scope) : null,
                payment: $attrs.payment != null ? $parse($attrs.payment)($scope) : null,
                forceHiddenPopup: $parse($attrs.forceHiddenPopup)($scope),
                offerIds: $attrs.offerIds != null ? $parse($attrs.offerIds)($scope) : [],
                cartAddType: $parse($attrs.cartAddType)($scope),
            };

            $scope.$watch($attrs.cartAddType, (newVal, oldVal) => {
                if (newVal === oldVal) return;
                ctrl.data.cartAddType = newVal;
                ctrl.refresh();
            });

            $scope.$watch($attrs.offerId, (newVal, oldVal) => {
                if (newVal === oldVal) return;
                ctrl.data.offerId = newVal;
                ctrl.refresh();
            });

            $scope.$watch($attrs.productId, (newVal, oldVal) => {
                if (newVal === oldVal) return;

                ctrl.data.productId = newVal;
                ctrl.refresh();
            });

            $scope.$watch($attrs.amount, (newVal, oldVal) => {
                if (newVal === oldVal) return;

                ctrl.data.amount = newVal;
                ctrl.refresh();
            });

            $scope.$watch($attrs.attributesXml, (newVal, oldVal) => {
                if (newVal === oldVal) return;

                ctrl.data.attributesXml = newVal;
            });

            $scope.$watch($attrs.payment, (newVal, oldVal) => {
                if (newVal === oldVal) return;

                ctrl.data.payment = newVal;
                ctrl.refresh();
            });

            $scope.$watchCollection($attrs.offerIds, (newVal, oldVal) => {
                if (newVal === oldVal) return;

                ctrl.data.offerIds = newVal;
                ctrl.refresh();
            });

            $attrs.$observe('href', (newVal, oldVal) => {
                if (newVal === oldVal) return;

                ctrl.data.href = newVal;
            });
        };

        ctrl.checkSizeAndColor = function (sizeOrColor, type, additionalData) {
            if (additionalData.offer == null || additionalData.offer.ProductId !== ctrl.data.productId) return;
            ctrl.productCartData = cartService.findInCart(ctrl.data.productId, additionalData.offer.OfferId, ctrl.productSelectedOptions);
        };

        ctrl.$onInit = function () {
            ctrl.parseAttributes();

            if (ctrl.data.source != null && ctrl.data.source === 'mobile') {
                cartService.setStateInfo(true);
            }

            ctrl.needAdd = true;
            ctrl.isLoading = false;
            ctrl.updateStateButton();
            ctrl.refresh();

            PubSub.subscribe('cart.updateAmount', () => ctrl.refresh());
            PubSub.subscribe('cart.remove', (offerId) => {
                if (offerId === ctrl.data.offerId || ctrl.data.offerId === 0) {
                    ctrl.refresh();
                }
            });

            PubSub.subscribe('cart.clear', () => ctrl.refresh());
            PubSub.subscribe('product.customOptions.change', ({productId, offerId, items}) => {
                if (offerId != null && ctrl.data.offerId !== offerId) return;
                if (productId !== ctrl.data.productId) return;
                ctrl.productSelectedOptions = customOptionsService.getSelectedOptions(items);
                ctrl.productCartData = cartService.findInCart(ctrl.data.productId, ctrl.data.offerId, ctrl.productSelectedOptions);

                if (ctrl.productCartData == null) {
                    ctrl.needAdd = true;
                }

                ctrl.updateStateButton();
            });
        };

        ctrl.addItem = (event) => {
            event.preventDefault();

            var isValid = ctrl.data.cartAddValid == null || ctrl.data.cartAddValid($scope);

            if (isValid === false) {
                return;
            }

            if (ctrl.state === cartConfig.cartStateButton.loading) {
                return $q.resolve(null);
            }
            ctrl.isLoadingAdd = true;

            ctrl.updateStateButton();

            return cartService
                .addItem(ctrl.data)
                .then(function (resultAdd) {
                    const status = resultAdd[0].status;
                    if (status === 'redirect') {
                        if (resultAdd[0].url != null && resultAdd[0].url.length > 0) {
                            $window.location.assign(resultAdd[0].url);
                        } else {
                            $window.location.assign(ctrl.data.href);
                        }
                    } else if (status === 'fail') {
                        toaster.pop('error', '', $translate.instant('Js.CartAdd.Fail'));
                    } else {
                        PubSub.publish('add_to_cart', ctrl.data.href);
                        PubSub.publish(
                            'cart.add',
                            ctrl.data.offerId,
                            ctrl.data.productId,
                            ctrl.data.amount,
                            ctrl.data.attributesXml,
                            resultAdd['0'].cartId,
                            event.target,
                        );
                        PubSub.publish('cart.addv2', ctrl.data.productId, resultAdd['0'].cartId, resultAdd['0'].CartItem, event.target);
                        moduleService.update(['minicartmessage', 'fullcartmessage']).then(ctrl.popoverModule);
                    }
                    ctrl.refresh(true);
                    return resultAdd;
                })
                .then(function (result) {
                    if (ctrl.data.source != null && ctrl.data.source === 'mobile' && result[0].status !== 'redirect') {
                        cartService.showInfoWithDebounce();
                    }

                    return result;
                })
                .finally((result) => {
                    ctrl.isLoadingAdd = false;
                    ctrl.isLoading = false;
                    ctrl.updateStateButton();
                    return result;
                });
        };
        ctrl.isLoadingAll = function () {
            ctrl.isLoadingAllTotal = Boolean(ctrl.isLoading) || Boolean(ctrl.isLoadingAdd);
            return ctrl.isLoadingAllTotal;
        };

        ctrl.isEmptyProductCart = function () {
            return ctrl.productCartData == null || ctrl.productCartData.Amount === 0;
        };

        ctrl.getStateButton = function () {
            let result = null;
            if (ctrl.isLoadingAll()) {
                result = cartConfig.cartStateButton.loading;
            } else if (!ctrl.isEmptyProductCart() && ctrl.data.cartAddType === cartConfig.cartAddType.WithSpinbox) {
                result = cartConfig.cartStateButton.update;
            } else if (
                ctrl.needAdd ||
                ctrl.data.cartAddType == null ||
                ctrl.data.cartAddType === '' ||
                ctrl.data.cartAddType === cartConfig.cartAddType.Classic
            ) {
                result = cartConfig.cartStateButton.add;
            }
            return result;
        };
        ctrl.updateStateButton = function () {
            ctrl.state = ctrl.getStateButton() ?? ctrl.state;
        };
        ctrl.updateAmount = function (value, itemId) {
            if (ctrl.state === cartConfig.cartStateButton.loading) {
                return $q.resolve(null);
            }

            var item = {
                Key: itemId,
                Value: value,
            };
            ctrl.isLoading = true;

            if (ctrl.isEmptyProductCart()) {
                ctrl.updateStateButton();

                const removedProductOfferId = ctrl.productCartData.OfferId;
                return cartService
                    .removeItem(ctrl.productCartData.ShoppingCartItemId)
                    .then(() => {
                        moduleService.update('fullcartmessage');
                        return ctrl.refresh();
                    })
                    .then(() => {
                        PubSub.publish('cart.remove', removedProductOfferId);
                    })
                    .finally(() => {
                        ctrl.needAdd = true;
                        ctrl.isLoading = false;
                        ctrl.updateStateButton();
                    });
            } else {
                return cartService
                    .updateAmount([item])
                    .then(() => {
                        moduleService.update('minicartmessage');
                        return ctrl.refresh();
                    })
                    .then(() => {
                        PubSub.publish('cart.updateAmount');
                    })
                    .finally(() => {
                        ctrl.isLoading = false;
                        ctrl.updateStateButton();
                    });
            }
        };

        ctrl.refresh = function (cache) {
            return cartService.getData(cache).then(function (data) {
                ctrl.cartData = data;
                if (ctrl.cartData.CartProducts.length === 0) {
                    ctrl.productCartData = null;
                    ctrl.needAdd = true;
                    ctrl.updateStateButton();

                    return null;
                }

                const item = cartService.findInCart(
                    ctrl.data.productId,
                    ctrl.data.offerId === 0 ? null : ctrl.data.offerId,
                    ctrl.productSelectedOptions,
                );
                if (item) {
                    ctrl.productCartData = item;
                    ctrl.needAdd = false;
                } else {
                    ctrl.productCartData = null;
                    ctrl.needAdd = true;
                }
                ctrl.updateStateButton();
                return data
            });
        };
        ctrl.popoverModule = function (content) {
            if (moduleService.getModule('minicartmessage') != null && content[0].trim().length > 0) {
                $timeout(function () {
                    popoverService.getPopoverScope('popoverCartToolbar').then(function (popoverScope) {
                        cartToolbar = cartToolbar || document.getElementById('cartToolbar');

                        popoverScope.active(cartToolbar);

                        popoverScope.updatePosition(cartToolbar);

                        if (timerPopoverHide != null) {
                            $timeout.cancel(timerPopoverHide);
                        }

                        timerPopoverHide = $timeout(function () {
                            popoverScope.deactive();
                        }, 5000);
                    });
                }, 0);
            }
        };
    };

    angular.module('cart').controller('CartAddCtrl', CartAddCtrl);
})(window.angular);
