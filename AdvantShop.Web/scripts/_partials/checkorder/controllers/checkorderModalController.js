/* @ngInject */
function CheckOrderModalCtrl(checkOrderService, checkOrderData) {
    var ctrl = this;

    ctrl.isLoaded = false;
    ctrl.historyCountVisible = 5;

    checkOrderService
        .getStatus(checkOrderData.orderNumber)
        .then(function (status) {
            ctrl.data = status;
        })
        .finally(function () {
            ctrl.isLoaded = true;
        });
}

export default CheckOrderModalCtrl;
