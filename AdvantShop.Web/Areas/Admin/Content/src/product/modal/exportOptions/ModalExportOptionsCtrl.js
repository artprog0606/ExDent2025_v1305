(function (ng) {
    'use strict';

    var ModalExportOptionsCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.productId = params.productId != null ? params.productId : 0;

            ctrl.getExportOptions();
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getExportOptions = function () {
            $http.get('product/getExportOptions', { params: { productId: ctrl.productId } }).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    ctrl.data = data.obj;
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        };

        ctrl.save = function () {
            $http.post('product/saveExportOptions', ctrl.data).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.success('', $translate.instant('Admin.Js.Product.ChangesSaved'));
                    $uibModalInstance.close('saveExportOptions');
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        };
    };

    ModalExportOptionsCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalExportOptionsCtrl', ModalExportOptionsCtrl);
})(window.angular);
