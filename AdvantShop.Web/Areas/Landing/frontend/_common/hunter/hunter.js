(function (window, document, $, ng) {
    'use strict';

    function trackYaEvent(target) {
        try {
            var yaCounter = window['yaCounter' + window.yaCounterId];
            yaCounter.reachGoal(target);
        } catch (err) {
            console.warn('tracking yandex: ' + err.message);
        }
    }

    function trackGaEvent(category, action) {
        try {
            if (category == null || category.length == 0) {
                category = 'Advantshop_lp_events';
            }

            if (typeof ga != 'undefined') {
                ga('send', 'event', category, action, document.URL);
            }

            if (typeof dataLayer != 'undefined') {
                dataLayer.push({ event: action, eventCategory: category });
            }
        } catch (err) {
            console.warn('tracking ga: ' + err.message);
        }
    }

    function trackEvent(params) {
        trackGaEvent(params.category, params.action);
        trackYaEvent(params.action);
    }

    var TrackingService = function () {
        var service = this;

        service.trackEvent = trackEvent;
        service.trackYaEvent = trackYaEvent;
        service.trackGaEvent = trackGaEvent;
    };

    var TrackingCtrl = function (trackingService) {
        var ctrl = this;

        ctrl.trackEvent = function () {
            trackingService.trackEvent.apply(ctrl, arguments);
        };
        ctrl.trackYaEvent = function () {
            trackingService.trackYaEvent.apply(ctrl, arguments);
        };
        ctrl.trackGaEvent = function () {
            trackingService.trackGaEvent.apply(ctrl, arguments);
        };
    };

    TrackingCtrl.$inject = ['trackingService'];

    ng.module('tracking', [])
        .service('trackingService', TrackingService)
        .controller('TrackingCtrl', TrackingCtrl)
        .directive('tracking', function () {
            return {
                scope: true,
                controller: 'TrackingCtrl',
                controllerAs: 'tracking',
            };
        });

    document.addEventListener('DOMContentLoaded', function () {
        // Tracking events
        // Trigger example: $(document).trigger("add_to_cart");
        $(document)
            .on('add_to_cart', function (e, url) {
                try {
                    var path =
                        url.indexOf('products/') != -1
                            ? url.split('products/')[1]
                            : window.location.pathname.replace('products/', '').replace('/', '');
                    if (typeof ga != 'undefined') {
                        ga('send', 'pageview', '/addtocart/' + path);
                    }
                } catch (err) {
                    console.warn('tracking: ' + err.message);
                }

                trackEvent({ action: 'addToCart' });
            })
            .on('order.add', function () {
                trackEvent({ action: 'order' });
            })
            .on('buy_one_click_pre', function () {
                trackEvent({ action: 'buyOneClickForm' });
            })
            .on('buy_one_click_confirm', function () {
                trackEvent({ action: 'buyOneClickConfirm' });
            })
            .on('send_feedback', function () {
                trackEvent({ action: 'sendFeedback' });
            })
            .on('send_preorder', function () {
                trackEvent({ action: 'sendPreOrder' });
            })
            .on('add_response', function () {
                trackEvent({ action: 'addResponse' });
            })
            .on('callback', function () {
                trackEvent({ action: 'callBack' });
            })
            .on('callback_request', function () {
                trackEvent({ action: 'callBackRequest' });
            })
            .on('module_callback', function () {
                trackEvent({ action: 'getCallBack' });
            })
            .on('order_from_mobile', function () {
                trackEvent({ action: 'orderFromMobile' });
            });

        $(document).on('cart.add', function (e, offerId, productId, amount, attributesXml, cartId) {
            if (window.dataLayer == null) {
                return false;
            }

            $.ajax({
                type: 'GET',
                async: false,
                dataType: 'json',
                data: {
                    offerId: offerId,
                    productId: productId,
                    cartId: cartId,
                },
                url: 'landingTracking/getProductById',
                success: function (data) {
                    if (data != null && data.artno != null) {
                        window.dataLayer.push({
                            ecommerce: {
                                add: {
                                    products: [
                                        {
                                            id: data.artno,
                                            name: data.name,
                                            price: data.price,
                                            brand: data.brand,
                                            category: data.category,
                                            quantity: amount || data.amount,
                                        },
                                    ],
                                },
                            },
                        });
                    }
                },
            });
        });

        $(document).on('cart.remove', function (e, offerId) {
            if (window.dataLayer == null) {
                return false;
            }

            $.ajax({
                type: 'GET',
                async: false,
                dataType: 'json',
                data: {
                    offerid: offerId,
                },
                url: 'landingTracking/getProductByOfferId',
                success: function (data) {
                    if (data != null && data.artno != null) {
                        window.dataLayer.push({
                            ecommerce: {
                                remove: {
                                    products: [{ id: data.artno, name: data.name }],
                                },
                            },
                        });
                    }
                },
            });
        });
    });
})(window, document, window.jQuery, window.angular);
