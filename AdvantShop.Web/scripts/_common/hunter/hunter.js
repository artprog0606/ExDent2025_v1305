import { PubSub } from '../PubSub/PubSub.js';

// Tracking events
// Trigger example: PubSub.publish("add_to_cart")
// Trigger example for Modules: window.PubSub.publish("add_to_cart")

PubSub.subscribe('add_to_cart', function (url) {
    /* eslint-disable no-empty */
    try {
        var path = url.indexOf('products/') != -1 ? url.split('products/')[1] : window.location.pathname.replace('products/', '').replace('/', '');

        // virtual page view '/addtocart/some-product-name'
        if (typeof ga != 'undefined') {
            ga('send', 'pageview', '/addtocart/' + path);
            ga('send', 'event', 'Advantshop_events', 'addToCart', document.URL);
        }

        if (typeof dataLayer != 'undefined') {
            dataLayer.push({ event: 'addToCart' });
        }
    } catch (err) {}
});

PubSub.subscribe('order.add', function () {
    /* eslint-disable no-empty */
    try {
        if (typeof ga != 'undefined') {
            ga('send', 'event', 'Advantshop_events', 'order', document.URL);
        }

        if (typeof dataLayer != 'undefined') {
            dataLayer.push({ event: 'order' });
        }
    } catch (err) {}
});

PubSub.subscribe('buy_one_click_pre', function () {
    /* eslint-disable no-empty */
    try {
        if (typeof ga != 'undefined') {
            ga('send', 'event', 'Advantshop_events', 'buyOneClickForm', document.URL);
        }

        if (typeof dataLayer != 'undefined') {
            dataLayer.push({ event: 'buyOneClickForm' });
        }
    } catch (err) {}
});

PubSub.subscribe('buy_one_click_confirm', function () {
    /* eslint-disable no-empty */
    try {
        if (typeof ga != 'undefined') {
            ga('send', 'event', 'Advantshop_events', 'buyOneClickConfirm', document.URL);
        }

        if (typeof dataLayer != 'undefined') {
            dataLayer.push({ event: 'buyOneClickConfirm' });
        }
    } catch (err) {}
});

PubSub.subscribe('compare.add', function () {
    /* eslint-disable no-empty */
    try {
        if (typeof ga != 'undefined') {
            ga('send', 'event', 'Advantshop_events', 'addToCompare', document.URL);
        }

        if (typeof dataLayer != 'undefined') {
            dataLayer.push({ event: 'addToCompare' });
        }
    } catch (err) {}
});

PubSub.subscribe('add_to_wishlist', function () {
    /* eslint-disable no-empty */
    try {
        if (typeof ga != 'undefined') {
            ga('send', 'event', 'Advantshop_events', 'addToWishlist', document.URL);
        }

        if (typeof dataLayer != 'undefined') {
            dataLayer.push({ event: 'addToWishlist' });
        }
    } catch (err) {}
});

PubSub.subscribe('send_feedback', function () {
    /* eslint-disable no-empty */
    try {
        if (typeof ga != 'undefined') {
            ga('send', 'event', 'Advantshop_events', 'sendFeedback', document.URL);
        }

        if (typeof dataLayer != 'undefined') {
            dataLayer.push({ event: 'sendFeedback' });
        }
    } catch (err) {}
});

PubSub.subscribe('send_preorder', function () {
    /* eslint-disable no-empty */
    try {
        if (typeof ga != 'undefined') {
            ga('send', 'event', 'Advantshop_events', 'sendPreOrder', document.URL);
        }

        if (typeof dataLayer != 'undefined') {
            dataLayer.push({ event: 'sendPreOrder' });
        }
    } catch (err) {}
});

PubSub.subscribe('add_response', function () {
    /* eslint-disable no-empty */
    try {
        if (typeof ga != 'undefined') {
            ga('send', 'event', 'Advantshop_events', 'addResponse', document.URL);
        }

        if (typeof dataLayer != 'undefined') {
            dataLayer.push({ event: 'addResponse' });
        }
    } catch (err) {}
});

PubSub.subscribe('module_callback', function () {
    /* eslint-disable no-empty */
    try {
        if (typeof ga != 'undefined') {
            ga('send', 'event', 'Advantshop_events', 'getCallBack', document.URL);
        }

        if (typeof dataLayer != 'undefined') {
            dataLayer.push({ event: 'getCallBack' });
        }
    } catch (err) {}
});

PubSub.subscribe('callback', function () {
    /* eslint-disable no-empty */
    try {
        if (typeof ga != 'undefined') {
            ga('send', 'event', 'Advantshop_events', 'callBack', document.URL);
        }

        if (typeof dataLayer != 'undefined') {
            dataLayer.push({ event: 'callBack' });
        }
    } catch (err) {}
});

PubSub.subscribe('subscribe.email', function () {
    /* eslint-disable no-empty */
    try {
        if (typeof ga != 'undefined') {
            ga('send', 'event', 'Advantshop_events', 'subscribeNews', document.URL);
        }

        if (typeof dataLayer != 'undefined') {
            dataLayer.push({ event: 'subscribeNews' });
        }
    } catch (err) {}
});

PubSub.subscribe('callback_request', function () {
    /* eslint-disable no-empty */
    try {
        if (typeof ga != 'undefined') {
            ga('send', 'event', 'Advantshop_events', 'callBackRequest', document.URL);
        }

        if (typeof dataLayer != 'undefined') {
            dataLayer.push({ event: 'callBackRequest' });
        }
    } catch (err) {}
});

PubSub.subscribe('user.registration', function () {
    /* eslint-disable no-empty */
    try {
        if (typeof ga != 'undefined') {
            ga('send', 'event', 'Advantshop_events', 'userRegistration', document.URL);
        }

        if (typeof dataLayer != 'undefined') {
            dataLayer.push({ event: 'userRegistration' });
        }
    } catch (err) {}
});
