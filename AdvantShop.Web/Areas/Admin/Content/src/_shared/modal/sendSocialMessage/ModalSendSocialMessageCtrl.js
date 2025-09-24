(function (ng) {
    'use strict';

    var ModalSendSocialMessageCtrl = function ($uibModalInstance, $http, $window, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.type = params.type;
            ctrl.customerId = params.customerId;
            ctrl.customerSegmentId = params.customerSegmentId;
            ctrl.customerIds = params.customerIds;

            ctrl.checkEnabled();

            if (ctrl.type == 'vk') {
                ctrl.modalTitle = 'ВКонтакте';
            } else if (ctrl.type == 'instagram') {
                ctrl.modalTitle = 'Instagram Direct';
            } else if (ctrl.type == 'telegram') {
                ctrl.modalTitle = 'Telegram';
            } else if (ctrl.type == 'ok') {
                ctrl.modalTitle = 'OK';
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.checkEnabled = function () {
            $http.get('common/getSocialEnabled', { params: { type: ctrl.type } }).then(function (response) {
                ctrl.enabled = response.data.result;
                if (ctrl.type == 'vk') {
                    ctrl.errorEnabled = 'ВКонтакте';
                } else if (ctrl.type == 'instagram') {
                    ctrl.errorEnabled = 'Instagram Direct';
                } else if (ctrl.type == 'telegram') {
                    ctrl.errorEnabled = 'Telegram';
                } else if (ctrl.type == 'ok') {
                    ctrl.errorEnabled = 'OK';
                }
            });
        };

        ctrl.send = function () {
            var url = '';

            if (ctrl.type === 'vk') {
                url = 'vk/sendVkMessageByCustomers';
            } else if (ctrl.type === 'instagram') {
                url = 'instagram/sendInstagramMessage';
            } else if (ctrl.type === 'telegram') {
                url = 'telegram/sendTelegramMessage';
            } else if (ctrl.type === 'ok') {
                url = 'ok/sendOkMessageByCustomers';
            }

            ctrl.btnLoading = true;

            $http
                .post(url, {
                    customerId: ctrl.customerId,
                    message: ctrl.message,
                    customerSegmentId: ctrl.customerSegmentId,
                    customerIds: ctrl.customerIds,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        if (data.obj != null && data.obj.Errors != null && data.obj.Errors.length > 0) {
                            if (ctrl.customerId != null) {
                                toaster.pop('error', '', data.obj.Errors[0], 10000);
                            } else {
                                toaster.pop('success', '', 'Отправлено ' + data.obj.SendedCount + ' сообщений');
                            }
                        } else {
                            toaster.pop('success', '', $translate.instant('Admin.Js.SendSocialMessage.MessageSent'));
                        }
                        $uibModalInstance.close();
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.SendSocialMessage.ErrorSendingMessage'));
                    }
                })
                .finally(function () {
                    ctrl.btnLoading = false;
                });
        };
    };

    ModalSendSocialMessageCtrl.$inject = ['$uibModalInstance', '$http', '$window', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalSendSocialMessageCtrl', ModalSendSocialMessageCtrl);
})(window.angular);
