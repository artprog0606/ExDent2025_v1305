/* @ngInject */
function OrderPayCtrl() {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.rnd = Math.random();
    };

    ctrl.refresh = function () {
        ctrl.rnd = Math.random();
    };
}
export default OrderPayCtrl;
