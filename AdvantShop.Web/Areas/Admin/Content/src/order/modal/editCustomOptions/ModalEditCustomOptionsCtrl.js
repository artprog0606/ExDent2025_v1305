import './editCustomOptions.html';

(function (ng) {
    'use strict';

    var ModalEditCustomOptionsCtrl = function ($uibModalInstance, $http, toaster, $translate, orderItemCustomOptionsService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.orderItemId = params.orderItemId;
            ctrl.productId = params.productId;
            ctrl.artno = params.artno;
        };

        ctrl.customOptionsInitFn = function (customOptions) {
            ctrl.customOptions = customOptions;
        };

        ctrl.customOptionsChange = function () {};

        ctrl.save = function () {
            let isValid = true;
            const { invalidOptions, isValidOptions } = orderItemCustomOptionsService.isValidOptions(ctrl.customOptions.items);

            if (ctrl.customOptions.orderItemCustomOptionsForm.$invalid === true || isValidOptions === false) {
                ctrl.customOptions.orderItemCustomOptionsForm.$setSubmitted();
                ctrl.customOptions.orderItemCustomOptionsForm.$setDirty();

                if (invalidOptions.size > 0) {
                    for (const option of invalidOptions) {
                        let errorText = '';
                        if (option.MinQuantity != null && option.MaxQuantity != null && option.MaxQuantity === option.MinQuantity) {
                            errorText = `: Выберите ${option.MaxQuantity} варианта`;
                        } else {
                            if (option.MinQuantity != null || option.MaxQuantity != null) {
                                errorText = ':';
                            }
                            if (option.MinQuantity != null) {
                                errorText = errorText + ` значение должно быть от ${option.MinQuantity}`;
                            }
                            if (option.MaxQuantity != null && option.MaxQuantity !== option.MinQuantity) {
                                errorText = errorText + ` до ${option.MaxQuantity}`;
                            }
                        }

                        toaster.pop(
                            'error',
                            `Неверно заполнено поле
                                ${option.Title}
                                ${errorText}`,
                        );
                    }
                } else {
                    toaster.pop('error', $translate.instant('Js.Product.InvalidCustomOptions'));
                }

                isValid = invalidOptions.size === 0;
            }

            if (isValid === false) return;

            var params = {
                orderItemId: ctrl.orderItemId,
                artno: ctrl.artno,
                customOptionsXml: ctrl.customOptions.xml,
            };

            $http.post('orders/changeOrderItemCustomOptions', params).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.success('', 'Доп. опция сохранена');
                    $uibModalInstance.close();
                } else {
                    data.errors.forEach(function (error) {
                        ctrl.error = error;
                    });
                }
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalEditCustomOptionsCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate', 'orderItemCustomOptionsService'];

    ng.module('uiModal').controller('ModalEditCustomOptionsCtrl', ModalEditCustomOptionsCtrl);
})(window.angular);
