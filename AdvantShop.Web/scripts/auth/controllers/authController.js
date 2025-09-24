/* @ngInject */
function AuthCtrl($window, toaster, authService, $sce) {
    var ctrl = this;

    ctrl.login = function (email, password, redirect, forceRedirect) {
        var captchaExist = typeof CaptchaSource != 'undefined' && CaptchaSource != null;
        var captchaInstanceId = captchaExist ? CaptchaSource.InstanceId : null;

        authService.login(email, password, ctrl.captchaCode, captchaInstanceId).then(function (result) {
            if (result.error != null && result.error.length > 0) {
                toaster.pop('error', result.error);

                if (result.requestCaptcha == true && ctrl.showCaptcha != result.requestCaptcha) {
                    ctrl.showCaptcha = result.requestCaptcha;
                    ctrl.initCaptcha();
                }

                if (captchaExist) {
                    CaptchaSource.ReloadImage();
                }
            } else {
                if (redirect != null && redirect.length > 0) {
                    if (!forceRedirect && result.redirectTo != null && redirect.indexOf('checkout') == -1 && redirect.indexOf('/pay/') == -1) {
                        redirect = result.redirectTo;
                    }

                    $window.location = redirect;
                } else {
                    $window.location.reload();
                }
            }
        });
    };

    ctrl.initCaptcha = function () {
        authService.getCaptchaHtml('auth.captchaCode').then(function (result) {
            ctrl.captchaHtml = $sce.trustAsHtml(result);
        });
    };
}

export default AuthCtrl;
