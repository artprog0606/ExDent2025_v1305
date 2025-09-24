(function (ng) {
    'use strict';

    var ModalAddEditCustomerFieldCtrl = function ($uibModalInstance, SweetAlert, $filter, $timeout, customerFieldsService, toaster, $translate, $q) {
        var ctrl = this;
        ctrl.formInited = false;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.id = params.id != null ? params.id : 0;
            ctrl.mode = ctrl.id != 0 ? 'edit' : 'add';

            customerFieldsService.getFormData().then(function (data) {
                if (data != null) {
                    ctrl.fieldTypes = data.fieldTypes;
                    ctrl.customerTypes = data.customerTypes;
                    ctrl.fieldAssignments = data.fieldAssignments;
                    ctrl.isEnabledBothTypesOfCustomers = data.isRegistrationAsPhysicalEntity && data.isRegistrationAsLegalEntity;
                    ctrl.fieldValues = [];
                    if (ctrl.mode == 'add') {
                        ctrl.fieldType = ctrl.fieldTypes.length > 0 ? ctrl.fieldTypes[1] : ctrl.fieldTypes[0];
                        ctrl.customerType = ctrl.isEnabledBothTypesOfCustomers
                            ? ctrl.customerTypes[0]
                            : data.isRegistrationAsPhysicalEntity
                              ? ctrl.customerTypes[0]
                              : ctrl.customerTypes[1];
                        ctrl.fieldAssignment = ctrl.fieldAssignments[0];
                        ctrl.sortOrder = 0;
                        ctrl.enabled = true;
                        ctrl.showInRegistration = true;
                        ctrl.showInCheckout = true;
                        ctrl.showInUserEditing = false;
                        ctrl.fieldValues.push({});
                        ctrl.formInited = true;
                    } else {
                        ctrl.getCustomerField(ctrl.id);
                    }
                }
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getCustomerField = function (id) {
            customerFieldsService.getCustomerField(id).then(function (data) {
                if (data != null) {
                    ctrl.name = data.Name;
                    ctrl.fieldType = $filter('filter')(ctrl.fieldTypes, { value: data.FieldType }, true)[0];
                    ctrl.customerType = $filter('filter')(ctrl.customerTypes, { value: data.CustomerType }, true)[0];
                    ctrl.sortOrder = data.SortOrder;
                    ctrl.required = data.Required;
                    ctrl.showInRegistration = data.ShowInRegistration;
                    ctrl.showInCheckout = data.ShowInCheckout;
                    ctrl.disableCustomerEditing = data.DisableCustomerEditing;
                    ctrl.enabled = data.Enabled;
                    ctrl.fieldValues = data.FieldValues;
                    ctrl.fieldAssignment = $filter('filter')(ctrl.fieldAssignments, { value: data.FieldAssignment }, true)[0];
                    ctrl.showInUserEditing = data.ShowInUserEditing;
                }
                ctrl.fieldValues.push({});
                ctrl.form.$setPristine();
                ctrl.formInited = true;
            });
        };

        ctrl.save = function () {
            ctrl.btnSleep = true;

            var params = {
                id: ctrl.id,
                name: ctrl.name,
                fieldType: ctrl.fieldType.value,
                customerType: ctrl.customerType.value,
                sortOrder: ctrl.sortOrder,
                required: ctrl.required,
                showInRegistration: ctrl.showInRegistration,
                showInCheckout: ctrl.showInCheckout,
                disableCustomerEditing: ctrl.disableCustomerEditing,
                enabled: ctrl.enabled,
                fieldValues: ctrl.fieldValues,
                fieldAssignment: ctrl.fieldAssignment.value,
                showInUserEditing: ctrl.showInUserEditing,
            };

            customerFieldsService.addOrUpdateCustomerField(ctrl.mode == 'add', params).then(function (data) {
                if (data.result == true) {
                    toaster.pop(
                        'success',
                        '',
                        ctrl.mode == 'add'
                            ? $translate.instant('Admin.Js.SettingsCustomers.FieldAdded')
                            : $translate.instant('Admin.Js.SettingsCustomers.ChangesSaved'),
                    );
                    $uibModalInstance.close('saveCustomerField');
                } else if (data.error) {
                    toaster.pop('error', $translate.instant('Admin.Js.SettingsCustomers.Error'), data.error);
                    ctrl.btnSleep = false;
                } else {
                    toaster.pop(
                        'error',
                        $translate.instant('Admin.Js.SettingsCustomers.Error'),
                        $translate.instant('Admin.Js.SettingsCustomers.ErrorWhile') +
                            (ctrl.mode == 'add'
                                ? $translate.instant('Admin.Js.SettingsCustomers.CreatingSmall')
                                : $translate.instant('Admin.Js.SettingsCustomers.EditingSmall')),
                    );
                    ctrl.btnSleep = false;
                }
            });
        };

        ctrl.sortableFieldValues = {
            orderChanged: function (event) {
                ctrl.form.modified = true;
            },
        };

        ctrl.addFieldValue = function (fieldValue, value) {
            ctrl.focusOnValue = false;
            $timeout(function () {
                ctrl.focusOnValue = true;
            }, 0);
            if (value == null || value == '') {
                return;
            }
            fieldValue.Id = 0;
            fieldValue.Value = value;
            ctrl.fieldValues.push({});
        };

        ctrl.deleteFieldValue = function (fieldValue) {
            SweetAlert.confirm($translate.instant('Admin.Js.Customer.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Customer.Deleting'),
            }).then(function (result) {
                if (result.value === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCustomers.ChangesSaved'));
                }
            });
            ctrl.fieldValues = $filter('filter')(ctrl.fieldValues, function (value) {
                return value !== fieldValue;
            });
        };
    };

    ModalAddEditCustomerFieldCtrl.$inject = [
        '$uibModalInstance',
        'SweetAlert',
        '$filter',
        '$timeout',
        'customerFieldsService',
        'toaster',
        '$translate',
        '$q',
    ];

    ng.module('uiModal').controller('ModalAddEditCustomerFieldCtrl', ModalAddEditCustomerFieldCtrl);
})(window.angular);
