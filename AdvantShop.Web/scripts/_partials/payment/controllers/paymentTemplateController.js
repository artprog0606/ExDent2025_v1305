/* @ngInject */
function PaymentTemplateCtrl($timeout) {
    var ctrl = this,
        timer;

    ctrl.changePrepare = function (event, field) {
        if (field == null) {
            return;
        }

        if (event != null && event.type == 'keyup') {
            if (timer != null) {
                $timeout.cancel(timer);
            }

            timer = $timeout(function () {
                ctrl.changeControl({ payment: ctrl.payment });
            }, 500);
        } else {
            ctrl.changeControl({ payment: ctrl.payment });
        }
    };

    ctrl.changeSpinbox = function (value, proxy) {
        ctrl.changeControl({ payment: ctrl.payment });
    };
}

export default PaymentTemplateCtrl;
