(function (ng) {
    'use strict';

    var SettingsWarehousesCtrl = function ($http) {
        var ctrl = this;

        ctrl.initWarehouses = function (defaultWarehouse) {
            ctrl.DefaultWarehouseId = defaultWarehouse.value;
            ctrl.warehouses = [defaultWarehouse];
            ctrl.getWarehouses();
        };

        ctrl.getWarehouses = function () {
            return $http.get('warehouse/getWarehousesList').then(function (response) {
                return (ctrl.warehouses = response.data);
            });
        };

        ctrl.updateWarehouses = function () {
            ctrl.getWarehouses();
        };

        ctrl.onSelectTab = function (indexTab) {
            ctrl.tabActiveIndex = indexTab;
        };
    };

    SettingsWarehousesCtrl.$inject = ['$http'];

    ng.module('settingsWarehouses', ['warehousesList', 'warehouseTypes']).controller('SettingsWarehousesCtrl', SettingsWarehousesCtrl);
})(window.angular);
