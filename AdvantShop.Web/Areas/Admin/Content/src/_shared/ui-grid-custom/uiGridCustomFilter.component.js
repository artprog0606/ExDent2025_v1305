(function (ng) {
    'use strict';

    ng.module('uiGridCustomFilter').component('uiGridCustomFilter', {
        template: '<div ng-include="$ctrl._templateUrl"></div>',
        controller: 'UiGridCustomFilterCtrl',
        require: {
            uiGridCustom: '?^uiGridCustom',
        },
        bindings: {
            gridColumnDefs: '<',
            gridParams: '<?',
            gridSearchText: '<?',
            gridSearchPlaceholder: '<?',
            gridSearchVisible: '<?',
            gridOptions: '<?',
            onInit: '&',
            onChange: '&',
            onRemove: '&',
            hiddenTotalItemsCount: '<?',
            templateUrl: '<?',
            searchAutofocus: '<?',
        },
    });
})(window.angular);
