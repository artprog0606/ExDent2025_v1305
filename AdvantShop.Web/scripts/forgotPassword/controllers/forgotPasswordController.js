/* @ngInject */
function ForgotPasswordCtrl($http, toaster, $sce) {
    var ctrl = this;

    ctrl.submitForgotPassword = function () {
        var captchaExist = typeof CaptchaSource != 'undefined' && CaptchaSource != null;
        var captchaInstanceId = captchaExist ? CaptchaSource.InstanceId : null;

        $http
            .post('/user/forgotPasswordJson', {
                email: ctrl.email,
                lpId: ctrl.lpId,
                captchaCode: ctrl.captchaCode,
                captchaSource: captchaInstanceId,
            })
            .then(function (response) {
                var result = response.data;

                if (result.error != null && result.error.length > 0) {
                    toaster.pop('error', result.error);

                    if (result.requestCaptcha == true && ctrl.showCaptcha != result.requestCaptcha) {
                        ctrl.showCaptcha = result.requestCaptcha;
                        ctrl.initCaptcha('forgotPassword.captchaCode').then(function (data) {
                            ctrl.captchaHtml = data;
                        });
                    }

                    if (captchaExist) {
                        CaptchaSource.ReloadImage();
                    }
                } else {
                    ctrl.view = 'forgotpassSuccess';
                }
            });
    };

    ctrl.submitRecover = function () {
        var params = {
            newPassword: ctrl.newPassword,
            newPasswordConfirm: ctrl.newPasswordConfirm,
            email: ctrl.email,
            recoveryCode: ctrl.recoveryCode,
            lpId: ctrl.lpId,
        };

        $http.post('/user/changePasswordJson', params).then(function (response) {
            var result = response.data;

            if (result.error != null && result.error.length > 0) {
                toaster.pop('error', result.error);
            } else {
                ctrl.view = 'recoverySuccess';

                if (ctrl.lpId != null) {
                    window.location.assign('/lp/user/redirect/' + ctrl.lpId);
                }
            }
        });
    };

    ctrl.initCaptcha = function (ngModel) {
        return $http.post('/commonExt/getCaptchaHtml', { ngModel: ngModel }).then(function (response) {
            return $sce.trustAsHtml(response.data);
        });
    };
}

export default ForgotPasswordCtrl;
