(function (ng) {
    'use strict';

    var ModalAddEditStockLabelCtrl = function ($uibModalInstance, stockLabelService, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.stockLabelId = params.stockLabelId != null ? params.stockLabelId : 0;
            ctrl.mode = ctrl.stockLabelId ? 'edit' : 'add';

            ctrl.colorPickerOptions = {
                swatchBootstrap: false,
                format: 'hex',
                alpha: false,
                swatchOnly: false,
                case: 'lower',
                allowEmpty: false,
                required: false,
                preserveInputFormat: false,
                restrictToFormat: false,
                inputClass: 'form-control',
            };

            if (ctrl.mode === 'add') {
                // ctrl.colorPickerApi.getScope().AngularColorPickerController.setNgModel(ctrl.color);
                ctrl.color = tinycolor.random().toHexString().slice(1);
            } else {
                ctrl.getStockLabel(ctrl.stockLabelId).then(function (data) {
                    ctrl.colorPickerApi.getScope().AngularColorPickerController.setNgModel(ctrl.color);
                });
            }

            ctrl.colorPickerEventApi = {};

            ctrl.colorPickerEventApi.onBlur = function () {
                ctrl.colorPickerApi.getScope().AngularColorPickerController.update();
            };
        };

        ctrl.$postLink = function () {
            if (ctrl.mode === 'add') {
                ctrl.colorPickerApi.getScope().AngularColorPickerController.setNgModel(ctrl.color);
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getStockLabel = function (stockLabelId) {
            return stockLabelService.getStockLabel(stockLabelId).then(function (data) {
                if (data.result === true) {
                    ctrl.name = data.obj.Name;
                    ctrl.clientName = data.obj.ClientName;
                    ctrl.color = data.obj.Color;
                    ctrl.amountUpTo = data.obj.AmountUpTo;
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });

                    if (!data.errors) {
                        toaster.pop(
                            'error',
                            $translate.instant('Admin.Js.AddEditStockLabel.Error'),
                            $translate.instant('Admin.Js.AddEditStockLabel.Error.Data'),
                        );
                    }
                }
            });
        };

        ctrl.save = function () {
            var params = {
                id: ctrl.stockLabelId,
                name: ctrl.name,
                clientName: ctrl.clientName,
                color: ctrl.color,
                amountUpTo: ctrl.amountUpTo,
            };

            var promise = ctrl.mode === 'add' ? stockLabelService.addStockLabel(params) : stockLabelService.updateStockLabel(params);

            promise.then(function (data) {
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.AddEditStockLabel.Success'));
                    $uibModalInstance.close(params);
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });

                    if (!data.errors) {
                        toaster.pop(
                            'error',
                            $translate.instant('Admin.Js.AddEditStockLabel.Error'),
                            $translate.instant('Admin.Js.AddEditStockLabel.Error.Save'),
                        );
                    }
                }
            });
        };
    };

    ModalAddEditStockLabelCtrl.$inject = ['$uibModalInstance', 'stockLabelService', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalAddEditStockLabelCtrl', ModalAddEditStockLabelCtrl);
})(window.angular);
