(function (ng) {
    'use strict';

    var ChangeAdminShopNameCtrl = function ($rootScope, $filter) {
        var ctrl = this;

        ctrl.$onInit = function () {
            $rootScope.$on('adminShopNameUpdated', function (event, data) {
                if (data != null && data.shopName) {
                    ctrl.shopname = data.shopName;
                }
            });
        };

        ctrl.decodeAndSetShopName = function (shopName) {
            if (shopName != null) {
                const decodeStringFilter = $filter('decodeString');
                if (decodeStringFilter) {
                    ctrl.shopname = decodeStringFilter(shopName);
                }
            }
        };

        ctrl.save = function (result) {
            ctrl.shopname = result.name;
        };
    };

    ChangeAdminShopNameCtrl.$inject = ['$rootScope', '$filter'];

    ng.module('changeAdminShopName', []).controller('ChangeAdminShopNameCtrl', ChangeAdminShopNameCtrl);
})(window.angular);
