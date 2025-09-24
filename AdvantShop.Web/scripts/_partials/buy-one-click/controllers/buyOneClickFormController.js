/* @ngInject */
function BuyOneClickFormCtrl($sce, $timeout, $window, buyOneClickService, toaster, $scope, $http) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.success = false;
        ctrl.process = false;
        ctrl.showRedirectButton = false;
        //ctrl.compactMode = ctrl.compactMode === 'True' ? true : false;

        buyOneClickService.getFieldsOptions().then(function (fields) {
            ctrl.fields = angular.extend(fields, ctrl.fieldsOptions);
            ctrl.fields.BuyInOneClickFirstText = $sce.trustAsHtml(ctrl.fields.BuyInOneClickFirstText);
            ctrl.fields.BuyInOneClickFinalText = $sce.trustAsHtml(ctrl.fields.BuyInOneClickFinalText);
            ctrl.fields.UserAgreementForPromotionalNewsletter = $sce.trustAsHtml(ctrl.fields.UserAgreementForPromotionalNewsletter);
            ctrl.fullNameListMaxHeight =
                50 +
                (50 *
                    (ctrl.fields.IsShowBuyInOneClickEmail +
                        ctrl.fields.IsShowBuyInOneClickPhone +
                        ctrl.fields.IsShowBuyInOneClickComment * 2 +
                        ctrl.fields.IsShowUserAgreementText) || 50);

            if (ctrl.fields.EnableCaptchaInBuyInOneClick) {
                ctrl.initCaptcha('buyOneClickForm.captchaCode').then(function (data) {
                    ctrl.captchaHtml = data;
                });
            }
        });

        buyOneClickService.getCustomerInfo().then(function (data) {
            ctrl.name = data.name;
            ctrl.email = data.email;
            ctrl.phone = data.phone;
        });

        if (ctrl.formInit != null) {
            ctrl.formInit({ form: ctrl });
        }
    };

    ctrl.reset = function () {
        ctrl.name = '';
        ctrl.email = '';
        ctrl.phone = '';
        ctrl.comment = '';

        ctrl.success = false;
        ctrl.showRedirectButton = false;
        ctrl.result = null;

        ctrl.form.$setPristine();
    };

    ctrl.send = function () {
        var isValid = ctrl.buyOneClickValid();

        if (isValid === true || isValid == null) {
            ctrl.process = true;

            var captchaExist = typeof CaptchaSourceBuyInOneClick != 'undefined' && CaptchaSourceBuyInOneClick != null;
            var captchaInstanceId = captchaExist ? CaptchaSourceBuyInOneClick.InstanceId : null;

            buyOneClickService
                .checkout(
                    ctrl.page,
                    ctrl.orderType,
                    ctrl.offerId,
                    ctrl.productId,
                    ctrl.amount,
                    ctrl.attributesXml,
                    ctrl.name,
                    ctrl.email,
                    ctrl.phone,
                    ctrl.comment,
                    ctrl.captchaCode,
                    captchaInstanceId,
                    ctrl.isAgreeForPromotionalNewsletter,
                )
                .then(function (result) {
                    if (result.error != null && result.error.length > 0) {
                        toaster.pop('error', null, result.error);

                        if (captchaExist) {
                            CaptchaSourceBuyInOneClick.ReloadImage();
                        }
                    } else {
                        ctrl.result = result;
                        ctrl.success = true;

                        ctrl.successFn({ result: result });

                        if (ctrl.autoReset != null) {
                            $timeout(ctrl.reset, ctrl.autoReset);
                        }
                    }

                    ctrl.process = false;
                });
        }
    };

    ctrl.initCaptcha = function (ngModel) {
        return $http.post('/commonExt/getCaptchaHtml', { ngModel: ngModel, captchaId: 'CaptchaSourceBuyInOneClick' }).then(function (response) {
            return $sce.trustAsHtml(response.data);
        });
    };
}

export default BuyOneClickFormCtrl;
