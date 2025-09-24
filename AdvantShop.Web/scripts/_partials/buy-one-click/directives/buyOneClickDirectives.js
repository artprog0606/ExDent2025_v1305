import formTemplate from '../templates/form.html';
/* @ngInject */
function buyOneClickTriggerDirective(buyOneClickService) {
    return {
        restrict: 'A',
        scope: true,
        controller: 'BuyOneClickTriggerCtrl',
        controllerAs: 'buyOneClickTrigger',
        bindToController: true,
        link: function (scope, element, attrs, ctrl) {
            element.on('click', function (event) {
                event.preventDefault();

                var modalId = element[0].getAttribute('data-buy-one-click-modal');

                ctrl.modalId = modalId != null ? modalId : 'modalBuyOneClick';

                scope.$apply(function () {
                    buyOneClickService.showDialog(ctrl.modalId);
                });
            });
        },
    };
}

function buyOneClickFormDirective() {
    return {
        restrict: 'A',
        scope: {
            buttonText: '@',
            page: '@',
            orderType: '@',
            offerId: '=?',
            productId: '=?',
            amount: '=?',
            attributesXml: '=?',
            formInit: '&',
            successFn: '&',
            fieldsOptions: '=?',
            autoReset: '=?',
            buyOneClickValid: '&',
            compactMode: '@',
            agreementDefaultChecked: '<?',
            enablePhoneMask: '<?',
        },
        controller: 'BuyOneClickFormCtrl',
        controllerAs: 'buyOneClickForm',
        bindToController: true,
        templateUrl: formTemplate,
        replace: true,
    };
}

export { buyOneClickTriggerDirective, buyOneClickFormDirective };
