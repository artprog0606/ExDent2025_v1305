/* @ngInject */
function InplacePriceButtonsCtrl($element, inplaceService, $window, $document) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.inplacePrice = inplaceService.getInplacePrice(ctrl.inplacePriceButtons);
        ctrl.inplacePrice.buttons = {
            element: $element,
            ctrl: ctrl,
        };

        if (ctrl.onInit != null) {
            ctrl.onInit({ callback: ctrl.calcAndSetPositionButtons });
        }
    };

    ctrl.$postLink = function () {
        ctrl.calcAndSetPositionButtons();
    };

    ctrl.calcAndSetPositionButtons = function () {
        var triggerElementRect = ctrl.inplacePrice.elementTrigger.getBoundingClientRect();
        $element.css({
            top: $window.pageYOffset + triggerElementRect.bottom,
            right: $document[0].body.clientWidth - triggerElementRect.right,
        });
    };

    ctrl.btnSave = function () {
        ctrl.inplacePrice.clickedButtons = true;
        ctrl.inplacePrice.save();
    };

    ctrl.btnCancel = function () {
        ctrl.inplacePrice.clickedButtons = true;
        ctrl.inplacePrice.cancel();
    };
}

export default InplacePriceButtonsCtrl;
