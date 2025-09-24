/* @ngInject */
function SmsAuthCtrl($window, $timeout, $translate, toaster, $q, smsConfirmationService, $interval, modalService, $sce) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.returnToFirstStep();
    };

    ctrl.returnToFirstStep = function () {
        ctrl.currentForm = 'enterPhoneForm';
        ctrl.enableRetrySend = true;
        ctrl.phone = '';
        ctrl.smsCode = '';
        ctrl.wrongConfirmation = 0;
    };

    ctrl.sendSmsCode = function () {
        if (ctrl.phone === undefined || ctrl.phone === null || ctrl.phone === '' || ctrl.phone.length === 0) {
            toaster.pop('error', '', $translate.instant('Js.ConfirmSms.ErrorEmptyPhone'));
            return;
        }

        smsConfirmationService.sendSmsCode(ctrl.phone).then(function (data) {
            if (data.result === true) {
                toaster.pop('info', '', $translate.instant('Js.ConfirmSms.CodeSended') + ctrl.phone);
                ctrl.currentForm = 'confirmSmsForm';
                ctrl.setCountdown(data.obj.secondsToRetry);
            } else {
                if (data.errors != null && data.errors.length > 0) {
                    toaster.pop('error', null, data.errors[0]);
                } else {
                    toaster.pop('error', null, $translate.instant('Js.ConfirmSms.ErrorSendSms'));
                }

                // if (data.obj.showCaptcha) {
                //     ctrl.showCaptchaModal();
                // }
            }
        });
    };

    ctrl.confirmSmsCodeComplete = function (code) {
        ctrl.smsCode = code;
        ctrl.confirmSmsCode();
    };

    ctrl.confirmSmsCode = async function () {
        if (ctrl.smsCode == null || ctrl.smsCode.length < 4) {
            return;
        }

        if (ctrl.showCaptcha) {
            ctrl.showCaptchaModal();
            return;
        }

        smsConfirmationService.confirmSmsCode(ctrl.phone, ctrl.smsCode, false).then(function (data) {
            if (data.result) {
                var redirect;

                if (!ctrl.redirect || ctrl.redirect.indexOf('checkout') == -1) {
                    redirect = data.obj.redirectTo;
                } else {
                    redirect = ctrl.redirect;
                }

                if (redirect != null) {
                    $window.location = redirect;
                } else {
                    $window.location.reload();
                }
            } else {
                if (data.result.error != null && data.result.error.length > 0) {
                    toaster.pop('error', data.result.error);
                } else {
                    toaster.pop('error', '', $translate.instant('Js.ConfirmSms.WrongCode'));
                }
                ctrl.wrongConfirmation++;
                if (ctrl.wrongConfirmation > 5) {
                    //  || data.obj.showCaptcha
                    ctrl.showCaptchaModal();
                }
            }
        });
    };

    ctrl.setCountdown = function (seconds) {
        ctrl.enableRetrySend = false;
        ctrl.countdownSeconds = seconds;

        var counter = $interval(
            function () {
                ctrl.countdownSeconds--;

                var timerBlock = document.getElementById('smsConfirmationCountdownTimer');
                if (timerBlock !== null) {
                    timerBlock.innerHTML = $translate.instant('Js.ConfirmSms.RetryPhoneCountdownText', {
                        sec: ctrl.countdownSeconds,
                    });
                }

                if (ctrl.countdownSeconds <= 0) {
                    if (timerBlock !== null) {
                        timerBlock.innerHTML = '';
                    }

                    ctrl.enableRetrySend = true;
                }
            },
            1000,
            seconds,
        );
    };

    ctrl.initSmsConfirmationInput = function (smsConfirmationInput) {
        ctrl.smsConfirmationInput = smsConfirmationInput;
    };

    ctrl.checkCaptcha = function () {
        var captchaCode = SmsCaptchaSource.GetInputElement().value;
        if (captchaCode == null || captchaCode.length == 0) {
            return $q.resolve({ result: false });
        }
        return smsConfirmationService.checkCaptcha(SmsCaptchaSource.ValidationUrl, captchaCode).then((result) => {
            if (result.data === true) {
                ctrl.showCaptcha = false;
                ctrl.code = '';
                if (ctrl.smsConfirmationInput != null) {
                    ctrl.smsConfirmationInput.clear();
                }
                modalService.close('modalSmsConfirmationCaptcha');

                return $q.resolve({ result: true });
            } else {
                ctrl.reloadCaptcha();
                toaster.pop('error', '', $translate.instant('Js.Captcha.Wrong'));
                return $q.resolve({ result: false });
            }
        });
    };

    ctrl.reloadCaptcha = function () {
        if (ctrl.showCaptcha && typeof SmsCaptchaSource != 'undefined') {
            $timeout(function () {
                SmsCaptchaSource.ReloadImage();
            }, 1000);
            SmsCaptchaSource.GetInputElement().value = '';
        }
    };

    ctrl.initCaptcha = function () {
        smsConfirmationService.getCaptchaHtml('smsConfirmation.captchaCode', 'SmsCaptchaSource', 'smsCaptchaCode').then(function (result) {
            ctrl.captchaHtml = $sce.trustAsHtml(result);
        });
    };

    ctrl.showCaptchaModal = function () {
        ctrl.initCaptcha();
        modalService.open('modalSmsConfirmationCaptcha');
        ctrl.showCaptcha = true;
    };
}

export default SmsAuthCtrl;
