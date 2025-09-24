import './modalAddEditOkCatalog.html';

(function (ng) {
    'use strict';

    var ModalAddEditOkCatalogCtrl = function ($uibModalInstance, $http, toaster) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.catalog = {};

            var params = ctrl.$resolve;
            ctrl.catalog.Id = params.id != null ? params.id : 0;
            ctrl.type = ctrl.catalog.Id !== 0 ? 'edit' : 'add';

            if (ctrl.type == 'edit') ctrl.getCatalog();
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.getCatalog = function () {
            return $http.get('okMArket/GetCatalog', { params: { id: ctrl.catalog.Id } }).then(function (response) {
                ctrl.catalog = response.data;
            });
        };

        ctrl.selectCategories = function (categories) {
            ctrl.catalog.CategoryIds = categories.categoryIds;
        };

        ctrl.save = function () {
            if (ctrl.catalog.CategoryIds == null || ctrl.catalog.CategoryIds.length === 0) {
                toaster.pop('error', 'Ошибка сохранения', 'Выберите категории магазина');
                return;
            }

            $http.post('okMarket/SaveCatalog', { model: ctrl.catalog }).then(function (response) {
                if (response.data.result === true) {
                    $uibModalInstance.close();
                } else {
                    if (response.data.errors) {
                        response.data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });
                    } else {
                        toaster.pop('error', '', 'Ошибка при сохранении');
                    }
                }
            });
        };

        ctrl.removeCatalogs = function () {
            ctrl.catalog.CategoryIds = [];
        };
    };

    ModalAddEditOkCatalogCtrl.$inject = ['$uibModalInstance', '$http', 'toaster'];

    ng.module('uiModal').controller('ModalAddEditOkCatalogCtrl', ModalAddEditOkCatalogCtrl);
})(window.angular);
