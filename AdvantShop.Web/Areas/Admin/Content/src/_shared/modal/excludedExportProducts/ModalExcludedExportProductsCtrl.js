(function (ng) {
    'use strict';

    var ModalExcludedExportProductsCtrl = function ($uibModalInstance) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;

            ctrl.countMissingArtNo = params.listMissingArtNo.length;
            ctrl.countExcluded = params.countExcludedArtNo;
            ctrl.missingArtNo = params.listMissingArtNo.join('\n');
            ctrl.isExclude = params.isExclude;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalExcludedExportProductsCtrl.$inject = ['$uibModalInstance'];

    ng.module('uiModal').controller('ModalExcludedExportProductsCtrl', ModalExcludedExportProductsCtrl);
})(window.angular);
