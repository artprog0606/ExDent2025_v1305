let isHasEvent = false;
(function () {
    'use strict';
    const STATE = {
        RIGHT_STOPED: 'rightStoped',
        LEFT_STOPED: 'leftStoped',
    };

    const stateClasses = {
        RIGHT_STOPED: 'swipe-line__inner--right-stoped',
        LEFT_STOPED: 'swipe-line__inner--left-stoped',
        MOVING: 'swipe-line__inner--moving',
        MOVING_WITHOUT_RIGHT_EL: 'swipe-line__inner--moving-without-right-el',
        MOVING_WITHOUT_LEFT_EL: 'swipe-line__inner--moving-without-left-el',
    };
    const reinitSwipeLineEvent = 'reinitSwipeLineEvent';

    angular
        .module('swipeLine', [])
        .directive('swipeLine', function () {
            return {
                scope: true,
                controllerAs: 'swipeLine',
                controller: [
                    '$element',
                    '$document',
                    '$window',
                    '$scope',
                    '$attrs',
                    function ($element, $document, $window, $scope, $attrs) {
                        var ctrl = this;
                        var disabledSwipeElementClass = $attrs.disabledSwipeElement;

                        var unbindWatcher;
                        var overflowDistance = 0; //насколько мы можем перемотать ещё, после того как показались элементы
                        var ratio = 20 / 100; //процент, при котором мы доматываем до конца
                        var moveValueCurrent = 0;
                        var moveValueLast = 0;
                        ctrl.currentState = null;

                        ctrl.widthSwipeLineLeftElement = 0;
                        ctrl.widthSwipeLineRightElement = 0;

                        var isValidSwipe = false;

                        ctrl.touchstartHandler = function (e) {
                            // which
                            const _event = e || $window.event; //don't know how mobile browsers behave here
                            if (_event.type === 'mousedown' && _event.which !== 1) return;
                            if (disabledSwipeElementClass != null) {
                                const elem = $document[0].elementFromPoint(
                                    _event.clientX || _event.touches[0].clientX,
                                    _event.clientY || _event.touches[0].clientY,
                                );
                                if (elem.classList.contains(disabledSwipeElementClass) || elem.closest(`.${disabledSwipeElementClass}`) != null) {
                                    return;
                                }
                            }

                            isValidSwipe = null;

                            ctrl.startCoordinates = {
                                x: _event.clientX || _event.changedTouches[0].clientX,
                                y: _event.clientY || _event.changedTouches[0].clientY,
                            };

                            ctrl.maxDistanceRight = ctrl.widthSwipeLineRightElement;
                            ctrl.maxDistanceLeft = ctrl.widthSwipeLineLeftElement;

                            if (!ctrl.maxDistanceLeft && !ctrl.maxDistanceRight) {
                                return;
                            }
                            if ('ontouchstart' in window) {
                                $element[0].addEventListener('touchmove', ctrl.moveHandler, { passive: true });
                                $element[0].addEventListener('touchend', ctrl.endHandler);
                            } else {
                                $document[0].addEventListener('mousemove', ctrl.moveHandler);
                                $document[0].addEventListener('mouseup', ctrl.endHandler);
                            }
                        };

                        ctrl.$postLink = function () {
                            ctrl.init();
                            // TODO убрать костыль когда переделают табы разобьют на формы
                            $scope.$on(reinitSwipeLineEvent, function (evt, data) {
                                ctrl.destroy();
                                ctrl.init();
                            });
                        };

                        ctrl.moveHandler = function (e) {
                            const _event = e || $window.e;

                            if (isValidSwipe === null && _event.type !== 'mousemove' && _event.type !== 'mouseup') {
                                isValidSwipe = ctrl.validSwipe(ctrl.startCoordinates, {
                                    x: _event.clientX || _event.changedTouches[0].clientX,
                                    y: _event.clientY || _event.changedTouches[0].clientY,
                                });
                            }

                            if (isValidSwipe === false && _event.type !== 'mouseup' && _event.type !== 'mousemove') {
                                return;
                            }

                            $element[0].classList.add(stateClasses.MOVING);
                            if (ctrl.swipeLineLeftElement == null) {
                                $element[0].classList.add(stateClasses.MOVING_WITHOUT_LEFT_EL);
                            }
                            if (ctrl.swipeLineRightElement == null) {
                                $element[0].classList.add(stateClasses.MOVING_WITHOUT_RIGHT_EL);
                            }

                            /*e.preventDefault();*/

                            var diffCalc = moveValueLast * -1 + ctrl.startCoordinates.x - (_event.clientX || _event.changedTouches[0].clientX);
                            var diff;
                            var diffCalcAbs = Math.abs(diffCalc);

                            var maxDistance = diffCalc > 0 ? ctrl.maxDistanceRight : ctrl.maxDistanceLeft * -1;

                            if (diffCalcAbs > Math.abs(maxDistance + overflowDistance)) {
                                diff = maxDistance + overflowDistance;
                            } else if (diffCalcAbs < overflowDistance * -1) {
                                diff = overflowDistance * -1;
                            } else {
                                diff = diffCalc;
                            }

                            ctrl.move(diff * -1);
                        };

                        ctrl.endHandler = function (e) {
                            const _event = e || $window.e;
                            if (isValidSwipe === null && _event.type !== 'mousemove' && _event.type !== 'mouseup') {
                                isValidSwipe = ctrl.validSwipe(ctrl.startCoordinates, {
                                    x: _event.clientX || _event.changedTouches[0].clientX,
                                    y: _event.clientY || _event.changedTouches[0].clientY,
                                });
                            }
                            if (isValidSwipe === false && _event.type !== 'mousemove' && _event.type !== 'mouseup') {
                                return;
                            }
                            var diff = ctrl.startCoordinates.x - (_event.clientX || _event.changedTouches[0].clientX);

                            if (diff === 0) return;
                            var diffOverride;

                            var maxDistance = ctrl.maxDistanceRight;

                            var minDistance = ctrl.maxDistanceLeft * -1;

                            if (diff > 0) {
                                if ((diff >= maxDistance || ratio <= diff / maxDistance) && !ctrl.currentState) {
                                    ctrl.currentState = STATE.RIGHT_STOPED;
                                    diffOverride = maxDistance;

                                    if (ctrl.swipeLineRightElement != null) {
                                        $element[0].classList.add(stateClasses.RIGHT_STOPED);
                                    }
                                    $element[0].classList.remove(stateClasses.LEFT_STOPED);
                                    if (ctrl.onStickRightHandler) {
                                        ctrl.onStickRightHandler($scope);
                                        $scope.$apply();
                                    }
                                }
                                if (ctrl.currentState === STATE.RIGHT_STOPED) {
                                    diffOverride = maxDistance;
                                } else {
                                    diffOverride = 0;
                                    ctrl.currentState = null;
                                    $element[0].classList.remove(stateClasses.LEFT_STOPED);
                                    $element[0].classList.remove(stateClasses.RIGHT_STOPED);
                                    if (ctrl.onUnstickLeftHandler) {
                                        ctrl.onUnstickLeftHandler($scope);
                                        $scope.$apply();
                                    }
                                }
                            } else {
                                if ((diff <= minDistance || ratio <= diff / minDistance) && !ctrl.currentState) {
                                    ctrl.currentState = STATE.LEFT_STOPED;
                                    diffOverride = minDistance;
                                    if (ctrl.swipeLineLeftElement != null) {
                                        $element[0].classList.add(stateClasses.LEFT_STOPED);
                                    }
                                    $element[0].classList.remove(stateClasses.RIGHT_STOPED);
                                    if (ctrl.onStickLeftHandler) {
                                        ctrl.onStickLeftHandler($scope);
                                        $scope.$apply();
                                    }
                                }
                                if (ctrl.currentState === STATE.LEFT_STOPED) {
                                    diffOverride = minDistance;
                                } else {
                                    diffOverride = 0;
                                    ctrl.currentState = null;
                                    $element[0].classList.remove(stateClasses.LEFT_STOPED);
                                    $element[0].classList.remove(stateClasses.RIGHT_STOPED);
                                    if (ctrl.onUnstickRightHandler) {
                                        ctrl.onUnstickRightHandler($scope);
                                        $scope.$apply();
                                    }
                                }
                            }

                            //if (diff > maxDistance || (moveValueCurrent < 0 && ratio < moveValueCurrent / maxDistance)) {
                            //    diffOverride = maxDistance;
                            //} else {
                            //    diffOverride = 0;
                            //}

                            if (diffOverride != null) {
                                ctrl.move(diffOverride * -1, true);
                                moveValueCurrent = diffOverride * -1;
                            }

                            moveValueLast = moveValueCurrent;

                            $element[0].classList.remove(stateClasses.MOVING);
                            $element[0].classList.remove(stateClasses.MOVING_WITHOUT_LEFT_EL);
                            $element[0].classList.remove(stateClasses.MOVING_WITHOUT_RIGHT_EL);

                            if ('ontouchstart' in window) {
                                $element[0].removeEventListener('touchmove', ctrl.moveHandler);
                                $element[0].removeEventListener('touchend', ctrl.endHandler);
                            } else {
                                $document[0].removeEventListener('mousemove', ctrl.moveHandler);
                                $document[0].removeEventListener('mouseup', ctrl.endHandler);
                            }
                        };

                        ctrl.moveToDefault = function () {
                            ctrl.move(0, true);
                            if (ctrl.onUnstickLeftHandler || ctrl.onUnstickRightHandler) {
                                if (ctrl.currentState === STATE.LEFT_STOPED) {
                                    ctrl.onUnstickLeftHandler($scope);
                                }
                                if (ctrl.currentState === STATE.RIGHT_STOPED) {
                                    ctrl.onUnstickRightHandler($scope);
                                }
                            }
                            ctrl.currentState = null;
                        };

                        ctrl.stickToLeft = function () {
                            var maxDistanceLeft = ctrl.widthSwipeLineLeftElement;
                            ctrl.move(maxDistanceLeft, true);
                            moveValueLast = maxDistanceLeft;
                            ctrl.currentState = STATE.LEFT_STOPED;
                            if (ctrl.onStickLeftHandler) {
                                ctrl.onStickLeftHandler($scope);
                                //$scope.$apply();
                            }
                        };

                        ctrl.stickToRight = function () {
                            var maxDistanceRight = ctrl.widthSwipeLineRightElement;
                            ctrl.move(maxDistanceRight, true);
                            moveValueLast = maxDistanceRight;
                            ctrl.currentState = STATE.RIGHT_STOPED;
                            if (ctrl.onStickRightHandler) {
                                ctrl.onStickRightHandler($scope);
                                $scope.$apply();
                            }
                        };

                        ctrl.runAnimation = function (callback) {
                            function fnOnTransitionend() {
                                $element.removeClass('swipe-line-animate');

                                if (callback != null) {
                                    callback();
                                }

                                $element[0].removeEventListener('transitionend', fnOnTransitionend);
                            }
                            $element.addClass('swipe-line-animate');
                            $element[0].addEventListener('transitionend', fnOnTransitionend);
                        };

                        ctrl.move = function (moveValue, useAnimate, callback) {
                            if (useAnimate === true) {
                                ctrl.runAnimation(callback);
                            }
                            $element[0].style.transform = `translate3d(${moveValue - ctrl.widthSwipeLineLeftElement}px, 0, 0)`;
                            moveValueCurrent = moveValue;
                        };

                        ctrl.addSwipeLineRight = function (swipeLineRightElement) {
                            ctrl.swipeLineRightElement = swipeLineRightElement;
                        };

                        ctrl.addSwipeLineLeft = function (swipeLineLeftElement) {
                            ctrl.swipeLineLeftElement = swipeLineLeftElement;
                        };

                        ctrl.addOnStickLeftHandler = function (onStickLeftHandler) {
                            ctrl.onStickLeftHandler = onStickLeftHandler;
                        };

                        ctrl.addOnUnstickLeftHandler = function (onUnstickLeftHandler) {
                            ctrl.onUnstickLeftHandler = onUnstickLeftHandler;
                        };

                        ctrl.addOnStickRightHandler = function (onStickRightHandler) {
                            ctrl.onStickRightHandler = onStickRightHandler;
                        };

                        ctrl.addOnUnstickRightHandler = function (onUnstickRightHandler) {
                            ctrl.onUnstickRightHandler = onUnstickRightHandler;
                        };

                        ctrl.setWidthLeftRightElements = function (objMapElemnts) {
                            if (objMapElemnts.widthElementLeft != null) {
                                ctrl.widthSwipeLineLeftElement = objMapElemnts.widthElementLeft;
                            }
                            if (objMapElemnts.widthElementRight != null) {
                                ctrl.widthSwipeLineRightElement = objMapElemnts.widthElementRight;
                            }
                        };

                        ctrl.calcPositionСonsiderLeftEl = function () {
                            ctrl.widthSwipeLineLeftElement = ctrl.swipeLineLeftElement[0].offsetWidth;
                            $element[0].classList.add('swipe-line__inner--initialized');
                            $element[0].style.transform = `translate3d(-${ctrl.swipeLineLeftElement[0].offsetWidth}px, 0, 0)`;
                        };

                        ctrl.addSwipeLeftHandler = function (swipeLeftHandler) {
                            ctrl.swipeLeftHandler = swipeLeftHandler;
                        };

                        ctrl.validSwipe = function (startCoords, coords) {
                            var deltaAlt = Math.abs(coords.y - startCoords.y);
                            var deltaMain = Math.abs(coords.x - startCoords.x);
                            var touchAngle = (Math.atan2(Math.abs(deltaAlt), Math.abs(deltaMain)) * 180) / Math.PI;
                            return touchAngle <= 20;
                        };

                        $element.on('$destroy', () => ctrl.destroy());

                        ctrl.addSwipeLineLeftScope = function (swipeLineLeftScope) {
                            ctrl.swipeLineLeftScope = swipeLineLeftScope;
                        };

                        ctrl.addSwipeLineRightScope = function (swipeLineRightScope) {
                            ctrl.swipeLineRightScope = swipeLineRightScope;
                        };

                        ctrl.init = function () {
                            if ('ontouchstart' in window) {
                                $element[0].addEventListener('touchstart', ctrl.touchstartHandler, { passive: true });
                            } else {
                                $element[0].addEventListener('mousedown', ctrl.touchstartHandler);
                            }
                        };

                        ctrl.destroy = function () {
                            $element[0].style.transform = `translate3d(0, 0, 0)`;

                            if ('ontouchstart' in window) {
                                $element[0].removeEventListener('touchmove', ctrl.moveHandler);
                                $element[0].removeEventListener('touchend', ctrl.endHandler);
                                $element[0].removeEventListener('touchstart', ctrl.touchstartHandler);
                            } else {
                                $document[0].removeEventListener('mousemove', ctrl.moveHandler);
                                $document[0].removeEventListener('mouseup', ctrl.endHandler);
                                $element[0].removeEventListener('mousedown', ctrl.touchstartHandler);
                            }

                            if (unbindWatcher != null) {
                                unbindWatcher();
                            }
                        };

                        ctrl.reinit = function () {
                            ctrl.destroy();
                            ctrl.init();
                            if (ctrl.swipeLineLeftScope) {
                                ctrl.swipeLineLeftScope.reinit();
                            }
                            if (ctrl.swipeLineRightScope) {
                                ctrl.swipeLineRightScope.reinit();
                            }
                        };
                    },
                ],
            };
        })
        .directive('swipeLineRight', function () {
            return {
                require: {
                    swipeLine: '^swipeLine',
                },
                bindToController: true,
                controller: [
                    '$element',
                    '$timeout',
                    '$attrs',
                    '$parse',
                    '$scope',
                    '$document',
                    function ($element, $timeout, $attrs, $parse, $scope, $document) {
                        var ctrl = this;
                        var onStickRightHandler = $parse($attrs.onStickRight);
                        var onUnstickRightHandler = $parse($attrs.onUnstickLeft);
                        let unbindWatcher;

                        ctrl.$postLink = function () {
                            ctrl.init();
                            // TODO убрать костыль когда переделают табы разобьют на формы
                            $scope.$on(reinitSwipeLineEvent, function (evt, data) {
                                ctrl.destroy();
                                ctrl.init();
                            });
                        };

                        ctrl.init = function () {
                            ctrl.swipeLine.addSwipeLineRightScope(ctrl);
                            ctrl.swipeLine.addSwipeLineRight($element);
                            $timeout(() => {
                                ctrl.swipeLine.setWidthLeftRightElements({
                                    widthElementRight: $element[0].offsetWidth,
                                });
                                ctrl.swipeLine.addOnStickRightHandler(onStickRightHandler);
                                ctrl.swipeLine.addOnUnstickRightHandler(onUnstickRightHandler);

                                unbindWatcher = $scope.$watch($attrs['isSticked'], function (value) {
                                    if (value && ctrl.swipeLine.currentState !== STATE.RIGHT_STOPED) {
                                        ctrl.swipeLine.stickToRight();
                                    } else if (value === false && ctrl.swipeLine.currentState === STATE.RIGHT_STOPED) {
                                        ctrl.swipeLine.moveToDefault();
                                    }
                                });
                            }, 0);
                        };

                        $element.on('$destroy', () => ctrl.destroy());

                        ctrl.destroy = function () {
                            if ('ontouchstart' in window) {
                                $element[0].removeEventListener('touchmove', ctrl.moveHandler);
                                $element[0].removeEventListener('touchend', ctrl.endHandler);
                                $element[0].removeEventListener('touchstart', ctrl.touchstartHandler);
                            } else {
                                $document[0].removeEventListener('mousemove', ctrl.moveHandler);
                                $document[0].removeEventListener('mouseup', ctrl.endHandler);
                                $element[0].removeEventListener('mousedown', ctrl.touchstartHandler);
                            }

                            if (unbindWatcher != null) {
                                unbindWatcher();
                            }
                        };

                        ctrl.reinit = function () {
                            ctrl.destroy();
                            ctrl.init();
                        };
                    },
                ],
            };
        })
        .directive('swipeLineLeft', function () {
            return {
                require: {
                    swipeLine: '^swipeLine',
                },
                bindToController: true,
                controller: [
                    '$element',
                    '$timeout',
                    '$attrs',
                    '$parse',
                    '$scope',
                    function ($element, $timeout, $attrs, $parse, $scope) {
                        var ctrl = this;
                        var onStickLeftHandler = $parse($attrs.onStickLeft);
                        var onUnstickLeftHandler = $parse($attrs.onUnstickLeft);
                        let unbindWatcher;

                        ctrl.$postLink = function () {
                            ctrl.init();
                            // TODO убрать костыль когда переделают табы разобьют на формы
                            $scope.$on(reinitSwipeLineEvent, function (evt, data) {
                                ctrl.destroy();
                                ctrl.init();
                            });
                        };

                        ctrl.init = function () {
                            ctrl.swipeLine.addSwipeLineLeftScope(ctrl);
                            ctrl.swipeLine.addSwipeLineLeft($element);
                            $timeout(() => {
                                ctrl.swipeLine.setWidthLeftRightElements({
                                    widthElementLeft: $element[0].offsetWidth,
                                });
                                ctrl.swipeLine.calcPositionСonsiderLeftEl();
                                ctrl.swipeLine.addOnStickLeftHandler(onStickLeftHandler);
                                ctrl.swipeLine.addOnUnstickLeftHandler(onUnstickLeftHandler);

                                unbindWatcher = $scope.$watch($attrs['isSticked'], function (value, prevValue) {
                                    if (value && ctrl.swipeLine.currentState !== STATE.LEFT_STOPED) {
                                        ctrl.swipeLine.stickToLeft();
                                    } else if (value === false && ctrl.swipeLine.currentState === STATE.LEFT_STOPED) {
                                        ctrl.swipeLine.moveToDefault();
                                    }
                                });
                            }, 0);
                        };

                        $element.on('$destroy', () => ctrl.destroy());

                        ctrl.destroy = function () {
                            if (unbindWatcher != null) {
                                unbindWatcher();
                            }
                        };

                        ctrl.reinit = function () {
                            ctrl.destroy();
                            ctrl.init();
                        };
                    },
                ],
            };
        });
})();
