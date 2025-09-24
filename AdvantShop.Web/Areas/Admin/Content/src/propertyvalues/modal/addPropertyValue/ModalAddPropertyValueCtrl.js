(function (ng) {
    'use strict';

    var ModalAddPropertyValueCtrl = function ($uibModalInstance, $http, toaster, $translate, isMobileService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.propertyData = params.value;
            ctrl.mode = params.value.PropertyValueId != null ? 'edit' : 'add';
            if (ctrl.mode == 'add') {
                ctrl.propertyId = ctrl.propertyData.propertyId;
            } else {
                ctrl.propertyId = ctrl.propertyData.PropertyId;
                ctrl.propertyValueId = ctrl.propertyData.PropertyValueId;
                ctrl.value = ctrl.propertyData.Value;
                ctrl.sortOrder = ctrl.propertyData.SortOrder;
                ctrl.name = ctrl.propertyData.Name;
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.addPropertyValue = function () {
            if (ctrl.mode == 'add') {
                $http
                    .post('propertyValues/addPropertyValue', {
                        propertyId: ctrl.propertyId,
                        value: ctrl.value,
                        sortOrder: ctrl.sortOrder,
                        name: ctrl.name,
                    })
                    .then(function (response) {
                        $uibModalInstance.close('addpropertyValue');
                    })
                    .then(function (res) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Properties.ChangesSaved'));
                        if (isMobileService.getValue()) {
                            document.querySelector('.popover-backdrop').click();
                        }
                    })
                    .catch(function (error) {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Properties.Error'));
                    });
            } else {
                $http
                    .post('propertyValues/inplacePropertyValue', {
                        propertyValueId: ctrl.propertyValueId,
                        value: ctrl.value,
                        sortOrder: ctrl.sortOrder,
                        name: ctrl.name,
                    })
                    .then(function (response) {
                        ctrl.propertyData.Value = ctrl.value;
                        ctrl.propertyData.SortOrder = ctrl.sortOrder;
                        ctrl.propertyData.Name = ctrl.name;
                        $uibModalInstance.close('addpropertyValue');
                    })
                    .then(function (res) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Properties.ChangesSaved'));
                    })
                    .catch(function (error) {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Properties.Error'));
                    });
            }
        };
    };

    ModalAddPropertyValueCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate', 'isMobileService'];

    ng.module('uiModal').controller('ModalAddPropertyValueCtrl', ModalAddPropertyValueCtrl);
})(window.angular);
