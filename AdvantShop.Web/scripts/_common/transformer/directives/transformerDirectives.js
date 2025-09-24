(function (ng) {
    'use strict';

    ng.module('transformer')
        .directive('transformer', [
            '$window',
            '$parse',
            'transformerService',
            function ($window, $parse, transformerService) {
                return {
                    restrict: 'A',
                    controller: 'TransformerCtrl',
                    controllerAs: 'transformer',
                    bindToController: true,
                    scope: true,
                    link: function (scope, element, attrs, ctrl) {
                        //options
                        ctrl.responsiveOptions = $parse(attrs.responsiveOptions)(scope);
                        ctrl.limitPos = attrs.limitPos != null;
                        ctrl.offsetTop = attrs.offsetTop != null ? parseFloat(attrs.offsetTop) : 0;
                        ctrl.sortOrder = attrs.sortOrder != null ? parseFloat(attrs.sortOrder) : null;
                        ctrl.limitVisibleScroll =
                            attrs.limitVisibleScroll != null; /* сначала блок прижимается к низу а потом к верху в зависимоти от скролла*/

                        ctrl.onInit = attrs.onInit != null ? $parse(attrs.onInit) : null;
                        //init media query
                        ctrl.responsiveMediaQuery = ctrl.responsiveOptions != null ? $window.matchMedia(ctrl.responsiveOptions) : null;
                        ctrl.stickyPosition = attrs.stickyPosition != null ? $parse(attrs.stickyPosition)(scope) : 'top'; //variants: top, bottom
                        ctrl.stickyHorizontalPosition =
                            attrs.stickyHorizontalPosition != null ? $parse(attrs.stickyHorizontalPosition)(scope) : 'left';

                        ctrl.backupStylesElementString = element[0].style.cssText;

                        if (attrs.watchEnabled != null) {
                            ctrl.watch = scope.$watch(attrs.watchEnabled, function (newValue, oldValue) {
                                var isFirstCall = newValue === oldValue;

                                if (isFirstCall && newValue === true) {
                                    if (ctrl.checkLoadDocument()) {
                                        ctrl.destroyFn = ctrl.start();
                                    } else {
                                        $window.addEventListener('load', function () {
                                            ctrl.destroyFn = ctrl.start();
                                        });
                                    }
                                } else if (isFirstCall === false) {
                                    ctrl.toggleDestroyOrLoad(newValue);
                                }
                            });
                        } else {
                            if (ctrl.responsiveMediaQuery != null) {
                                ctrl.responsiveMediaQuery.addListener(function (obj) {
                                    if (obj.matches === true) {
                                        if (ctrl.destroyFn != null) {
                                            ctrl.destroyFn();
                                        }
                                        ctrl.destroyFn = ctrl.start();
                                    } else if (ctrl.destroyFn != null) {
                                        ctrl.destroyFn();
                                    }
                                });

                                if (ctrl.responsiveMediaQuery.matches === true) {
                                    ctrl.destroyFn = ctrl.start();
                                } else if (ctrl.destroyFn != null) {
                                    ctrl.destroyFn();
                                }
                            } else {
                                if (ctrl.checkLoadDocument()) {
                                    ctrl.destroyFn = ctrl.start();
                                } else {
                                    $window.addEventListener('load', function () {
                                        ctrl.destroyFn = ctrl.start();
                                    });
                                }
                            }
                        }

                        ctrl.scrollWidth = transformerService.getWidthScroll(); //при включении адаптивного дизайна при ресайзе могут быть коллизии так как в таче и дескотопе разные скроллы в ширину
                    },
                };
            },
        ])
        .directive('transformerMutable', [
            '$window',
            function () {
                return {
                    require: '^transformer',
                    restrict: 'A',
                    link: function (scope, element, attrs, transformerCtrl) {
                        transformerCtrl.addMutableItem(element[0]);
                    },
                };
            },
        ]);
})(window.angular);
