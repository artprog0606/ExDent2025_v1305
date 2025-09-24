(function (ng) {
    'use strict';

    var ModalAddEditLocalizationCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.ResourceKey = params.ResourceKey != null && params.ResourceKey.value != null ? params.ResourceKey.value : null;
            ctrl.LanguageCode = params.LanguageCode != null && params.LanguageCode.value != null ? params.LanguageCode.value : null;
            ctrl.mode = ctrl.LanguageCode != null || ctrl.ResourceKey != null ? 'edit' : 'add';

            if (ctrl.mode == 'edit') {
                ctrl.getLocalize(ctrl.ResourceKey, ctrl.LanguageCode);
            } else {
                ctrl.id = 0;
            }
            $http
                .get('Localization/GetLanguage', {
                    params: {
                        lang: ctrl.LanguageCode,
                        rnd: Math.random(),
                    },
                })
                .then(function (response) {
                    var data = response.data;
                    if (data != null) {
                        ctrl.Langs = data;
                        var mass = data.filter(function (item) {
                            return item.Selected === true;
                        });
                        ctrl.langSelected = mass.length > 0 ? mass[0] : null;
                    }
                });
        };

        ctrl.getLocalize = function (key, langCode) {
            $http
                .get('Localization/GetLocalizeItem', {
                    params: {
                        LanguageCode: langCode,
                        ResourceKey: key,
                        rnd: Math.random(),
                    },
                })
                .then(function (response) {
                    var data = response.data;
                    if (data != null && data.length > 0) {
                        ctrl.ResourceKey = data[0].ResourceKey;
                        ctrl.ResourceValue = data[0].ResourceValue == null ? '' : data[0].ResourceValue;
                    }
                });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.saveLocalization = function () {
            ctrl.btnSleep = true;

            var params = {
                ResourceKey: ctrl.ResourceKey,
                ResourceValue: ctrl.ResourceValue,
                LanguageCode: ctrl.langSelected.Value,
                rnd: Math.random(),
            };

            var url = 'Localization/AddEditLocalization';

            $http.post(url, params).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.SettingsSystem.ChangesSaved'));
                    $uibModalInstance.close('saveLocalization');
                } else {
                    toaster.pop(
                        'error',
                        $translate.instant('Admin.Js.SettingsSystem.Error'),
                        $translate.instant('Admin.Js.SettingsSystem.ErrorEditingLocalization'),
                    );
                    ctrl.btnSleep = false;
                }
            });
        };
    };

    ModalAddEditLocalizationCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalAddEditLocalizationCtrl', ModalAddEditLocalizationCtrl);
})(window.angular);
