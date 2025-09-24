import smsConfirmationInputTemplate from '../templates/smsConfirmationInput.html';
import smsConfirmationTemplate from '../templates/smsConfirmation.html';

function smsConfirmationDirective() {
    return {
        restrict: 'A',
        scope: {
            confirmationPhone: '<?',
            pageName: '<?',
        },
        controller: 'SmsConfirmationCtrl',
        controllerAs: 'smsConfirmation',
        templateUrl: smsConfirmationTemplate,
        bindToController: true,
    };
}

const smsConfirmationInputComponent = {
    controller: 'SmsConfirmationInputCtrl',
    templateUrl: smsConfirmationInputTemplate,
    bindings: {
        count: '<',
        disabled: '<?',
        focusOnStart: '<?',
        onComplete: '&',
        onInit: '&',
    },
};

export { smsConfirmationDirective, smsConfirmationInputComponent };
