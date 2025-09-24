import mobileDetailsTemplate from '../templates/mobileDetails.html';
import detailsTemplate from '../templates/details.html';
import itemsTemplate from '../templates/items.html';
function orderHistoryDirective() {
    return {
        restrict: 'A',
        scope: {
            mode: '=',
            onChangeView: '&',
            titleText: '@',
            isLp: '<?',
            photoWidth: '<?',
        },
        bindToController: true,
        controller: 'OrderHistoryCtrl',
        controllerAs: 'orderHistory',
        replace: true,
        template: function (elem, attr) {
            if (attr.isMobile) {
                return '<div data-ng-switch="orderHistory.mode"><div data-order-history-items data-ng-switch-when="all"></div><div data-order-history-details data-is-mobile="true" data-is-lp="orderHistory.isLp" data-ng-switch-when="details"></div></div>';
            } else {
                return '<div><div data-ng-if="orderHistory.isGetOrdersData"><div data-ng-switch="orderHistory.mode"><div data-order-history-items data-ng-switch-when="all"></div><div data-order-history-details data-is-lp="orderHistory.isLp" data-ng-switch-when="details"></div></div></div><div class="order-history__spinner svg-spinner" data-ng-if="!orderHistory.isGetOrdersData"></div></div>';
            }
        },
    };
}

function orderHistoryItemsDirective() {
    return {
        require: '^orderHistory',
        restrict: 'A',
        scope: {
            isLp: '<?',
        },
        replace: true,
        templateUrl: itemsTemplate,
        link: function (scope, element, attrs, ctrl) {
            scope.parentScope = ctrl;
        },
    };
}

function orderHistoryDetailsDirective() {
    return {
        require: '^orderHistory',
        restrict: 'A',
        scope: {
            isLp: '<?',
        },
        replace: true,
        templateUrl: function (elem, attr) {
            if (attr.isMobile) {
                return mobileDetailsTemplate;
            } else {
                return detailsTemplate;
            }
        },
        link: function (scope, element, attrs, ctrl) {
            scope.parentScope = ctrl;
        },
    };
}

function orderPayDirective() {
    return {
        restrict: 'AE',
        scope: {
            orderCode: '<?',
            // orderId: '<?',
            paymentMethodId: '<?',
            pageWithPaymentButton: '@',
            validationDisabled: '<?',
        },
        bindToController: true,
        controller: 'OrderPayCtrl',
        controllerAs: 'orderPay',
        template: function (elem, attr) {
            return `<div data-ng-include="'/checkout/getorderpay?ordercode='+orderPay.orderCode+'&orderid='+orderPay.orderId+'&paymentmethodid='+orderPay.paymentMethodId+'&pagewithpaymentbutton='+orderPay.pageWithPaymentButton+'&validationDisabled='+orderPay.validationDisabled+'&rnd='+orderPay.rnd"></div>`;
        },
    };
}

function orderReviewDirective() {
    return {
        restrict: 'AE',
        scope: {
            orderNumber: '<?',
            review: '<?',
        },
        bindToController: true,
        controller: 'OrderReviewCtrl',
        controllerAs: 'orderReview',
        //replace: true,
        template: function (elem, attr) {
            return `<input type="button" value="{{ (orderReview.review != null && orderReview.review.Readonly ? ('Js.Order.MyOrderReview' | translate) : ('Js.Order.OrderReview' | translate)) }}" class="btn btn-middle btn-action" />`;
        },
        link: function (scope, element, attrs, ctrls) {
            const orderReview = ctrls;
            element[0].addEventListener('click', function (event) {
                orderReview.showModal();
            });
        },
    };
}

export { orderHistoryDirective, orderHistoryItemsDirective, orderHistoryDetailsDirective, orderPayDirective, orderReviewDirective };
