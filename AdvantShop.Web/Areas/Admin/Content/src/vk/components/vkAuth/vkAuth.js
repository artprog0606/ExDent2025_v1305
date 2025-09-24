import vkAuthTemplate from './vkAuth.html';
(function (ng) {
    const vkAuthCtrl = function ($http, toaster, SweetAlert, vkService, $translate, $window) {
        const ctrl = this;
        ctrl.$onInit = function () {
            ctrl.getSettings();
        };
        ctrl.getSettings = function () {
            vkService.getVkSettings().then((data) => {
                ctrl.clientId = data.clientId;
                ctrl.groups = data.groups;
                if (ctrl.groups != null && ctrl.groups.length > 0) {
                    ctrl.selectedGroup = ctrl.groups[0];
                }
                ctrl.group = data.group;
                ctrl.groupId = data.group != null ? data.group.Id : null;
                ctrl.groupName = data.group != null ? data.group.Name : null;
                ctrl.groupScreenName = data.group != null ? data.group.ScreenName : null;

                ctrl.state = data.state;
                ctrl.codeChallenge = data.codeChallenge;
            });
        };

        ctrl.initVkId = function () {
            const VKID = window.VKIDSDK;

            VKID.Config.init({
                app: ctrl.clientId,
                redirectUrl: ctrl.redirectUrl,
                responseMode: VKID.ConfigResponseMode.Callback,
                source: VKID.ConfigSource.LOWCODE,
                scope: 'wall groups market photos',
                state: ctrl.state,
                codeChallenge: ctrl.codeChallenge,
            });

            const oneTap = new VKID.OneTap();

            oneTap
                .render({
                    container: document.getElementById('vkIdContainer'),
                    showAlternativeLogin: true,
                })
                .on(VKID.WidgetEvents.ERROR, (error) => {
                    console.log(error);
                    toaster.pop('error', '', 'Ошибка при авторизации');
                })
                .on(VKID.OneTapInternalEvents.LOGIN_SUCCESS, (payload) => {
                    const exchangeCode = {
                        redirect_uri: ctrl.redirectUrl,
                        client_id: ctrl.clientId,
                        code: payload.code,
                        device_id: payload.device_id,
                        state: payload.state,
                    };

                    vkService.getUserAccessToken(exchangeCode).then((data) => {
                        if (data.errors != null) {
                            data.errors.forEach((error) => toaster.pop('error', '', error));
                        } else {
                            vkService.getGroups().then((groups) => {
                                ctrl.groups = groups;
                                if (ctrl.groups != null && ctrl.groups.length > 0) {
                                    ctrl.selectedGroup = ctrl.groups[0];
                                }
                                ctrl.showVkId = false;
                            });
                        }
                    });
                });
        };

        // Авторизация в vk с правами пользователя, чтобы получить список групп
        ctrl.authVk = function () {
            ctrl.showVkId = true;
            ctrl.hideAppIdBlock = true;
        };

        // Авторизация в vk с правами пользователя, чтобы получить список групп
        ctrl.authGroup = function () {
            if (ctrl.selectedGroup == null) {
                return;
            }
            const group = ctrl.selectedGroup;
            const w = 700;
            const h = 525;
            const left = screen.width / 2 - w / 2;
            const top = screen.height / 2 - h / 2;
            const url =
                `https://oauth.vk.com/authorize?` +
                `client_id=${ctrl.clientId}` +
                `&redirect_uri=${ctrl.redirectUrl}` +
                `&group_ids=${group.Id}` +
                `&display=page` +
                `&scope=messages,manage` +
                `&response_type=token` +
                `&v=5.199`;
            const win = window.open(url, '', `width=${w}, height=${h}, top=${top}, left=${left}`);

            win.focus();

            const timer = window.setInterval(() => {
                try {
                    if (win.document.URL.indexOf(ctrl.redirectUrl) !== -1) {
                        let accessToken = '';
                        const urlParts = win.document.URL.split('#')[1].split('&');
                        for (const item of urlParts) {
                            if (item.indexOf('access_token') !== -1) {
                                accessToken = item.split('=')[1];
                            }
                        }
                        win.close();
                        window.clearInterval(timer);
                        return vkService
                            .saveAuthVkGroup({
                                group,
                                accessToken,
                            })
                            .then((data) => {
                                if (data.result === true) {
                                    $window.location.reload(true);
                                    toaster.pop(
                                        'success',
                                        '',
                                        $translate.instant('Admin.Js.SettingsCrm.Group') +
                                            group.Name +
                                            $translate.instant('Admin.Js.SettingsCrm.GroupIsConnected'),
                                    );
                                    if (ctrl.onAddDelVk) {
                                        ctrl.onAddDelVk();
                                    }
                                } else {
                                    data.errors.forEach((error) => {
                                        if (error) {
                                            toaster.error('', error);
                                        }
                                    });
                                }
                            });
                    }
                } catch (e) {
                    console.log(e);
                }
            }, 100);
        };

        ctrl.saveSettings = function () {
            vkService.saveSettings(ctrl.salesFunnelId, ctrl.createLeadFromMessages, ctrl.createLeadFromComments).then((data) => {
                toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCrm.ChangesSaved'));
            });
        };
    };
    vkAuthCtrl.$inject = ['$http', 'toaster', 'SweetAlert', 'vkService', '$translate', '$window'];
    ng.module('vkAuth', [])
        .controller('vkAuthCtrl', vkAuthCtrl)
        .component('vkAuth', {
            templateUrl: vkAuthTemplate,
            controller: 'vkAuthCtrl',
            bindings: {
                redirectUrl: '<?',
                onAddDelVk: '&',
            },
        });
})(window.angular);
