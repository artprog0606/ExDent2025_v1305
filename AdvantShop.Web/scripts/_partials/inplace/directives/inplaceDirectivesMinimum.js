/* @ngInject */
function inplaceStartDirective($window, $compile, inplaceService) {
    return {
        restrict: 'A',
        scope: {},
        link: function (scope, element, attrs, ctrl) {
            var isLanding = document.documentElement.classList.contains('js-landing-page');
            var selector =
                '[data-inplace-rich], [data-inplace-modal], [data-inplace-image], [data-inplace-autocomplete], [data-inplace-properties-new], [data-inplace-price], [data-inplace-price-panel], [data-inplace-switch]';

            if (!isLanding) {
                var mq = $window.matchMedia('(min-width: 980px)');
            }

            if (isLanding || (mq != null && mq.matches)) {
                init();
            }

            if (mq != null) {
                mq.addListener(function (result) {
                    result.matches ? init() : destroy();
                });
            }

            function init() {
                var objs = document.querySelectorAll(selector);

                if (objs != null && objs.length > 0) {
                    Array.prototype.slice.call(objs).forEach(function (item) {
                        var _item = angular.element(item);
                        var _scope = _item.scope() || scope;
                        _item.addClass('inplace-initialized');
                        $compile(item)(_scope.$parent.$new());
                    });
                }
            }

            function destroy() {
                inplaceService.destroyAll();

                var objs = document.querySelectorAll(selector);

                if (objs != null && objs.length > 0) {
                    Array.prototype.slice.call(objs).forEach(function (item) {
                        var _item = angular.element(item);
                        _item.removeClass('inplace-initialized');
                    });
                }
            }
        },
    };
}

function inplaceSwitchDirective() {
    return {
        restrict: 'A',
        scope: true,
        controller: 'InplaceSwitchCtrl',
        controllerAs: 'inplaceSwitch',
        bindToController: true,
    };
}

function inplaceProgressDirective() {
    return {
        restrict: 'A',
        scope: {},
        controller: 'InplaceProgressCtrl',
        controllerAs: 'inplaceProgress',
        bindToController: true,
        replace: true,
        template:
            '<div class="inplace-progress icon-spinner-before icon-animate-spin-before" data-ng-if="inplaceProgress.state.show === true"></div>',
    };
}

export { inplaceStartDirective, inplaceSwitchDirective, inplaceProgressDirective };
