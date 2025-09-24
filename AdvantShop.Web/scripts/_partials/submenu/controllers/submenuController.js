(function (ng) {
    'use strict';

    var SubmenuCtrl = function ($element, $timeout, submenuService, submenuConfig) {
        var ctrl = this,
            lastDelayLoc;

        //ctrl.isSubmenuVisible = false;
        //ctrl.isHiddenSubmenu = false;

        ctrl.$postLink = function () {
            if (ctrl.isSubmenuVisible === true) {
                $element[0].classList.add('show-submenu');
            } else {
                $element[0].classList.remove('show-submenu');
            }

            if (ctrl.isHiddenSubmenu === true) {
                $element[0].classList.add('visible-submenu');
            } else {
                $element[0].classList.remove('visible-submenu');
            }

            ctrl.originalCountCols = Number(getComputedStyle($element[0]).getPropertyValue('--dropdownSubCountColsProductsInRow'));
        };

        ctrl.visibleSubmenu = function () {
            ctrl.isHiddenSubmenu = false;
            $element[0].classList.remove('visible-submenu');
        };

        ctrl.hiddenSubmenu = function () {
            ctrl.isHiddenSubmenu = true;
            $element[0].classList.add('visible-submenu');
        };

        ctrl.open = function () {
            if (ctrl.originalCountCols > 0) {
                ctrl.checkSubmenuOutsideWindow();
            }
            ctrl.isSubmenuVisible = true;
            $element[0].classList.add('show-submenu');
        };

        ctrl.checkSubmenuOutsideWindow = function () {
            const menuDropdownSubInner = $element[0].querySelector('.menu-dropdown-sub-inner');

            if (
                submenuService.isSubmenuOutsideWindow(menuDropdownSubInner) &&
                ctrl.options.type !== 'accordion' &&
                ctrl.options.type !== 'treeview'
            ) {
                ctrl.setCountColsProductInSubmenu(menuDropdownSubInner);
            }
        };

        ctrl.setCountColsProductInSubmenu = function (menuDropdownSubInner) {
            $element[0].style.setProperty(
                `--dropdownSubCountColsProductsInRow`,
                `${submenuService.calcCountColsProduct(
                    submenuService.getDiffSubmenuAndWindow(menuDropdownSubInner),
                    menuDropdownSubInner.querySelector('.menu-dropdown-sub-columns-item-category').clientWidth,
                    ctrl.originalCountCols,
                )}`,
            );

            if (submenuService.getSubmenuRect(menuDropdownSubInner).left < 0) {
                ctrl.checkSubmenuOrientation(
                    ctrl.submenuContainer.getContainerRect(),
                    ctrl.options.verticalOrientation,
                    ctrl.submenuContainer.getBlockOrientation() || ctrl.submenuContainer.container,
                );
            }
        };

        ctrl.close = function () {
            ctrl.isSubmenuVisible = false;
            $element[0].classList.remove('show-submenu');
            if (ctrl.originalCountCols > 0) {
                ctrl.setOriginalCountColsProductInSubmenu();
            }
        };

        ctrl.setOriginalCountColsProductInSubmenu = function () {
            $element[0].style.setProperty('--dropdownSubCountColsProductsInRow', `${ctrl.originalCountCols}`);
        };

        ctrl.toggle = function () {
            ctrl.isSubmenuVisible ? ctrl.close() : ctrl.open();
        };

        ctrl.touchClick = function (event) {
            if (ctrl.isSubmenuVisible === false) {
                event.preventDefault();
            }
        };

        //https://github.com/kamens/jQuery-menu-aim
        ctrl.checkInTriangle = function (containerRect) {
            var mouseLocs = submenuService.getMouseLocs(),
                upperLeft = {
                    x: containerRect.left,
                    y: containerRect.top - ctrl.options.tolerance,
                },
                upperRight = {
                    x: containerRect.left + containerRect.width,
                    y: upperLeft.y,
                },
                lowerLeft = {
                    x: containerRect.left,
                    y: containerRect.top + containerRect.height + ctrl.options.tolerance,
                },
                lowerRight = {
                    x: containerRect.left + containerRect.width,
                    y: lowerLeft.y,
                },
                loc = mouseLocs[mouseLocs.length - 1],
                prevLoc = mouseLocs[0];

            if (!loc) {
                return 0;
            }

            if (!prevLoc) {
                prevLoc = loc;
            }

            if (prevLoc.x < containerRect.left || prevLoc.x > lowerRight.x || prevLoc.y < containerRect.top || prevLoc.y > lowerRight.y) {
                // If the previous mouse location was outside of the entire
                // menu's bounds, immediately activate.
                return 0;
            }

            if (lastDelayLoc && loc.x == lastDelayLoc.x && loc.y == lastDelayLoc.y) {
                // If the mouse hasn't moved since the last time we checked
                // for activation status, immediately activate.
                return 0;
            }

            // Detect if the user is moving towards the currently activated
            // submenu.
            //
            // If the mouse is heading relatively clearly towards
            // the submenu's content, we should wait and give the user more
            // time before activating a new row. If the mouse is heading
            // elsewhere, we can immediately activate a new row.
            //
            // We detect this by calculating the slope formed between the
            // current mouse location and the upper/lower right points of
            // the menu. We do the same for the previous mouse location.
            // If the current mouse location's slopes are
            // increasing/decreasing appropriately compared to the
            // previous's, we know the user is moving toward the submenu.
            //
            // Note that since the y-axis increases as the cursor moves
            // down the screen, we are looking for the slope between the
            // cursor and the upper right corner to decrease over time, not
            // increase (somewhat counterintuitively).
            function slope(a, b) {
                return (b.y - a.y) / (b.x - a.x);
            }

            var decreasingCorner = upperRight,
                increasingCorner = lowerRight;

            // Our expectations for decreasing or increasing slope values
            // depends on which direction the submenu opens relative to the
            // main menu. By default, if the menu opens on the right, we
            // expect the slope between the cursor and the upper right
            // corner to decrease over time, as explained above. If the
            // submenu opens in a different direction, we change our slope
            // expectations.

            if (ctrl.options.submenuDirection == 'left') {
                decreasingCorner = lowerLeft;
                increasingCorner = upperLeft;
            } else if (ctrl.options.submenuDirection == 'below') {
                decreasingCorner = lowerRight;
                increasingCorner = lowerLeft;
            } else if (ctrl.options.submenuDirection == 'above') {
                decreasingCorner = upperLeft;
                increasingCorner = upperRight;
            }

            var decreasingSlope = slope(loc, decreasingCorner),
                increasingSlope = slope(loc, increasingCorner),
                prevDecreasingSlope = slope(prevLoc, decreasingCorner),
                prevIncreasingSlope = slope(prevLoc, increasingCorner);

            if (decreasingSlope < prevDecreasingSlope && increasingSlope > prevIncreasingSlope) {
                // Mouse is moving from previous location towards the
                // currently activated submenu. Delay before activating a
                // new menu row, because user may be moving into submenu.
                lastDelayLoc = loc;
                return ctrl.options.delay;
            }

            lastDelayLoc = null;
            return 0;
        };

        ctrl.checkSubmenuOrientation = function (containerRect, verticalOrientation, blockOrientation) {
            var prop = verticalOrientation ? 'bottom' : 'right',
                submenuRect,
                scrollDiff = blockOrientation.offsetWidth - blockOrientation.clientWidth,
                needMoving;

            $element.css(verticalOrientation ? 'marginTop' : 'marginLeft', 0 + 'px');

            submenuRect = $element[0].getBoundingClientRect();

            needMoving = containerRect[prop] < submenuRect[prop] + ctrl.offset[prop];

            if (needMoving === true) {
                $element.css(
                    verticalOrientation ? 'marginTop' : 'marginLeft',
                    containerRect[prop] - (submenuRect[prop] + ctrl.offset[prop] + scrollDiff) + 'px',
                );
            }
        };

        ctrl.setInitilazed = function () {
            ctrl.isInit = true;
            $element.addClass('submenu-initialized');
        };
    };

    angular.module('submenu').controller('SubmenuCtrl', SubmenuCtrl);

    SubmenuCtrl.$inject = ['$element', '$timeout', 'submenuService', 'submenuConfig'];
})(window.angular);
