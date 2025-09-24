import { PubSub } from '../../_common/PubSub/PubSub.js';

/* @ngInject */
function FeedbackCtrl($http, toaster) {
    var ctrl = this;

    ctrl.switchTheme = function (val) {
        ctrl.curTheme = val;
    };

    ctrl.isSelectedTheme = function (val) {
        return ctrl.curTheme === val;
    };

    ctrl.send = function () {
        var captchaExist = typeof CaptchaSource != 'undefined' && CaptchaSource != null;
        var captchaSource = captchaExist ? CaptchaSource.InstanceId : null;

        var params = {
            messageType: ctrl.curTheme,
            message: ctrl.message,
            orderNumber: ctrl.orderNumber,
            name: ctrl.name,
            email: ctrl.email,
            phone: ctrl.phone,
            agree: ctrl.agreement,
            captchaCode: ctrl.captchaCode,
            captchaSource: captchaSource,
        };

        $http.post('feedback/feedbackForm', params).then(function (response) {
            var result = response.data;

            if (result.error != null && result.error.length > 0) {
                toaster.pop('error', result.error);

                if (captchaExist) {
                    CaptchaSource.ReloadImage();
                }
            } else {
                ctrl.view = 'success';
                PubSub.publish('send_feedback');
            }
        });
    };

    ctrl.hideSecret = function () {
        ctrl.secret = null;
    };
}

export default FeedbackCtrl;
