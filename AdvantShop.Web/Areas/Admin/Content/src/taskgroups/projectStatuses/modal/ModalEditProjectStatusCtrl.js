import './editProjectStatus.html';

(function (ng) {
    'use strict';

    var ModalEditProjectStatusCtrl = function ($uibModalInstance, $http, $timeout, toaster, $translate) {
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
            ctrl.item = { Id: params.obj.item.Id };
            ctrl.statusTypeList = params.obj.statusTypeList;
            if (ctrl.item.Id === 0) {
                ctrl.item = params.obj.item;
                //ctrl.item.Name = params.item.Name;
                //ctrl.item.Color = params.item.Color;
                //ctrl.item.Status = params.item.Status;
                //ctrl.item.StatusType = params.item.StatusType;
                $timeout(function () {
                    ctrl.colorPickerApi.getScope().AngularColorPickerController.setNgModel(ctrl.item.Color);
                });
            }

            if (ctrl.item.Id != 0) {
                ctrl.getProjectStatus();
            }
        };

        ctrl.getProjectStatus = function () {
            $http.get('taskgroups/getProjectStatus', { params: { id: ctrl.item.Id } }).then(function (response) {
                var data = response.data;
                if (data != null) {
                    ctrl.item = data.obj;
                    ctrl.item.Color = data.obj.Color != null && data.obj.Color.trim() != '' ? data.obj.Color : '#000000';
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
                .post('taskgroups/updateProjectStatus', {
                    Id: ctrl.item.Id,
                    Name: ctrl.item.Name,
                    SortOrder: ctrl.item.SortOrder,
                    Color: ctrl.item.Color,
                    StatusType: ctrl.item.StatusType,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        toaster.success('', $translate.instant('Admin.Js.TaskStatuses.ChangesSaved'));
                        $uibModalInstance.close();
                    } else {
                        toaster.error(
                            $translate.instant('Admin.Js.TaskStatuses.Error'),
                            $translate.instant('Admin.Js.TaskStatuses.ErrorWhileEditing'),
                        );
                        ctrl.btnSleep = false;
                    }
                });
        };
    };

    ModalEditProjectStatusCtrl.$inject = ['$uibModalInstance', '$http', '$timeout', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalEditProjectStatusCtrl', ModalEditProjectStatusCtrl);
})(window.angular);
