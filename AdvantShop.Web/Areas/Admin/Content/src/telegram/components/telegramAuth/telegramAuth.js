import telegramAuthTemplate from './telegramAuth.html';
(function (ng) {
    'use strict';

    var telegramAuthCtrl = function ($http, toaster, SweetAlert, $translate, telegramAuthService) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.getSettings();
        };
        ctrl.getSettings = function () {
            telegramAuthService.getSettings().then(function (data) {
                ctrl.settings = data;
            });
        };
        ctrl.save = function () {
            ctrl.btnLoading = true;
            telegramAuthService
                .saveSettings({
                    token: ctrl.settings.token,
                })
                .then(function (data) {
                    if (data.result) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCrm.SettingsSuccessfulySaved'));
                        ctrl.getSettings();
                        if (ctrl.onAddDelTelegram) {
                            ctrl.onAddDelTelegram();
                        }
                    } else {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', '', error);
                        });
                    }
                    ctrl.btnLoading = false;
                });
        };
        ctrl.deActivate = function () {
            SweetAlert.confirm('Вы уверены, что хотите отключить канал?', {
                title: 'Удаление',
            }).then(function (result) {
                if (result && !result.isDismissed) {
                    telegramAuthService.deActivate().then(function () {
                        ctrl.getSettings();
                        if (ctrl.onAddDelTelegram) {
                            ctrl.onAddDelTelegram();
                        }
                    });
                }
            });
        };
        ctrl.changeSalesFunnel = function () {
            telegramAuthService.changeSalesFunnel(ctrl.settings.salesFunnelId).then(function (data) {
                toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCrm.ChangesSaved'));
            });
        };
    };
    telegramAuthCtrl.$inject = ['$http', 'toaster', 'SweetAlert', '$translate', 'telegramAuthService'];
    ng.module('telegramAuth', ['salesChannels'])
        .controller('telegramAuthCtrl', telegramAuthCtrl)
        .component('telegramAuth', {
            templateUrl: telegramAuthTemplate,
            controller: 'telegramAuthCtrl',
            bindings: {
                saasData: '<',
                onAddDelTelegram: '&',
            },
        });
})(window.angular);
