(function (ng) {
    'use strict';

    var ModalAddEditSettingsSearchCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.value || {};
            ctrl.Id = params.Id != null ? params.Id : 0;
            ctrl.Title = params.Title;
            ctrl.Link = params.Link;
            ctrl.KeyWords = params.KeyWords;
            ctrl.SortOrder = params.SortOrder != null ? params.SortOrder : 0;

            ctrl.mode = ctrl.Id != 0 ? 'edit' : 'add';
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function () {
            var params = {
                Id: ctrl.Id,
                Title: ctrl.Title,
                Link: ctrl.Link,
                KeyWords: ctrl.KeyWords,
                SortOrder: ctrl.SortOrder,
            };

            var url = ctrl.mode == 'add' ? 'SettingsSearch/AddSettingsSearch' : 'SettingsSearch/EditSettingsSearch';

            $http.post(url, params).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.SettingsSystem.ChangesSaved'));
                    $uibModalInstance.close('saveSize');
                } else {
                    toaster.pop(
                        'error',
                        $translate.instant('Admin.Js.SettingsSystem.Error'),
                        data.error || $translate.instant('Admin.Js.SettingsSystem.ErrorCreatingEditing'),
                    );
                }
            });
        };
    };

    ModalAddEditSettingsSearchCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalAddEditSettingsSearchCtrl', ModalAddEditSettingsSearchCtrl);
})(window.angular);
