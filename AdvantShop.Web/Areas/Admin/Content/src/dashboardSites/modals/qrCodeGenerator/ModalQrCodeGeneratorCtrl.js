(function (ng) {
    'use strict';

    var ModalQrCodeGeneratorCtrl = function ($uibModalInstance, $http, toaster) {
        var ctrl = this;
        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            if (params) {
                ctrl.text = params.text;
                ctrl.generate();
            }
        };

        ctrl.generate = function () {
            $http
                .post('dashboard/generateQrCode', {
                    text: ctrl.text,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result) ctrl.qrCodeInBase64 = data.obj;
                    else {
                        if (data.errors)
                            data.errors.forEach(function (error) {
                                toaster.pop('error', error);
                            });
                        else toaster.pop('error', 'Ошибка при генерации');
                    }
                });
        };

        ctrl.download = function () {
            var a = document.createElement('a');
            a.href = 'data:image/png;base64,' + ctrl.qrCodeInBase64;
            a.download = 'QrCode.png';
            a.click();
            a.remove();
            $uibModalInstance.close();
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalQrCodeGeneratorCtrl.$inject = ['$uibModalInstance', '$http', 'toaster'];

    ng.module('uiModal').controller('ModalQrCodeGeneratorCtrl', ModalQrCodeGeneratorCtrl);
})(window.angular);
