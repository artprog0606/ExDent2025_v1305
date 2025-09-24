(function (ng) {
    'use strict';

    var ModalEditRewardPercentCtrl = function ($uibModalInstance, toaster, $translate, $http) {
        var ctrl = this;

        ctrl.CategoryId = ctrl.$resolve.item.CategoryId;
        ctrl.RewardPercent = ctrl.$resolve.item.RewardPercent;

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.save = function () {
            $http
                .post('settingsPartners/updateCategoryRewardPercent', {
                    categoryId: ctrl.CategoryId,
                    rewardPercent: ctrl.RewardPercent,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                    } else {
                        toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                    }
                })
                .then(function (res) {
                    $uibModalInstance.close(ctrl.RewardPercent);
                });
        };
    };

    ModalEditRewardPercentCtrl.$inject = ['$uibModalInstance', 'toaster', '$translate', '$http'];

    ng.module('uiModal').controller('ModalEditRewardPercentCtrl', ModalEditRewardPercentCtrl);
})(window.angular);
