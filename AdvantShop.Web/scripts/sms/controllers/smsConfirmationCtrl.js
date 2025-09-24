/* @ngInject */
function SmsConfirmationCtrl($timeout, smsConfirmationService, toaster, $translate, $sce, $parse, $scope, $attrs, $q, $interval, modalService) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.reInit(true);
        ctrl.wrongConfirmation = 0;
        smsConfirmationService.getCodeDescription().then(function (description) {
            ctrl.codeDescription = description;
        });
    };

    ctrl.reInit = function (firstInit) {
        ctrl.phone = ctrl.confirmationPhone;
        ctrl.codeConfirmed = false;
        ctrl.codeInputsEnabled = true;
        ctrl.sendCodeButtonEnabled = true;
        ctrl.isCodeSended = false;
        ctrl.code = '';
        ctrl.sendCodeButtonText = $translate.instant('Js.SmsConfirmation.SendCode');

        if (firstInit) {
            ctrl.checkPhoneConfirmed();
        }
    };

    ctrl.sendSmsCode = function () {
        ctrl.reInit();

        if (ctrl.phone == null || ctrl.phone === '' || ctrl.phone.length === 0) {
            toaster.pop('error', '', $translate.instant('Js.ConfirmSms.ErrorEmptyPhone'));
            return;
        }

        smsConfirmationService.sendSmsCode(ctrl.phone, true).then(function (data) {
            if (data.result === true) {
                toaster.pop('info', '', $translate.instant('Js.ConfirmSms.CodeSended') + ctrl.phone);

                ctrl.isCodeSended = true;

                ctrl.sendCodeButtonEnabled = false;
                var seconds = data.obj.secondsToRetry;
                ctrl.countdownSeconds = seconds;

                var timer = $interval(
                    function () {
                        ctrl.sendCodeButtonText = $translate.instant('Js.ConfirmSms.GetNewSmsCodeCountdown', {
                            sec: ctrl.countdownSeconds,
                        });
                        ctrl.countdownSeconds--;

                        if (ctrl.countdownSeconds <= 0) {
                            ctrl.sendCodeButtonEnabled = true;
                            ctrl.sendCodeButtonText = $translate.instant('Js.SmsConfirmation.SendCode');
                        }
                        if (ctrl.isPhoneChanged()) {
                            ctrl.sendCodeButtonEnabled = true;
                            ctrl.sendCodeButtonText = $translate.instant('Js.SmsConfirmation.SendCode');
                            ctrl.countdownSeconds = 0;
                            timer.cancel();
                        }
                    },
                    1000,
                    seconds,
                );
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
        ctrl.code = code;
        ctrl.confirmSmsCode();
    };

    ctrl.confirmSmsCode = async function () {
        if (ctrl.isPhoneChanged()) {
            ctrl.reInit();
            toaster.pop('info', '', $translate.instant('Js.ConfirmSms.ErrorPhoneChanged') + ctrl.phone);
            return;
        }

        if (!ctrl.isCodeSended) {
            toaster.pop('error', '', $translate.instant('Js.ConfirmSms.NoClickSendSmsCode'));
            return;
        }

        if (ctrl.code == null || ctrl.code.length < 4) {
            return;
        }

        if (ctrl.showCaptcha) {
            ctrl.showCaptchaModal();
            return;
        }

        smsConfirmationService.confirmSmsCode(ctrl.phone, ctrl.code, true).then(function (data) {
            if (data.result) {
                ctrl.codeConfirmed = true;
                ctrl.codeInputsEnabled = false;
                ctrl.sendCodeButtonEnabled = false;
            } else {
                toaster.pop('error', '', $translate.instant('Js.ConfirmSms.WrongCode'));
                ctrl.wrongConfirmation++;
                if (ctrl.wrongConfirmation > 5) {
                    //  || data.obj.showCaptcha
                    ctrl.showCaptchaModal();
                }
            }
            ctrl.code = '';
        });
    };

    ctrl.checkPhoneConfirmed = function () {
        if (ctrl.phone == null || ctrl.phone === '' || ctrl.phone.length === 0) return;

        smsConfirmationService.checkPhoneConfirmed(ctrl.phone).then(function (data) {
            var isConfirmed = data.obj.isConfirmed;
            if (isConfirmed) {
                ctrl.codeConfirmed = true;
                ctrl.codeInputsEnabled = false;
                ctrl.sendCodeButtonEnabled = false;
            }
        });
    };

    ctrl.isPhoneChanged = function () {
        let isPhoneChanged = ctrl.phone !== ctrl.confirmationPhone;

        if (isPhoneChanged) {
            ctrl.reInit();
        }

        return isPhoneChanged;
    };

    ctrl.isConfirmed = function () {
        return ctrl.codeConfirmed && !ctrl.isPhoneChanged();
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

export default SmsConfirmationCtrl;
