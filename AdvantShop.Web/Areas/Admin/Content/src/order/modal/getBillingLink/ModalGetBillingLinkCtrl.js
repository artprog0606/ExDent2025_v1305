(function (ng) {
    'use strict';

    var ModalGetBillingLinkCtrl = function ($uibModalInstance, $http, $timeout, $window, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.orderId = params.orderId;

            $http.get('orders/getBillingLink', { params: { orderId: ctrl.orderId } }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    ctrl.link = data.obj.link;
                    ctrl.shortLink = data.obj.shortLink;
                    ctrl.showSendToCustomerLink = data.obj.showSendToCustomerLink;

                    setTimeout(ctrl.select, 200);
                } else {
                    data.errors.forEach(function (error) {
                        ctrl.error = error;
                    });
                }
            });
        };

        ctrl.generateShortBillingLink = function () {
            $http.post('orders/generateShortBillingLink', { orderId: ctrl.orderId }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.success('', $translate.instant('Admin.Js.GetBillingLink.ShortLinkGenerated'));
                    ctrl.shortLink = data.obj;
                    $timeout(function () {
                        ctrl.select('.js-copy-short');
                    }, 200);
                } else {
                    data.errors.forEach(function (error) {
                        ctrl.error = error;
                    });
                }
            });
        };

        ctrl.copyToClipboard = function (selector) {
            selector = selector || '.js-copy';
            var copyTextarea = document.querySelector(selector);
            copyTextarea.select();

            try {
                var successful = document.execCommand('copy');
                if (successful) toaster.pop('success', '', $translate.instant('Admin.Js.Order.LinkCopied'));
            } catch (err) {
                console.log('Oops, unable to copy');
            }
        };

        ctrl.select = function (selector) {
            selector = selector || '.js-copy';
            var copyTextarea = document.querySelector(selector);
            copyTextarea.select();
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalGetBillingLinkCtrl.$inject = ['$uibModalInstance', '$http', '$timeout', '$window', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalGetBillingLinkCtrl', ModalGetBillingLinkCtrl);
})(window.angular);
