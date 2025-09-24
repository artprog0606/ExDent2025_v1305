import customOptionsTemplate from './../templates/orderItemCustomOptions.html';

/* @ngInject */
function orderItemCustomOptionsDirective(urlHelper) {
    return {
        scope: {
            productId: '<',
            orderItemId: '<?',
            initFn: '&',
            changeFn: '&',
        },
        replace: true,
        templateUrl: customOptionsTemplate,
        controller: 'OrderItemCustomOptionsCtrl',
        controllerAs: 'orderItemCustomOptions',
        bindToController: true,
    };
}
export { orderItemCustomOptionsDirective };
