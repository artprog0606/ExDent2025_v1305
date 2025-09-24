export const scrollSpyDirective = /* @ngInject */ ($parse, scrollSpyService, scrollSpyConfig) => {
    return {
        restrict: 'AE',
        controller: /* @ngInject */ function ScrollSpyCtrl($attrs, $element, $scope, scrollSpyService) {
            const ctrl = this;
            let selector;
            ctrl.$postLink = () => {
                selector = $attrs.scrollSpy || $attrs.href;
                ctrl.options = Object.assign({}, scrollSpyConfig, $attrs.scrollSpyOptions != null && $parse($attrs.scrollSpyOptions)($scope));
                const target = getTarget();
                if (target) {
                    const dereg = scrollSpyService.addSpy($element[0], target, ctrl);
                    $element.on('$destroy', () => dereg());
                }
            };

            ctrl.activate = () => scrollSpyService.activate();

            ctrl.deactivate = () => scrollSpyService.deactivate();

            ctrl.setActive = () => {
                scrollSpyService.setActive(getTarget());
            };

            const getTarget = () => document.querySelector(selector);
        },
        controllerAs: 'scrollSpy',
        scope: true,
    };
};
