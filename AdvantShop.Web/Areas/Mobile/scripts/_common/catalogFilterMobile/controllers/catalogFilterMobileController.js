/*@ngInject*/
function CatalogFilterMobileCtrl($window, $cookies, catalogFilterService, sidebarsContainerService, $translate, urlHelper) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.getFiltersCount();
    };

    ctrl.changeSort = function (curSort) {
        var search = catalogFilterService.parseSearchString($window.location.search);
        var sortValue = curSort;
        delete search.page;
        var objForUrl = angular.extend({}, search, {}, { sort: sortValue });
        $window.location.search = catalogFilterService.buildUrl(objForUrl);
    };

    //classic mobile template
    ctrl.setView = function (viewmode) {
        $cookies.put('mobile_viewmode', viewmode);
        $window.location.reload();
    };

    ctrl.setFilterVisibility = function (visible) {
        ctrl.isFilterVisible = visible;
    };

    ctrl.openInSidebar = function (templateUrl) {
        sidebarsContainerService.open({
            sidebarClass: 'sidebar-catalog-filter',
            contentId: 'sidebarCatalogFilter',
            templateUrl: templateUrl,
            title: $translate.instant('Js.CatalogFilter.Filters'),
            hideFooter: true,
        });
    };

    ctrl.getFiltersCount = function () {
        var count = 0;

        if (urlHelper.getUrlParamByName('pricefrom') != null || urlHelper.getUrlParamByName('priceto') != null) {
            count++;
        }
        var brands = urlHelper.getUrlParamByName('brand');
        if (brands != null) {
            count += ctrl.getSplitCount(brands, ',');
        }

        var colors = urlHelper.getUrlParamByName('color');
        if (colors != null) {
            count += ctrl.getSplitCount(colors, ',');
        }

        var sizes = urlHelper.getUrlParamByName('size');
        if (sizes != null) {
            count += ctrl.getSplitCount(sizes, ',');
        }
        var props = null;
        var paramsAsObject = urlHelper.getUrlParamsAsObject();
        if (paramsAsObject != null) {
            props = urlHelper.getUrlParamDictionaryByNameFunc(function (paramName) {
                return paramName === 'prop' || (paramName.indexOf('prop_') === 0 && paramName.indexOf('_min') === -1);
            }, paramsAsObject);
        }

        if (props != null && props.length > 0) {
            count += Object.keys(props).length;
        }

        if (urlHelper.getUrlParamByName('available') != null) {
            count++;
        }

        ctrl.selectedFiltersCount = count;
    };

    ctrl.getSplitCount = function (props, separator) {
        var count = 0;
        var arr = props.split(separator);
        for (var i = 0; i < arr.length; i++) {
            if (arr[i] != '') {
                count++;
            }
        }
        return count;
    };
}

export default CatalogFilterMobileCtrl;
