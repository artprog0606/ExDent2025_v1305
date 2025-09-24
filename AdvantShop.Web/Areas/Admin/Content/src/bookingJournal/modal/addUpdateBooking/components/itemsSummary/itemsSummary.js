import itemsSummaryTemplate from './itemsSummary.html';
(function (ng) {
    'use strict';

    var BookingItemsSummaryCtrl = function ($timeout, $q) {
        var ctrl = this,
            popoverPaymentTimer;
        ctrl.$onInit = function () {
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    bookingItemsSummary: ctrl,
                });
            }
        };
        ctrl.callToggleSelectCurrencyLabel = function () {
            ctrl.toggleselectCurrencyLabel(ctrl.summary.BookingDiscount > 0 ? '1' : '0');
        };
        ctrl.toggleselectCurrencyLabel = function (val) {
            ctrl.typeDiscountPercent = val === '1' ? true : false;
            ctrl.selectCurrency = val;
        };
        ctrl.callChangeSummary = function () {
            if (ctrl.updateSummaryFn) {
                ctrl.updateSummaryFn().then(function () {
                    ctrl.callToggleSelectCurrencyLabel();
                });
            }
        };
        ctrl.callChangePayment = function (payment) {
            if (ctrl.changePaymentFn) {
                return ctrl.changePaymentFn({
                    payment: payment,
                });
            }
            return $q.resolve();
        };
        ctrl.discountPopoverOpen = function () {
            ctrl.BookinDiscountNew = ctrl.typeDiscountPercent ? ctrl.summary.BookingDiscount : ctrl.summary.DiscountCost;
            ctrl.discountPopoverIsOpen = true;
        };
        ctrl.discountPopoverClose = function () {
            ctrl.discountPopoverIsOpen = false;
        };
        ctrl.discountPopoverToggle = function () {
            ctrl.discountPopoverIsOpen === true ? ctrl.discountPopoverClose() : ctrl.discountPopoverOpen();
        };
        ctrl.changeDiscount = function (discount) {
            if (ctrl.selectCurrency === '1') {
                ctrl.summary.BookingDiscount = discount;
                ctrl.summary.BookingDiscountValue = 0;
            } else {
                ctrl.summary.BookingDiscountValue = discount;
                ctrl.summary.BookingDiscount = 0;
            }
            ctrl.callChangeSummary();
            ctrl.discountPopoverClose();
        };
        ctrl.changePayment = function (result) {
            if (result && result.payment) {
                ctrl.callChangePayment(result.payment).then(ctrl.callChangeSummary);
            }
        };
        ctrl.getPaymentDetailsLink = function () {
            var link = ctrl.summary.PrintPaymentDetailsLink;
            if (ctrl.summary.PaymentDetails != null) {
                if (ctrl.summary.PaymentDetails.INN != null && ctrl.summary.PaymentDetails.INN.length > 0) {
                    link += '&bill_INN=' + ctrl.summary.PaymentDetails.INN;
                }
                if (ctrl.summary.PaymentDetails.CompanyName != null && ctrl.summary.PaymentDetails.CompanyName.length > 0) {
                    link += '&bill_CompanyName=' + ctrl.summary.PaymentDetails.CompanyName;
                }
                if (ctrl.summary.PaymentDetails.Contract != null && ctrl.summary.PaymentDetails.Contract.length > 0) {
                    link += '&bill_Contract=' + ctrl.summary.PaymentDetails.Contract;
                }
            }
            return link;
        };
        ctrl.popoverPaymentOpen = function () {
            if (popoverPaymentTimer != null) {
                $timeout.cancel(popoverPaymentTimer);
            }
            ctrl.popoverPaymentIsOpen = true;
        };
        ctrl.popoverPaymentClose = function () {
            popoverPaymentTimer = $timeout(function () {
                ctrl.popoverPaymentIsOpen = false;
            }, 500);
        };
        ctrl.savePaymentDetails = function () {};
    };
    BookingItemsSummaryCtrl.$inject = ['$timeout', '$q'];
    ng.module('bookingItemsSummary', [])
        .controller('BookingItemsSummaryCtrl', BookingItemsSummaryCtrl)
        .component('bookingItemsSummary', {
            templateUrl: itemsSummaryTemplate,
            controller: 'BookingItemsSummaryCtrl',
            bindings: {
                onInit: '&',
                updateSummaryFn: '&',
                changePaymentFn: '&',
                params: '<?',
                summary: '<',
                mode: '<',
                canBeEditing: '<?',
                onStopEdit: '&',
            },
        });
})(window.angular);
