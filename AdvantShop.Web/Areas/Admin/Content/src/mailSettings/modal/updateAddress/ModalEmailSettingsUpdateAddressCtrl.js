(function (ng) {
    'use strict';

    var ModalEmailSettingsUpdateAddressCtrl = function ($uibModalInstance, $http, toaster, $translate, SweetAlert) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.value || {};
            ctrl.email = params.email;
            var errorMessage =
                $translate.instant('Admin.Js.MailSettings.PublicMailDomainsAreNotSupported') +
                '<a href="https://www.advantshop.net/help/pages/nastroiky-pochti-na-domene" target="_blank">' +
                $translate.instant('Admin.Js.MailSettings.Instruction') +
                '</a>';
            ctrl.emailsInvalid = [
                { value: '@mail.ru', error: errorMessage },
                { value: '@yandex.ru', error: errorMessage },
                { value: '@ya.ru', error: errorMessage },
                { value: '@inbox.ru', error: errorMessage },
                { value: '@list.ru', error: errorMessage },
                { value: '@bk.ru', error: errorMessage },
                { value: '@mail.ua', error: errorMessage },
                { value: '@gmail.com', error: errorMessage },
                { value: '@yandex.by', error: errorMessage },
                { value: '@yandex.kz', error: errorMessage },
                { value: '@yandex.ua', error: errorMessage },
                { value: '@yandex.com', error: errorMessage },
                { value: '@rambler.ru', error: errorMessage },
                { value: '@lenta.ru', error: errorMessage },
                { value: '@autorambler.ru', error: errorMessage },
                { value: '@myrambler.ru', error: errorMessage },
                { value: '@ro.ru', error: errorMessage },
                { value: '@rambler.ua', error: errorMessage },
                { value: '@ukr.net', error: errorMessage },
                { value: '@i.ua', error: errorMessage },
                { value: '@yahoo.com', error: errorMessage },
                { value: '@icloud.com', error: errorMessage },
                { value: '@tut.by', error: errorMessage },
                { value: '@outlook.com', error: errorMessage },
                { value: '@hotmail.com', error: errorMessage },
                { value: '@live.ru', error: errorMessage },
                { value: '@me.com', error: errorMessage },
                { value: '@bigmir.net', error: errorMessage },
                { value: '@qip.ru', error: errorMessage },
                { value: '@narod.ru', error: errorMessage },
                { value: '@live.com', error: errorMessage },
                { value: '@pochta.ru', error: errorMessage },
                { value: '@nm.ru', error: errorMessage },
                { value: '@protonmail.com', error: errorMessage },
            ];
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function (email) {
            var emailsError = ctrl.emailsInvalid.filter(function (item) {
                return email.indexOf(item.value) !== -1;
            });

            if (emailsError != null && emailsError.length > 0) {
                return SweetAlert.alert(
                    emailsError
                        .map(function (item) {
                            return item.error;
                        })
                        .join('<br>'),
                    { title: $translate.instant('Admin.Js.AdminWebNotifications.Attention') },
                );
            } else {
                $http
                    .post('/SettingsMail/UpdateAddress', { email: email })
                    .then(function (response) {
                        if (response.data.result) {
                            $uibModalInstance.close(response.data.obj);
                            toaster.pop('success', '', $translate.instant('Admin.Js.MainPageProducts.ChangesSaved'));
                        } else {
                            toaster.pop('error', $translate.instant('Admin.Js.MailSettings.Error'), response.data.errors.join('<br>'));
                        }
                    })
                    .catch(function (err) {
                        toaster.pop('error', $translate.instant('Admin.Js.MailSettings.Error'));
                    });
            }
        };
    };

    ModalEmailSettingsUpdateAddressCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate', 'SweetAlert'];

    ng.module('uiModal').controller('ModalEmailSettingsUpdateAddressCtrl', ModalEmailSettingsUpdateAddressCtrl);
})(window.angular);
