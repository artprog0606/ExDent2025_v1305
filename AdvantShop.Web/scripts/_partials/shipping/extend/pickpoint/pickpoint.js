(function (ng) {
    'use strict';

    angular.module('pickpoint', []).controller('PickpointCtrl', [
        '$scope',
        'shippingService',
        function ($scope, shippingService) {
            var ctrl = this,
                _callback;

            var pickpointCallback = function (result) {
                ctrl.shipping.PickpointId = result.id;
                ctrl.shipping.PickpointAddress = result.name + ', ' + result.address;
                //эта функция вызывается вне контекста ангуляра, поэтому нужно вызвать $digest
                _callback('pickpointSelect', ctrl.shipping.PickpointId);
                $scope.$digest();
            };

            ctrl.$onInit = function () {
                shippingService.fireTemplateReady($scope);
            };

            ctrl.open = function (shipping, callback) {
                _callback = callback;
                ctrl.shipping = shipping;
                var params = {};
                if (shipping.ShippingType == 'Edost') {
                    params = { city: shipping.Pickpointmap, ids: null };
                }
                if (shipping.ShippingType == 'PickPoint') {
                    params = shipping.WidgetConfigParams;
                }
                PickPoint.open(pickpointCallback, params);
            };
        },
    ]);
})(window.angular);
