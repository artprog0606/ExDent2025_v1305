import facebookAuthTemplate from './facebookAuth.html';
(function (ng) {
    'use strict';

    var facebookAuthCtrl = function ($http, toaster, SweetAlert, facebookAuthService, $translate) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.getSettings();
        };
        ctrl.getSettings = function () {
            facebookAuthService.getSettings().then(function (data) {
                ctrl.clientId = data.clientId;
                ctrl.groupId = data.groupId;
                ctrl.groupName = data.groupName;
                ctrl.isActive = data.isActive;
                ctrl.salesFunnels = data.salesFunnels;
                ctrl.salesFunnelId = data.salesFunnelId;
                ctrl.verifyToken = data.verifyToken;
                ctrl.verifyUrl = data.verifyUrl;
                ctrl.createLeadFromMessages = data.createLeadFromMessages;
                ctrl.createLeadFromComments = data.createLeadFromComments;
            });
        };

        // Авторизация в facebook с правами пользователя, чтобы получить список групп
        ctrl.authUser = function () {
            var w = 700;
            var h = 525;
            var left = screen.width / 2 - w / 2;
            var top = screen.height / 2 - h / 2;
            var url =
                'https://www.facebook.com/v2.11/dialog/oauth?' +
                'client_id=' +
                ctrl.clientId +
                '&redirect_uri=' +
                ctrl.redirectUrl +
                '&scope=manage_pages,pages_messaging,publish_pages,email,read_page_mailboxes' +
                '&response_type=code';
            var win = window.open(url, '', 'width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
            win.focus();
            var timer = window.setInterval(function () {
                try {
                    var url = win.document.URL;
                    if (url.indexOf(ctrl.redirectUrl) !== -1) {
                        var code = '';
                        var urlParts = url.split('?')[1].split('&');
                        for (var i = 0; i < urlParts.length; i++) {
                            if (urlParts[i].indexOf('code') !== -1) {
                                code = urlParts[i].replace('code=', '');
                            }
                        }
                        win.close();
                        window.clearInterval(timer);
                        return facebookAuthService
                            .saveAuthUser({
                                clientId: ctrl.clientId,
                                clientSecret: ctrl.clientSecret,
                                code: code,
                                redirectUrl: ctrl.redirectUrl,
                            })
                            .then(function (data) {
                                if (data.result) {
                                    ctrl.groups = data.obj;
                                } else {
                                    data.errors.forEach(function (error) {
                                        toaster.pop('error', '', error);
                                    });
                                }
                            });
                    }
                } catch (e) {
                    console.log(e);
                }
            }, 100);
        };
        ctrl.saveGroup = function () {
            if (ctrl.selectedGroup == null) {
                return;
            }
            facebookAuthService
                .saveGroupToken({
                    group: ctrl.selectedGroup,
                })
                .then(function (data) {
                    if (data.Id != null) {
                        ctrl.groupId = data.Id;
                        ctrl.groupName = data.Name;
                        ctrl.isActive = data.IsActive;
                        toaster.pop(
                            'success',
                            '',
                            $translate.instant('Admin.Js.SettingsCrm.Group') +
                                data.Name +
                                $translate.instant('Admin.Js.SettingsCrm.GroupIsConnected'),
                        );
                        if (ctrl.onAddDelFacebook) {
                            ctrl.onAddDelFacebook();
                        }
                    } else if (data.errors && data.errors.length) {
                        data.errors.forEach(function (error) {
                            if (error) {
                                toaster.error('', error);
                            }
                        });
                    }
                });
        };
        ctrl.deleteGroup = function () {
            facebookAuthService.deleteGroup().then(function () {
                ctrl.getSettings();
                if (ctrl.onAddDelFacebook) {
                    ctrl.onAddDelFacebook();
                }
            });
        };
        ctrl.saveSettings = function () {
            facebookAuthService.saveSettings(ctrl.salesFunnelId, ctrl.createLeadFromMessages, ctrl.createLeadFromComments).then(function (data) {
                toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCrm.ChangesSaved'));
            });
        };
    };
    facebookAuthCtrl.$inject = ['$http', 'toaster', 'SweetAlert', 'facebookAuthService', '$translate'];
    ng.module('facebookAuth', [])
        .controller('facebookAuthCtrl', facebookAuthCtrl)
        .component('facebookAuth', {
            templateUrl: facebookAuthTemplate,
            controller: 'facebookAuthCtrl',
            bindings: {
                redirectUrl: '=',
                saasData: '<',
                onAddDelFacebook: '&',
            },
        });
})(window.angular);
