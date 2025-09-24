/* @ngInject */
function checkoutService($http, toaster, $q, cartService) {
    var service = this,
        callbackStorage = {},
        contact;

    service.getContactFromCache = function () {
        return contact;
    };

    service.processContact = function (address) {
        return $http.post('/checkout/CheckoutProcessContactPost', { address: address, rnd: Math.random() }).then(function (response) {
            return response.data;
        });
    };

    service.saveContact = function (address, $httpOptions) {
        return $http.post('/checkout/CheckoutContactPost', { address: address, rnd: Math.random() }, $httpOptions).then(function (response) {
            contact = address;
            return response.data;
        });
    };

    service.getShipping = function (preorderList, typeCalculationVariants) {
        var params = { rnd: Math.random(), typeCalculationVariants };
        if (preorderList != null) {
            params.preorderList = preorderList;
        }

        return $http.post('/checkout/CheckoutShippingJson', params).then(function (response) {
            return response.data;
        });
    };

    service.saveShipping = function (shipping, preorderList, typeCalculationVariants) {
        var params = { shipping: shipping, rnd: Math.random() };
        if (preorderList != null) {
            params.preorderList = preorderList;
        }
        if (typeCalculationVariants != null) {
            params.typeCalculationVariants = typeCalculationVariants;
        }

        return $http.post('/checkout/CheckoutShippingPost', params).then(function (response) {
            return response.data;
        });
    };

    service.saveDeliveryInterval = function (shipping) {
        var params = { shipping: shipping, rnd: Math.random() };

        return $http.post('/checkout/CheckoutDeliveryIntervalPost', params).then(function (response) {
            return response.data;
        });
    };
    service.getPayment = function (preorderList) {
        var params = { rnd: Math.random() };
        if (preorderList != null) {
            params.preorderList = preorderList;
        }

        return $http.post('/checkout/CheckoutPaymentJson', params).then(function (response) {
            return response.data;
        });
    };

    service.savePayment = function (payment, preorderList) {
        var params = { payment: payment, rnd: Math.random() };
        if (preorderList != null) {
            params.preorderList = preorderList;
        }

        return $http.post('/checkout/CheckoutPaymentPost', params).then(function (response) {
            return response.data;
        });
    };

    service.getCheckoutCart = function () {
        return $http.get('/checkout/CheckoutCartJson', { params: { rnd: Math.random() } }).then(function (response) {
            return response.data;
        });
    };

    service.autorizeBonus = function (cardNumber) {
        return $http.post('/checkout/CheckoutBonusAutorizePost', { cardNumber: cardNumber, rnd: Math.random() }).then(function (response) {
            return response.data;
        });
    };

    service.toggleBonus = function (appliedBonuses) {
        return $http.post('/checkout/CheckoutBonusApplyPost', { appliedBonuses: appliedBonuses, rnd: Math.random() }).then(function (response) {
            if (!response.data.result) {
                toaster.pop('error', '', response.data.msg);
            }
            return response.data;
        });
    };

    service.couponApplied = function () {
        return $http.post('/checkout/CheckoutCouponApplied', { irnd: Math.random() }).then(function (response) {
            return response.data;
        });
    };

    service.commentSave = function (message) {
        return $http.post('/checkout/CommentPost', { message: message, rnd: Math.random() }).then(function (response) {
            return response.data;
        });
    };

    service.saveDontCallBack = function (dontCallBack) {
        return $http.post('/checkout/saveDontCallBack', { dontCallBack: dontCallBack, rnd: Math.random() }).then(function (response) {
            return response.data;
        });
    };

    service.saveCountDevices = function (countDevices) {
        return $http.post('/checkout/saveCountDevices', { countDevices: countDevices, rnd: Math.random() }).then(function (response) {
            return response.data;
        });
    };

    service.getCheckoutAttachments = function () {
        return $http.get('/checkout/getCheckoutAttachments').then(function (response) {
            return response.data;
        });
    };

    service.deleteAttachment = function (id) {
        return $http.post('/checkout/deleteAttachment', { id: id }).then(function (response) {
            return response.data;
        });
    };

    service.saveAgreementForNewsletter = function (isAgreeForPromotionalNewsletter) {
        let defer = $q.defer();

        if (isAgreeForPromotionalNewsletter == null) {
            defer.resolve(null);
        } else {
            return $http
                .post('/checkout/saveAgreementForNewsletter', {
                    isAgreeForPromotionalNewsletter: isAgreeForPromotionalNewsletter,
                })
                .then(function (response) {
                    return response.data;
                });
        }
        return defer.promise;
    };

    service.saveNewCustomer = function (customer) {
        return $http.post('/checkout/CheckoutUserPost', { customer: customer, rnd: Math.random() }).then(function (response) {
            return response.data;
        });
    };

    service.saveRecipient = function (customer) {
        return $http.post('/checkout/CheckoutRecipientPost', { customer: customer, rnd: Math.random() }).then(function (response) {
            return response.data;
        });
    };

    service.saveCustomerRequiredFields = function (customer) {
        return $http.post('/checkout/checkoutUserRequiredFieldsPost', { customer: customer, rnd: Math.random() }).then(function (response) {
            return response.data;
        });
    };

    service.saveWantBonusCard = function (wantBonusCard) {
        return $http.post('/checkout/saveWantBonusCard', { wantBonusCard: wantBonusCard, rnd: Math.random() }).then(function (response) {
            return response.data;
        });
    };

    // billing
    service.getBillingPayment = function (orderId) {
        return $http.post('/checkout/BillingPaymentJson', { rnd: Math.random(), orderId: orderId }).then(function (response) {
            return response.data;
        });
    };

    service.getBillingCart = function (orderId) {
        return $http.get('/checkout/BillingCartJson', { params: { orderId: orderId, rnd: Math.random() } }).then(function (response) {
            return response.data;
        });
    };

    service.saveBillingPayment = function (payment, orderId) {
        return $http.post('/checkout/BillingPaymentPost', { payment: payment, orderId: orderId, rnd: Math.random() }).then(function (response) {
            return response.data;
        });
    };

    //events: address, shipping, payment, bonus, coupon, relationshipEnd
    service.addCallback = function (eventName, callback) {
        callbackStorage[eventName] = callbackStorage[eventName] || [];
        callbackStorage[eventName].push(callback);
    };

    service.removeCallback = function (eventName, callback) {
        var index;
        if (callbackStorage[eventName] != null && callbackStorage[eventName].length > 0) {
            index = callbackStorage[eventName].indexOf(callback);

            if (index !== -1) {
                callbackStorage[eventName].splice(index, 1);
            }
        }
    };

    service.processCallbacks = function (eventName, data) {
        if (callbackStorage[eventName] != null) {
            callbackStorage[eventName].forEach(function (fn) {
                fn(data);
            });
        }
    };

    service.receivingMethodSave = function (receivingMethod) {
        return $http.post('/checkout/receivingMethodSave', { receivingMethod: receivingMethod, rnd: Math.random() }).then(function (response) {
            return response.data;
        });
    };

    service.getGeoModeDeliveries = function (typeCalculationVariants) {
        var params = { rnd: Math.random(), typeCalculationVariants };

        return $http.post('/checkout/GetGeoModeDeliveries', params).then(function (response) {
            return response.data;
        });
    };

    service.updateCartAmount = function (items, queryParams) {
        return $http
            .post(
                '/checkout/updateCart',
                angular.extend(
                    {
                        items: items,
                        rnd: Math.random(),
                    },
                    queryParams || {},
                ),
            )
            .then(function (response) {
                cartService.processCallback('update', response.data);
                return cartService.getData(false, queryParams);
            });
    };

    service.removeCartItem = function (shoppingCartItemId, queryParams) {
        return $http
            .post('/checkout/removeFromCart', angular.extend({ itemId: shoppingCartItemId }, queryParams || {}))
            .then(function (response) {
                return cartService.getData(false, queryParams).then(() => response.data);
            })
            .then((data) => {
                cartService.processCallback('remove', data);
                return data;
            });
    };
}
export default checkoutService;
