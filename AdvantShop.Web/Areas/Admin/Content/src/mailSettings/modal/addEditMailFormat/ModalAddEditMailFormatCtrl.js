(function (ng) {
    'use strict';

    var ModalAddEditMailFormatCtrl = function ($uibModalInstance, $http, toaster, $translate, $sce) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.value || {};
            ctrl.MailFormatID = params.MailFormatID != null ? params.MailFormatID : 0;
            ctrl.mode = ctrl.MailFormatID !== 0 ? 'edit' : 'add';

            ctrl.getMailFormatTypes();

            if (ctrl.MailFormatID !== 0) {
                ctrl.getMailFormat(ctrl.MailFormatID).then(ctrl.getTypeDescription);
            } else {
                ctrl.SortOrder = 0;
                ctrl.MailFormatTypeId = 1;

                ctrl.getTypeDescription();
            }
        };

        ctrl.getMailFormat = function (id) {
            return $http.get('settingsMail/getMailFormat', { params: { id: id } }).then(function (response) {
                var data = response.data;
                if (data != null) {
                    ctrl.FormatName = data.FormatName;
                    ctrl.Enable = data.Enable;
                    ctrl.MailFormatTypeId = data.MailFormatTypeId;
                    ctrl.FormatSubject = data.FormatSubject;
                    ctrl.FormatText = data.FormatText;
                    ctrl.SortOrder = data.SortOrder;
                    ctrl.FormatDescription = data.FormatDescription;
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.MailSettings.Error'), response.data.error);
                }
            });
        };

        ctrl.getMailFormatTypes = function (id) {
            $http.get('settingsMail/getMailFormatTypes').then(function (response) {
                var data = response.data;
                if (data != null) {
                    ctrl.MailFormatTypes = data;
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.MailSettings.Error'), response.data.error);
                }
            });
        };

        ctrl.getTypeDescription = function () {
            $http.get('settingsMail/getTypeDescription', { params: { mailFormatTypeId: ctrl.MailFormatTypeId } }).then(function (response) {
                if (response.data.result) {
                    ctrl.Description = $sce.trustAsHtml(response.data.message);
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.MailSettings.Error'), response.data.error);
                }
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function () {
            var params = {
                MailFormatID: ctrl.MailFormatID,
                MailFormatTypeId: ctrl.MailFormatTypeId,
                FormatName: ctrl.FormatName,
                Enable: ctrl.Enable,
                FormatSubject: ctrl.FormatSubject,
                FormatText: ctrl.FormatText,
                SortOrder: ctrl.SortOrder,
            };

            var url = ctrl.mode === 'add' ? 'settingsMail/Add' : 'settingsMail/Edit';

            $http.post(url, params).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.MainPageProducts.ChangesSaved'));
                    $uibModalInstance.close('saveSize');
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

    ModalAddEditMailFormatCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate', '$sce'];

    ng.module('uiModal').controller('ModalAddEditMailFormatCtrl', ModalAddEditMailFormatCtrl);
})(window.angular);
