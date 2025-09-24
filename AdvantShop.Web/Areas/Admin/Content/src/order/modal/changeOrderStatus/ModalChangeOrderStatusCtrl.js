(function (ng) {
    'use strict';

    var ModalChangeOrderStatusCtrl = function ($uibModalInstance, $window, toaster, $q, $http, lastStatisticsService, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.orderId = params.orderId;
            ctrl.statusId = params.statusId;
            ctrl.statusName = params.statusName;
            ctrl.showNotifyEmail = false;
            ctrl.showNotifySms = false;
            ctrl.orderCtrl = params.orderCtrl;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancelChangeOrderStatus');
        };

        ctrl.closeNotify = function () {
            $uibModalInstance.close(
                ctrl.orderBasis != null
                    ? {
                          basis: ctrl.orderBasis,
                          color: ctrl.color,
                      }
                    : null,
            );
        };

        ctrl.save = function () {
            ctrl.orderBasis = ctrl.basis;

            $http
                .post('orders/changeOrderStatus', {
                    orderId: ctrl.orderId,
                    statusId: ctrl.statusId,
                    basis: ctrl.basis,
                    rnd: Math.random(),
                })
                .then(function (response) {
                    if (response.data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.OrderStatusSaved'));

                        lastStatisticsService.getLastStatistics();

                        if (response.data.isNotifyUserEmail === true) {
                            ctrl.showNotifyEmail = true;
                        }
                        if (response.data.isNotifyUserSms === true) {
                            ctrl.showNotifySms = true;
                        }

                        ctrl.orderBasis = response.data.basis;
                        ctrl.color = response.data.color;

                        if (response.data.isNotifyUserEmail === false && response.data.isNotifyUserSms === false) {
                            $uibModalInstance.close({
                                basis: ctrl.orderBasis,
                                color: ctrl.color,
                            });
                        }

                        ctrl.showNotifyMsg = true;
                        ctrl.orderCtrl.isSave = true;
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Order.FailedToUpdateStatus'));
                        $uibModalInstance.close();
                    }
                });
        };

        ctrl.notifyStatusChanged = function (type) {
            var notClose = ctrl.showNotifyEmail && ctrl.showNotifySms;

            if (type === 'email') {
                ctrl.showNotifyEmail = false;
            }
            if (type === 'sms') {
                ctrl.showNotifySms = false;
            }

            $http
                .post('orders/notifyStatusChanged', {
                    orderId: ctrl.orderId,
                    type: type,
                })
                .then(function (response) {
                    if (response.data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.NotificationOfOrder'));

                        if (!notClose) {
                            $uibModalInstance.close(
                                ctrl.orderBasis != null
                                    ? {
                                          basis: ctrl.orderBasis,
                                          color: ctrl.color,
                                      }
                                    : null,
                            );
                        }
                    }
                });
        };
    };

    ModalChangeOrderStatusCtrl.$inject = ['$uibModalInstance', '$window', 'toaster', '$q', '$http', 'lastStatisticsService', '$translate'];

    ng.module('uiModal').controller('ModalChangeOrderStatusCtrl', ModalChangeOrderStatusCtrl);
})(window.angular);
