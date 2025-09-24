import catalogFilterTemplate from '../templates/catalogFilter.html';
function catalogFilterDirective() {
    return {
        restrict: 'A',
        scope: {
            url: '@',
            urlCount: '@',
            parameters: '&',
            countVisibleCollapse: '&',
            onFilterInit: '&',
            advPopoverOptions: '<?',
            footerSticky: '<?',
            isMobile: '<?',
        },
        replace: true,
        templateUrl: catalogFilterTemplate,
        controller: 'CatalogFilterCtrl',
        controllerAs: 'catalogFilter',
        bindToController: true,
    };
}

function catalogFilterSortDirective() {
    return {
        restrict: 'A',
        scope: {
            asc: '@',
            desc: '@',
        },
        replace: true,
        transclude: true,
        template: '<a data-ng-transclude data-ng-click="catalogFilterSort.sort()"></a>',
        controller: 'CatalogFilterSortCtrl',
        controllerAs: 'catalogFilterSort',
        bindToController: true,
    };
}

function catalogFilterSelectSortDirective() {
    return {
        restrict: 'A',
        scope: true,
        controller: 'CatalogFilterSortCtrl',
        controllerAs: 'catalogFilterSort',
        bindToController: true,
    };
}

export { catalogFilterDirective, catalogFilterSortDirective, catalogFilterSelectSortDirective };
