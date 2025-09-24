import templateUrl from './templates/fivePostRateDeliverySLReference.html';
(function (ng) {
    'use strict';

    var FivePostRateDeliverySLReferenceCtrl = function () {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.mappedData = [];
            ctrl.deliverySlList = JSON.parse(ctrl.deliverySlJson);
            var rateDeliverySlReference = [];
            if (ctrl.references) {
                rateDeliverySlReference = ctrl.references
                    .split(';')
                    .map(function (x) {
                        var arr = x.split(':');
                        return {
                            rateCode: arr[0],
                            deliverySlList: arr[1].split(','),
                        };
                    });
            }

            ctrl.mappedData = [];

            if (ctrl.rates) {
                ctrl.rates.forEach(function (rate) {
                    var rateDeliverySl = rateDeliverySlReference.find(x => x.rateCode == rate.Value);
                    ctrl.mappedData.push(rateDeliverySl == null || rateDeliverySl.deliverySlList == null
                        ? []
                        : rateDeliverySl.deliverySlList.filter(deliverySlCode => ctrl.deliverySlList.some(deliverySl => deliverySl.Value == deliverySlCode)));
                });
            }
        };
        ctrl.getReference = function () {
            var referenceList = [];
            for (var index in ctrl.mappedData) {
                var deliverySl = ctrl.mappedData[index];
                if (deliverySl.length == 0)
                    continue;
                referenceList.push(ctrl.rates[index].Value + ':' + deliverySl.join(','));
            }
            return referenceList.join(';');
        };
    };
    FivePostRateDeliverySLReferenceCtrl.$inject = [];
    ng.module('shippingMethod')
        .controller('FivePostRateDeliverySLReferenceCtrl', FivePostRateDeliverySLReferenceCtrl)
        .component('fivePostRateDeliverySLReference', {
            templateUrl: templateUrl,
            controller: 'FivePostRateDeliverySLReferenceCtrl',
            bindings: {
                onInit: '&',
                deliverySlJson: '@',
                references: '@',
                rates: '=',
            },
        });
})(window.angular);
