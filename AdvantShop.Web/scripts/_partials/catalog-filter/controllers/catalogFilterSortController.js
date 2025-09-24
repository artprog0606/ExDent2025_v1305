/* @ngInject */
function CatalogFilterSortCtrl($window, catalogFilterService) {
    var ctrl = this;

    ctrl.sort = function () {
        var search, filterData, selectedItems, objForUrl, sortValue;

        search = catalogFilterService.parseSearchString($window.location.search);

        filterData = catalogFilterService.getFilterData();
        selectedItems = catalogFilterService.getSelectedData(filterData) || {};

        if (search != null && search.sort != null) {
            sortValue = search.sort.toLowerCase() == ctrl.asc.toLowerCase() ? ctrl.desc : ctrl.asc;
        } else {
            sortValue = ctrl.asc;
        }

        delete search.page;
        objForUrl = angular.extend({}, search, selectedItems, { sort: sortValue.toLowerCase() });
        $window.location.search = catalogFilterService.buildUrl(objForUrl);
    };

    ctrl.sortBy = function () {
        var search, filterData, selectedItems, objForUrl, sortValue;

        search = catalogFilterService.parseSearchString($window.location.search);

        filterData = catalogFilterService.getFilterData();
        selectedItems = catalogFilterService.getSelectedData(filterData) || {};

        sortValue = ctrl.sorting;

        delete search.page;
        objForUrl = angular.extend({}, search, selectedItems, { sort: sortValue.toLowerCase() });
        $window.location.search = catalogFilterService.buildUrl(objForUrl);
    };
}

export default CatalogFilterSortCtrl;
