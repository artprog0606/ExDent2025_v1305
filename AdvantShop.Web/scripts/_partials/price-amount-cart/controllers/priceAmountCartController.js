import { PubSub } from '../../../_common/PubSub/PubSub.js';

/* @ngInject */
function PriceAmountCartCtrl($http, $q) {
    let ctrl = this;

    ctrl.$postLink = function () {
        ctrl.getItems();

        PubSub.subscribe('cart.remove', ctrl.getItems);
        PubSub.subscribe('cart.clear', ctrl.getItems);
        PubSub.subscribe('cart.updateAmount', ctrl.getItems);
    };

    ctrl.getItems = function () {
        $http.get('cart/getPriceAmountNextDiscountItems').then(function (response) {
            ctrl.items = response.data;
        });
    };
}

export default PriceAmountCartCtrl;
