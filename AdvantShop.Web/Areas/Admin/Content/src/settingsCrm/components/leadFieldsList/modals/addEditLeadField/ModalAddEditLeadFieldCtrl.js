import './addEditLeadField.html';
(function (ng) {
    'use strict';

    var ModalAddEditLeadFieldCtrl = function ($uibModalInstance, $timeout, leadFieldsService, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.field = angular.copy(params.field || { SalesFunnelId: params.salesFunnelId });
            ctrl.editing = ctrl.field.Id != null;
            ctrl.saveImmediately = !!ctrl.field.SalesFunnelId;

            leadFieldsService.getFormData().then(function (data) {
                if (data != null) {
                    ctrl.fieldTypes = data.fieldTypes;
                    if (!ctrl.editing) {
                        ctrl.field.Enabled = true;
                        ctrl.field.FieldValues = [{}];
                        ctrl.field.Id = 0;
                    } else if (ctrl.saveImmediately) {
                        ctrl.getLeadField(ctrl.field.Id);
                    }
                }
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getLeadField = function (id) {
            leadFieldsService.getLeadField(id).then(function (data) {
                if (data != null) {
                    ctrl.field = data.obj;
                }
                if (!ctrl.field.FieldValues) {
                    ctrl.field.FieldValues = [];
                }
                ctrl.field.FieldValues.push({});
                ctrl.form.$setPristine();
            });
        };

        ctrl.save = function () {
            if (!ctrl.saveImmediately) {
                ctrl.field.FieldTypeFormatted = ctrl.fieldTypes.find(function (fieldType) {
                    return fieldType.value == ctrl.field.FieldType;
                }).label;
                $uibModalInstance.close(ctrl.field);
                return;
            }
            ctrl.btnSleep = true;
            leadFieldsService.addOrUpdateLeadField(ctrl.field).then(function (data) {
                if (data.result == true) {
                    toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                    $uibModalInstance.close();
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                    ctrl.btnSleep = false;
                }
            });
        };

        ctrl.sortableFieldValues = {
            orderChanged: function (event) {
                ctrl.form.modified = true;
            },
        };

        ctrl.addFieldValue = function (fieldValue) {
            ctrl.focusOnValue = false;
            $timeout(function () {
                ctrl.focusOnValue = true;
            }, 0);
            if (fieldValue.Value == null || fieldValue.Value == '') {
                return;
            }
            fieldValue.Id = 0;
            ctrl.field.FieldValues.push({});
        };

        ctrl.deleteFieldValue = function (index) {
            ctrl.field.FieldValues.splice(index, 1);
            ctrl.form.modified = true;
        };
    };

    ModalAddEditLeadFieldCtrl.$inject = ['$uibModalInstance', '$timeout', 'leadFieldsService', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalAddEditLeadFieldCtrl', ModalAddEditLeadFieldCtrl);
})(window.angular);
