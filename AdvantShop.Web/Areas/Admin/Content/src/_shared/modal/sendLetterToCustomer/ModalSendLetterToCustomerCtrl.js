(function (ng) {
    'use strict';

    var ModalSendLetterToCustomerCtrl = function ($uibModalInstance, $http, $window, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;

            ctrl.customerId = params.customerId;
            ctrl.email = params.email;
            ctrl.firstName = params.firstName;
            ctrl.lastName = params.lastName;
            ctrl.patronymic = params.patronymic;
            ctrl.subject = params.subject;
            ctrl.isRe = params.isRe;
            ctrl.reId = params.reId;
            ctrl.orderId = params.orderId;
            ctrl.leadId = params.leadId;

            if (ctrl.isRe && ctrl.subject != null && ctrl.subject.indexOf('Re:') != 0) {
                ctrl.subject = 'Re: ' + ctrl.subject;
            }

            ctrl.customerIds = params.customerIds;
            ctrl.recipients = params.recipients;
            ctrl.subscriptionIds = params.subscriptionIds;
            ctrl.pageType = params.pageType;

            ctrl.mailAnswerTemplate = {
                TemplateId: -1,
                Name: $translate.instant('Admin.Js.SendLetterToCustomer.Empty'),
            };

            ctrl.showCkeditor = false;

            ctrl.getLetterFormat();
            ctrl.getAnswerTemplates();
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getLetterFormat = function () {
            var params = {
                customerId: ctrl.customerId,
                email: ctrl.email,
                firstName: ctrl.firstName,
                lastName: ctrl.lastName,
                patronymic: ctrl.patronymic,
                templateId: ctrl.mailAnswerTemplate.TemplateId,
                reId: ctrl.reId,
                orderId: ctrl.orderId,
                leadId: ctrl.leadId,
            };

            $http.get('customers/getLetterToCustomer', { params: params }).then(function (response) {
                var data = response.data;
                ctrl.subject = ctrl.isRe && ctrl.subject != null && ctrl.subject.length > 0 ? ctrl.subject : data.subject;
                ctrl.text = data.text;
                ctrl.showCkeditor = true;
                ctrl.error = data.error;
                ctrl.userNotAgree = data.userNotAgree;
            });
        };

        ctrl.send = function () {
            if (ctrl.error != null) return;

            ctrl.btnLoading = true;

            var params = {
                customerId: ctrl.customerId,
                customerIds: ctrl.customerIds,
                recipients: ctrl.recipients,
                subscriptionIds: ctrl.subscriptionIds,
                email: ctrl.email,
                subject: ctrl.subject,
                text: ctrl.text,
                pageType: ctrl.pageType,
                orderId: ctrl.orderId,
                leadId: ctrl.leadId,
            };

            $http
                .post('customers/sendLetterToCustomer', params)
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.SendLetterToCustomer.EmailSuccessfullySent'));
                        $uibModalInstance.close();
                    } else {
                        data.errors.forEach(function (err) {
                            toaster.pop('error', '', err);
                        });
                    }
                })
                .finally(function () {
                    ctrl.btnLoading = false;
                });
        };

        ctrl.getAnswerTemplates = function () {
            $http.get('customers/getAnswerTemplates').then(function (response) {
                var data = response.data;
                ctrl.templates = data.obj;
            });
        };

        ctrl.getModalTitle = function () {
            if (ctrl.customerIds != null || ctrl.recipients != null) {
                return $translate.instant('Admin.Js.SendLetterToCustomer.LettersToUsers');
            }
            if (ctrl.subscriptionIds != null) {
                return $translate.instant('Admin.Js.SendLetterToCustomer.LettersToSubscribers');
            }
            return $translate.instant('Admin.Js.SendLetterToCustomer.LetterToCustomer') + ctrl.email;
        };
    };

    ModalSendLetterToCustomerCtrl.$inject = ['$uibModalInstance', '$http', '$window', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalSendLetterToCustomerCtrl', ModalSendLetterToCustomerCtrl);
})(window.angular);
