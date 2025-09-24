/* @ngInject */
function PreOrderTriggerCtrl($window, toaster, preOrderService) {
    var ctrl = this;

    ctrl.formInit = function (form) {
        ctrl.form = form;
    };

    ctrl.modalCallbackClose = function (modalScope) {
        if (ctrl.form.result != null && ctrl.form.showRedirectButton === true) {
            window.location = ctrl.form.result.url;
        }

        if (ctrl.form.success === true) {
            ctrl.form.reset();
        }
    };

    ctrl.successFn = function (result) {
        if (result != null) {
            window.location = result;
        } else {
            preOrderService.modalFooterShow(ctrl.modalId, false);
        }
    };
}

export default PreOrderTriggerCtrl;
