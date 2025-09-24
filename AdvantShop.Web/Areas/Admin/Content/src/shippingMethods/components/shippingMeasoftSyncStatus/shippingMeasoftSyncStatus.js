import shippingMeasoftSyncStatusTemplate from './templates/shippingMeasoftSyncStatus.html';
(function (ng) {
    'use strict';

    var ShippingMeasoftSyncStatusCtrl = function ($http, toaster) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.listStatuses = [];
            $http.get('orders/GetMeasoftOrderStatusList').then(function (response) {
                if (response.data.result) {
                    ctrl.listStatuses = response.data.obj;
                } else {
                    if (response.data.errors) {
                        response.data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });
                    } else {
                        toaster.pop('error', 'Не удалось получить список статусов Measoft');
                    }
                }
            });
            $http
                .get('orders/getorderstatuses')
                .then(function (response) {
                    ctrl.advStatuses = response.data;
                })
                .then(function () {
                    ctrl.syncStatuses = [];
                    if (ctrl.statusesReference != null && ctrl.statusesReference !== '') {
                        ctrl.syncStatuses = ctrl.statusesReference
                            .split(';')
                            .filter(function (x) {
                                return x;
                            })
                            .map(function (x) {
                                var arr = x.split(',');
                                return {
                                    measoftStatus: arr[0],
                                    advStatus: arr[1],
                                };
                            })
                            // фильтруем существующие статусы
                            .filter(function (x) {
                                return ctrl.getStatusNameByObj(x) && ctrl.getAdvStatusName(x.advStatus);
                            });
                        ctrl.syncStatuses.sort(compare);
                        ctrl.updateStatusesReference();
                    }
                });
        };
        ctrl.addSyncStatus = function () {
            if (
                ctrl.syncStatuses.some(function (item) {
                    return item.measoftStatus === ctrl.Status;
                })
            ) {
                toaster.error('Данный статус уже указан. Чтобы обновить необходимо удалить.');
                return;
            }
            ctrl.syncStatuses.push({
                measoftStatus: ctrl.Status,
                advStatus: ctrl.advStatus,
            });
            ctrl.syncStatuses.sort(compare);
            ctrl.updateStatusesReference();
        };
        ctrl.deleteSyncStatus = function (index) {
            ctrl.syncStatuses.splice(index, 1);
            ctrl.updateStatusesReference();
        };
        ctrl.updateStatusesReference = function () {
            ctrl.statusesReference = ctrl.syncStatuses
                .map(function (x) {
                    return x.measoftStatus + ',' + x.advStatus;
                })
                .join(';');
            ctrl.update = true;
        };
        ctrl.getStatusNameByObj = function (obj) {
            return ctrl.getStatusName(obj.measoftStatus);
        };
        ctrl.getStatusName = function (id) {
            return ctrl.listStatuses[id].Name;
        };
        ctrl.getStatusComment = function (id) {
            return ctrl.listStatuses[id].Comment;
        };
        ctrl.getAdvStatusName = function (id) {
            var status = ctrl.advStatuses.find(function (item) {
                return item.value === id;
            });
            return status ? status.label : undefined;
        };
        function compare(a, b) {
            if (a.measoftStatus < b.measoftStatus) return -1;
            if (a.measoftStatus > b.measoftStatus) return 1;
            return 0;
        }
    };
    ShippingMeasoftSyncStatusCtrl.$inject = ['$http', 'toaster'];
    ng.module('shippingMethod')
        .controller('ShippingMeasoftSyncStatusCtrl', ShippingMeasoftSyncStatusCtrl)
        .component('shippingMeasoftSyncStatus', {
            templateUrl: shippingMeasoftSyncStatusTemplate,
            controller: 'ShippingMeasoftSyncStatusCtrl',
            bindings: {
                onInit: '&',
                methodId: '@',
                statusesReference: '@',
            },
        });
})(window.angular);
