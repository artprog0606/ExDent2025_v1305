/* @ngInject */

function OrderProductCtrl(orderProductService) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.isLoaded = false;
        orderProductService
            .getOrderProducts()
            .then(function (orderPrdoducts) {
                ctrl.items = orderPrdoducts;
            })
            .finally(() => (ctrl.isLoaded = true));
    };
}

export default OrderProductCtrl;
