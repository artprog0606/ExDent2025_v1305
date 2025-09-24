(function (ng) {
    'use strict';

    var ModalAddEditNotificationTemplateCtrl = function ($uibModalInstance, $http, $window, toaster, $q, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.NotificationTypeId = params != null && params.NotificationTypeId != null ? params.NotificationTypeId : null;
            ctrl.BonusNotificationMethod = params != null && params.BonusNotificationMethod != null ? params.BonusNotificationMethod : null;
            ctrl.NotificationTemplateId = params != null && params.NotificationTemplateId != null ? params.NotificationTemplateId : null;
            ctrl.BodyOfNotification = params != null && params.NotificationBody != null ? params.NotificationBody : null;
            ctrl.IsNew = params != null && params.IsNew != null ? params.IsNew : false;

            $http
                .get('notificationtemplates/AddEditNotificationTemplate', {
                    params: {
                        notificationTypeId: ctrl.NotificationTypeId || 'None',
                        bonusNotificationMethod: ctrl.BonusNotificationMethod,
                    },
                })
                .then(
                    function (result) {
                        ctrl.NotificationTypes = result.data.obj.NotificationTypes;
                        ctrl.NotificationMethods = result.data.obj.NotificationMethods;
                        ctrl.AvalibleVarible = result.data.obj.AvalibleVarible;
                        ctrl.NotificationBody = result.data.obj.NotificationBody || ctrl.BodyOfNotification;
                    },
                    function (err) {
                        toaster.pop('error', '', $translate.instant('Admin.Js.SmsTemplate.ErrorRetrievingTemplate') + err);
                    },
                );
        };

        ctrl.refresh = function () {
            $http
                .get('notificationtemplates/AddEditNotificationTemplate', {
                    params: {
                        notificationTypeId: ctrl.NotificationTypeId || 'None',
                        bonusNotificationMethod: ctrl.BonusNotificationMethod,
                    },
                })
                .then(
                    function (result) {
                        ctrl.NotificationTypes = result.data.obj.NotificationTypes;
                        ctrl.NotificationMethods = result.data.obj.NotificationMethods;
                        ctrl.AvalibleVarible = result.data.obj.AvalibleVarible;
                    },
                    function (err) {
                        toaster.pop('error', '', $translate.instant('Admin.Js.SmsTemplate.ErrorRetrievingTemplate') + err);
                    },
                );
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.saveTemplate = function () {
            ctrl.btnLoading = true;
            $http
                .post('notificationtemplates/addeditnotificationtemplate', {
                    NotificationTypeId: ctrl.NotificationTypeId,
                    BonusNotificationMethod: ctrl.BonusNotificationMethod,
                    NotificationBody: ctrl.NotificationBody,
                    NotificationTemplateId: ctrl.NotificationTemplateId,
                    IsNew: ctrl.IsNew,
                })
                .then(
                    function (result) {
                        var data = result.data.result;
                        if (data === true) {
                            toaster.pop(
                                'success',
                                '',
                                ctrl.IsNew
                                    ? $translate.instant('Admin.Js.SmsTemplate.TemplateAdded')
                                    : $translate.instant('Admin.Js.SmsTemplate.TemplateEditedSuccessfully'),
                            );
                            $uibModalInstance.close();
                        } else {
                            var message = result.data.errors.reduce(function (old, current) {
                                return (old += current);
                            }, '');

                            toaster.pop('error', message);
                        }
                    },
                    function () {
                        toaster.pop(
                            'error',
                            '',
                            ctrl.IsNew
                                ? $translate.instant('Admin.Js.SmsTemplate.ErrorWhileAddingTemplate')
                                : $translate.instant('Admin.Js.SmsTemplate.ErrorEditingTemplate'),
                        );
                    },
                )
                .finally(function () {
                    ctrl.btnLoading = false;
                });
        };

        ctrl.copy = function (data) {
            var input = document.createElement('input');
            input.setAttribute('value', data);
            input.style.opacity = 0;
            document.body.appendChild(input);
            input.select();
            if (document.execCommand('copy')) {
                toaster.success($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.LinkCopiedToClipboard'));
            } else {
                toaster.error($translate.instant('Admin.Js.Tasks.ModalEditTaskCtrl.FailedToCopyLink'));
            }
            document.body.removeChild(input);
        };
    };

    ModalAddEditNotificationTemplateCtrl.$inject = ['$uibModalInstance', '$http', '$window', 'toaster', '$q', '$translate'];

    ng.module('uiModal').controller('ModalAddEditNotificationTemplateCtrl', ModalAddEditNotificationTemplateCtrl);
})(window.angular);
