/* @ngInject */
function ShippingVariantsCtrl($http, zoneService) {
    var ctrl = this;

    ctrl.$postLink = function () {
        if (ctrl.type === 'display') {
            ctrl.getData(ctrl.startOfferId, ctrl.startAmount, ctrl.startSvCustomOptions, ctrl.zip);
        }
        if (ctrl.type === 'none') {
            ctrl.getData(ctrl.startOfferId, ctrl.startAmount, ctrl.startSvCustomOptions, ctrl.zip);
        }

        if (ctrl.type === 'Always') {
            ctrl.getData(ctrl.startOfferId, ctrl.startAmount, ctrl.startSvCustomOptions, ctrl.zip);
        }

        zoneService.addCallback('set', function (data) {
            ctrl.zip = data.Zip;
            ctrl.getData(ctrl.offerId, ctrl.amount != null ? ctrl.amount : ctrl.startAmount, ctrl.svCustomOptions, ctrl.zip);
        });

        ctrl.initFn({ shippingVariants: ctrl });
    };

    ctrl.getData = function (offerId, amount, customOptions, zip) {
        if (offerId == null || amount == null) {
            return null;
        }

        ctrl.isProgress = true;

        return $http
            .post('productExt/getshippings', {
                offerId: offerId,
                amount: amount,
                customOptions: customOptions,
                zip: zip,
                rnd: Math.random(),
            })
            .then(function (response) {
                if (response.data != null) {
                    ctrl.items = response.data.Shippings;
                    if (response.data.AdvancedObj) {
                        ctrl.showZip = response.data.AdvancedObj.ShowZip;
                    }
                } else {
                    ctrl.items = [];
                }
                ctrl.isProgress = false;

                return response.data;
            });
    };

    ctrl.update = function () {
        return ctrl.getData(ctrl.offerId, ctrl.amount, ctrl.svCustomOptions, ctrl.zip).then(function (data) {
            return data;
        });
    };

    ctrl.calcShippings = function () {
        ctrl.type = 'Always';
        ctrl.getData(ctrl.startOfferId, ctrl.startAmount, ctrl.startSvCustomOptions, ctrl.zip);
    };
}
export default ShippingVariantsCtrl;
