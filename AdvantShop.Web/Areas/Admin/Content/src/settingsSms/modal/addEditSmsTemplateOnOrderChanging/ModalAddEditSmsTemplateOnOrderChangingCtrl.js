(function (ng) {
    'use strict';

    var ModalAddEditSmsTemplateOnOrderChangingCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.value || {};
            ctrl.Id = params.id != null ? params.id : 0;
            ctrl.mode = ctrl.Id !== 0 ? 'edit' : 'add';

            ctrl.getOrderStatuses().then(function () {
                if (ctrl.Id !== 0) {
                    ctrl.getTemplate(ctrl.Id);
                } else {
                    ctrl.template = { Enabled: true };
                    if (ctrl.orderStatuses != null && ctrl.orderStatuses.length > 0) {
                        ctrl.template.OrderStatusId = ctrl.orderStatuses[0].StatusID;
                    }
                }
            });
        };

        ctrl.getTemplate = function (id) {
            return $http.get('settingsSms/getTemplate', { params: { id: id } }).then(function (response) {
                ctrl.template = response.data;
            });
        };

        ctrl.getOrderStatuses = function () {
            return $http.get('orderStatuses/getAllStatuses').then(function (response) {
                ctrl.orderStatuses = response.data;
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function () {
            var url = ctrl.mode === 'add' ? 'settingsSms/addTemplate' : 'settingsSms/editTemplate';

            $http.post(url, ctrl.template).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.MainPageProducts.ChangesSaved'));
                    $uibModalInstance.close();
                } else {
                    toaster.pop(
                        'error',
                        $translate.instant('Admin.Js.MailSettings.Error'),
                        data.errors || $translate.instant('Admin.Js.MailSettings.ErrorWhileCreatingEditing'),
                    );
                }
            });
        };
    };

    ModalAddEditSmsTemplateOnOrderChangingCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalAddEditSmsTemplateOnOrderChangingCtrl', ModalAddEditSmsTemplateOnOrderChangingCtrl);
})(window.angular);
