/* @ngInject */
function PopoverControlCtrl($element, popoverService) {
    var ctrl = this,
        popoverScope;

    ctrl.$onInit = function () {
        popoverService
            .getPopoverScope(ctrl.popoverId)
            .then(function () {
                return popoverService.addControl(ctrl.popoverId, $element[0]);
            })
            .then(function (result) {
                popoverScope = result;
            });
    };

    ctrl.active = function () {
        if (popoverScope != null) {
            popoverScope.active($element[0]);
        }
    };

    ctrl.deactive = function () {
        if (popoverScope != null) {
            popoverScope.deactive();
        }
    };

    ctrl.toggle = function () {
        if (popoverScope != null) {
            popoverScope.toggle();
        }
    };
}

export default PopoverControlCtrl;
