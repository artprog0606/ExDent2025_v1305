import formTemplate from '../templates/preOrderForm.html';
/* @ngInject */
function preOrderTriggerDirective(preOrderService) {
    return {
        restrict: 'A',
        scope: true,
        controller: 'PreOrderTriggerCtrl',
        controllerAs: 'preOrderTrigger',
        bindToController: true,
        link: function (scope, element, attrs, ctrl) {
            element.on('click', function (event) {
                event.preventDefault();

                var modalId = element[0].getAttribute('data-pre-order-modal');

                ctrl.modalId = modalId != null ? modalId : 'modalPreOrder';

                scope.$apply(function () {
                    preOrderService.showDialog(ctrl.modalId);
                });
            });
        },
    };
}

function preOrderFormDirective() {
    return {
        restrict: 'A',
        scope: {
            offerId: '=?',
            productId: '=?',
            formInit: '&',
            successFn: '&',
            preOrderValid: '&',
            amount: '=?',
            jsonHash: '=?',
            isLanding: '=',
        },
        controller: 'PreOrderFormCtrl',
        controllerAs: 'preOrderForm',
        bindToController: true,
        templateUrl: formTemplate,
    };
}

export { preOrderTriggerDirective, preOrderFormDirective };
