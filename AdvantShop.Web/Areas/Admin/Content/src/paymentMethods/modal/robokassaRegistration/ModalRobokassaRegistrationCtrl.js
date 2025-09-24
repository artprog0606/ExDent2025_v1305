(function (ng) {
    'use strict';

    var ModalRobokassaRegistrationCtrl = function ($uibModalInstance, toaster, $translate, $scope, $modalStack, $q) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.currentModal = $modalStack.getTop();

            $scope.$on('modal.closing', function () {
                window.removeEventListener('message', ctrl.listenerMessage);
            });

            window.addEventListener('message', ctrl.listenerMessage, false);

            ctrl.siteUrl = ctrl.$resolve ? ctrl.$resolve.params.siteUrl : null;
            ctrl.notificationUrl = ctrl.$resolve ? ctrl.$resolve.params.notificationUrl : null;
            ctrl.returnUrl = ctrl.$resolve ? ctrl.$resolve.params.returnUrl : null;
            ctrl.cancelUrl = ctrl.$resolve ? ctrl.$resolve.params.cancelUrl : null;
        };

        ctrl.close = function () {
            if (ctrl.shopId !== undefined) {
                $uibModalInstance.close({
                    shopId: ctrl.shopId,
                    key1: ctrl.key1,
                    key2: ctrl.key2,
                });
            } else {
                $uibModalInstance.dismiss('cancel');
            }
        };

        ctrl.listenerMessage = function (event) {
            if (event.origin !== 'https://reg.robokassa.ru') return;

            ctrl.iframeWindow = ctrl.iframeWindow || $modalStack.getTop().value.modalDomEl[0].querySelector('.modal-body iframe').contentWindow;

            if (event.data['rk_reg_ready'] === true) {
                event.source.postMessage(
                    {
                        rk_reg: true,
                        site_url: ctrl.siteUrl,
                        result_url: ctrl.notificationUrl,
                        success_url: ctrl.returnUrl,
                        fail_url: ctrl.cancelUrl,
                        callback_url: 'https://reg.robokassa.ru/z_callback.php',
                    },
                    event.origin,
                );
            }

            if (event.data['act'] === 'rk_reg_created') {
                ctrl.shopId = event.data.shopId;
                ctrl.key1 = event.data.key_1;
                ctrl.key2 = event.data.key_2;

                ctrl.isRegistered = true;
            }
        };
    };

    ModalRobokassaRegistrationCtrl.$inject = ['$uibModalInstance', 'toaster', '$translate', '$scope', '$uibModalStack', '$q'];

    ng.module('uiModal').controller('ModalRobokassaRegistrationCtrl', ModalRobokassaRegistrationCtrl);
})(window.angular);
