(function (ng) {
    'use strict';

    var ModalReferralLinkCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.getAdvReferralData();
        };

        ctrl.getAdvReferralData = function () {
            return $http.get('home/getAdvReferralInfo', { params: { rnd: Math.random() } }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    ctrl.data = data.obj;
                } else {
                    toaster.error('', $translate.instant('Admin.Js.ReferralLink.LoadingError'));
                }
            });
        };

        ctrl.copy = function (data) {
            var input = document.createElement('input');
            input.setAttribute('value', data);
            input.style.opacity = 0;
            document.body.appendChild(input);
            input.select();
            if (document.execCommand('copy')) {
                toaster.success($translate.instant('Admin.Js.ReferralLink.ModalReferralLinkCtrl.LinkCopiedToClipboard'));
            } else {
                toaster.error($translate.instant('Admin.Js.ReferralLink.ModalReferralLinkCtrl.FailedToCopyLink'));
            }
            document.body.removeChild(input);
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalReferralLinkCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalReferralLinkCtrl', ModalReferralLinkCtrl);
})(window.angular);
