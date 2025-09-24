import shippingByPriceLimitTemplate from './templates/shippingByPriceLimit.html';
(function (ng) {
    'use strict';

    var ShippingByPriceLimitCtrl = function ($http, toaster) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.ranges = [];
            if (ctrl.priceLimit != null && ctrl.priceLimit !== '') {
                ctrl.ranges = JSON.parse(ctrl.priceLimit);
                ctrl.ranges.sort(compare);
            }
        };
        ctrl.addRange = function () {
            // todo: use globalization for parseFloat
            ctrl.ranges.push({
                OrderPrice: ctrl.OrderPrice,
                Price: ctrl.Price,
                Distance: ctrl.Distance,
                PerUnit: ctrl.PerUnit,
                PriceDistance: ctrl.PriceDistance,
            });
            ctrl.ranges.sort(compare);
            ctrl.updatePriceLimit();
            ctrl.OrderPrice = null;
            ctrl.Price = null;
            ctrl.Distance = 0;
            ctrl.PerUnit = false;
            ctrl.PriceDistance = null;
        };
        ctrl.deleteRange = function (index) {
            ctrl.ranges.splice(index, 1);
            ctrl.updatePriceLimit();
        };
        ctrl.updatePriceLimit = function () {
            ctrl.priceLimit = JSON.stringify(ctrl.ranges);
            ctrl.update = true;
        };
        function compare(a, b) {
            if (a.OrderPrice < b.OrderPrice) return -1;
            if (a.OrderPrice > b.OrderPrice) return 1;
            return 0;
        }
    };
    ShippingByPriceLimitCtrl.$inject = ['$http', 'toaster'];
    ng.module('shippingMethod')
        .controller('ShippingByPriceLimitCtrl', ShippingByPriceLimitCtrl)
        .component('shippingByPriceLimit', {
            templateUrl: shippingByPriceLimitTemplate,
            controller: 'ShippingByPriceLimitCtrl',
            bindings: {
                onInit: '&',
                methodId: '@',
                priceLimit: '@',
                currencyLabel: '@',
            },
        });
})(window.angular);
