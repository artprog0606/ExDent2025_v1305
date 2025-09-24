(function (ng) {
    'use strict';

    var ModalSdekDownloadBarCodeOrderCtrl = function ($uibModalInstance, $window, toaster, $q, $http, urlHelper) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.obj;
            ctrl.orderId = params.orderId;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getSendUrl = function () {
            var params = {
                orderId: ctrl.orderId,
                copyCount: ctrl.copyCount,
                format: ctrl.format,
                lang: ctrl.lang,
            };
            for (let key in params) {
                if (Object.prototype.hasOwnProperty.call(params, key)) {
                    if (!params[key]) {
                        delete params[key];
                    }
                }
            }

            return 'orders/sdekBarCodeOrder?' + urlHelper.paramsToString(params);
        };
    };

    ModalSdekDownloadBarCodeOrderCtrl.$inject = ['$uibModalInstance', '$window', 'toaster', '$q', '$http', 'urlHelper'];

    ng.module('uiModal').controller('ModalSdekDownloadBarCodeOrderCtrl', ModalSdekDownloadBarCodeOrderCtrl);
})(window.angular);
