/*@ngInject*/
function MobileMenuRootCtrl($element, $q, $scope, domService, sidebarsContainerService) {
    var ctrl = this;
    var level = 0;
    var el = $element[0];
    var mobileMenuSelected;
    var mobileMenuItemDictionary = {};

    ctrl.$onInit = function () {
        ctrl.styles = {};
    };

    ctrl.$postLink = function () {
        if (mobileMenuSelected != null) {
            mobileMenuSelected.showOnLoad();
        }

        $element[0].addEventListener('click', function (event) {
            var trigger = domService.closest(event.target, '[data-mobile-menu-item-trigger]', '[data-mobile-menu]');
            if (trigger != null && mobileMenuItemDictionary[trigger.dataset.mobileMenuItemTrigger]) {
                mobileMenuItemDictionary[trigger.dataset.mobileMenuItemTrigger](event);

                $scope.$digest();
            } else {
                const link = domService.closest(event.target, 'a', $element[0]);
                if (link) {
                    const path = link.href.replace(window.location.href.replace(window.location.hash, ''), '');
                    if (/^\/?#/u.test(path)) {
                        sidebarsContainerService.close();
                    }
                }
            }
        });
    };

    let isMoving = false;

    ctrl.move = function (goToLevel, beforeCallback) {
        if (isMoving === true) {
            return $q.reject('Mobile menu: is moving');
        }

        if (typeof goToLevel === 'number') {
            level = goToLevel;
        } else if (typeof goToLevel === 'boolean') {
            level += goToLevel === true ? 1 : -1;
        }

        if (level < 0) {
            level = 0;
            return $q.reject('Mobile menu: level not possible negative');
        }

        if (beforeCallback != null) {
            beforeCallback();
        }

        let defer = $q.defer();

        var fn = function () {
            el.removeEventListener('transitionend', fn);
            isMoving = false;
            defer.resolve();
        };

        el.addEventListener('transitionend', fn);

        el.parentNode.scrollTo(0, 0);

        var value = -100 * level + '%';
        ctrl.styles.transform = 'translateX(' + value + ')';

        return defer.promise;
    };

    ctrl.addMobileMenuSelected = function (mobileMenu) {
        mobileMenuSelected = mobileMenu;
    };

    ctrl.addMobileMenuItem = function (id, mobileMenuItemClickFn) {
        mobileMenuItemDictionary[id] = mobileMenuItemClickFn;
    };
}

export default MobileMenuRootCtrl;
