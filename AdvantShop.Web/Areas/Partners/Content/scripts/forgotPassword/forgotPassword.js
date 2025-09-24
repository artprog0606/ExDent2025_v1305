(function (ng) {
    'use strict';

    var ForgotPasswordCtrl = function ($http, $window, toaster, $sce) {
        var ctrl = this;

        ctrl.submitForgotPassword = function () {
            var captchaExist = typeof CaptchaSource != 'undefined' && CaptchaSource != null;
            var captchaInstanceId = captchaExist ? CaptchaSource.InstanceId : null;

            $http
                .post('account/forgotPasswordJson', {
                    email: ctrl.email,
                    captchaCode: ctrl.captchaCode,
                    captchaSource: captchaInstanceId,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result == true) {
                        ctrl.emailSent = true;
                    } else {
                        toaster.error(data.error);
                        if (data.obj.ShowCaptcha && !ctrl.showCaptcha) {
                            ctrl.showCaptcha = true;
                            ctrl.initCaptcha('forgotPassword.captchaCode').then(function (data) {
                                ctrl.captchaHtml = data;
                            });
                        }
                        if (captchaExist) {
                            CaptchaSource.ReloadImage();
                        }
                    }
                });
        };

        ctrl.submitRecover = function () {
            var params = {
                newPassword: ctrl.newPassword,
                newPasswordConfirm: ctrl.newPasswordConfirm,
                email: ctrl.email,
                hash: ctrl.hash,
            };

            $http.post('account/changePasswordJson', params).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    ctrl.passwordChanged = true;
                } else {
                    toaster.error(data.error);
                }
            });
        };

        ctrl.initCaptcha = function (ngModel) {
            return $http.post('../commonExt/getCaptchaHtml', { ngModel: ngModel }).then(function (response) {
                return $sce.trustAsHtml(response.data);
            });
        };
    };

    ForgotPasswordCtrl.$inject = ['$http', '$window', 'toaster', '$sce'];

    ng.module('forgotPassword', []).controller('ForgotPasswordCtrl', ForgotPasswordCtrl);
})(window.angular);
