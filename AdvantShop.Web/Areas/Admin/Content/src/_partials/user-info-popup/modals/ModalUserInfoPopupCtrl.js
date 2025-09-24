(function (ng) {
    'use strict';
    var ModalUserInfoPopupCtrl = function ($uibModalInstance, $http, $window, toaster, $translate, isMobileService) {
        let ctrl = this,
            isMobile;
        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            isMobile = isMobileService.getValue;
            ctrl.meta = null;

            // проверка для того что заходим ли с мобилки или с десктопа
            if ($window.screen.width < 769 && !isMobile) {
                ctrl.meta = document.createElement(`meta`);
                ctrl.meta.setAttribute(`name`, `viewport`);
                ctrl.meta.setAttribute(`content`, `width=device-width,initial-scale=1`);
                document.head.appendChild(ctrl.meta);
            }

            ctrl.q2Options = [
                { text: $translate.instant('Admin.Js.UserInfoPopup.NotYetSold') },
                { text: $translate.instant('Admin.Js.UserInfoPopup.SoldInSocialNetworks') },
                { text: $translate.instant('Admin.Js.UserInfoPopup.DidAdvertisingInInternet') },
                { text: $translate.instant('Admin.Js.UserInfoPopup.ThereWasThereIs') },
            ];

            ctrl.q3Options = [
                { text: $translate.instant('Admin.Js.UserInfoPopup.Retail') },
                { text: $translate.instant('Admin.Js.UserInfoPopup.Wholesale') },
                { text: $translate.instant('Admin.Js.UserInfoPopup.WholesaleAndRetail') },
                { text: $translate.instant('Admin.Js.UserInfoPopup.NotYet') },
            ];

            ctrl.userData = params;
            //if (ctrl.userData.Map != null && ctrl.userData.Map.length >= 3) {

            //ctrl.Question1 = parseInt(ctrl.userData.Map.filter(function (x) { return x.Name === $translate.instant('Admin.Js.UserInfoPopup.NumberOfEmployeesInTheStore') })[0].Value);
            //ctrl.Question2 = ctrl.userData.Map.filter(function (x) { return x.Name === $translate.instant('Admin.Js.UserInfoPopup.YourSalesExperience') })[0].Value;
            //ctrl.Question3 = ctrl.userData.Map.filter(function (x) { return x.Name === $translate.instant('Admin.Js.UserInfoPopup.HaveOfflineSalePoint') })[0].Value;
            //}
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.saveUserInfo = function () {
            ctrl.userData.Map = [
                {
                    Name: $translate.instant('Admin.Js.UserInfoPopup.NumberOfEmployeesInTheStore'),
                    Value: ctrl.Question1,
                },
                {
                    Name: $translate.instant('Admin.Js.UserInfoPopup.YourSalesExperience'),
                    Value: ctrl.Question2,
                },
                //{
                //    Name: $translate.instant('Admin.Js.UserInfoPopup.HaveOfflineSalePoint'),
                //    Value: ctrl.Question3,
                //}
            ];

            $http.post('home/saveUserInformation', ctrl.userData).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.UserInfoPopup.ThanksForAnswers'));
                    $uibModalInstance.close({ username: data.fio });
                } else {
                    toaster.pop('error', 'Ошибка при сохранении данных');
                }
                if (ctrl.meta != null && !isMobile) {
                    ctrl.meta.setAttribute(`content`, `width=device-width,initial-scale=0.3`);
                }

                ctrl.btnLoading = false;
            });
        };
    };

    ModalUserInfoPopupCtrl.$inject = ['$uibModalInstance', '$http', '$window', 'toaster', '$translate', 'isMobileService'];

    ng.module('uiModal').controller('ModalUserInfoPopupCtrl', ModalUserInfoPopupCtrl);
})(window.angular);
