import './AddEditCitys.html';
(function (ng) {
    'use strict';

    var ModalAddEditCitysCtrl = function ($uibModalInstance, $http, toaster, $translate, SweetAlert) {
        var ctrl = this;

        ctrl.$onInit = function () {
            if (ctrl.$resolve != null) {
                ctrl.regionId = ctrl.$resolve.entity.RegionId || 0;
                ctrl.cityId = ctrl.$resolve.entity.CityId || 0;
                ctrl.mode = ctrl.cityId != 0 ? 'edit' : 'add';
                if (ctrl.cityId == 0) ctrl.getFormData(ctrl.regionId);
                else ctrl.getSettings(ctrl.regionId, ctrl.cityId);
            }
        };

        ctrl.init = function (form) {
            ctrl.form = form;
        };

        ctrl.getSettings = function (regionId, сityId) {
            $http.get('Cities/GetCity', { params: { сityId: сityId, regionId: regionId, rnd: Math.random() } }).then(function (response) {
                var data = response.data;
                if (data != null) {
                    ctrl.data = data;
                    ctrl.form.$setPristine();
                }
            });
        };

        ctrl.getFormData = function (regionId) {
            $http.get('Cities/GetFormData', { params: { regionId: regionId } }).then(function (response) {
                var data = response.data;
                if (data != null) {
                    ctrl.data = data;
                    ctrl.form.$setPristine();
                }
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.openAdditionalSettings = function (formModified) {
            if (formModified) {
                SweetAlert.confirm('Необходимо сохранить основные настройки', {
                    title: 'Настройки не сохранены',
                    confirmButtonText: 'Сохранить и открыть',
                }).then(function (result) {
                    if (result === true || result.value === true) {
                        ctrl.saveCity(false).then(function (data) {
                            if (data.result) {
                                $uibModalInstance.close({
                                    openAdditionalSettings: true,
                                    entity: {
                                        CityId: ctrl.cityId,
                                        RegionId: ctrl.regionId,
                                    },
                                });
                            }
                        });
                    }
                });
            } else {
                $uibModalInstance.close({
                    openAdditionalSettings: true,
                    entity: {
                        CityId: ctrl.cityId,
                        RegionId: ctrl.regionId,
                    },
                });
            }
        };

        ctrl.saveCity = function (closeModal) {
            ctrl.btnSleep = true;

            var url = ctrl.mode == 'add' ? 'Cities/AddCity' : 'Cities/EditCity';

            return $http
                .post(url, ctrl.data)
                .then(function (response) {
                    var data = response.data;
                    if (data.result == true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.SettingsSystem.ChangesSaved'));
                        if (closeModal) $uibModalInstance.close('saveCity');
                    } else {
                        toaster.pop(
                            'error',
                            $translate.instant('Admin.Js.SettingsSystem.Error'),
                            $translate.instant('Admin.Js.SettingsSystem.ErrorCreatingCity'),
                        );
                    }

                    return data;
                })
                .finally(function () {
                    ctrl.btnSleep = false;
                });
        };
    };

    ModalAddEditCitysCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate', 'SweetAlert'];

    ng.module('uiModal').controller('ModalAddEditCitysCtrl', ModalAddEditCitysCtrl);
})(window.angular);
