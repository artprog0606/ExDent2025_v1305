(function (ng) {
    'use strict';

    var ModalAddEditAdditionalSettingsRegionCtrl = function ($uibModalInstance, $http, toaster, $translate, SweetAlert) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.regionId = ctrl.$resolve.entity.RegionId;
            ctrl.getSetings(ctrl.regionId);
        };

        ctrl.init = function (formAddEditAdditionalSettings) {
            ctrl.form = formAddEditAdditionalSettings;
        };

        ctrl.getSetings = function (regionId) {
            $http.get('regions/getadditionalsettings', { params: { regionId: regionId } }).then(function (response) {
                var data = response.data;
                if (data.result) {
                    ctrl.settings = data.obj;
                }
                ctrl.form.$setPristine();
            });
        };

        ctrl.return = function (formModified) {
            if (formModified) {
                SweetAlert.confirm('Необходимо сохранить дополнительные настройки', {
                    title: 'Настройки не сохранены',
                    confirmButtonText: 'Сохранить и открыть',
                }).then(function (result) {
                    if (result === true || result.value === true) {
                        ctrl.save().then(function (data) {
                            if (data.result) {
                                $uibModalInstance.close({
                                    returnToMainSettings: true,
                                    entity: {
                                        RegionId: ctrl.regionId,
                                    },
                                });
                            }
                        });
                    }
                });
            } else {
                $uibModalInstance.close({
                    returnToMainSettings: true,
                    entity: {
                        RegionId: ctrl.regionId,
                    },
                });
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.saveAndReturn = function () {
            ctrl.save().then(function (data) {
                if (data.result) ctrl.return();
            });
        };

        ctrl.save = function () {
            ctrl.btnSleep = true;
            return $http
                .post('regions/editadditionalsettings', ctrl.settings)
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        ctrl.form.$setPristine();
                        toaster.pop('success', '', $translate.instant('Admin.Js.SettingsSystem.ChangesSaved'));
                    } else {
                        if (data.errors && data.errors.length > 0) {
                            data.errors.forEach(function (error) {
                                toaster.pop('error', error);
                            });
                        } else {
                            toaster.pop(
                                'error',
                                $translate.instant('Admin.Js.SettingsSystem.Error'),
                                $translate.instant('Admin.Js.SettingsSystem.ErrorCreatingRegion'),
                            );
                        }
                    }

                    return data;
                })
                .finally(function () {
                    ctrl.btnSleep = false;
                });
        };
    };

    ModalAddEditAdditionalSettingsRegionCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate', 'SweetAlert'];

    ng.module('uiModal').controller('ModalAddEditAdditionalSettingsRegionCtrl', ModalAddEditAdditionalSettingsRegionCtrl);
})(window.angular);
