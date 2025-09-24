import './editableGridRow.html';

(function (ng) {
    'use strict';

    var ModalEditableGridRowCtrl = function ($uibModalInstance, $http, toaster, $translate, $filter, $scope) {
        const ctrl = this;

        ctrl.$onInit = function () {
            $scope.$on('modal.closing', ctrl.onClose);
            ctrl.uiGridCellCustomScopes = [];
            ctrl.params = ctrl.$resolve.params;
            ctrl.editableColumns = ctrl.$resolve.params.editableColumns || [];
            ctrl.columnDefs = ctrl.$resolve.params.columnDefs || [];
            ctrl.row = ctrl.$resolve.params.row;
            ctrl.backupRowData = Object.assign({}, ctrl.row.entity);
            ctrl.uiGridCustom = $scope.$ctrl;
        };

        ctrl.addUiGridCellCustomScope = function (scope) {
            ctrl.uiGridCellCustomScopes.push(scope);
        };

        ctrl.onClose = function ($event, reason, closed) {
            if (closed === false) {
                ctrl.row.entity = ctrl.backupRowData;
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function () {
            ctrl.uiGridCustom.inplaceApplyAll(ctrl.editableColumns, ctrl.uiGridCellCustomScopes, ctrl.row);

            $uibModalInstance.close();
        };
    };

    ModalEditableGridRowCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate', '$filter', '$scope'];

    ng.module('uiModal').controller('ModalEditableGridRowCtrl', ModalEditableGridRowCtrl);
})(window.angular);
