import instagramAuthTemplate from './instagramAuth.html';
(function (ng) {
    'use strict';

    var instagramAuthCtrl = function ($http, toaster, SweetAlert, instagramAuthService, $translate, $window) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.getSettings();
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    instagramCtrl: ctrl,
                });
            }
        };
        ctrl.getSettings = function () {
            instagramAuthService.getInstagramSettings().then(function (data) {
                ctrl.settings = data;
            });
        };
        ctrl.save = function () {
            var login = ctrl.settings.login;
            if (
                ctrl.settings.login == null ||
                ctrl.settings.login.length == 0 ||
                ctrl.settings.password == null ||
                ctrl.settings.password.length == 0
            ) {
                toaster.pop('error', '', 'Заполните обязательные поля');
                return;
            }
            if (login.indexOf('@') != -1 && login[0] != '@') {
                toaster.pop('error', '', 'Укажите логин вида @mylogin');
                return;
            }
            instagramAuthService
                .saveLoginSettings({
                    login: ctrl.settings.login,
                    password: ctrl.settings.password,
                })
                .then(function (data) {
                    if (data.result) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCrm.SettingsSuccessfulySaved'));
                        ctrl.getSettings();
                        if (ctrl.onAddDelInstagram) {
                            ctrl.onAddDelInstagram();
                        }
                    } else {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', '', error);
                        });
                        if (data.obj.IsChallengeRequired == true) {
                            ctrl.ApiPath = data.obj.ApiPath;
                            ctrl.IsChallengeRequired = data.obj.IsChallengeRequired;
                        }
                    }
                });
        };
        ctrl.deActivate = function () {
            instagramAuthService.deActivate().then(function () {
                ctrl.getSettings();
                if (ctrl.onAddDelInstagram) {
                    ctrl.onAddDelInstagram();
                }
            });
        };
        ctrl.removeSaleChannel = function () {
            SweetAlert.confirm('Вы уверены, что хотите отключить канал?', {
                title: 'Удаление',
            }).then(function (result) {
                if (result && !result.isDismissed) {
                    instagramAuthService.removeSaleChannel().then(function () {
                        var basePath = document.getElementsByTagName('base')[0].getAttribute('href');
                        $window.location.assign(basePath);
                    });
                }
            });
        };
        ctrl.saveSettings = function () {
            instagramAuthService
                .saveSettings(ctrl.settings.salesFunnelId, ctrl.settings.createLeadFromDirectMessages, ctrl.settings.createLeadFromComments)
                .then(function (data) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCrm.ChangesSaved'));
                });
        };
        ctrl.requireChallengeCode = function (choice) {
            instagramAuthService.requireChallengeCode(ctrl.ApiPath, choice).then(function (data) {
                if (data.result) {
                    ctrl.IsChallengeRequired = false;
                    ctrl.IsChallengeRequiredCode = true;
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', '', error);
                    });
                }
            });
        };
        ctrl.sendChallengeCode = function () {
            instagramAuthService.sendChallengeCode(ctrl.ApiPath, ctrl.code).then(function (data) {
                if (data.result) {
                    ctrl.IsChallengeRequiredCode = false;
                    toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCrm.SettingsSuccessfulySaved'));
                    ctrl.getSettings();
                    if (ctrl.onAddDelInstagram) {
                        ctrl.onAddDelInstagram();
                    }
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', '', error);
                    });
                }
            });
        };
    };
    instagramAuthCtrl.$inject = ['$http', 'toaster', 'SweetAlert', 'instagramAuthService', '$translate', '$window'];
    ng.module('instagramAuth', [])
        .controller('instagramAuthCtrl', instagramAuthCtrl)
        .component('instagramAuth', {
            templateUrl: instagramAuthTemplate,
            controller: 'instagramAuthCtrl',
            bindings: {
                saasData: '<',
                onAddDelInstagram: '&',
                onInit: '&',
            },
        });
})(window.angular);
