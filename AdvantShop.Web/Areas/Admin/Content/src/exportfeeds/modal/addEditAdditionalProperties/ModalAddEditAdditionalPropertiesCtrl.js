(function (ng) {
    'use strict';

    var ModalAddEditAdditionalPropertiesCtrl = function ($uibModalInstance, $http, toaster, $q) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.value;
            ctrl.exportFeedId = params.exportFeedId;

            ctrl.propertiesSize = 200;

            ctrl.getExceptionProperties();

            // загружаем первые св-ва иначе не будет работать reach-infinity
            ctrl.firstCallProperties();
        };

        ctrl.findProperty = function (q, $select) {
            ctrl.$selectProperty = $select;
            ctrl.propertiesQ = q;
            ctrl.propertiesPage = 0;
            ctrl.propertiesTotalPageCount = 0;

            ctrl.getMore();
        };

        ctrl.getMore = function () {
            if (ctrl.loadingProperties === true || (ctrl.propertiesPage > 0 && ctrl.propertiesPage >= ctrl.propertiesTotalPageCount)) {
                return $q.resolve();
            }

            ctrl.propertiesPage += 1;
            ctrl.loadingProperties = true;
            return ctrl
                .getAllProperties(ctrl.propertiesPage, ctrl.propertiesSize, ctrl.propertiesQ)
                .then(function (data) {
                    ctrl.propertiesList = ctrl.propertiesPage === 1 ? data.DataItems : ctrl.propertiesList.concat(data.DataItems);
                    ctrl.propertiesTotalPageCount = data.TotalPageCount;
                    return data;
                })
                .finally(function () {
                    ctrl.loadingProperties = false;
                });
        };

        ctrl.firstCallProperties = function () {
            ctrl.propertiesPage = 0;
            ctrl.propertiesList = [];
            ctrl.propertiesTotalPageCount = 0;
            ctrl.propertiesQ = null;
            ctrl.getMore();
        };

        ctrl.selectProperty = function ($item, $model) {
            if ($item) {
                ctrl.choosedItem = $item;

                if (ctrl.$selectProperty) {
                    ctrl.$selectProperty.search = $model.Name;
                }
            }
        };

        ctrl.clickProperty = function () {
            if (ctrl.propertiesList != null && ctrl.propertiesList.length > 0) {
                let notSkip =
                    ctrl.propertiesList.length === 1 && ctrl.choosedItem != null && ctrl.propertiesList[0].PropertyId === ctrl.choosedItem.PropertyId;
                if (!notSkip) {
                    return;
                }
            }

            ctrl.firstCallProperties();
        };

        ctrl.getExceptionProperties = function () {
            $http
                .get('exportfeeds/getExportOnlyUseInDetailsPropertiesExceptions', {
                    params: { exportFeedId: ctrl.exportFeedId },
                })
                .then(function (response) {
                    var data = response.data.obj;
                    ctrl.exceptionProperties = data.exceptionProperties || [];
                });
        };

        ctrl.getAllProperties = function (page, count, q) {
            return $http
                .get('product/getAllProperties', { params: { page: page, count: count, q: q, useInDetails: false } })
                .then(function (response) {
                    return response.data;
                });
        };

        ctrl.addProperty = function () {
            ctrl.exceptionProperties = ctrl.exceptionProperties || [];

            if (ctrl.selectedProperty != null && ctrl.exceptionProperties.find((x) => x.value === ctrl.selectedProperty.PropertyId) == null) {
                ctrl.exceptionProperties.push({
                    label: ctrl.selectedProperty.Name,
                    value: ctrl.selectedProperty.PropertyId,
                });
            }
        };

        ctrl.removeProperty = function (item) {
            var index = ctrl.exceptionProperties.indexOf(item);
            if (index !== -1) {
                ctrl.exceptionProperties.splice(index, 1);
            }
        };

        ctrl.save = function () {
            $http
                .post('exportfeeds/saveExportOnlyUseInDetailsPropertiesExceptions', {
                    exportFeedId: ctrl.exportFeedId,
                    exceptionProperties: ctrl.exceptionProperties,
                })
                .then(function (responce) {
                    var data = responce.data;

                    if (data.result) {
                        $uibModalInstance.close(responce.data.obj);
                        toaster.success('', 'Настройки успешно сохранены');
                    } else if (data.errors != null) {
                        data.errors.forEach(function (err) {
                            toaster.pop('error', 'Ошибка', err);
                        });
                    }
                });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalAddEditAdditionalPropertiesCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$q'];

    ng.module('uiModal').controller('ModalAddEditAdditionalPropertiesCtrl', ModalAddEditAdditionalPropertiesCtrl);
})(window.angular);
