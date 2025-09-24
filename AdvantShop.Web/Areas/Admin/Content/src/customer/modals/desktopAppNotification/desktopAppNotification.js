(function (ng) {
    'use strict';

    var CustomerModalDesktopAppNotificationCtrl = function ($uibModalInstance, $http, toaster) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;

            ctrl.appName = params.appName;
            ctrl.link = params.link;
        };

        ctrl.linkClick = function () {
            if (ctrl.dontShowMessage === true) {
                $http
                    .post('customer/DisableDesktopAppNotification', {
                        appName: ctrl.appName,
                    })
                    .then(function (response) {
                        if (response.data.result == true) {
                            $uibModalInstance.close(!ctrl.dontShowMessage);
                        } else {
                            toaster.pop('error', '', 'Ошибка отключения уведомления');
                        }
                    });
            } else {
                $uibModalInstance.close(!ctrl.dontShowMessage);
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    CustomerModalDesktopAppNotificationCtrl.$inject = ['$uibModalInstance', '$http', 'toaster'];

    ng.module('uiModal').controller('CustomerModalDesktopAppNotificationCtrl', CustomerModalDesktopAppNotificationCtrl);
})(window.angular);
