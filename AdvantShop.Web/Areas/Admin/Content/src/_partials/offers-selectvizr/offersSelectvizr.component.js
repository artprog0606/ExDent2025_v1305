import offersSelectvizrTemplate from './templates/offers-selectvizr.html';
(function (ng) {
    'use strict';

    ng.module('offersSelectvizr').component('offersSelectvizr', {
        templateUrl: offersSelectvizrTemplate,
        controller: 'OffersSelectvizrCtrl',
        transclude: true,
        bindings: {
            selectvizrTreeUrl: '<',
            selectvizrGridUrl: '<',
            selectvizrGridOptions: '<',
            selectvizrGridParams: '<?',
            selectvizrOnChange: '&',
            selectvizrGridOnFetch: '&',
            selectvizrTreeSearch: '<?',
            selectvizrGridSelectionItemsSelectedFn: '&',
            selectvizrProperty: '<?',
        },
    });
})(window.angular);
