(function (ng) {
    'use strict';

    var ModalAddEditTypeWarehouseCtrl = function ($uibModalInstance, warehouseTypesService, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.typeId = params.typeId != null ? params.typeId : 0;
            ctrl.mode = ctrl.typeId ? 'edit' : 'add';

            if (ctrl.mode === 'add') {
                ctrl.sortOrder = 0;
                ctrl.enabled = true;
            } else {
                ctrl.getType(ctrl.typeId);
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getType = function (typeId) {
            warehouseTypesService.getWarehouseType(typeId).then(function (data) {
                if (data.result === true) {
                    ctrl.name = data.obj.Name;
                    ctrl.sortOrder = data.obj.SortOrder;
                    ctrl.enabled = data.obj.Enabled;
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });

                    if (!data.errors) {
                        toaster.pop(
                            'error',
                            $translate.instant('Admin.Js.AddEditTypeWarehouse.Error'),
                            $translate.instant('Admin.Js.AddEditTypeWarehouse.Error.Data'),
                        );
                    }
                }
            });
        };

        ctrl.save = function () {
            var params = {
                id: ctrl.typeId,
                name: ctrl.name,
                sortOrder: ctrl.sortOrder,
                enabled: ctrl.enabled,
            };

            var promise = ctrl.mode === 'add' ? warehouseTypesService.addWarehouseType(params) : warehouseTypesService.updateWarehouseType(params);

            promise.then(function (data) {
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.AddEditTypeWarehouse.Success'));
                    $uibModalInstance.close(params);
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });

                    if (!data.errors) {
                        toaster.pop(
                            'error',
                            $translate.instant('Admin.Js.AddEditTypeWarehouse.Error'),
                            $translate.instant('Admin.Js.AddEditTypeWarehouse.Error.Save'),
                        );
                    }
                }
            });
        };
    };

    ModalAddEditTypeWarehouseCtrl.$inject = ['$uibModalInstance', 'warehouseTypesService', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalAddEditTypeWarehouseCtrl', ModalAddEditTypeWarehouseCtrl);
})(window.angular);
