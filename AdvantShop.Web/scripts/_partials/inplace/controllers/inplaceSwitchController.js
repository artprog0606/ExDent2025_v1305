/* @ngInject */
function InplaceSwitchCtrl($window, inplaceService) {
    var ctrl = this;

    ctrl.change = function (enabled) {
        inplaceService.setEnable(enabled).then(function (result) {
            if (result === true) {
                $window.location.reload();
            }
        });
    };
}
export default InplaceSwitchCtrl;
