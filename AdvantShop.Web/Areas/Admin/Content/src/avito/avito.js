(function (ng) {
    'use strict';

    var AvitoCtrl = function ($q, $http, $timeout, toaster) {
        var ctrl = this;

        ctrl.avitoInit = function (productId) {
            ctrl.getProperties(productId);
        };

        ctrl.getProperties = function (productId) {
            $http.post('avito/getProperties', { productId: productId }).then(function (response) {
                if (response.data.result) {
                    ctrl.AvitoProductProperties = response.data.obj;
                }
            });
        };

        ctrl.saveProperties = function (productId) {
            return $http.post('avito/saveProperties', { productId: productId, properties: ctrl.AvitoProductProperties }).then(function (response) {
                if (response.data.result) {
                    toaster.pop('success', '', 'Настройки Авито сохранены');
                } else {
                    toaster.pop('error', '', 'Не удалось сохранить настройки Авито, незаполнены поля или дублирование данных');
                }
                return response.data;
            });
        };

        ctrl.addPropertyPair = function (productId) {
            if (ctrl.AvitoProductProperties == null) {
                ctrl.AvitoProductProperties = [];
            }
            ctrl.AvitoProductProperties.push({ ProductId: productId, PropertyName: '', PropertyValue: '' });
        };

        ctrl.deleteProductPropertyPair = function (index) {
            ctrl.AvitoProductProperties.splice(index, 1);
        };
    };

    AvitoCtrl.$inject = ['$q', '$http', '$timeout', 'toaster'];

    ng.module('avito', []).controller('AvitoCtrl', AvitoCtrl);
})(window.angular);
