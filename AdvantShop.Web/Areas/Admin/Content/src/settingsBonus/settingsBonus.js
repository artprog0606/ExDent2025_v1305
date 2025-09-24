(function (ng) {
    'use strict';

    var SettingsBonusCtrl = function ($http, toaster, $translate) {
        var ctrl = this;

        ctrl.uniSenderRegister = function () {
            if (!ctrl.uniSenderRegEmail || !ctrl.uniSenderRegLogin || !ctrl.uniSenderRegPassword) {
                toaster.error($translate.instant('Admin.Js.SettingsBonus.SpecifyData'));
                return;
            }
            ctrl.uniSenderRegProgress = true;
            $http
                .post('settingsbonus/unisenderregister', {
                    email: ctrl.uniSenderRegEmail,
                    login: ctrl.uniSenderRegLogin,
                    password: ctrl.uniSenderRegPassword,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        toaster.success($translate.instant('Admin.Js.SettingsBonus.SuccessfulyRegisteredInUniSender'));
                        ctrl.UniSenderApiKey = data.obj.apiKey;
                    } else {
                        data.errors.forEach(function (error) {
                            toaster.error($translate.instant('Admin.Js.SettingsBonus.Error'), error);
                        });
                    }
                    ctrl.uniSenderRegEmail = ctrl.uniSenderRegLogin = ctrl.uniSenderRegPassword = '';
                    ctrl.uniSenderRegProgress = false;
                    ctrl.uniSenderRegistered = false;
                });
        };
    };

    SettingsBonusCtrl.$inject = ['$http', 'toaster', '$translate'];

    ng.module('settingsBonus', []).controller('SettingsBonusCtrl', SettingsBonusCtrl);
})(window.angular);
