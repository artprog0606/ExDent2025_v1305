import priceAmountCartTemplate from '../templates/priceAmountCart.html';
/* @ngInject */
function priceAmountCartDirective() {
    return {
        restrict: 'A',
        scope: {
            isMobile: '<?',
        },
        controller: 'PriceAmountCartCtrl',
        controllerAs: 'priceAmountCart',
        bindToController: true,
        replace: true,
        templateUrl: priceAmountCartTemplate,
    };
}

export { priceAmountCartDirective };
