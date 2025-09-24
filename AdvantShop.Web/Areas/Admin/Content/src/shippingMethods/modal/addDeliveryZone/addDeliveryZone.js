import './addDeliveryZone.html';

(function (ng) {
    'use strict';

    var ModalAddDeliveryZoneCtrl = function ($uibModal, $uibModalInstance, $http) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.mode = (ctrl.$resolve.params ? ctrl.$resolve.params.zone : null) ? 'edit' : 'add';
            ctrl.currencyLabel = ctrl.$resolve.params ? ctrl.$resolve.params.currencyLabel : null;
            ctrl.yaMapsApiKey = ctrl.$resolve.params ? ctrl.$resolve.params.yaMapsApiKey : null;
            ctrl.methodId = ctrl.$resolve.params ? ctrl.$resolve.params.methodId : null;
            ctrl.warehousesActive = ctrl.$resolve.params ? ctrl.$resolve.params.warehousesActive : null;
            ctrl.zone = (ctrl.$resolve.params.zone ? ng.extend({}, ctrl.$resolve.params.zone) : null) || {
                Id: Math.floor(Math.random() * 1000000 + 1),
                Name: '',
                Description: '',
                Rate: 0.0,
                DeliveryTime: '',
                Coordinates: [],
                FillColor: '#ed4543',
                FillOpacity: 0.2,
                StrokeColor: '#ed4543',
                StrokeWidth: '5',
                StrokeOpacity: 0.5,
            };

            ctrl.isProgress = true;
            ctrl.getPayments().then(function () {
                ctrl.isProgress = false;
            });
            ctrl.getWarehouses();
        };

        ctrl.getPayments = function () {
            return $http.get('shippingMethods/getPayments', { params: { methodId: ctrl.methodId } }).then(function (response) {
                ctrl.payments = response.data.filter(function (x) {
                    return x.Active === true;
                });
                ctrl.selectedPaymentMethods =
                    ctrl.payments != null
                        ? ctrl.payments
                              .filter(function (x) {
                                  return !ctrl.zone.NotAvailablePayments || !ctrl.zone.NotAvailablePayments.includes(x.PaymentMethodId);
                              })
                              .map(function (x) {
                                  return x.PaymentMethodId;
                              })
                        : null;
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function () {
            ctrl.zone.NotAvailablePayments =
                ctrl.payments != null
                    ? ctrl.payments
                          .filter(function (x) {
                              return !ctrl.selectedPaymentMethods || !ctrl.selectedPaymentMethods.includes(x.PaymentMethodId);
                          })
                          .map(function (x) {
                              return x.PaymentMethodId;
                          })
                    : null;

            $uibModalInstance.close(ctrl.zone);
        };

        ctrl.getWarehouses = function () {
            return $http.get('warehouse/getWarehousesList').then(function (response) {
                return (ctrl.warehouses = response.data);
            });
        };

        ctrl.deleteWarehouse = function (warehouse) {
            var index = ctrl.zone.CheckWarehouses.indexOf(warehouse);

            if (index !== -1) {
                ctrl.zone.CheckWarehouses.splice(index, 1);
            }
        };

        ctrl.selectWarehouses = function (result) {
            if (!ctrl.zone.CheckWarehouses) {
                ctrl.zone.CheckWarehouses = [];
            }

            if (!Array.isArray(result)) {
                ctrl.zone.CheckWarehouses.push(result.warehouseId);
            } else {
                result.forEach(function (item) {
                    ctrl.zone.CheckWarehouses.push(item.WarehouseId);
                });
            }
        };

        ctrl.getWarehouseName = function (id) {
            var warehouse = ctrl.warehouses.find((w) => w.value == id);
            if (warehouse) {
                return warehouse.label;
            }

            return id;
        };
    };

    ModalAddDeliveryZoneCtrl.$inject = ['$uibModal', '$uibModalInstance', '$http'];

    ng.module('uiModal').controller('ModalAddDeliveryZoneCtrl', ModalAddDeliveryZoneCtrl);
})(window.angular);
