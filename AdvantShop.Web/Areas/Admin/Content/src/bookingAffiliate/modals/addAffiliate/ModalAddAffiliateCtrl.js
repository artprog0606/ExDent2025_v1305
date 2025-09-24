(function (ng) {
    'use strict';

    var ModalAddAffiliateCtrl = function ($uibModalInstance, $http, $window, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.goToAffiliatePage = params.goToAffiliatePage;

            ctrl.name = '';
            ctrl.sortOrder = 0;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function () {
            var params = {
                name: ctrl.name,
                sortOrder: ctrl.sortOrder,
                bookingIntervalMinutes: 60,
            };

            $http.post('bookingAffiliate/addAffiliate', params).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.BookingAffiliate.AffiliateAdded'));
                    $uibModalInstance.close(data.obj);
                    if (ctrl.goToAffiliatePage === true) {
                        $window.location.assign('bookingaffiliate/settings?id=' + data.obj);
                    }
                } else {
                    if (data.errors) {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });
                    } else {
                        toaster.pop(
                            'error',
                            $translate.instant('Admin.Js.BookingAffiliate.Error'),
                            $translate.instant('Admin.Js.BookingAffiliate.ErrorWhileCreating'),
                        );
                    }
                }
            });
        };
    };

    ModalAddAffiliateCtrl.$inject = ['$uibModalInstance', '$http', '$window', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalAddAffiliateCtrl', ModalAddAffiliateCtrl);
})(window.angular);
