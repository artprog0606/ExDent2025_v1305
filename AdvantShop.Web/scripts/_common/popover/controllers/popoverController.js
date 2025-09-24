/* @ngInject */
function PopoverCtrl($q, $element, $window, $timeout, popoverService, popoverConfig) {
    var ctrl = this;

    ctrl.$onInit = function () {
        var popoverShowOnLoad = ctrl.popoverShowOnLoad(),
            popoverOverlayEnabled = ctrl.popoverOverlayEnabled(),
            popoverIsFixed = ctrl.popoverIsFixed(),
            popoverIsCanHover = ctrl.popoverIsCanHover();

        ctrl.popoverShowOnLoad = popoverShowOnLoad != null ? popoverShowOnLoad : popoverConfig.popoverShowOnLoad;
        ctrl.popoverOverlayEnabled = popoverOverlayEnabled != null ? popoverOverlayEnabled : popoverConfig.popoverOverlayEnabled;
        ctrl.popoverIsFixed = popoverShowOnLoad != null ? popoverIsFixed : popoverConfig.popoverIsFixed;
        ctrl.popoverIsCanHover = popoverIsCanHover != null ? popoverIsCanHover : popoverConfig.popoverIsCanHover;
        ctrl.popoverShowOne = ctrl.popoverShowOne != null ? ctrl.popoverShowOne : popoverConfig.popoverShowOne;
        ctrl.popoverShowDelay = ctrl.popoverShowDelay != null ? ctrl.popoverShowDelay : popoverConfig.popoverShowDelay;
        ctrl.popoverShowCross = ctrl.popoverShowCross != null ? ctrl.popoverShowCross : popoverConfig.popoverShowCross;

        popoverService.addStorage(ctrl.id, ctrl);
    };

    ctrl.updatePosition = function (targetElement) {
        ctrl.position = popoverService.getPosition($element[0], targetElement || ctrl.controlElement[0], ctrl.popoverPosition, ctrl.popoverIsFixed);
    };

    ctrl.active = function (targetElement) {
        if (ctrl.popoverShowOne === true && $window.localStorage.getItem(ctrl.id)) {
            return $q.resolve('Popover show is one');
        }
        return $timeout(function () {
            ctrl.popoverIsShow = true;
            $element[0].classList.add('active');
            if (!ctrl.popoverCustomStyles) {
                ctrl.updatePosition(targetElement);
                ctrl.popoverPosition = ctrl.position.position;
            }

            if (ctrl.popoverOverlayEnabled === true) {
                popoverService.showOverlay(ctrl.id);
            }
            if (ctrl.popoverOnOpen) {
                ctrl.popoverOnOpen();
            }
        }, ctrl.popoverShowDelay);
    };

    ctrl.deactive = function () {
        ctrl.popoverIsShow = false;
        $element[0].classList.remove('active');
        if (ctrl.popoverShowOne === true) {
            $window.localStorage.setItem(ctrl.id, true);
        }

        if (ctrl.popoverOverlayEnabled === true) {
            popoverService.getPopoverOverlay().then(function (overlayScope) {
                overlayScope.overlayHide();
            });
        }

        if (ctrl.popoverOnClose) {
            ctrl.popoverOnClose();
        }
    };

    ctrl.toggle = function () {
        if (ctrl.popoverIsShow === true) {
            ctrl.deactive();
        } else {
            ctrl.active();
        }
    };

    ctrl.getClasses = function () {
        var result = [];

        result.push('adv-popover-position-' + ctrl.popoverPosition);

        if (ctrl.popoverIsFixed === true) {
            result.push('adv-popover-fixed');
        }

        return result;
    };
}

export default PopoverCtrl;
