/*@ngInject*/
function MobileMenuItemCtrl($attrs, $compile, $q, $http, $scope) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.id = $attrs.mobileMenuItem;
        ctrl.isSelected = false;
        ctrl.hasChild = $attrs.hasChild != null && $attrs.hasChild === 'true';
        ctrl.isLoadedChilds = false;
        ctrl.clickInProgress = false;
        ctrl.parentMobileMenu.addMenuItem(ctrl, ctrl.isSelected);

        ctrl.mobileMenuRoot.addMobileMenuItem(ctrl.id, ctrl.click);
    };

    ctrl.addMenuChild = function (menuChild) {
        ctrl.menuChild = menuChild;
    };

    ctrl.click = function (event) {
        if (ctrl.clickInProgress) {
            return;
        }

        event.stopPropagation();

        if (ctrl.menuChild != null) {
            ctrl.clickInProgress = true;
            if (ctrl.menuChild.checkOpen() === true) {
                ctrl.menuChild.back().finally(() => (ctrl.clickInProgress = false));
            } else {
                ctrl.menuChild
                    .open()
                    .then(
                        ctrl.isLoadedChilds === false && ctrl.menuChild.list != null && ctrl.menuChild.list.length > 0 ? ctrl.loadChilds : $q.resolve,
                    )
                    .finally(() => (ctrl.clickInProgress = false));
            }
        }
    };

    ctrl.loadChilds = function () {
        return $http
            .get('./mobile/catalog/catalogmenu', { params: { categoryId: ctrl.id } })
            .then(function (response) {
                var menuSubmenu = angular.element(response.data);

                menuSubmenu.attr('is-open', 'true');

                ctrl.menuChild.$element.replaceWith(menuSubmenu);

                $compile(menuSubmenu)($scope);

                return menuSubmenu;
            })
            .finally(function () {
                ctrl.isLoadedChilds = true;
            });
    };
}

export default MobileMenuItemCtrl;
