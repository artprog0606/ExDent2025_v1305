(function (ng) {
    'use strict';

    var CatalogFilterCtrl = /* @ngInject */ function (
        $http,
        $window,
        $timeout,
        popoverService,
        domService,
        catalogFilterService,
        catalogFilterAdvPopoverOptionsDefault,
    ) {
        var ctrl = this,
            pageParameters,
            timerPopoverHide,
            timerRange;

        ctrl.$onInit = function () {
            pageParameters = catalogFilterService.parseSearchString($window.location.search);

            ctrl.countVisibleCollapse = ctrl.countVisibleCollapse() || 10;
            ctrl.storageSelectedItems = {};
            ctrl.collapsed = true;
            ctrl.isRenderBlock = false;
            ctrl.isLoaded = false;

            ctrl.itemsOptions = [];
            ctrl.advPopoverOptions = ng.extend({}, catalogFilterAdvPopoverOptionsDefault, ctrl.advPopoverOptions);

            ctrl.init();

            if (ctrl.onInit != null) {
                ctrl.onInit({ catalogFilter: ctrl });
            }
        };

        ctrl.init = function () {
            ctrl.getFilterData().then(function (catalogFilterData) {
                ctrl.catalogFilterData = catalogFilterData.map(function (filter) {
                    filter.dirty = false;
                    return filter;
                });
                ctrl.isRenderBlock = catalogFilterData != null && catalogFilterData.length > 0;
                ctrl.isLoaded = true;
                if (ctrl.onFilterInit) {
                    ctrl.onFilterInit({ visible: ctrl.isRenderBlock });
                }

                //Fill all <select> ng-model by properly <option> on filter initialization
                var selectIndex;
                for (var i = 0, length = catalogFilterData.length; i < length; i++) {
                    if (
                        catalogFilterData[i] != null &&
                        (catalogFilterData[i].Control == 'select' || catalogFilterData[i].Control == 'selectSearch')
                    ) {
                        for (var j = 0, len = catalogFilterData[i].Values.length; j < len; j++) {
                            if (catalogFilterData[i].Values[j].Selected) {
                                selectIndex = j;
                                break;
                            }
                        }
                        catalogFilterData[i].Selected = catalogFilterData[i].Values[selectIndex];
                        selectIndex = -1;
                    } else {
                        ctrl.itemsOptions[i] = {
                            countVisibleItems: ctrl.countVisibleCollapse,
                            collapsed: true,
                        };
                    }
                }

                catalogFilterService.saveFilterData(catalogFilterData);
            });
        };

        ctrl.getCssClassForContent = function (controlType) {
            var cssClasses = {};

            cssClasses['catalog-filter-block-content-' + controlType] = true;

            return cssClasses;
        };

        ctrl.inputKeypress = function ($event, indexFilter) {
            if (timerPopoverHide != null) {
                $timeout.cancel(timerPopoverHide);
            }

            timerPopoverHide = $timeout(function () {
                var element = $event.currentTarget.parentNode;

                if (element != null) {
                    ctrl.changeItem(element, indexFilter);
                }
            }, 1200);
        };

        ctrl.clickCheckbox = function ($event, indexFilter) {
            var element = domService.closest($event.target, '.catalog-filter-row');

            if (element != null) {
                ctrl.changeItem(element, indexFilter);
            }
        };

        ctrl.clickSelect = function ($event, indexFilter, item, selectedItemIndex) {
            var element = $event.currentTarget.parentNode.parentNode;

            if (element != null) {
                let selected;
                for (let i = 0; i < ctrl.catalogFilterData[indexFilter].Values.length; i++) {
                    if (i === selectedItemIndex) {
                        selected = ctrl.catalogFilterData[indexFilter].Values[i];
                        ctrl.catalogFilterData[indexFilter].Values[i].Selected = true;
                    } else {
                        ctrl.catalogFilterData[indexFilter].Values[i].Selected = false;
                    }
                }
                item.Selected = selected;
                if (item.Selected == null) {
                    delete ctrl.storageSelectedItems[indexFilter.toString()];
                }
                ctrl.changeItem(element, indexFilter);
            }
        };

        ctrl.clickRangeDown = function (event, originalEvent, indexFilter) {
            ctrl.rangeElementClicked = originalEvent.target;
        };

        ctrl.clickRange = function (event, indexFilter) {
            if (timerRange != null) {
                $timeout.cancel(timerRange);
            }

            timerRange = $timeout(function () {
                var element = domService.closest(event.target, '.js-range-slider-block');

                //if (element != null) {
                ctrl.changeItem(element, indexFilter);
                //}

                ctrl.rangeElementClicked = null;
            }, 500);
        };

        ctrl.changeColor = function (event, indexFilter) {
            var element = domService.closest(event.target, '.js-color-viewer');

            if (element != null) {
                ctrl.changeItem(element, indexFilter);
            }
        };

        ctrl.changeItem = function (element, indexFilter) {
            var selectedItems, params;
            if (indexFilter != null) {
                ctrl.catalogFilterData[indexFilter].dirty = true;
                ctrl.updateSelectedItems(indexFilter);
            }
            ctrl.submit();
        };

        ctrl.toggleVisible = function (totalItems, index) {
            ctrl.itemsOptions[index].countVisibleItems = ctrl.itemsOptions[index].collapsed === true ? totalItems : ctrl.countVisibleCollapse;
            ctrl.itemsOptions[index].collapsed = !ctrl.itemsOptions[index].collapsed;
        };

        ctrl.reset = function (index) {
            for (const itemValue of ctrl.catalogFilterData[index].Values) {
                itemValue.Selected = false;
            }

            delete ctrl.storageSelectedItems[index.toString()];

            ctrl.filterSelectedData = catalogFilterService.getSelectedData(ctrl.catalogFilterData);
            ctrl.filter();

            ctrl.init();
        };

        ctrl.submit = function () {
            ctrl.filterSelectedData = catalogFilterService.getSelectedData(ctrl.catalogFilterData);
            ctrl.filter();
        };

        ctrl.getFilterCount = function (filterString) {
            return $http
                .get(ctrl.urlCount + (filterString != null && filterString.length > 0 ? '?' + filterString : ''), {
                    params: ng.extend(ctrl.parameters(), { rnd: Math.random() }),
                })
                .then(function (response) {
                    return response.data;
                });
        };

        ctrl.getFilterData = function () {
            var params = {
                hideFilterByPrice: ctrl.hideFilterByPrice,
                hideFilterByBrand: ctrl.hideFilterByBrand,
                hideFilterByColor: ctrl.hideFilterByColor,
                hideFilterBySize: ctrl.hideFilterBySize,
                hideFilterByProperty: ctrl.hideFilterByProperty,
            };

            return $http
                .get(ctrl.url, { params: ng.extend(pageParameters, ctrl.parameters(), params, { rnd: Math.random() }) })
                .then(function (response) {
                    return response.data;
                });
        };

        ctrl.getCountString = function (index) {
            const count = ctrl.storageSelectedItems[index.toString()];
            return count > 0 ? `(${count})` : `<span class="catalog-filter-block-count-zero">(0)</span>`;
        };

        ctrl.getSelectedCount = function (index) {
            const item = ctrl.catalogFilterData[index];
            if (!item.dirty) {
                return 0;
            }
            let count;
            if (['select', 'input'].includes(item.Control)) {
                count = item.Values.some((item) => item.Selected) ? 1 : 0;
            } else if (item.Control === 'range') {
                count = item.Values[0].CurrentMin !== item.Values[0].Min || item.Values[0].CurrentMax !== item.Values[0].Max ? 1 : 0;
            } else {
                count = item.Values.reduce(function (acc, curr) {
                    return curr.Selected ? (acc += 1) : acc;
                }, 0);
            }

            return count ?? 0;
        };

        ctrl.hasSelectedItems = function () {
            return Object.keys(ctrl.storageSelectedItems).length > 0;
        };

        ctrl.updateSelectedItems = (index) => {
            const count = ctrl.getSelectedCount(index);
            count > 0 ? (ctrl.storageSelectedItems[index.toString()] = count) : delete ctrl.storageSelectedItems[index.toString()];
        };
    };

    ng.module('catalogFilter').controller('CatalogFilterCtrl', CatalogFilterCtrl);
})(window.angular);
