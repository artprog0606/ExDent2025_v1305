(function (ng) {
    'use strict';

    var options = {
        enableHorizontalScrollbar: 0,
        enableVerticalScrollbar: 0,
        useExternalPagination: true,
        enablePaginationControls: false,
        useExternalSorting: true,
        enableCellEditOnFocus: false,
        enableCellEdit: false,
        enableSelectAll: true,
        enableColumnMenus: false,
        rowHeight: 60,
        paginationCurrentPage: 1,
        paginationPageSize: 10,
        paginationPageSizes: [10, 20, 50, 100],
        uiGridCustom: {
            selectionOptions: null,
        },
        virtualizationThreshold: 101, //просто большое число чтобы отключить виртуализациюб скролла
        selectionRowHeaderWidth: 40,
        treeRowHeaderBaseWidth: 40,
        //http://ui-grid.info/docs/#!/tutorial/Tutorial:%20404%20Large%20Data%20Sets%20and%20Performance
        flatEntityAccess: true,
        fastWatch: true,
    };

    ng.module('uiGridCustom')
        .constant('uiGridCustomConfig', options)
        .constant('uiGridCustomParamsConfig', {
            paginationCurrentPage: options.paginationCurrentPage,
            paginationPageSize: options.paginationPageSize,
        })
        .constant('uiGridCustomDataScheme', {
            DataItems: 'data',
            ItemsPerPage: 'paginationPageSize',
            Page: 'paginationCurrentPage',
            TotalItemsCount: 'totalItems',
        });
})(window.angular);
