import productsSelectvizrTemplate from './templates/products-selectvizr.html';
(function (ng) {
    'use strict';

    ng.module('productsSelectvizr').component('productsSelectvizr', {
        templateUrl: productsSelectvizrTemplate,
        controller: 'ProductsSelectvizrCtrl',
        transclude: true,
        bindings: {
            selectvizrTreeUrl: '<',
            selectvizrTreeItemsSelected: '<?',
            selectvizrGridUrl: '<',
            selectvizrGridOptions: '<',
            selectvizrGridParams: '<?',
            selectvizrGridInplaceUrl: '<?',
            selectvizrGridItemsSelected: '<?',
            selectvizrOnChange: '&',
            selectvizrOnInit: '&',
            selectvizrTreeSearch: '<?',
            selectvizrGridSelectionItemsSelectedFn: '&',
            selectvizrProperty: '<?',
        },
    });
})(window.angular);
