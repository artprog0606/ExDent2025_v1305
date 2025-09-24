(function (ng) {
    'use strict';

    //TODO: вынести работу с DOMом

    var isTouchDevice = 'ontouchstart' in document.documentElement;

    var TransformerCtrl = function ($window, $element, $scope, transformerService, $attrs, $document) {
        var ctrl = this,
            container,
            containerStartRect,
            elBoxIndents,
            elWrapBoxIndents,
            containerElementBoxIndents,
            elementStartRect,
            mutableItems = [],
            currentDirectionVerticalScroll = null,
            directionVerticalScroll = null,
            pageYOffsetPrev = 0;

        ctrl._element = $element;

        ctrl.styles = {};

        ctrl.isTouchDevice = isTouchDevice;

        ctrl.init = function (containerElement) {
            transformerService.addInStorage(ctrl);

            ctrl.addContainer(containerElement);

            ctrl.calc();

            if (ctrl.onInit != null) {
                ctrl.onInit($scope, { transformer: ctrl });
            }
        };

        ctrl.addContainer = function (containerElement) {
            var temp, tempEl;

            container = containerElement;
            temp = container.getBoundingClientRect();
            tempEl = $element[0].getBoundingClientRect();

            elBoxIndents = {
                paddingLeft: parseFloat($($element[0]).css('padding-left')),
                paddingRight: parseFloat($($element[0]).css('padding-right')),
                paddingTop: parseFloat($($element[0]).css('padding-top')),
                paddingBottom: parseFloat($($element[0]).css('padding-bottom')),
                marginLeft: parseFloat($($element[0]).css('margin-left')),
                marginRight: parseFloat($($element[0]).css('margin-right')),
                marginTop: parseFloat($($element[0]).css('margin-Top')),
                marginBottom: parseFloat($($element[0]).css('margin-bottom')),
            };

            elWrapBoxIndents = {
                paddingLeft: parseFloat($element.parent().css('padding-left')),
                marginLeft: parseFloat($element.parent().css('margin-left')),
                paddingTop: parseFloat($element.parent().css('padding-top')),
                paddingBottom: parseFloat($element.parent().css('padding-bottom')),
                marginBottom: parseFloat($element.parent().css('margin-bottom')),
            };

            containerElementBoxIndents = {
                paddingLeft: parseFloat($(containerElement).css('padding-left')),
                marginLeft: parseFloat($(containerElement).css('margin-left')),
                paddingTop: parseFloat($(containerElement).css('padding-top')),
                paddingBottom: parseFloat($(containerElement).css('padding-bottom')),
                marginBottom: parseFloat($(containerElement).css('margin-bottom')),
            };

            elementStartRect = {
                top: tempEl.top,
                topWithScroll: tempEl.top + $window.pageYOffset, //используется в scrooltoblock.service.js
                bottom: tempEl.bottom,
                bottomWithScroll: tempEl.bottom + $window.pageYOffset, //используется в scrooltoblock.service.js
                height: tempEl.height,
            };

            containerStartRect = {
                top: temp.top + $window.pageYOffset,
                bottom: temp.bottom + $window.pageYOffset,
                right: temp.right + $window.pageXOffset,
                left: temp.left + $window.pageXOffset,
            };

            ctrl._elementStartRect = elementStartRect;
        };

        ctrl.resetClassesElement = function ($element) {
            $element[0].classList.remove('transformer-scroll-over');
            $element[0].classList.remove('transformer-scroll-over--top');
            $element[0].classList.remove('transformer-scroll-over--bottom');
            $element[0].classList.remove('transformer-freeze');
            $element[0].classList.remove('transformer-scroll-over--bottom');
            $element[0].style.bottom = 'auto';
            $element[0].style.top = 'auto';
        };

        ctrl.calc = function () {
            var freezeNew,
                containerComputedStyle = $window.getComputedStyle(container),
                containerRect = container.getBoundingClientRect(),
                elementRect = $element[0].getBoundingClientRect(),
                offsetByParents = transformerService.getOffsetByParents(ctrl),
                scrollOverNew = $window.pageYOffset + ctrl.offsetTop + offsetByParents > containerStartRect[ctrl.stickyPosition];

            var containerPaddingLeft = parseFloat(containerComputedStyle['padding-left']);
            var containerPaddingRight = parseFloat(containerComputedStyle['padding-right']);

            ctrl.styles.width = containerRect.width - containerPaddingLeft - containerPaddingRight;

            currentDirectionVerticalScroll = $window.pageYOffset >= pageYOffsetPrev ? 'bottom' : 'top';

            var scrollSize = $window.pageYOffset - pageYOffsetPrev;

            var isHaveScroll = document.documentElement.scrollWidth > document.documentElement.clientWidth;

            if (ctrl.limitVisibleScroll === true) {
                if ($window.pageYOffset < 0) {
                    //когда в Safari overscroll top
                    return;
                }

                if (elementRect.height + ctrl.offsetTop > $window.innerHeight) {
                    /*Если в Safari есть overscroll bottom*/
                    $element[0].classList.remove('transformer-sticky');
                    var scrollHeight = Math.max(
                        document.body.scrollHeight,
                        document.documentElement.scrollHeight,
                        document.body.offsetHeight,
                        document.documentElement.offsetHeight,
                        document.body.clientHeight,
                        document.documentElement.clientHeight,
                    );
                    if ($window.pageYOffset > scrollHeight - window.innerHeight + ctrl.scrollWidth) {
                        $element[0].classList.remove('transformer-scroll-over');
                        $element[0].classList.add('transformer-freeze');
                        $element[0].style.top = elementRect.top + $window.pageYOffset - elementStartRect.topWithScroll + 'px';
                        $element[0].style.bottom = 'auto';
                        $element[0].style.transform = 'translate3d(0px, 0px, 0)';

                        return;
                    }

                    if (ctrl.isResetClasses) {
                        ctrl.resetClassesElement($element);
                    }

                    if (directionVerticalScroll == null) {
                        //когда первая загрузка идет со скроллом
                        if (
                            elementRect.bottom + $window.pageYOffset + (isHaveScroll ? ctrl.scrollWidth : 0) <
                            $window.innerHeight + $window.pageYOffset
                        ) {
                            // может быть 0 если переходить из тача в desktop и считаваться неправильно

                            $element[0].classList.remove('transformer-freeze');
                            $element[0].classList.add('transformer-scroll-over');
                            $element[0].style.top = 'auto';
                            $element[0].style.bottom = 0 - elBoxIndents.marginBottom + 'px';
                        } else if (elementRect.top - ctrl.offsetTop - elementStartRect.topWithScroll + $window.pageYOffset > $window.pageYOffset) {
                            $element[0].classList.remove('transformer-freeze');
                            $element[0].classList.add('transformer-scroll-over');
                            $element[0].style.bottom = 'auto';
                            $element[0].style.top = 0 + 'px';
                        }
                    } else {
                        if (currentDirectionVerticalScroll !== directionVerticalScroll) {
                            $element[0].classList.remove('transformer-scroll-over');
                            $element[0].classList.add('transformer-freeze');
                            $element[0].style.top = elementRect.top + $window.pageYOffset - elementStartRect.topWithScroll + 'px';
                            $element[0].style.bottom = 'auto';

                            if (
                                elementRect.bottom + $window.pageYOffset + (isHaveScroll ? ctrl.scrollWidth : 0) + Math.abs(scrollSize) <
                                $window.innerHeight + $window.pageYOffset
                            ) {
                                $element[0].classList.remove('transformer-freeze');
                                $element[0].classList.add('transformer-scroll-over');
                                $element[0].style.top = 'auto';
                                $element[0].style.bottom = 0 - elBoxIndents.marginBottom + 'px';
                            } else if (
                                currentDirectionVerticalScroll === 'top' &&
                                elementStartRect.topWithScroll >= elementRect.top + $window.pageYOffset
                            ) {
                                ctrl.resetClassesElement($element);
                                return;
                            } else if (elementRect.top - ctrl.offsetTop + $window.pageYOffset - Math.abs(scrollSize) > $window.pageYOffset) {
                                $element[0].classList.remove('transformer-freeze');
                                $element[0].classList.add('transformer-scroll-over');
                                $element[0].style.bottom = 'auto';
                                $element[0].style.top = 0 + ctrl.offsetTop + 'px';
                            }
                        } else {
                            if (
                                elementRect.bottom + $window.pageYOffset + (isHaveScroll ? ctrl.scrollWidth : 0) <
                                $window.innerHeight + $window.pageYOffset
                            ) {
                                $element[0].classList.remove('transformer-freeze');

                                $element[0].classList.add('transformer-scroll-over');
                                $element[0].style.top = 'auto';
                                $element[0].style.bottom = 0 - elBoxIndents.marginBottom + 'px';
                            } else if (
                                currentDirectionVerticalScroll === 'top' &&
                                elementStartRect.topWithScroll >= elementRect.top + $window.pageYOffset
                            ) {
                                ctrl.resetClassesElement($element);
                                return;
                            } else if (
                                elementRect.top -
                                    (currentDirectionVerticalScroll === 'bottom' ? elementStartRect.topWithScroll : ctrl.offsetTop) +
                                    $window.pageYOffset >
                                $window.pageYOffset
                            ) {
                                $element[0].classList.remove('transformer-freeze');
                                $element[0].classList.add('transformer-scroll-over');
                                $element[0].style.bottom = 'auto';
                                $element[0].style.top = 0 + ctrl.offsetTop + 'px';
                            }
                        }
                    }

                    ctrl.isResetClasses = false;
                    pageYOffsetPrev = $window.pageYOffset;
                    directionVerticalScroll = currentDirectionVerticalScroll;
                } else {
                    $element[0].classList.remove('transformer-freeze');
                    $element[0].classList.remove('transformer-scroll-over--bottom');
                    $element[0].style.bottom = 'auto';
                    $element[0].classList.add('transformer-sticky');
                    $element[0].style.top = ctrl.offsetTop + elBoxIndents.marginTop + elWrapBoxIndents.paddingTop + 'px';
                    $element[0].style[ctrl.stickyHorizontalPosition] = 0;
                    ctrl.isResetClasses = true;
                }

                /*установление translate3d при горизонтальном скролле*/
                if ($element[0].classList.contains('transformer-scroll-over')) {
                    $element[0].style.transform =
                        'translate3d(' + ($window.pageXOffset <= 0 ? Math.abs($window.pageXOffset) : -$window.pageXOffset) + 'px, 0px, 0)';
                } else {
                    $element[0].style.transform = 'translate3d(0px, 0px, 0)';
                }
            } else {
                if (ctrl.limitPos === true) {
                    if (scrollOverNew === false) {
                        //default state
                        freezeNew = false;
                        ctrl.styles.transform = 'translate3d(0, 0, 0)';
                        ctrl.styles[ctrl.stickyPosition] = 'auto';
                        ctrl.expandMutableItems();
                    } else {
                        if (
                            containerRect.height + containerStartRect.top <=
                            elementRect.height + window.pageYOffset + ctrl.offsetTop + offsetByParents
                        ) {
                            //bottom sticky
                            freezeNew = true;
                            ctrl.styles.transform = 'translate3d(0, ' + (containerRect.height - elementRect.height) + 'px, 0)';
                            ctrl.styles[ctrl.stickyPosition] = 'auto';
                        } else {
                            //fixed
                            freezeNew = false;
                            ctrl.styles.transform = 'translate3d(0, 0, 0)';
                            ctrl.styles[ctrl.stickyPosition] =
                                ctrl.offsetTop +
                                offsetByParents +
                                (ctrl.stickyPosition === 'top' ? elWrapBoxIndents.paddingTop : elWrapBoxIndents.paddingBottom) +
                                'px';

                            var indentContainer = containerElementBoxIndents.paddingLeft + containerElementBoxIndents.marginLeft;

                            ctrl.styles.left = containerRect.left + indentContainer;

                            if (elementRect.height + ctrl.offsetTop > $window.innerHeight) {
                                ctrl.collapseMutableItems(elementRect.height + containerStartRect.top - $window.innerHeight);
                            } else if (ctrl.offsetTop + elementStartRect.height < $window.innerHeight) {
                                ctrl.expandMutableItems();
                            }
                        }
                    }
                } else {
                    ctrl.styles[ctrl.stickyPosition] = offsetByParents || 0;
                }

                if (freezeNew === true) {
                    ctrl.scrollOver = false;
                } else {
                    ctrl.scrollOver = scrollOverNew;
                }

                ctrl.freeze = freezeNew;

                if (ctrl.scrollOver === true) {
                    $element[0].classList.remove('transformer-scroll-default');
                    $element[0].classList.add('transformer-scroll-over');
                    $element[0].classList.add('transformer-scroll-over--' + ctrl.stickyPosition);
                } else if (ctrl.scrollOver === false) {
                    $element[0].classList.remove('transformer-scroll-over');
                    $element[0].classList.add('transformer-scroll-default');
                    ctrl.styles.backgroundColor = null;
                    ctrl.styles[ctrl.stickyPosition] = 'auto';
                }

                if (ctrl.freeze) {
                    $element[0].classList.add('transformer-freeze');
                } else {
                    $element[0].classList.remove('transformer-freeze');
                }

                $element.css(ctrl.styles);
            }
        };

        ctrl.windowScroll = function () {
            if (ctrl.initialize !== true) {
                ctrl.wait === true;
                return;
            }

            ctrl.calc();

            $scope.$digest();
        };

        ctrl.addMutableItem = function (element) {
            mutableItems.push({
                el: element,
                height: element.offsetHeight,
            });
        };

        ctrl.collapseMutableItems = function (dim) {
            var sumMutable = 0;

            for (var i = 0, len = mutableItems.length; i < len; i++) {
                if (sumMutable < dim) {
                    sumMutable += mutableItems[i].height;
                    mutableItems[i].el.classList.add('transformer-hidden');
                } else {
                    break;
                }
            }
        };

        ctrl.expandMutableItems = function () {
            for (var i = 0, len = mutableItems.length; i < len; i++) {
                mutableItems[i].el.classList.remove('transformer-hidden');
            }
        };

        ctrl.getBottomPoint = function () {
            return ctrl.offsetTop + transformerService.getOffsetByParents(ctrl) + $element[0].offsetHeight;
        };

        ctrl.getHeightElement = function () {
            return ctrl.offsetTop + $element[0].offsetHeight;
        };

        ctrl.start = function () {
            ctrl.isDestroyed = false;

            ctrl.elStartRect = $element[0].getBoundingClientRect();

            var containerLimit = document.getElementById($attrs.containerLimit) || document.body,
                parent = $element.parent();

            if (parent.length > 0) {
                ctrl.backupStylesParentString = parent[0].style.cssText;
            }

            //parent.css('minHeight', parent.height());
            parent.css('minHeight', $element.height()); //ставлю высоту элемента так как ui grid и canvas сжимается позже при ресайзе = неверная высота
            //parent.css('minWidth', parent.width());

            $element.addClass(ctrl.isTouchDevice === true ? 'transformer-touch' : 'transformer-notouch');

            ctrl.init(containerLimit);

            ctrl.initialize = true;

            if (ctrl.wait === true) {
                ctrl.calc();
                $scope.$digest();
            }
            var windowScrollFunc = function (event) {
                if (event.type === 'resize') {
                    ctrl.destroyAndLoad();
                    //setTimeout(ctrl.windowScroll.bind(ctrl), 0);
                } else {
                    ctrl.windowScroll();
                }
            };

            $window.addEventListener('scroll', windowScrollFunc, { passive: true });
            $window.addEventListener('resize', windowScrollFunc);

            return function () {
                ctrl.isDestroyed = true;

                transformerService.deleteFromStorageDestroyedCtrl(ctrl);

                $window.removeEventListener('scroll', windowScrollFunc);
                $window.removeEventListener('resize', windowScrollFunc);

                parent.css('minHeight', 'auto');
                parent.css('minWidth', 'auto');

                pageYOffsetPrev = 0;
                currentDirectionVerticalScroll = null;
                directionVerticalScroll = null;

                $element[0].style.cssText = ctrl.backupStylesElementString;
                if (parent.length > 0) {
                    parent[0].style.cssText = ctrl.backupStylesParentString;
                }

                $element.removeClass(ctrl.isTouchDevice === true ? 'transformer-touch' : 'transformer-notouch');
                $element.removeClass('transformer-scroll-default');
                $element.removeClass('transformer-scroll-over');
                $element.removeClass('transformer-scroll-over--' + ctrl.stickyPosition);
                $element.removeClass('transformer-freeze');
            };
        };

        ctrl.destroy = function () {
            ctrl.destroyFn();
        };

        ctrl.toggleDestroyOrLoad = function (state) {
            setTimeout(function () {
                //нужен для того если меняются стили чтобы они успевали проинициализироваться
                if (state) {
                    ctrl.destroyFn = ctrl.start();
                } else {
                    ctrl.destroy();
                }
            }, 0);
        };

        ctrl.destroyAndLoad = function () {
            ctrl.destroy();

            setTimeout(() => {
                ctrl.destroyFn = ctrl.start();
            }, 0);

            //ctrl.load();
        };

        ctrl.checkLoadDocument = function () {
            return $document[0].readyState === 'complete';
        };

        ctrl.getStateDestroy = function () {
            return ctrl.isDestroyed;
        };
    };

    angular.module('transformer').controller('TransformerCtrl', TransformerCtrl);

    TransformerCtrl.$inject = ['$window', '$element', '$scope', 'transformerService', '$attrs', '$document'];
})(window.angular);
