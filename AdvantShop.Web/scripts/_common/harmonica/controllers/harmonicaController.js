(function (ng, body) {
    'use strict';

    var HarmonicaCtrl = function ($element, $attrs, $q) {
        var ctrl = this,
            tileWidth = null,
            scopeTile,
            tileDefer = $q.defer(),
            itemWidthProcessed = false;

        ctrl.$onInit = function () {
            ctrl.harmonicaClassTile = $attrs.harmonicaClassTile;
            ctrl.harmonicaClassTileRow = $attrs.harmonicaClassTileRow;
            ctrl.harmonicaClassTileLink = $attrs.harmonicaClassTileLink;
            ctrl.harmonicaClassTileSubmenu = $attrs.harmonicaClassTileSubmenu;
            ctrl.harmonicaTileOuterWidth = $attrs.harmonicaTileOuterWidth;
            ctrl.harmonicaTileOnOpen = $attrs.harmonicaTileOnOpen;

            ctrl.links = [];
            ctrl.items = [];
        };

        ctrl.addItem = function (itemElement, itemScope) {
            ctrl.items.push({
                itemElement: itemElement,
                itemScope: itemScope,
            });
        };

        ctrl.addLink = function (linkHref, linkText, linkClassesInTile, linkTarget, linkCopyAttrs, linkScope) {
            ctrl.links.push({
                linkHref,
                linkText,
                linkScope,
                linkTarget,
                linkCopyAttrs,
                linkClassesInTile,
            });
        };

        ctrl.getLinks = function () {
            return ctrl.links;
        };

        ctrl.saveTileScope = function (scope) {
            scopeTile = scope;
            tileDefer.resolve(scope);
        };

        ctrl.calc = function (containerWidth, items) {
            containerWidth = containerWidth || Math.ceil($element[0].getBoundingClientRect().width); //$element[0].offsetWidth
            items = items || ctrl.items;

            var sumWidth = 0,
                sliceIndex = null,
                dimSumWidth = 0;

            for (var i = 0, l = items.length; i < l; i++) {
                sumWidth += ctrl.getItemWidth(items[i]);

                if (containerWidth < sumWidth) {
                    sliceIndex = i;
                    break;
                }
            }

            if (sliceIndex === null) {
                sliceIndex = items.length;
            } else {
                dimSumWidth = ctrl.calcSumWidth(items.slice(0, sliceIndex));

                tileWidth = tileWidth || angular.isDefined(ctrl.harmonicaTileOuterWidth) ? parseInt(ctrl.harmonicaTileOuterWidth) : 0;

                while (containerWidth < dimSumWidth + tileWidth && sliceIndex !== 0) {
                    sliceIndex -= 1;
                    dimSumWidth -= ctrl.getItemWidth(items[sliceIndex]);
                }
            }

            return sliceIndex;
        };

        ctrl.calcSumWidth = function (items) {
            return items.reduce(function (accumulator, currentValue) {
                return accumulator + ctrl.getItemWidth(currentValue);
            }, 0);
        };

        ctrl.setVisible = function (indexHidden) {
            ctrl.setVisibleForItems(indexHidden, ctrl.items);
            ctrl.setVisibleForLinks(indexHidden, ctrl.links);

            if (angular.isDefined(scopeTile)) {
                scopeTile.isVisibleTile = indexHidden !== ctrl.items.length;
                if (scopeTile.isVisibleTile) {
                    scopeTile.setVisibleTile();
                }
            }
        };

        ctrl.getCssClassesForTile = function () {
            return {
                harmonicaClassTile: ctrl.harmonicaClassTile,
                harmonicaClassTileRow: ctrl.harmonicaClassTileRow,
                harmonicaClassTileLink: ctrl.harmonicaClassTileLink,
                harmonicaClassTileSubmenu: ctrl.harmonicaClassTileSubmenu,
            };
        };

        ctrl.setVisibleForItems = function (indexHidden, items) {
            for (var i = 0, l = items.length; i < l; i++) {
                items[i].itemScope.isVisibleInMenu = i < indexHidden;
            }
        };

        ctrl.setVisibleForLinks = function (indexHidden, links) {
            for (var i = 0, l = links.length; i < l; i++) {
                links[i].linkScope.isVisibleInTile = i >= indexHidden;
            }
        };

        ctrl.getOuterWidth = function (element) {
            var el = element[0] != null ? element[0] : element,
                computedStyle = window.getComputedStyle(el);

            return (
                parseFloat(element[0].getBoundingClientRect().width) + parseFloat(computedStyle.marginLeft) + parseFloat(computedStyle.marginRight)
            );
        };

        ctrl.start = function () {
            tileDefer.promise.then(function () {
                if (itemWidthProcessed === false) {
                    ctrl.items.forEach((itemsItem) => {
                        itemsItem.itemWidth = ctrl.getOuterWidth(itemsItem.itemElement);
                    });
                }

                itemWidthProcessed = true;

                var index = ctrl.calc();
                ctrl.setVisible(index);
                ctrl.active = true;
                $element.addClass(`harmonica-initialized`);
                $element.addClass('harmonica-post-calc');
            });
        };

        ctrl.stop = function () {
            tileDefer.promise.then(function () {
                ctrl.setVisible(ctrl.items.length);
                ctrl.active = false;
            });
        };

        ctrl.getItemWidth = function (item) {
            if (!item.itemWidth && isNaN(item.itemWidth)) {
                item.itemWidth = ctrl.getOuterWidth(item.itemElement);
            }

            return item.itemWidth && !isNaN(item.itemWidth) ? item.itemWidth : 0;
        };
    };

    angular.module('harmonica').controller('HarmonicaCtrl', HarmonicaCtrl);

    HarmonicaCtrl.$inject = ['$element', '$attrs', '$q'];
})(angular, document.body);
