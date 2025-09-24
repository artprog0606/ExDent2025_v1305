(function (ng) {
    'use strict';

    var ModalPartnersReportCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.dateFrom = params.dateFrom;
            ctrl.dateTo = params.dateTo;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalPartnersReportCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalPartnersReportCtrl', ModalPartnersReportCtrl);
})(window.angular);
