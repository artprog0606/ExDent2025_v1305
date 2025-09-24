import uiGridCustomPaginationTemplate from './templates/ui-grid-custom-pagination.html';
(function (ng) {
    'use strict';

    ng.module('uiGridCustomPagination').component('uiGridCustomPagination', {
        templateUrl: uiGridCustomPaginationTemplate,
        controller: 'UiGridCustomPaginationCtrl',
        bindings: {
            gridTotalItems: '<',
            gridPaginationPageSize: '<',
            gridPaginationPageSizes: '<',
            gridPaginationCurrentPage: '<',
            onChange: '&',
        },
        transclude: true,
    });
})(window.angular);
