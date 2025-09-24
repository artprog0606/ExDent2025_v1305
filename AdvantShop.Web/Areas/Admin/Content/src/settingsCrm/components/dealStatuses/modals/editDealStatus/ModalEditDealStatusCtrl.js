import './editDealStatus.html';
(function (ng) {
    'use strict';

    var ModalEditDealStatusCtrl = function ($uibModalInstance, $http, $timeout, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.colorPickerOptions = {
                swatchBootstrap: false,
                format: 'hex',
                alpha: false,
                swatchOnly: false,
                case: 'lower',
                allowEmpty: true,
                required: false,
                preserveInputFormat: false,
                restrictToFormat: false,
                inputClass: 'form-control',
            };

            ctrl.colorPickerEventApi = {};

            ctrl.colorPickerEventApi.onBlur = function () {
                ctrl.colorPickerApi.getScope().AngularColorPickerController.update();
            };

            var params = ctrl.$resolve;
            ctrl.item = { Id: params.item.Id };
            if (ctrl.item.Id === 0) {
                ctrl.item.Name = params.item.Name;
                ctrl.item.Color = params.item.Color;
                ctrl.item.Status = params.item.Status;
                $timeout(function () {
                    ctrl.colorPickerApi.getScope().AngularColorPickerController.setNgModel(ctrl.item.Color);
                });
            }

            if (ctrl.item.Id != 0) {
                ctrl.getDealStatus();
            }
        };

        ctrl.getDealStatus = function () {
            $http.get('leads/getDealStatus', { params: { id: ctrl.item.Id } }).then(function (response) {
                var data = response.data;
                if (data != null) {
                    ctrl.item.Name = data.Name;
                    ctrl.item.SortOrder = data.SortOrder;
                    ctrl.item.Color = data.Color != null && data.Color.trim() != '' ? data.Color : '#000000';
                    $timeout(function () {
                        ctrl.colorPickerApi.getScope().AngularColorPickerController.setNgModel(ctrl.item.Color);
                    });
                }
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function () {
            if (ctrl.item.Id === 0) {
                $uibModalInstance.close(ctrl.item);
                return;
            }

            ctrl.btnSleep = true;

            $http
                .post('leads/updateDealStatus', {
                    Id: ctrl.item.Id,
                    Name: ctrl.item.Name,
                    SortOrder: ctrl.item.SortOrder,
                    Color: ctrl.item.Color,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        toaster.success('', $translate.instant('Admin.Js.SettingsCustomers.ChangesSaved'));
                        $uibModalInstance.close();
                    } else {
                        toaster.error($translate.instant('Admin.Js.SettingsCrm.Error'), $translate.instant('Admin.Js.SettingsCrm.ErrorWhileEditing'));
                        ctrl.btnSleep = false;
                    }
                });
        };
    };

    ModalEditDealStatusCtrl.$inject = ['$uibModalInstance', '$http', '$timeout', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalEditDealStatusCtrl', ModalEditDealStatusCtrl);
})(window.angular);
