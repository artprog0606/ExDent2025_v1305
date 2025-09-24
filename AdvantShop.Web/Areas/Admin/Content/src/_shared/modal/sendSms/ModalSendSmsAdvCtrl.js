(function (ng) {
    'use strict';

    var ModalSendSmsAdvCtrl = function ($uibModalInstance, $http, $window, toaster, $translate, $sce) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.customerId = params.customerId;
            ctrl.phone = params.phone;
            ctrl.customerIds = params.customerIds;
            ctrl.recipients = params.recipients;
            ctrl.subscriptionIds = params.subscriptionIds;
            ctrl.pageType = params.pageType;
            ctrl.throwError = params.throwError;

            ctrl.orderId = params.orderId != null && params.orderId !== '0' && params.orderId !== 0 ? params.orderId : null;
            ctrl.leadId = params.leadId != null && params.leadId !== '0' && params.leadId !== 0 ? params.leadId : null;

            ctrl.smsAnswerTemplate = {
                TemplateId: -1,
                Name: $translate.instant('Admin.Js.SendLetterToCustomer.Empty'),
            };

            ctrl.checkEnabled();
            ctrl.getSmsFormat();
            ctrl.getAnswerTemplates();
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.checkEnabled = function () {
            $http.get('sms/getSmsModuleEnabled').then(function (response) {
                ctrl.enabled = response.data.result;
            });
        };

        ctrl.getSmsFormat = function () {
            ctrl.isProcessingTemplate = true;
            ctrl.moduleTemplateId = null;

            var params = {
                templateId: ctrl.smsAnswerTemplate.TemplateId,
            };

            if (!ctrl.isMassAction()) {
                params.customerId = ctrl.customerId;
                params.orderId = ctrl.orderId;
                params.leadId = ctrl.leadId;
                params.isMassSending = false;
            } else {
                params.isMassSending = true;
            }

            $http.get('sms/getSmsToCustomer', { params: params }).then(function (response) {
                var data = response.data;
                ctrl.text = data.text;
                ctrl.userNotAgree = data.userNotAgree;
                ctrl.isSmsAndSocialMediaActive = data.isSmsAndSocialMediaActive;
                ctrl.customerKeys = $sce.trustAsHtml(data.customerKeys);
                ctrl.orderKeys = $sce.trustAsHtml(data.orderKeys);
                ctrl.leadKeys = $sce.trustAsHtml(data.leadKeys);
                ctrl.isProcessingTemplate = false;
            });
        };

        ctrl.isMassAction = function () {
            return ctrl.customerIds != null || ctrl.subscriptionIds != null || ctrl.recipients != null;
        };

        ctrl.send = function () {
            ctrl.btnLoading = true;

            var params = {
                customerId: ctrl.customerId,
                phone: ctrl.phone,
                customerIds: ctrl.customerIds,
                recipients: ctrl.recipients,
                subscriptionIds: ctrl.subscriptionIds,
                orderId: ctrl.orderId,
                leadId: ctrl.leadId,
                text: ctrl.text,
                pageType: ctrl.pageType,
                throwError: ctrl.throwError,
                moduleTemplateId: ctrl.moduleTemplateId,
            };

            $http
                .post('sms/sendSms', params)
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.SendSMS.MessageSent'));
                        $uibModalInstance.close();
                    } else if (data.errors != null) {
                        data.errors.forEach(function (err) {
                            toaster.pop('error', '', err);
                        });
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.SendSMS.ErrorSending'));
                    }
                })
                .finally(function () {
                    ctrl.btnLoading = false;
                });
        };

        ctrl.getAnswerTemplates = function () {
            $http.get('sms/getAnswerTemplates').then(function (response) {
                var data = response.data;
                ctrl.templates = data.obj;
            });
        };

        ctrl.selectSmsTemplate = function (template) {
            ctrl.moduleTemplateId = template.Id;
            ctrl.text = template.Text;
            ctrl.smsAnswerTemplate = ctrl.templates[0];
        };
    };

    ModalSendSmsAdvCtrl.$inject = ['$uibModalInstance', '$http', '$window', 'toaster', '$translate', '$sce'];

    ng.module('uiModal').controller('ModalSendSmsAdvCtrl', ModalSendSmsAdvCtrl);
})(window.angular);
