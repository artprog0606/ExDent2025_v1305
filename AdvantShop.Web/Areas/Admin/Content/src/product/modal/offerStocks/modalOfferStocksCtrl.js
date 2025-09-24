(function (ng) {
    'use strict';

    var ModalOfferStocksCtrl = function ($uibModalInstance, $http, productService, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.offerId = params.offerId != null ? params.offerId : 0;
            ctrl.offerInfo = params.offerInfo ? '(' + params.offerInfo + ')' : '';
            ctrl.newQuantity = 1;

            ctrl.getOfferStocks(ctrl.offerId).then(function (data) {
                ctrl.getDataForOfferStocks(ctrl.offerId).then(function () {
                    if (data != null && Array.isArray(data)) {
                        ctrl.stocks = data.map(function (stock) {
                            stock.warehouseName = ctrl.getWarehouseName(stock.WarehouseId);
                            return stock;
                        });
                        ctrl.updateListNotUsedWarehouses();
                        if (ctrl.notUsedWarehouses.length) {
                            ctrl.newWarehouse = ctrl.notUsedWarehouses[0].Id;
                        }
                    }
                });
            });
        };

        ctrl.getOfferStocks = function (offerId) {
            return productService.getOfferStocks(offerId).then(function (data) {
                if (data.result === true) {
                    return data.obj;
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });

                    if (!data.errors) {
                        toaster.pop('error', $translate.instant('Admin.Js.OfferStocks.Error'), $translate.instant('Admin.Js.OfferStocks.Error.Data'));
                    }
                }
                return data;
            });
        };

        ctrl.getDataForOfferStocks = function (offerId) {
            return $http.get('product/getDataForOfferStocks', { params: { offerId: offerId } }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    ctrl.warehouses = data.obj.Warehouses;
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });

                    if (!data.errors) {
                        toaster.pop(
                            'error',
                            $translate.instant('Admin.Js.OfferStocks.Error'),
                            $translate.instant('Admin.Js.OfferStocks.Error.AdditionalData'),
                        );
                    }
                }
                return data;
            });
        };

        ctrl.updateListNotUsedWarehouses = function () {
            ctrl.notUsedWarehouses = ctrl.warehouses.filter(function (warehouse) {
                return ctrl.stocks.every(function (stock) {
                    return stock.WarehouseId !== warehouse.Id;
                });
            });
        };

        ctrl.addStock = function () {
            if (!ctrl.newWarehouse) {
                toaster.error($translate.instant('Admin.Js.OfferStocks.Error.Warehouse'));
                return;
            }
            ctrl.stocks.push({
                OfferId: ctrl.offerId,
                WarehouseId: ctrl.newWarehouse,
                Quantity: ctrl.newQuantity || '0',
                warehouseName: ctrl.getWarehouseName(ctrl.newWarehouse),
            });

            ctrl.updateListNotUsedWarehouses();
            ctrl.newQuantity = 1;
            delete ctrl.newWarehouse;
            if (ctrl.notUsedWarehouses.length) {
                ctrl.newWarehouse = ctrl.notUsedWarehouses[0].Id;
            }
        };

        ctrl.getWarehouseName = function (warehouseId) {
            if (!ctrl.warehouses.length) {
                return;
            }

            var warehouse = ctrl.warehouses.reduce(function (prev, current) {
                if (prev) {
                    return prev;
                }

                if (current.Id == warehouseId) {
                    return current;
                }

                return null;
            }, null);

            return warehouse ? warehouse.Name : null;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function () {
            productService.saveOfferStocks(ctrl.stocks).then(function (data) {
                if (data.result === true) {
                    $uibModalInstance.close(ctrl.stocks);
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });

                    if (!data.errors) {
                        toaster.pop('error', $translate.instant('Admin.Js.OfferStocks.Error'), $translate.instant('Admin.Js.OfferStocks.Error.Save'));
                    }
                }
                return data;
            });
        };
    };

    ModalOfferStocksCtrl.$inject = ['$uibModalInstance', '$http', 'productService', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalOfferStocksCtrl', ModalOfferStocksCtrl);
})(window.angular);
