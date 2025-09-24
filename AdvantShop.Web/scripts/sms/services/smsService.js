/* @ngInject */
function authSmsService($http) {
    var service = this;
    service.isPhoneChanged = false;

    service.sendSmsCode = function (phone, signUp) {
        return $http.post('user/sendCode', { phone: phone, signUp: signUp, rnd: Math.random() }).then(function (response) {
            return response.data;
        });
    };

    service.confirmSmsCode = function (phone, code, signUp) {
        return $http.post('user/confirmCode', { phone: phone, code: code, signUp: signUp, rnd: Math.random() }).then(function (response) {
            return response.data;
        });
    };

    service.checkPhoneConfirmed = function (phone) {
        return $http.post('user/isPhoneConfirmed', { phone: phone, rnd: Math.random() }).then(function (response) {
            return response.data;
        });
    };

    service.checkCaptcha = function (url, captchaCode) {
        return $http.get(url + '&i=' + captchaCode).then(function (result) {
            return result;
        });
    };

    service.getCaptchaHtml = function (ngModel, captchaId, captchaCode) {
        return $http.post('/commonExt/getCaptchaHtml', { ngModel, captchaId, captchaCode }).then(function (response) {
            return response.data;
        });
    };

    service.getCodeDescription = function () {
        return $http.get('user/getCodeDescription').then(function (response) {
            return response.data;
        });
    };
}

export default authSmsService;
