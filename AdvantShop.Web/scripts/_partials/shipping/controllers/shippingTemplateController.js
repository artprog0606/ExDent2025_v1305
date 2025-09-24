/* @ngInject */
function ShippingTemplateCtrl($scope, $timeout, $ocLazyLoad, urlHelper, shippingService) {
    var ctrl = this,
        timer;
    ctrl.$onInit = function () {
        shippingService.whenTemplateReady($scope, (event) => {
            $timeout(() => shippingService.saveTemplateState(ctrl.templateUrl));
        });
    };

    ctrl.fireTemplateReady = function () {
        shippingService.fireTemplateReady($scope);
    };

    ctrl.prepereLazyLoadUrl = function (params) {
        for (var i = 0, len = params.length; i < len; i++) {
            params[i] = urlHelper.getAbsUrl(params[i], true);
        }
        return params;
    };

    ctrl.changePrepare = function (event, field, shipping) {
        if (field == null) {
            return;
        }

        angular.extend(ctrl.shipping, shipping);

        if (event != null && event.type === 'keyup') {
            if (timer != null) {
                $timeout.cancel(timer);
            }

            timer = $timeout(function () {
                ctrl.changeControl({ shipping: ctrl.shipping });
            }, 500);
        } else {
            ctrl.changeControl({ shipping: ctrl.shipping });
        }
    };

    ctrl.changeSpinbox = function (value, proxy) {
        ctrl.changeControl({ shipping: ctrl.shipping });
    };
}

export default ShippingTemplateCtrl;
