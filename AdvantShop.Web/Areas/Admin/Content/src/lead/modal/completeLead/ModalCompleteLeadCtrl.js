(function (ng) {
    'use strict';

    var ModalCompleteLeadCtrl = function ($http, $uibModalInstance, $window, lastStatisticsService, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.leadId = ctrl.$resolve.id;
            if (!ctrl.leadId) {
                $uibModalInstance.dismiss();
                return;
            }
            ctrl.getFormData();
        };

        ctrl.getFormData = function () {
            $http.post('leads/getCompleteLeadForm', { leadId: ctrl.leadId }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    ctrl.successStatus = data.obj.successStatus;
                    ctrl.cancelStatus = data.obj.cancelStatus;
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.Lead.ImpossibleToCompleteLead'));
                    $uibModalInstance.dismiss();
                }
            });
        };

        ctrl.completeLead = function () {
            ctrl.btnSleep = true;
            $http.post('leads/createOrder', { leadId: ctrl.leadId }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    if (data.orderId != null && data.orderId != 0) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Lead.OrderSuccessfullyCreated'));
                        $window.location.assign('orders/edit/' + data.orderId);
                        $uibModalInstance.close('redirect');
                    } else {
                        $window.location.reload();
                        $uibModalInstance.close(false);
                    }
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.Lead.UnableToCreateOrder'));
                    $uibModalInstance.close(false);
                }
                ctrl.btnSleep = false;
            });
        };

        ctrl.cancelLead = function () {
            ctrl.btnCancelSleep = true;
            $http.post('leads/cancelLead', { leadId: ctrl.leadId }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Lead.ChangesSaved'));
                    $uibModalInstance.close(true);
                    lastStatisticsService.getLastStatistics();
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.Lead.CouldNotSaveChanges'));
                    $uibModalInstance.close(false);
                }
                ctrl.btnCancelSleep = false;
            });
        };
    };

    ModalCompleteLeadCtrl.$inject = ['$http', '$uibModalInstance', '$window', 'lastStatisticsService', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalCompleteLeadCtrl', ModalCompleteLeadCtrl);
})(window.angular);
