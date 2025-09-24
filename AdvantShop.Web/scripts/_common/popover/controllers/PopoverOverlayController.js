/* @ngInject */
function PopoverOverlayCtrl(popoverService) {
    var ctrl = this;

    ctrl.overlayHide = function () {
        popoverService.getPopoverScope(ctrl.popoverId).then(function (popoverScope) {
            popoverScope.deactive();

            ctrl.popoverId = null;

            ctrl.isVisibleOverlay = false;
        });
    };

    popoverService.addPopoverOverlay(ctrl);
}

export default PopoverOverlayCtrl;
