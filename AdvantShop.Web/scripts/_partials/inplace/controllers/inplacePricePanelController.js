/* @ngInject */
function InplacePricePanelCtrl($element, $window, inplaceService) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.inplacePrice = inplaceService.getInplacePrice(ctrl.inplacePricePanel);
        ctrl.inplacePrice.panel = {
            element: $element,
            ctrl: ctrl,
        };

        if (ctrl.onInit != null) {
            ctrl.onInit({ callback: ctrl.calcAndSetPositionPanel });
        }
    };

    ctrl.$postLink = function () {
        ctrl.calcAndSetPositionPanel();
    };

    ctrl.calcAndSetPositionPanel = function () {
        var triggerElementRect = ctrl.inplacePrice.elementTrigger.getBoundingClientRect();
        var scrollHeight = Math.max(
            document.body.scrollHeight,
            document.documentElement.scrollHeight,
            document.body.offsetHeight,
            document.documentElement.offsetHeight,
            document.body.clientHeight,
            document.documentElement.clientHeight,
        );

        $element.css({
            bottom: scrollHeight - $window.pageYOffset - triggerElementRect.top, //height bottom panel inplace
            left: triggerElementRect.left,
        });
    };
}

export default InplacePricePanelCtrl;
