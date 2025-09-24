(function (ng) {
    'use strict';

    var SettingsTwoFactorAuthCtrl = function ($http, toaster) {
        var ctrl = this;
        ctrl.getQrCode = function (email, password, moduleId) {
            $http
                .get('settingsTwoFactorAuth/getQrCode', {
                    params: { email: email, password: password, moduleId: moduleId },
                })
                .then(function (response) {
                    ctrl.Codes = response.data;
                    if (ctrl.Codes == null) {
                        toaster.pop('error', '', 'Неверный логин или пароль или недостаточно прав');
                    }
                });
        };

        ctrl.getAuthActive = function (email, password, moduleId) {
            $http
                .get('settingsTwoFactorAuth/getAuthActive', {
                    params: { email: email, password: password, moduleId: moduleId },
                })
                .then(function (response) {
                    ctrl.TwoFactorAuthActive = response.data;
                });
        };

        ctrl.saveAuthActive = function (email, password, moduleId, twoFactorAuthActive) {
            $http
                .post('settingsTwoFactorAuth/saveAuthActive', {
                    email: email,
                    password: password,
                    moduleId: moduleId,
                    twoFactorAuthActive: twoFactorAuthActive,
                })
                .then(function (response) {
                    if (response.status === 200) {
                        toaster.pop('success', '', 'Настройка изменена');
                    } else {
                        toaster.pop('error', '', 'Ошибка изменения настройки');
                    }
                });
        };
    };

    SettingsTwoFactorAuthCtrl.$inject = ['$http', 'toaster'];

    ng.module('settingsTwoFactorAuth', []).controller('SettingsTwoFactorAuthCtrl', SettingsTwoFactorAuthCtrl);
})(window.angular);
