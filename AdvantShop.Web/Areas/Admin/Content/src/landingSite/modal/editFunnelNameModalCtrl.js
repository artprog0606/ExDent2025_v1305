(function (ng) {
    'use strict';

    var EditFunnelNameModalCtrl = function ($uibModalInstance, landingsService, toaster, $translate, $filter) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.title = $filter('htmlDecode')(ctrl.$resolve.title);
            ctrl.id = ctrl.$resolve.id;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function () {
            landingsService
                .updateTitle(ctrl.id, ctrl.title)
                .then(function (data) {
                    if (data.result == true) {
                        toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                    } else {
                        toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                    }
                })
                .then(function (res) {
                    $uibModalInstance.close(ctrl.title);
                });
        };
    };

    EditFunnelNameModalCtrl.$inject = ['$uibModalInstance', 'landingsService', 'toaster', '$translate', '$filter'];

    ng.module('uiModal').controller('EditFunnelNameModalCtrl', EditFunnelNameModalCtrl);
})(window.angular);
