/* @ngInject */
function InplaceProgressCtrl(inplaceService) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.state = inplaceService.getProgressState();
    };
}

export default InplaceProgressCtrl;
