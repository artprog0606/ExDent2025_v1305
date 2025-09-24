import paymentListTemplate from '../templates/paymentList.html';

/* @ngInject */
function paymentListDirective(urlHelper) {
    return {
        restrict: 'A',
        scope: {
            items: '=',
            selectPayment: '=',
            countVisibleItems: '=',
            change: '&',
            changeDetails: '&',
            anchor: '@',
            isProgress: '=?',
            iconWidth: '@',
            iconHeight: '@',
            enablePhoneMask: '<?',
        },
        controller: 'PaymentListCtrl',
        controllerAs: 'paymentList',
        bindToController: true,
        replace: true,
        templateUrl: paymentListTemplate,
    };
}

/* @ngInject */
function paymentTemplateDirective() {
    return {
        restrict: 'A',
        scope: {
            templateUrl: '=',
            payment: '=',
            changeControl: '&',
            enablePhoneMask: '<?',
        },
        controller: 'PaymentTemplateCtrl',
        controllerAs: 'paymentTemplate',
        bindToController: true,
        replace: true,
        template: '<div data-ng-include="paymentTemplate.templateUrl"></div>',
    };
}

export { paymentListDirective, paymentTemplateDirective };
