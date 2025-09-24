import cartStockInWarehousesTemplate from '../templates/cartStockInWarehouses.html';
function cartStockInWarehousesDirective() {
    return {
        restrict: 'AE',
        scope: {
            warehouses: '<',
        },
        controller: 'cartStockInWarehousesCtrl',
        controllerAs: 'cartStock',
        bindToController: true,
        templateUrl: cartStockInWarehousesTemplate,
    };
}

export { cartStockInWarehousesDirective };
