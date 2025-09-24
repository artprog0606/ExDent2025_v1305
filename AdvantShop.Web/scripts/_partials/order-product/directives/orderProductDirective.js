import orderProducts from '../templates/orderProducts.html';
import orderProductsMobile from '../templates/orderProducts.mobile.html';
/* @ngInject */
function orderProductDirective($parse, isMobileService) {
    return {
        restrict: 'A',
        scope: true,
        bindToController: true,
        controller: 'OrderProductCtrl',
        controllerAs: 'orderProduct',
        templateUrl: () => (isMobileService.getValue() ? orderProductsMobile : orderProducts),
        link: function (scope, element, attrs, ctrl) {
            ctrl.productViewOptions = attrs.productViewOptions != null ? $parse(attrs.productViewOptions)(scope) : null;
        },
    };
}

export default orderProductDirective;
