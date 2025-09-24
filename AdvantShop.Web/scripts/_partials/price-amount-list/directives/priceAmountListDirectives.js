import priceAmountListTemplate from '../templates/priceAmountList.html';
/* @ngInject */
function priceAmountListDirective() {
    return {
        restrict: 'A',
        scope: {
            productId: '=',
            offerId: '=',
            startOfferId: '=',
            initFn: '&',
        },
        controller: 'PriceAmountListCtrl',
        controllerAs: 'priceAmountList',
        bindToController: true,
        replace: true,
        templateUrl: priceAmountListTemplate,
    };
}

export { priceAmountListDirective };
