(function (ng) {
    'use strict';

    var ModalTestShippingCalculationCtrl = function ($uibModalInstance, $http, $timeout, toaster) {
        var ctrl = this;
        var timerProcessAddress;
        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.preOrder = {};
            ctrl.item = {};
            if (params) {
                ctrl.shippingId = params.shippingId;
                ctrl.currency = params.currency;
                ctrl.iso3 = params.iso3;
                ctrl.useWeight = params.useWeight;
                ctrl.useCargo = params.useCargo;
                ctrl.useExtracharge = params.useExtracharge;
                ctrl.preOrder.CountryDest = 'Россия';
                ctrl.preOrder.RegionDest = 'Москва';
                ctrl.preOrder.CityDest = 'Москва';
                ctrl.preOrder.Zip = '101000';
                ctrl.item.Amount = 1;
                ctrl.item.Price = 100;
                ctrl.item.Weight = 1;
                ctrl.item.Length = 10;
                ctrl.item.Width = 10;
                ctrl.item.Height = 10;
                ctrl.marginRuleEnabled = false;
                ctrl.extrachargeCargoEnabled = false;
                ctrl.geoRuleEnabled = false;
            }
        };

        ctrl.calculate = function () {
            $http
                .post('shippingMethods/testShippingCalculate', {
                    id: ctrl.shippingId,
                    item: ctrl.item,
                    iso3: ctrl.iso3,
                    preOrder: ctrl.preOrder,
                    extrachargeCargoEnabled: ctrl.extrachargeCargoEnabled,
                    marginEnabled: ctrl.marginRuleEnabled,
                    geoEnabled: ctrl.geoRuleEnabled,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result) ctrl.ShippingOptions = data.obj || [];
                    else {
                        if (data.errors)
                            data.errors.forEach(function (error) {
                                toaster.pop('error', error);
                            });
                        else toaster.pop('error', 'Ошибка при расчете доставки');
                        ctrl.ShippingOptions = [];
                    }
                    ctrl.btnSleep = false;
                });
        };

        ctrl.processCity = function (zone) {
            if (timerProcessAddress != null) {
                $timeout.cancel(timerProcessAddress);
            }

            return (timerProcessAddress = $timeout(
                function () {
                    if (zone != null) {
                        ctrl.preOrder.CountryDest = zone.Country;
                        ctrl.preOrder.RegionDest = zone.Region;
                        ctrl.preOrder.District = zone.District;
                        ctrl.preOrder.Zip = zone.Zip;
                    }
                },
                zone != null ? 0 : 300,
            ));
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalTestShippingCalculationCtrl.$inject = ['$uibModalInstance', '$http', '$timeout', 'toaster'];

    ng.module('uiModal').controller('ModalTestShippingCalculationCtrl', ModalTestShippingCalculationCtrl);
})(window.angular);
