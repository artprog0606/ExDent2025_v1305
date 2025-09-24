/* @ngInject */
function InplaceRichButtonsCtrl($element, inplaceService, $scope, $window, $document) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.richCtrl = inplaceService.getRich(ctrl.inplaceRichButtons);

        ctrl.richCtrl.buttons = {
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
        var triggerElementRect = ctrl.richCtrl.elementTrigger.getBoundingClientRect();
        $element.css({
            top: $window.pageYOffset + triggerElementRect.bottom + 'px',
            right: $document[0].body.clientWidth - triggerElementRect.right + 'px',
        });
    };

    ctrl.btnSave = function () {
        ctrl.richCtrl.clickedButtons = true;
        ctrl.richCtrl.save(ctrl.richCtrl.editor.getData());
    };

    ctrl.btnCancel = function () {
        ctrl.richCtrl.clickedButtons = true;
        ctrl.richCtrl.cancel();
    };

    ctrl.destroy = function () {
        $scope.$destroy();
        $element.remove();
    };
}

export default InplaceRichButtonsCtrl;
