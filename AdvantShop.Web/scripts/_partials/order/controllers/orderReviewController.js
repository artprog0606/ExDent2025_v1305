/* @ngInject */
function OrderReviewCtrl(orderService, modalService, toaster, $translate) {
    var ctrl = this;

    ctrl.showModal = function () {
        ctrl.modalId = 'modalOrderReview_' + ctrl.orderNumber;
        orderService.getOrderReview(ctrl.orderNumber).then(function (result) {
            if (result) ctrl.review = result;
            ctrl.readonly = !ctrl.review || ctrl.review.Readonly;
            modalService.renderModal(
                ctrl.modalId,
                '<div class="row"><div class="col-xs-11">Заказ №' + ctrl.orderNumber + '</div></div>',
                '<div data-ng-include="\'/scripts/_partials/order/templates/review.html\'"></div>',
                null,
                {
                    destroyOnClose: true,
                    callbackClose: 'ctrl.modalClose',
                    modalClass: 'adv-review-modal',
                },
                {
                    ctrl: ctrl,
                },
            );

            modalService.getModal(ctrl.modalId).then(function (modal) {
                modal.modalScope.open();
            });
        });
    };

    ctrl.modalClose = function () {
        modalService.destroy(ctrl.modalId);
    };

    ctrl.addReview = function () {
        if (ctrl.review.Ratio == 0 && !ctrl.review.Text) {
            return toaster.pop('error', '', $translate.instant('Js.Order.ReviewIsEmpty'));
        }
        return orderService.addOrderReview(ctrl.orderNumber, ctrl.review.Ratio, ctrl.review.Text).then(function (data) {
            if (data.result)
                modalService.getModal(ctrl.modalId).then(function (modal) {
                    ctrl.review.Readonly = true;
                    modal.modalScope.close();
                });
            else {
                //modalService.destroy(ctrl.modalId);
                if (data.errors != null) {
                    data.errors.forEach(function (err) {
                        toaster.pop('error', '', err);
                    });
                }
            }
        });
    };
}
export default OrderReviewCtrl;
