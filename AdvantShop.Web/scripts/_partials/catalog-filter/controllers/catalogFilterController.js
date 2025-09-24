var isIE = /Windows Phone|iemobile|WPDesktop/.test(navigator.userAgent);

/*@ngInject*/
function CatalogFilterCtrl($http, $window, $timeout, popoverService, domService, catalogFilterService, catalogFilterAdvPopoverOptionsDefault) {
    var ctrl = this,
        pageParameters,
        timerPopoverHide,
        timerRange;

    ctrl.$onInit = function () {
        pageParameters = catalogFilterService.parseSearchString($window.location.search);

        ctrl.isIE = isIE;

        ctrl.countVisibleCollapse = ctrl.countVisibleCollapse() || 10;

        ctrl.collapsed = true;
        ctrl.isRenderBlock = false;
        ctrl.isLoaded = false;

        ctrl.itemsOptions = [];

        ctrl.advPopoverOptions = angular.extend({}, catalogFilterAdvPopoverOptionsDefault, ctrl.advPopoverOptions);
        ctrl.isShowLoader = true;
        ctrl.getFilterData()
            .then(function (catalogFilterData) {
                ctrl.catalogFilterData = catalogFilterData?.map(function (filter) {
                    filter.dirty = false;
                    return filter;
                });
                ctrl.isRenderBlock = catalogFilterData != null && catalogFilterData.length > 0;
                ctrl.isLoaded = true;
                if (ctrl.onFilterInit) {
                    ctrl.onFilterInit({ visible: ctrl.isRenderBlock });
                }

                //Fill all <select> ng-model by properly <option> on filter initialization
                if (catalogFilterData != null) {
                    var selectIndex;
                    for (var i = 0; i < catalogFilterData.length; i++) {
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
                }

                catalogFilterService.saveFilterData(catalogFilterData);
            })
            .finally(() => {
                ctrl.isShowLoader = false;
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

    ctrl.clickSelect = function ($event, indexFilter) {
        var element = $event.currentTarget.parentNode.parentNode;

        if (element != null) {
            ctrl.changeItem(element, indexFilter);
        }
    };

    ctrl.clickRangeDown = function (event) {
        ctrl.rangeElementClicked = event.target;
    };

    ctrl.clickRange = function (event, indexFilter) {
        if (timerRange != null) {
            $timeout.cancel(timerRange);
        }

        ctrl.rangeElementClicked = ctrl.rangeElementClicked || event.target;

        timerRange = $timeout(function () {
            var element = domService.closest(ctrl.rangeElementClicked, '.js-range-slider-block'); //ctrl.rangeElementClicked

            if (element != null) {
                ctrl.changeItem(element, indexFilter);
            }

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
        }

        selectedItems = catalogFilterService.getSelectedData(ctrl.catalogFilterData);
        params = catalogFilterService.buildUrl(selectedItems);

        ctrl.getFilterCount(params).then(function (foundCount) {
            ctrl.foundCount = foundCount;

            popoverService.getPopoverScope('popoverCatalogFilter').then(function (popoverScope) {
                popoverScope.active(element);

                if (timerPopoverHide != null) {
                    $timeout.cancel(timerPopoverHide);
                }

                timerPopoverHide = $timeout(function () {
                    popoverScope.deactive();
                }, 5000);
            });
        });
    };

    ctrl.toggleVisible = function (totalItems, index) {
        ctrl.itemsOptions[index].countVisibleItems = ctrl.itemsOptions[index].collapsed === true ? totalItems : ctrl.countVisibleCollapse;
        ctrl.itemsOptions[index].collapsed = !ctrl.itemsOptions[index].collapsed;
    };

    ctrl.reset = function () {
        var cutParams =
            ctrl.catalogFilterData != null
                ? catalogFilterService.getSelectedData(
                      ctrl.catalogFilterData.filter(function (item) {
                          return item.Type === 'searchQuery';
                      }),
                  )
                : null;
        $window.location.search = catalogFilterService.buildUrl(cutParams);
    };

    ctrl.submit = function () {
        var pageParametersCopy = angular.copy(pageParameters);
        delete pageParametersCopy.page;
        var pageParametersKeys = Object.keys(pageParametersCopy);
        var selectedItems = catalogFilterService.getSelectedData(ctrl.catalogFilterData);
        var tempArray;

        ['brand', 'prop', 'color', 'size', 'pricefrom', 'priceto', 'categoryId', /prop_\d+_min/, /prop_\d+_max/, 'warehouse'].forEach(function (key) {
            if (key instanceof RegExp) {
                tempArray = pageParametersKeys.filter(function (item) {
                    return key.test(item) === true;
                });
            } else {
                tempArray = [key];
            }

            tempArray.forEach(function (tempItemKey) {
                if (pageParametersKeys.indexOf(tempItemKey) != -1 && (selectedItems == null || selectedItems[tempItemKey] == null)) {
                    delete pageParametersCopy[tempItemKey];
                }
            });
        });

        $window.location.search = '?' + catalogFilterService.buildUrl(angular.extend({}, pageParametersCopy, selectedItems));
    };

    ctrl.getFilterCount = function (filterString) {
        return $http
            .get(ctrl.urlCount + (filterString != null && filterString.length > 0 ? '?' + filterString : ''), {
                params: angular.extend(ctrl.parameters(), { rnd: Math.random() }),
            })
            .then(function (response) {
                return response.data;
            });
    };

    ctrl.getFilterData = function () {
        return $http
            .get(ctrl.url, { params: angular.extend({}, pageParameters, ctrl.parameters(), { rnd: Math.random() }) })
            .then(function (response) {
                return response.data;
            });
    };
}

export default CatalogFilterCtrl;
