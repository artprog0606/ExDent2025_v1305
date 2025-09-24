import okAuthTemplate from './okAuth.html';
(function (ng) {
    'use strict';

    var okAuthCtrl = function (toaster, $translate, okService, $window) {
        var ctrl = this;
        ctrl.validatePrimarySettings = function () {
            okService
                .validatePrimarySettings({
                    applicationPublicKey: ctrl.applicationPublicKey,
                    applicationAccessToken: ctrl.applicationAccessToken,
                    applicationSessionSecretKey: ctrl.applicationSessionSecretKey,
                    groupSocialAccessToken: ctrl.groupSocialAccessToken,
                })
                .then(function (response) {
                    if (response.result) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCrm.AuthSuccessfulSetupComplete'));
                        $window.location.reload(true);
                    } else {
                        response.errors.forEach(function (e) {
                            toaster.pop('error', $translate.instant('Admin.Js.SettingsCrm.FailedLogIn'), e);
                        });
                    }
                });
        };
    };
    okAuthCtrl.$inject = ['toaster', '$translate', 'okService', '$window'];
    ng.module('okAuth', []).controller('okAuthCtrl', okAuthCtrl).component('okAuth', {
        templateUrl: okAuthTemplate,
        controller: 'okAuthCtrl',
    });
})(window.angular);
