/*@ngInject*/
function MobileMenuCtrl($attrs, $element, $timeout, $scope) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.list = [];

        if (ctrl.parentMobileMenuItem != null) {
            ctrl.parentMobileMenuItem.addMenuChild(ctrl);
        }

        ctrl.$element = $element;

        ctrl.isOpen = $attrs.isOpen === 'true';
    };

    ctrl.addMenuItem = function (mobileMenuItem, isSelected) {
        ctrl.list.push(mobileMenuItem);

        if (isSelected === true) {
            ctrl.menuRoot.addMobileMenuSelected(ctrl);
        }
    };

    ctrl.back = function () {
        return ctrl.menuRoot
            .move(false, () => ctrl.toggleSiblings(true))
            .then(function () {
                ctrl.isOpen = false;
                return ctrl;
            });
    };

    ctrl.open = function () {
        ctrl.isOpen = true;
        return ctrl.menuRoot.move(true).then(() => {
            ctrl.toggleSiblings(false);
            return true;
        });
    };

    ctrl.checkOpen = function () {
        return ctrl.isOpen === true;
    };

    ctrl.toggleSiblings = function (isVisible) {
        if (ctrl.parentMobileMenuItem != null && ctrl.parentMobileMenuItem.parentMobileMenu != null) {
            ctrl.parentMobileMenuItem.parentMobileMenu.list.forEach(function (item) {
                if (item !== ctrl.parentMobileMenuItem) {
                    item.isHidden = !isVisible;
                }
            });
        }
    };

    ctrl.showOnLoad = function () {
        var parentMobileMenuItem,
            levelCount = 0;

        ctrl.isOpen = true;

        parentMobileMenuItem = ctrl.parentMobileMenuItem;

        ctrl.toggleSiblings(false);

        while (parentMobileMenuItem != null && parentMobileMenuItem.parentMobileMenu != null) {
            if (parentMobileMenuItem != null && parentMobileMenuItem.parentMobileMenu != null) {
                parentMobileMenuItem.parentMobileMenu.toggleSiblings(false);
            }

            parentMobileMenuItem.parentMobileMenu.isOpen = true;
            parentMobileMenuItem = parentMobileMenuItem.parentMobileMenuItem;
            levelCount += 1;
        }

        ctrl.menuRoot.move(levelCount);
    };
}

export default MobileMenuCtrl;
