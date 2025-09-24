(function (ng) {
    'use strict';

    var ModalAddEditShippingReplaceGeoCtrl = function ($http, $uibModalInstance, toaster, $translate) {
        var ctrl = this;
        ctrl.model = {};

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.id = params != null && params.id != null ? params.id : 0;
            ctrl.mode = ctrl.id != 0 ? 'edit' : 'add';

            ctrl.getShippingTypes().then(function (result) {
                if (result && ctrl.mode === 'edit') {
                    ctrl.getShippingReplaceGeo();
                }
            });
        };

        ctrl.getShippingTypes = function () {
            return $http.get('shippingMethods/getTypesList').then(function (response) {
                ctrl.types = response.data;

                return true;
            });
        };

        ctrl.getShippingReplaceGeo = function () {
            return $http.post('shippingReplaceGeo/get', { id: ctrl.id }).then(function (response) {
                var data = response.data;

                if (data.result === true) {
                    ctrl.model = data.obj;
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });

                    if (!data.errors) {
                        toaster.pop('error', 'Ошибка', 'Ошибка при загрузке данных');
                    }
                    ctrl.dismiss();
                }
            });
        };

        ctrl.save = function () {
            var url = ctrl.mode === 'add' ? 'shippingReplaceGeo/add' : 'shippingReplaceGeo/update';
            $http.post(url, ctrl.model).then(function (result) {
                var data = result.data;
                if (data.result === true) {
                    toaster.pop('success', '', ctrl.mode === 'add' ? 'Добавлено' : 'Сохранено');
                    $uibModalInstance.close();
                } else {
                    ctrl.btnLoading = false;
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });
                }
            });
        };

        ctrl.dismiss = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalAddEditShippingReplaceGeoCtrl.$inject = ['$http', '$uibModalInstance', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalAddEditShippingReplaceGeoCtrl', ModalAddEditShippingReplaceGeoCtrl);
})(window.angular);
