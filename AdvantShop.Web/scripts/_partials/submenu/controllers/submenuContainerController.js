(function (ng) {
    'use strict';

    var SubmenuContainerCtrl = function ($element, $window, $scope, windowService, submenuService, domService, $timeout) {
        var ctrl = this,
            blockOrientation,
            timeoutMove,
            timeoutLeave,
            timeoutHover,
            possiblyActivate,
            submenuShow;

        ctrl.items = [];

        ctrl.active = [];

        ctrl.options = {};

        ctrl.handleEvent = function (event) {
            submenuService.addCurrentSubmenuContainer(ctrl);

            switch (event.type) {
                case 'touchstart':
                    (function (event) {
                        event.stopPropagation();

                        var submenuParentEl = domService.closest(event.target, '[data-submenu-parent]', '[data-submenu-container]'),
                            submenuParentLink = domService.closest(event.target, 'a', '[data-submenu-container]'),
                            submenuParent;

                        if (submenuParentEl != null) {
                            submenuParent = ctrl.items[submenuParentEl.getAttribute('data-submenu-parent-index')];

                            if (submenuParent == null || submenuParent.submenu == null) {
                                return;
                            }

                            if (
                                submenuParentLink != null &&
                                (submenuParent.submenu.isSubmenuVisible == false || submenuParent.submenu.isSubmenuVisible == null)
                            ) {
                                event.preventDefault();
                                submenuShow(submenuParent); //переместил так как в сафари при тапе поссыфлка ничего не происходило
                            }
                        }
                    })(event);
                    break;
                case 'mouseenter':
                    (function () {
                        //var otherActive = submenuService.closeAnotherMenu();

                        //if (otherActive != null) {
                        //    otherActive.getBlockOrientation().style.zIndex = 0;
                        //}

                        submenuService.startSpyMove();

                        if (timeoutLeave != null) {
                            clearTimeout(timeoutLeave);
                        }
                    })(event);
                    break;
                case 'mouseover':
                    (function (event) {
                        if (timeoutMove != null) {
                            clearTimeout(timeoutMove);
                        }

                        if (timeoutHover != null) {
                            clearTimeout(timeoutHover);
                        }

                        timeoutHover = setTimeout(function () {
                            var submenuElLimit = domService.closest(event.target, '[data-submenu-parent]', '[data-submenu]'),
                                submenuEl = domService.closest(event.target, '[data-submenu]', submenuElLimit),
                                submenuParentIndex,
                                submenuParent;

                            //если событие было вызвано элементом в data-submenu-parent, а не в data-submenu
                            if (submenuEl == null && submenuElLimit != null) {
                                submenuParent = ctrl.items[submenuElLimit.getAttribute('data-submenu-parent-index')];

                                if (submenuParent != null) {
                                    possiblyActivate(submenuParent);
                                }
                            }
                        }, ctrl.options.delayHover);
                    })(event);
                    break;
                case 'mouseleave':
                    (function (event) {
                        if (timeoutHover != null) {
                            clearTimeout(timeoutHover);
                        }

                        timeoutLeave = setTimeout(function () {
                            submenuService.stopSpyMove();

                            if (timeoutMove != null) {
                                clearTimeout(timeoutMove);
                            }

                            ctrl.deactiveAll(ctrl.getBlockOrientation());
                            ctrl.getBlockOrientation().style.zIndex = 0;
                            $scope.$digest();
                        }, 350);
                    })(event);
                    break;
                default:
                    return;
            }
        };

        ctrl.init = function (options) {
            ctrl.limitedViewOffsetBottomArrClasses = null;
            ctrl.container = $element[0];
            if (options.type !== 'accordion' && options.type !== 'treeview') {
                submenuShow = function (submenuParent) {
                    ctrl.getBlockOrientation().style.zIndex = 999999;

                    ctrl.deactive(submenuParent);

                    if (submenuParent.submenu != null) {
                        submenuParent.submenu.hiddenSubmenu();

                        if (options.checkOrientation === true) {
                            submenuParent.submenu.checkSubmenuOrientation(
                                ctrl.getContainerRect(),
                                submenuParent.submenu.options.verticalOrientation,
                                blockOrientation || $element[0],
                            );
                        }

                        if (submenuParent.submenu.isInit == null || submenuParent.submenu.isInit === false) {
                            setTimeout(function () {
                                submenuParent.submenu.setInitilazed();
                                submenuParent.submenu.open();
                                submenuParent.submenu.visibleSubmenu();
                                $scope.$digest();
                            });
                        } else {
                            submenuParent.submenu.open();
                            submenuParent.submenu.visibleSubmenu();
                        }
                    }

                    $scope.$digest();
                };

                possiblyActivate = function (submenuParent) {
                    var delay = 0;

                    if (submenuParent.submenu != null) {
                        delay = submenuParent.submenu.checkInTriangle(ctrl.getContainerRect());
                    }

                    if (timeoutMove != null) {
                        clearTimeout(timeoutMove);
                    }

                    if (delay) {
                        timeoutMove = setTimeout(function () {
                            possiblyActivate(submenuParent);
                        }, delay);
                    } else {
                        submenuShow(submenuParent);
                    }
                };

                $element[0].addEventListener('touchstart', ctrl.handleEvent, { passive: true });

                windowService.addCallback('touchstart', function (eventObj) {
                    var isClickedMe = domService.closest(eventObj.event.target, '[data-submenu-container]') != null;

                    if (isClickedMe === false) {
                        ctrl.deactiveAll();
                        $scope.$digest();
                    }
                });

                $element[0].addEventListener('mouseenter', ctrl.handleEvent);

                $element[0].addEventListener('mouseover', ctrl.handleEvent);

                $element[0].addEventListener('mouseleave', ctrl.handleEvent);
            }

            if (options.limitedView === true) {
                ctrl.limitedViewOffsetBottomArrClasses =
                    ctrl.options?.limitedViewOffsetBottom != null && Array.isArray(ctrl.options?.limitedViewOffsetBottom)
                        ? ctrl.options?.limitedViewOffsetBottom
                        : [];
                ctrl.limitedViewOffsetBottom = ctrl.options?.limitedViewOffsetBottom;
                setTimeout(() => {
                    ctrl.limitedVerticalView(ctrl.items);
                }, 0);
            }
        };

        ctrl.reinit = function (options) {
            $element[0].removeEventListener('touchstart', ctrl.handleEvent, { passive: true });

            $element[0].removeEventListener('mouseenter', ctrl.handleEvent);

            $element[0].removeEventListener('mouseover', ctrl.handleEvent);

            $element[0].removeEventListener('mouseleave', ctrl.handleEvent);

            ctrl.deactiveAll(ctrl.getBlockOrientation());

            if (ctrl.activeSubmenus != null && ctrl.activeSubmenus.length > 0) {
                ctrl.activeSubmenus.forEach(function (submenu) {
                    submenu.close();
                });
            }

            ctrl.init(options);
        };

        ctrl.addContainerForOrientation = function (element) {
            blockOrientation = element;
        };

        ctrl.getBlockOrientation = function () {
            return blockOrientation;
        };

        ctrl.getContainerRect = function () {
            var rect;

            //if (ctrl.rect == null) {
            rect = blockOrientation.getBoundingClientRect();
            ctrl.rect = {
                top: rect.top + $window.pageYOffset,
                right: rect.right + $window.pageXOffset,
                bottom: rect.bottom,
                left: rect.left,
                height: rect.height,
                width: rect.width,
            };
            //}
            return ctrl.rect;
        };

        ctrl.addItem = function (item) {
            ctrl.items.push(item);
        };

        ctrl.deactiveAll = function () {
            for (var i = 0, len = ctrl.active.length; i < len; i++) {
                if (ctrl.active[i] != null && ctrl.active[i].submenu != null) {
                    ctrl.active[i].submenu.close();
                }
            }
        };

        ctrl.deactive = function (submenuParent) {
            ctrl.memoryActive(submenuParent);
        };

        ctrl.showOneOnly = function (submenuParent, event) {
            var method,
                rect = submenuParent.element.getBoundingClientRect();
            ctrl.activeSubmenus = [];
            for (var i = 0, len = ctrl.items.length; i < len; i++) {
                if (ctrl.items[i] != null && ctrl.items[i].submenu != null) {
                    method = ctrl.items[i] === submenuParent && submenuParent.submenu.isSubmenuVisible != true ? 'open' : 'close';
                    if (method === 'open') {
                        ctrl.activeSubmenus.push(ctrl.items[i].submenu);
                    }
                    ctrl.items[i].submenu[method]();
                }
            }

            if (event != null) {
                setTimeout(function () {
                    var currentRect = submenuParent.element.getBoundingClientRect();

                    if (currentRect.top != rect.top && (currentRect.bottom > $window.innerHeight || currentRect.top < 0)) {
                        //event.target.scrollIntoView(true);
                        var scrollValue;

                        //if (currentRect.bottom > $window.innerHeight) {
                        //    scrollValue = rect.top - currentRect.top;
                        //}

                        if (currentRect.top < 0) {
                            scrollValue = currentRect.top - rect.top;
                        }

                        $window.scrollBy(0, scrollValue);
                    }
                }, 0);
            }
        };

        ctrl.getOptions = function () {
            return ctrl.options;
        };

        ctrl.excludeDeactivateItems = function (parent) {
            if (parent == null) {
                return;
            }

            var isFind = false,
                newActive = [];

            for (var i = 0, len = ctrl.active.length; i < len; i++) {
                //if (ctrl.active[i] != null && ctrl.active[i].submenu != null) {
                //    ctrl.active[i].submenu.close();
                //}

                isFind = ctrl.findParent(ctrl.active[i], parent.parent);

                if (isFind) {
                    newActive.push(ctrl.active[i]);
                } else {
                    if (ctrl.active[i].submenu != null) {
                        ctrl.active[i].submenu.close();
                    }
                }
            }

            ctrl.active = newActive;
        };

        ctrl.findParent = function (item, currentParent) {
            var result = false;

            if (currentParent != null) {
                if (item === currentParent) {
                    result = true;
                } else {
                    result = ctrl.findParent(item, currentParent.parent);
                }
            }

            return result;
        };

        ctrl.memoryActive = function (activeParent) {
            ctrl.excludeDeactivateItems(activeParent);
            ctrl.active.push(activeParent);
        };

        ctrl.onChangeMatchMedia = function (changesOnBreakpointObj, matchesObj) {
            var changesOnBreakpoint = changesOnBreakpointObj;
            if (!matchesObj.matches && changesOnBreakpointObj.type !== ctrl.options.type) {
                ctrl.activeMatch = changesOnBreakpointObj;
                ctrl.reinit(changesOnBreakpoint);
            } else if (changesOnBreakpointObj.type !== ctrl.options.type) {
                ctrl.activeMatchString = ctrl.options.type;
                ctrl.reinit(ctrl.options);
            }

            return function (event) {
                if (!event.matches) {
                    ctrl.activeMatch = changesOnBreakpoint;
                    ctrl.reinit(changesOnBreakpoint);
                } else {
                    ctrl.activeMatch = ctrl.options;
                    ctrl.reinit(ctrl.options);
                }
            };
        };
        ctrl.limitedVerticalView = function (items) {
            const containerMenuCoords = $element[0].getBoundingClientRect();
            const viewAllBtn = $element[0].querySelector('.js-menu-dropdown-give-more-link');
            if (viewAllBtn && containerMenuCoords.bottom + $window.pageYOffset > window.innerHeight) {
                viewAllBtn.style.display = 'flex';
                const getMoreItem = $element[0].children[$element[0].children.length - 1];
                let coordsBot = containerMenuCoords.bottom + $window.pageYOffset + getMoreItem.offsetHeight;
                let count = items.length;
                const minCountItem = 7;
                const offsetBottom = ctrl.getOffsetElements(ctrl.limitedViewOffsetBottomArrClasses);
                while (coordsBot > window.innerHeight - offsetBottom && items.length - count < items.length - minCountItem) {
                    const item = items[count - 1].element;
                    coordsBot -= item.offsetHeight;
                    item.classList.add('menu-dropdown-item--hidden');
                    item.style.display = 'none';
                    count--;
                }
            } else {
                viewAllBtn?.remove();
            }
        };

        ctrl.getOffsetElements = function (arrClasses) {
            return arrClasses.reduce((prev, cur) => {
                const el = document.querySelector(cur);
                let elementHeight = 0;
                if (el) {
                    elementHeight = el.offsetHeight;
                }
                return prev + elementHeight;
            }, 0);
        };
    };

    angular.module('submenu').controller('SubmenuContainerCtrl', SubmenuContainerCtrl);

    SubmenuContainerCtrl.$inject = ['$element', '$window', '$scope', 'windowService', 'submenuService', 'domService', '$timeout'];
})(window.angular);
