/* @ngInject */
function PreOrderFormCtrl($sce, $timeout, $window, preOrderService, toaster, $scope, $http) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.getFormData();
        if (ctrl.formInit != null) {
            ctrl.formInit({ form: ctrl });
        }
    };

    ctrl.getFormData = function () {
        return preOrderService.getFormData().then(function (responseData) {
            ctrl.data = responseData.data;
            ctrl.field = responseData.field;
            if (ctrl.field.EnableCaptchaInPreOrder) {
                ctrl.initCaptcha('preOrderForm.captchaCode').then(function (data) {
                    ctrl.captchaHtml = data;
                });
            }
            return ctrl.data;
        });
    };

    ctrl.reset = function () {
        ctrl.data.FirstName = '';
        ctrl.data.LastName = '';
        ctrl.data.Email = '';
        ctrl.data.Phone = '';
        ctrl.data.Comment = '';

        ctrl.form.$setPristine();
    };

    ctrl.send = function () {
        var isValid = ctrl.preOrderValid();

        if (isValid === true || isValid == null) {
            ctrl.process = true;

            var captchaExist = typeof CaptchaSourcePreOrder != 'undefined' && CaptchaSourcePreOrder != null;
            var captchaInstanceId = captchaExist ? CaptchaSourcePreOrder.InstanceId : null;
            ctrl.data.OfferId = ctrl.offerId;
            ctrl.data.ProductId = ctrl.productId;
            ctrl.data.Amount = ctrl.amount;
            ctrl.data.OptionsHash = ctrl.jsonHash;
            ctrl.data.IsLanding = ctrl.isLanding;
            ctrl.data.CaptchaCode = ctrl.captchaCode;
            ctrl.data.CaptchaSource = captchaInstanceId;
            preOrderService.send(ctrl.data).then(function (data) {
                if (data.result === true) {
                    ctrl.result = data.obj;

                    ctrl.successFn({ result: ctrl.result });

                    if (ctrl.autoReset != null) {
                        $timeout(ctrl.reset, ctrl.autoReset);
                    }
                } else if (data.errors && data.errors.length) {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });
                    ctrl.captchaCode = null;
                } else {
                    toaster.pop('error', 'Ошибка при отправке');
                    ctrl.captchaCode = null;
                }
                if (captchaExist) {
                    CaptchaSourcePreOrder.ReloadImage();
                }

                ctrl.process = false;
            });
        }
    };

    ctrl.initCaptcha = function (ngModel) {
        return $http.post('/commonExt/getCaptchaHtml', { ngModel: ngModel, captchaId: 'CaptchaSourcePreOrder' }).then(function (response) {
            return $sce.trustAsHtml(response.data);
        });
    };
}

export default PreOrderFormCtrl;
