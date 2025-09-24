/*@ngInject*/
function BillingCtrl($http, $sce, $timeout, zoneService, checkoutService) {
    var ctrl = this,
        relationship;

    relationship = {
        payment: function () {
            return ctrl.fetchCart();
        },
        paybutton: function () {
            return ctrl.fetchCart();
        },
    };

    ctrl.$onInit = function () {
        ctrl.Payment = {};
        ctrl.PaymentId = 0;
        ctrl.Cart = {};
        ctrl.dataReceived = false;
        ctrl.useCart = false;
        ctrl.paymentLoading = true;
        ctrl.contact = {};
    };

    ctrl.setOrderId = function (orderId) {
        ctrl.orderId = orderId;
        ctrl.fetchPayment().then(function (response) {
            ctrl.changePayment(ctrl.ngSelectPayment);
        });
    };

    ctrl.fetchPayment = function () {
        ctrl.paymentLoading = true;

        return checkoutService.getBillingPayment(ctrl.orderId).then(function (response) {
            ctrl.ngSelectPayment = ctrl.getSelectedItem(response.option, response.selectPayment);

            if (ctrl.ngSelectPayment != null) {
                ctrl.PaymentId = ctrl.ngSelectPayment.Id;
            }

            ctrl.paymentLoading = false;

            return (ctrl.Payment = response);
        });
    };

    ctrl.changePayment = function (payment) {
        if (ctrl.ngSelectPayment != null) {
            if (ctrl.ngSelectPayment !== payment) {
                ctrl.ngSelectPayment = payment;
            }

            checkoutService.saveBillingPayment(payment, ctrl.orderId).then(function (response) {
                ctrl.PaymentId = payment.Id;
                relationship['payment']();
            });
        }
    };

    ctrl.fetchCart = function () {
        return checkoutService.getBillingCart(ctrl.orderId).then(function (response) {
            ctrl.showCart = true;
            ctrl.isShowCouponInput = response.Certificate == null && response.Coupon == null;
            if (ctrl.Cart.Discount != null) {
                ctrl.Cart.Discount.Key = $sce.trustAsHtml(ctrl.Cart.Discount.Key);
            }
            if (ctrl.Cart.Coupon != null) {
                ctrl.Cart.Coupon.Key = $sce.trustAsHtml(ctrl.Cart.Coupon.Key);
            }
            ctrl.paymentLoading = false;

            return (ctrl.Cart = response);
        });
        //    .then(function (response) {
        //        ctrl.dataReceived = true;
        //        ctrl.showCart = true;
        //        angular.extend(ctrl.Cart, response);

        //        return ctrl.Cart;
        //});
    };

    ctrl.fillAddress = function (contactData) {
        ctrl.contact = Object.assign(ctrl.contact, contactData);
    };

    ctrl.getSelectedItem = function (array, selectedItem) {
        var item;

        for (var i = array.length - 1; i >= 0; i--) {
            if (array[i].Id === selectedItem.Id) {
                //selectedItem имеет заполненные поля какие опции выбраны, поэтому объединяем
                array[i] = angular.extend(array[i], selectedItem);
                item = array[i];
                break;
            }
        }

        return item;
    };
}

export default BillingCtrl;
