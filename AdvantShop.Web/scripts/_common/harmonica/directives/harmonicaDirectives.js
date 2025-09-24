import tileTemplate from '../templates/tile.html?raw';

(function (ng) {
    'use strict';

    angular.module(`harmonica`).directive(
        `harmonica`,
        /* @ngInject */ function ($window, $document) {
            return {
                restrict: 'EA',
                scope: true,
                controller: 'HarmonicaCtrl',
                controllerAs: 'harmonica',
                bindToController: true,
                compile: function (cElement, cAttrs) {
                    let harmonicaTileTag = 'div';
                    if (cAttrs.harmonicaTileElement) {
                        harmonicaTileTag = cAttrs.harmonicaTileElement;
                    }
                    const harmonicaTileEL = document.createElement(harmonicaTileTag);
                    harmonicaTileEL.setAttribute('data-harmonica-tile', '');
                    harmonicaTileEL.setAttribute('data-harmonica-tile-element', harmonicaTileTag);
                    if (cAttrs.harmonicaTileOnOpen) {
                        harmonicaTileEL.setAttribute('data-on-open', cAttrs.harmonicaTileOnOpen);
                    }
                    cElement.append(harmonicaTileEL);
                    return function (scope, element, attrs, ctrl) {
                        if (attrs.harmonicaMatchMedia != null && attrs.harmonicaMatchMedia.length > 0) {
                            const mq = $window.matchMedia(attrs.harmonicaMatchMedia);

                            mq.addListener(function (obj) {
                                setTimeout(function () {
                                    if (obj.matches === true) {
                                        ctrl.start();
                                    } else {
                                        ctrl.stop();
                                    }

                                    scope.$digest();
                                }, 100);
                            });

                            if (mq.matches === true) {
                                startProcess();
                            } else {
                                ctrl.stop();
                            }
                        } else {
                            startProcess();
                        }

                        $window.addEventListener('resize', update);

                        function startProcess() {
                            if ($document[0].readyState == `complete`) {
                                $window.whenAdvantshopStylesLoaded().then(() => ctrl.start());
                            } else {
                                $window.addEventListener(`load`, function start() {
                                    $window.whenAdvantshopStylesLoaded().then(() => ctrl.start());
                                    $window.removeEventListener(`load`, start);
                                });
                            }
                        }

                        function update() {
                            if (ctrl.active === true) {
                                var index = ctrl.calc();
                                ctrl.setVisible(index);
                                scope.$digest();
                            }
                        }

                        if (attrs.harmonicaRecalc != null) {
                            scope.$watch(attrs.harmonicaRecalc, () => {
                                if (ctrl.active === true) {
                                    var index = ctrl.calc();
                                    ctrl.setVisible(index);
                                }
                            });
                        }
                    };
                },
            };
        },
    );

    angular.module('harmonica').directive('harmonicaItem', function () {
        return {
            //require: '^harmonica',
            require: {
                harmonicaCtrl: '^harmonica',
            },
            restrict: 'EA',
            scope: true,
            bindToController: true,
            controller: [
                '$element',
                '$scope',
                function ($element, $scope) {
                    var ctrl = this;

                    ctrl.$onInit = function () {
                        ctrl.harmonicaCtrl.addItem($element, $scope);

                        $scope.$watch('isVisibleInMenu', function (newValue, oldValue) {
                            $element[newValue === false ? 'addClass' : 'removeClass']('ng-hide');
                        });
                    };
                },
            ],
        };
    });

    angular.module('harmonica').directive(
        'harmonicaLink',
        /* @ngInject */ function ($parse) {
            return {
                //require: '^harmonica',
                require: {
                    harmonicaCtrl: '^harmonica',
                },
                restrict: 'EA',
                scope: true,
                bindToController: true,
                controller: /*@ngInject*/ function ($attrs, $element, $scope) {
                    var ctrl = this;

                    ctrl.$onInit = function () {
                        let attrsCopy;
                        if ($attrs.linkCopyAttrs != null) {
                            const attrValue = $parse($attrs.linkCopyAttrs)($scope);
                            attrsCopy = attrValue.map((x) => `${x}="${$element.attr(x)}"`);
                        }

                        ctrl.harmonicaCtrl.addLink(
                            $element.attr('href'),
                            $element.text(),
                            $attrs.linkClassesInTile,
                            $attrs.linkTarget,
                            attrsCopy,
                            $scope,
                        );
                    };
                },
            };
        },
    );

    angular.module('harmonica').directive('harmonicaTile', function () {
        return {
            //require: ['ctrl', '^harmonica'],
            require: {
                harmonicaCtrl: '^harmonica',
                submenuContainer: '?^submenuContainer',
            },
            restrict: 'EA',
            scope: {
                onOpen: '&',
            },
            replace: true,
            controller: 'HarmonicaTileCtrl',
            controllerAs: 'ctrl',
            bindToController: true,
            template: function (el, attrs) {
                return `<${attrs.harmonicaTileElement} class="{{::ctrl.cssClasses.harmonicaClassTile}}" data-ng-hide="!ctrl.isVisibleTile" ng-class="{'harmonica-tile-active' : ctrl.isVisibleTileSubmenu}">${tileTemplate}</${attrs.harmonicaTileElement}>`;
            },
        };
    });

    angular.module('harmonica').component(`harmonicaLinkInTile`, {
        bindings: {
            link: `<`,
            cssClasses: '<',
        },
        controller: /* @ngInject */ function ($compile, $scope, $element) {
            const ctrl = this;
            ctrl.$onInit = () => {
                const tpl = `<a class="harmonica-tile-link {{::$ctrl.harmonicaClassTileLink}}" data-ng-class="$ctrl.link.linkClassesInTile" 
                                href="{{::$ctrl.link.linkHref}}" target="{{$ctrl.link.linkTarget}}" data-ng-bind="::$ctrl.link.linkText"
                                ${ctrl.link.linkCopyAttrs != null ? ctrl.link.linkCopyAttrs : ''}></a>`;

                $element.html(tpl);
                $compile($element.contents())($scope);
            };
        },
    });
})(angular);
