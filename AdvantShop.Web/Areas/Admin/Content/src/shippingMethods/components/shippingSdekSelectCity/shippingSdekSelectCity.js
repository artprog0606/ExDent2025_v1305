(function (ng) {
    'use strict';

    var ShippingSdekSelectCityCtrl = function ($http, toaster) {
        var ctrl = this;

        ctrl.findCity = function (val) {
            return $http.get('shippingMethods/findcityforsdek', { params: { q: val } }).then(function (response) {
                return response.data;
            });
        };

        ctrl.selectCity = function (result) {
            if (result != null) {
                ctrl.sdekCityId = result.cityCode;
                ctrl.sdekCityName = result.cityName;
            }
        };
    };

    ShippingSdekSelectCityCtrl.$inject = ['$http', 'toaster'];

    ng.module('shippingMethod')
        .controller('ShippingSdekSelectCityCtrl', ShippingSdekSelectCityCtrl)
        .component('sdekSelectCity', {
            templateUrl: 'sdekSelectCity/tpl.html',
            controller: 'ShippingSdekSelectCityCtrl',
            bindings: {
                sdekCityName: '@',
                sdekCityId: '@',
            },
        });
})(window.angular);
