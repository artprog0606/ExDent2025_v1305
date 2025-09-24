(function (ng) {
    'use strict';

    var ModalChangeLeadSalesFunnelCtrl = function ($http, $uibModalInstance, toaster, $translate, urlHelper) {
        var ctrl = this,
            salesFunnelId;

        ctrl.$onInit = function () {
            var resolve = ctrl.$resolve;
            ctrl.params = resolve.params;
            salesFunnelId = urlHelper.getUrlParam('salesFunnelId');

            ctrl.getSalesFunnels().then(ctrl.getDealStatuses);
        };

        ctrl.getSalesFunnels = function () {
            return $http.get('salesFunnels/getSalesFunnels').then(function (response) {
                ctrl.funnels = response.data;
                if (salesFunnelId == null || salesFunnelId == '-1') ctrl.salesFunnelId = ctrl.funnels[0].value;
                else ctrl.salesFunnelId = salesFunnelId;
            });
        };

        ctrl.getDealStatuses = function () {
            return $http.get('salesFunnels/getDealStatuses', { params: { salesFunnelId: ctrl.salesFunnelId } }).then(function (response) {
                ctrl.statuses = response.data;
                if (ctrl.statuses.length > 0) {
                    ctrl.dealStatusId = ctrl.statuses[0].value;
                    ctrl.canCreateOrderOnFinalSuccess();
                }
            });
        };

        ctrl.changeDealStatus = function () {
            ctrl.canCreateOrderOnFinalSuccess();
        };

        ctrl.canCreateOrderOnFinalSuccess = function () {
            return $http
                .get('salesFunnels/createOrderOnFinalSuccess', {
                    params: { salesFunnelId: ctrl.salesFunnelId, dealStatusId: ctrl.dealStatusId },
                })
                .then(function (response) {
                    ctrl.showCreateOrderOnFinalSuccess = response.data;
                });
        };

        ctrl.save = function () {
            if (ctrl.dealStatusId == null) return;

            ctrl.btnSaveDisabled = true;

            $http
                .post(
                    'leads/changeSalesFunnelAndDealStatus',
                    ng.extend(ctrl.params || {}, {
                        newSalesFunnelId: ctrl.salesFunnelId,
                        newDealStatusId: ctrl.dealStatusId,
                        createOrderOnFinalSuccess: ctrl.createOrderOnFinalSuccess,
                    }),
                )
                .then(function (response) {
                    if (response.data.result === true) {
                        toaster.pop('success', '', 'Изменения успешно сохранены');
                    }
                    $uibModalInstance.close();
                    ctrl.btnSaveDisabled = false;
                });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalChangeLeadSalesFunnelCtrl.$inject = ['$http', '$uibModalInstance', 'toaster', '$translate', 'urlHelper'];

    ng.module('uiModal').controller('ModalChangeLeadSalesFunnelCtrl', ModalChangeLeadSalesFunnelCtrl);
})(window.angular);
