import './AddSizeChartProperty.html';
import './styles.scss';
(function (ng) {
    'use strict';

    var ModalAddSizeChartPropertyCtrl = function ($uibModalInstance, $http) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.getProperties();
            ctrl.propType = '0';
            ctrl.selectedPropertyValues = ctrl.$resolve.params.selectedPropertyValues;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getProperties = function () {
            $http.get('sizeChart/getProperties').then(function (response) {
                ctrl.properties = response.data;
            });
        };

        ctrl.getPropertyValues = function () {
            if (ctrl.property == null) return;

            $http.get('sizeChart/getPropertyValues', { params: { propertyId: ctrl.property.PropertyId } }).then(function (response) {
                ctrl.propertyValues = response.data;
                if (ctrl.propertyValues != null && ctrl.propertyValues.length > 0) {
                    ctrl.propertyValue = ctrl.propertyValues[0];
                }
            });
        };

        ctrl.changeProperty = function () {
            ctrl.getPropertyValues();
        };

        ctrl.saveProperty = function () {
            $uibModalInstance.close(ctrl.selectedPropertyValues);
        };

        ctrl.addProperty = function () {
            if (!ctrl.propertyValue || !ctrl.propertyValue.PropertyValueId || !ctrl.propertyValue.Value) return;
            if (ctrl.selectedPropertyValues.some((x) => x.PropertyValueId == ctrl.propertyValue.PropertyValueId)) return;
            ctrl.selectedPropertyValues.push({
                PropertyValueId: ctrl.propertyValue.PropertyValueId,
                PropertyName: ctrl.property.Name,
                PropertyValueName: ctrl.propertyValue.Value,
            });
            ctrl.propertiesModified = true;
            ctrl.propertyValue = null;
        };

        ctrl.deletePropertyValue = function (propertyValueId) {
            ctrl.selectedPropertyValues = ctrl.selectedPropertyValues.filter((x) => x.PropertyValueId != propertyValueId);
            ctrl.propertiesModified = true;
        };
    };

    ModalAddSizeChartPropertyCtrl.$inject = ['$uibModalInstance', '$http'];

    ng.module('uiModal').controller('ModalAddSizeChartPropertyCtrl', ModalAddSizeChartPropertyCtrl);
})(window.angular);
