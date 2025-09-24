import templateUrl from './templates/fivePostWarehouseDeliveryTypeReference.html';
(function (ng) {
    'use strict';

    var FivePostWarehouseDeliveryTypeReference = function ($http, toaster) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.mappedData = [];
            ctrl.deliveryTypes = JSON.parse(ctrl.deliveryTypesJson);
            ctrl.warehouses = JSON.parse(ctrl.warehousesJson);
            ctrl.availableDeliveryTypes = ctrl.deliveryTypes;
            ctrl.availableWarehouses = ctrl.warehouses;
            if (ctrl.references != null && ctrl.references !== '') {
                ctrl.mappedData = ctrl.references
                    .split(';')
                    .filter(function (x) {
                        return x;
                    })
                    .map(function (x) {
                        var arr = x.split(',');
                        return {
                            warehouseId: arr[0],
                            deliveryType: arr[1],
                        };
                    })
                    .filter(function (x) {
                        return ctrl.getDeliveryType(x.deliveryType) && ctrl.getWarehouseName(x.warehouseId);
                    });
            }
        };
        ctrl.newItem = function () {
            ctrl.mappedData.push({
                deliveryType: null,
                warehouseId: null,
            });
        };
        ctrl.removeItem = function (warehouseId) {
            let index = ctrl.mappedData.findIndex(function (element) {
                return element.warehouseId === warehouseId;
            });
            ctrl.mappedData.splice(index, 1);
        };
        ctrl.getReference = function () {
            return ctrl.mappedData
                .map(function (x) {
                    return x.warehouseId + ',' + x.deliveryType;
                })
                .join(';');
        };
        ctrl.getDeliveryType = function (value) {
            return ctrl.deliveryTypes.find((x) => x.Value === value);
        };
        ctrl.getWarehouseName = function (id) {
            var warehouse = ctrl.warehouses.find(function (item) {
                return item.Value === id;
            });
            return warehouse ? warehouse.Text : undefined;
        };
        ctrl.updateAvailableDeliveryTypes = function () {
            ctrl.availableDeliveryTypes = ctrl.deliveryTypes.filter(
                (deliveryType) => ctrl.mappedData.find((item) => item.deliveryType == deliveryType.Value) == null,
            );
        };
        ctrl.updateAvailableWarehouses = function () {
            ctrl.availableWarehouses = ctrl.warehouses.filter(
                (warehouse) => ctrl.mappedData.find((item) => item.warehouseId == warehouse.Value) == null,
            );
        };
    };
    FivePostWarehouseDeliveryTypeReference.$inject = ['$http', 'toaster'];
    ng.module('shippingMethod')
        .controller('FivePostWarehouseDeliveryTypeReference', FivePostWarehouseDeliveryTypeReference)
        .component('fivePostWarehouseDeliveryTypeReference', {
            templateUrl: templateUrl,
            controller: 'FivePostWarehouseDeliveryTypeReference',
            bindings: {
                onInit: '&',
                deliveryTypesJson: '@',
                references: '@',
                warehousesJson: '@',
            },
        });
})(window.angular);
