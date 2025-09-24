import modalTemplate from '../templates/modal.html';

(function (ng) {
    'use strict';

    var modalIdIncrement = 0,
        transformName = 'transform',
        inIframe = (function () {
            try {
                return window.self !== window.top;
            } catch (e) {
                return true;
            }
        })();

    angular.module('modal').directive('modalControl', [
        '$compile',
        '$document',
        '$parse',
        '$timeout',
        'modalService',
        'modalDefaultOptions',
        'domService',
        'urlHelper',
        '$location',
        function ($compile, $document, $parse, $timeout, modalService, modalDefaultOptions, domService, urlHelper, $location) {
            return {
                restrict: 'EA',
                transclude: true,
                scope: true,
                replace: true,
                controller: 'ModalCtrl',
                controllerAs: 'modal',
                bindToController: true,
                templateUrl: function (element, attrs) {
                    return attrs.templatePath || modalTemplate;
                },
                compile: function (cElement, cAttrs) {
                    return function (scope, element, attrs, ctrl, transclude) {
                        ctrl.id = attrs.id;

                        if (ctrl.id == null || ctrl.id.length === 0) {
                            ctrl.id = 'modal_' + modalIdIncrement;
                            modalIdIncrement += 1;
                            attrs.$set('id', ctrl.id);
                        }

                        if (modalService.hasModal(ctrl.id) === true) {
                            return;
                        }

                        ctrl._form = scope.form;
                        ctrl.inIframe = inIframe;
                        ctrl.modalClass =
                            (attrs.modalClass || modalDefaultOptions.modalClass) +
                            ' ' +
                            (attrs.appendModalClass != null ? attrs.appendModalClass : modalDefaultOptions.appendModalClass);
                        ctrl.modalOverlayClass = attrs.modalOverlayClass;
                        ctrl.isFloating = angular.isDefined(attrs.isFloating) ? attrs.isFloating == 'true' : modalDefaultOptions.isFloating;
                        ctrl.crossEnable = angular.isDefined(attrs.crossEnable) ? attrs.crossEnable == 'true' : modalDefaultOptions.crossEnable;
                        ctrl.backgroundEnable = angular.isDefined(attrs.backgroundEnable)
                            ? attrs.backgroundEnable === 'true'
                            : modalDefaultOptions.backgroundEnable;
                        ctrl.closeOut = angular.isDefined(attrs.closeOut) ? attrs.closeOut === 'true' : modalDefaultOptions.closeOut;

                        ctrl.isFloating = angular.isDefined(ctrl.isFloating) ? ctrl.isFloating : modalDefaultOptions.isFloating;
                        ctrl.crossEnable = angular.isDefined(ctrl.crossEnable) ? ctrl.crossEnable : modalDefaultOptions.crossEnable;
                        ctrl.backgroundEnable = angular.isDefined(ctrl.backgroundEnable)
                            ? ctrl.backgroundEnable
                            : modalDefaultOptions.backgroundEnable;
                        ctrl.closeOut = angular.isDefined(ctrl.closeOut) ? ctrl.closeOut : modalDefaultOptions.closeOut;
                        ctrl.isOpen = angular.isDefined(ctrl.isOpen) ? ctrl.isOpen : modalDefaultOptions.isOpen;
                        ctrl.startOpenDelay =
                            angular.isDefined(attrs.startOpenDelay) && attrs.startOpenDelay.length > 0 ? parseInt(attrs.startOpenDelay) : null;
                        ctrl.callbackOpen = $parse(attrs.callbackOpen);
                        ctrl.callbackClose = $parse(attrs.callbackClose);
                        ctrl.callbackInit = $parse(attrs.callbackInit);
                        ctrl.closeEsc = angular.isDefined(attrs.closeEsc) ? attrs.closeEsc === 'true' : modalDefaultOptions.closeEsc;
                        ctrl.isShowFooter = angular.isDefined(attrs.isShowFooter) ? attrs.isShowFooter === 'true' : modalDefaultOptions.isShowFooter;

                        ctrl.destroyOnClose = angular.isDefined(attrs.destroyOnClose) ? attrs.destroyOnClose === 'true' : false;

                        ctrl.anchor = attrs.anchor || ctrl.id;
                        ctrl.spyAddress = angular.isDefined(attrs.spyAddress) ? attrs.spyAddress === 'true' : false;

                        var modalElementTransclude = angular.element(element[0].querySelector('.js-modal-transclude'));

                        transclude(scope, function (clone) {
                            modalElementTransclude.replaceWith(clone);
                        });

                        modalService.addStorage(attrs.id, element, ctrl);

                        if (element[0].parentNode !== document.body) {
                            document.body.appendChild(element[0]);
                        }

                        if (attrs.callbackInit != null) {
                            ctrl.callbackInit(scope);
                        }

                        if (ctrl.startOpenDelay != null) {
                            $timeout(modalService.open.bind(this, ctrl.id), ctrl.startOpenDelay);
                        }

                        if (ctrl.closeEsc === true) {
                            $document[0].addEventListener('keyup', function (event) {
                                if (event.keyCode === 27 && ctrl.isOpen === true) {
                                    //esc
                                    ctrl.close();
                                    scope.$apply();
                                }
                            });
                        }

                        var hash;
                        if ($location.hash() != null) {
                            hash = $location.hash();
                            var splitedHash = $location.hash().split('?');
                            hash = splitedHash != null ? splitedHash[0] : hash;
                        }

                        if (ctrl.spyAddress === true && ctrl.anchor != null && ctrl.anchor.length > 0 && hash === ctrl.anchor) {
                            ctrl.open();
                        }

                        $document[0].addEventListener('click', function () {
                            ctrl.setMousedownOnContent(false);
                        });
                    };
                },
            };
        },
    ]);

    angular.module('modal').directive('modalHeader', [
        '$q',
        '$document',
        '$window',
        'domService',
        function ($q, $document, $window, domService) {
            return {
                restrict: 'EA',
                require: '^modalControl',
                transclude: true,
                replace: true,
                template: '<div data-ng-transclude class="modal-header js-modal-header"></div>',
                scope: true,
                link: function (scope, element, attrs, ctrl) {
                    ctrl.headerExist = true;

                    var modalHeader = element[0],
                        modal = ctrl.getModalElement()[0];

                    if (ctrl.isFloating === true) {
                        var mouseStartPosition = { x: 0, y: 0 },
                            transitionStartPosition = { x: 0, y: 0 },
                            transitionMovePosition = { x: 0, y: 0 },
                            isFirstMove = true,
                            isMove = false,
                            isModalForseFloatDisable = false;

                        var start = function (e) {
                            var computedStyleTransform, parsedTransform;

                            if (
                                isModalForseFloatDisable === true ||
                                domService.closest(e.target, modal) == null ||
                                domService.closest(e.target, '.js-modal-header') == null
                            ) {
                                return;
                            }

                            mouseStartPosition.x = e.pageX;
                            mouseStartPosition.y = e.pageY;

                            isMove = true;

                            if (isFirstMove === true) {
                                isFirstMove = false;

                                computedStyleTransform = window.getComputedStyle(modal)[transformName];

                                if (computedStyleTransform.length > 0) {
                                    if (computedStyleTransform === 'none') {
                                        transitionStartPosition.x = 0;
                                        transitionStartPosition.y = 0;
                                    } else {
                                        parsedTransform = JSON.parse(computedStyleTransform.replace(/^\w+\(/, '[').replace(/\)$/, ']'));
                                        transitionStartPosition.x = parsedTransform[4];
                                        transitionStartPosition.y = parsedTransform[5];
                                    }
                                }
                            }

                            $document[0].addEventListener('mousemove', move);
                            $document[0].addEventListener('touchmove', move, { passive: true });

                            modalHeader.addEventListener('mouseup', end);
                            modalHeader.addEventListener('touchend', end, { passive: true });

                            //e.stopPropagation();
                            //e.preventDefault();
                        };

                        var move = function (e) {
                            //if (isModalForseFloatDisable === true || domService.closest(e.target, modal) == null || domService.closest(e.target, '.js-modal-header') == null || isMove === false) {
                            //    isMove = false;
                            //    return;
                            //}

                            transitionMovePosition.x = transitionStartPosition.x + (e.pageX - mouseStartPosition.x);
                            transitionMovePosition.y = transitionStartPosition.y + (e.pageY - mouseStartPosition.y);

                            modal.style[ctrl.getTransformMethodString()] = ctrl.getTransformValue(transitionMovePosition.x, transitionMovePosition.y);

                            //e.stopPropagation();
                            //e.preventDefault();
                        };

                        var end = function (e) {
                            //if (isModalForseFloatDisable === true || domService.closest(e.target, modal) == null || domService.closest(e.target, '.js-modal-header') == null) {
                            //    return;
                            //}

                            mouseStartPosition.x = e.pageX;
                            mouseStartPosition.y = e.pageY;

                            transitionStartPosition.x = transitionMovePosition.x;
                            transitionStartPosition.y = transitionMovePosition.y;

                            transitionMovePosition.x = 0;
                            transitionMovePosition.y = 0;

                            isMove = false;

                            $document[0].removeEventListener('mousemove', move);
                            $document[0].removeEventListener('touchmove', move);

                            modalHeader.removeEventListener('mouseup', end);
                            modalHeader.removeEventListener('touchend', end);

                            //e.stopPropagation();
                            //e.preventDefault();
                        };

                        modalHeader.addEventListener('mousedown', start);
                        modalHeader.addEventListener('touchstart', start, { passive: true });

                        $window.matchMedia('(max-width: 30em)').addListener(function () {
                            isModalForseFloatDisable = true;
                        });

                        $window.matchMedia('(min-width: 31em)').addListener(function () {
                            isModalForseFloatDisable = false;
                        });
                    }
                },
            };
        },
    ]);

    angular.module('modal').directive('modalFooter', function () {
        return {
            restrict: 'EA',
            require: '^modalControl',
            transclude: true,
            replace: true,
            template: '<div class="modal-footer" data-ng-show="modal.isShowFooter" data-ng-transclude></div>',
            scope: true,
        };
    });

    angular.module('modal').directive('modalOpen', [
        '$parse',
        'modalService',
        function ($parse, modalService) {
            return {
                restrict: 'A',
                link: function (scope, element, attrs, ctrl) {
                    var callbackOpen, callbackClose;

                    if (attrs.modalOpenCallback != null && attrs.modalOpenCallback.length > 0) {
                        callbackOpen = $parse(attrs.modalOpenCallback);
                    }

                    if (attrs.modalOpenCallbackOnClose != null && attrs.modalOpenCallbackOnClose.length > 0) {
                        callbackClose = $parse(attrs.modalOpenCallbackOnClose);
                    }

                    element.on('click', function () {
                        modalService.getModal(attrs.modalOpen).then(function (modal) {
                            if (callbackOpen != null) {
                                callbackOpen(scope);
                            }

                            if (callbackClose != null) {
                                var oldFn = modal.modalScope.callbackClose;

                                modal.modalScope.callbackClose = function (modalScope) {
                                    oldFn.apply(this, arguments);
                                    callbackClose(scope);
                                };
                            }
                        });

                        var modalDataAdditional = $parse(attrs.modalDataAdditional)(scope);

                        modalService.open(attrs.modalOpen, attrs.modalOpenSkipQueue === 'true', modalDataAdditional);

                        scope.$apply();
                    });
                },
            };
        },
    ]);

    angular.module('modal').directive('modalClose', [
        '$parse',
        'modalService',
        function ($parse, modalService) {
            return {
                require: '?^modalControl',
                restrict: 'A',
                link: function (scope, element, attrs, ctrl) {
                    var modalCtrl = ctrl,
                        callback;

                    if (attrs.modalCloseCallback != null && attrs.modalCloseCallback.length > 0) {
                        callback = $parse(attrs.modalCloseCallback);
                    }

                    element.on('click', function () {
                        if (callback != null) {
                            callback(scope);
                        }

                        if (attrs.modalClose != null && attrs.modalClose.length > 0) {
                            modalService.close(attrs.modalClose);
                        } else if (modalCtrl != null) {
                            modalCtrl.close();
                        }

                        scope.$apply();
                    });
                },
            };
        },
    ]);
})(angular);
