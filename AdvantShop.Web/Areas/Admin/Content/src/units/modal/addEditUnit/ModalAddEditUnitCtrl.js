(function (ng) {
    'use strict';

    var ModalAddEditUnitCtrl = function ($uibModalInstance, $http, toaster, $translate, $filter) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.id = params.unitId != null ? params.unitId : 0;
            ctrl.mode = ctrl.id != 0 ? 'edit' : 'add';

            ctrl.getData().then(function (result) {
                if (ctrl.mode === 'add') {
                    ctrl.measureType = ctrl.measureTypes[0];
                    ctrl.sortOrder = 0;
                } else {
                    ctrl.getUnit(ctrl.id);
                }
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getData = function () {
            return $http.get('units/getUnitData').then(function (response) {
                var data = response.data.obj;
                if (response.data.result === true) {
                    ctrl.measureTypes = data.measureTypes;
                }

                return data;
            });
        };

        ctrl.getUnit = function (id) {
            $http.get('units/getUnit', { params: { unitId: id } }).then(function (response) {
                var data = response.data.obj;
                if (response.data.result === true) {
                    ctrl.id = data.Id;
                    ctrl.name = data.Name;
                    ctrl.displayName = data.DisplayName;
                    ctrl.measureType = $filter('filter')(ctrl.measureTypes, { Value: data.MeasureType }, true)[0];
                    ctrl.sortOrder = data.SortOrder;
                } else {
                    ctrl.close();
                    if (response.data.result.errors != null) {
                        response.data.result.errors.forEach(function (err) {
                            toaster.error('', err);
                        });
                    }
                }
            });
        };

        ctrl.save = function () {
            var params = {
                Id: ctrl.id,
                Name: ctrl.name,
                DisplayName: ctrl.displayName,
                MeasureType: ctrl.measureType != null ? ctrl.measureType.Value : null,
                SortOrder: ctrl.sortOrder,
            };

            var url = ctrl.mode === 'add' ? 'units/addUnit' : 'units/updateUnit';

            $http.post(url, params).then(function (response) {
                if (response.data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Units.AddEdit.ChangesSaved'));
                    $uibModalInstance.close({
                        id: ctrl.id,
                        name: ctrl.name,
                        displayName: ctrl.displayName,
                        measureType: ctrl.measureType,
                        sortOrder: ctrl.sortOrder,
                    });
                } else {
                    if (response.data.errors != null) {
                        response.data.errors.forEach(function (err) {
                            toaster.error('', err);
                        });
                    } else {
                        toaster.pop(
                            'error',
                            $translate.instant('Admin.Js.Units.AddEdit.Error'),
                            $translate.instant('Admin.Js.Units.AddEdit.ErrorAddingEditing'),
                        );
                    }
                }
            });
        };
    };

    ModalAddEditUnitCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate', '$filter'];

    ng.module('uiModal').controller('ModalAddEditUnitCtrl', ModalAddEditUnitCtrl);
})(window.angular);
