(function (ng) {
    'use strict';

    var _currentActiveSubmenuContainer;

    var submenuService = function ($document) {
        const service = this,
            mouseLocs = [];
        service.getSubmenuRect = function (submenu) {
            const rect = submenu.getBoundingClientRect();
            return {
                top: rect.top + window.pageYOffset,
                right: rect.right + window.pageXOffset,
                bottom: rect.bottom,
                left: rect.left,
                height: rect.height,
                width: rect.width,
            };
        };
        service.startSpyMove = function () {
            $document[0].addEventListener('mousemove', memory);
        };

        service.stopSpyMove = function () {
            $document[0].removeEventListener('mousemove', memory);
            mouseLocs.length = 0;
        };

        service.getMouseLocs = function () {
            return mouseLocs;
        };

        service.addCurrentSubmenuContainer = function (submenuContainer) {
            if (_currentActiveSubmenuContainer !== submenuContainer) {
                service.closeAnotherMenu(_currentActiveSubmenuContainer);
                _currentActiveSubmenuContainer = submenuContainer;
            }
        };

        service.closeAnotherMenu = function (currentActiveSubmenuContainer) {
            var _container = currentActiveSubmenuContainer || _currentActiveSubmenuContainer;

            if (_container != null) {
                _container.deactiveAll();
                _container.getBlockOrientation().style.zIndex = 0;
                return _container;
            }
        };

        function memory(e) {
            mouseLocs.push({ x: e.pageX, y: e.pageY });

            if (mouseLocs.length > 3) {
                mouseLocs.shift();
            }
        }

        service.isSubmenuOutsideWindow = function (submenu) {
            if (submenu != null) {
                return submenu.getBoundingClientRect().right >= window.innerWidth || submenu.getBoundingClientRect().left < 0;
            }
        };

        service.getDiffSubmenuAndWindow = function (submenu) {
            const { left, right } = service.getSubmenuRect(submenu);
            const diffRight = right - window.innerWidth;

            if (-left > 0 && diffRight > 0) {
                return Math.max(-left, diffRight);
            }

            if (-left > 0) {
                return -left;
            } else if (diffRight > 0) {
                return diffRight;
            }
            return 0;
        };

        /**
         *  calculation new count columns for product in menu and set count in css variables "dropdownSubCountColsProductsInRow"
         * @param {Number} widthSubmenuOutsideWindow - width that goes outside the window.
         * @example
         * Example service:
         * submenuService.getDiffSubmenuAndWindow(submenu)
         * @param {Number} menuDropdownSubCategoryClientWidth - menu dropdown category list client width
         * @param {Number} countCols - original count columns
         * @returns {Number} new count columns
         */

        service.calcCountColsProduct = function (widthSubmenuOutsideWindow, menuDropdownSubCategoryClientWidth, countCols) {
            const widthCols = Math.round(menuDropdownSubCategoryClientWidth / countCols);
            const countCloseColsOutsideWindow = widthSubmenuOutsideWindow / widthCols;

            return countCols - Math.ceil(countCloseColsOutsideWindow);
        };
    };

    angular.module('submenu').service('submenuService', submenuService);

    submenuService.$inject = ['$document'];
})(window.angular);
