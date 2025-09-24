/* @ngInject */
function cartStockInWarehousesCtrl(warehousesService, $scope, cartService) {
    var ctrl = this;

    ctrl.$onInit = function () {
        $scope.$watch('cartStock.warehouses', ctrl.getCartStock);
        cartService.addCallback('update', ctrl.getCartStock);
        cartService.addCallback('remove', ctrl.getCartStock);
    };

    ctrl.getCartStock = function () {
        warehousesService.getCartStockInWarehouses(ctrl.warehouses).then(function (data) {
            ctrl.stockInWarehouses = data.obj;
            ctrl.countOutStock = ctrl.stockInWarehouses.filter((item) => item.OutStock).length;
            ctrl.remainderCountOutStockOf100 = ctrl.countOutStock % 100;
            ctrl.remainderCountOutStockOf10 = ctrl.countOutStock % 10;
        });
    };
}

export default cartStockInWarehousesCtrl;
