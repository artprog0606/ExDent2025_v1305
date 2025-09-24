import shippingPecSyncStatusTemplate from './templates/shippingPecSyncStatus.html';
(function (ng) {
    'use strict';

    var ShippingPecSyncStatusCtrl = function ($http, toaster) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.statuses = {};
            var statusesArr = ctrl.statusesSource.split('@@');
            for (var i = 0; i < statusesArr.length; i++) {
                let value = statusesArr[i];
                if (value) {
                    var values = value.split(';;');
                    ctrl.statuses[values[0]] = {
                        Name: values[1],
                        Comment: '',
                    };
                }
            }
            ctrl.listStatuses = [];
            for (let value in ctrl.statuses) {
                if (ctrl.statuses.hasOwnProperty(value)) {
                    ctrl.listStatuses.push({
                        value: value,
                        label: ctrl.statuses[value].Name,
                    });
                }
            }
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
                                    pecStatus: arr[0],
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
                    return item.pecStatus === ctrl.Status;
                })
            ) {
                toaster.error('Данный статус уже указан. Чтобы обновить необходимо удалить.');
                return;
            }
            ctrl.syncStatuses.push({
                pecStatus: ctrl.Status,
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
                    return x.pecStatus + ',' + x.advStatus;
                })
                .join(';');
            ctrl.update = true;
        };
        ctrl.getStatusNameByObj = function (obj) {
            return ctrl.getStatusName(obj.pecStatus);
        };
        ctrl.getStatusName = function (id) {
            return ctrl.statuses[id].Name;
        };
        ctrl.getStatusComment = function (id) {
            return ctrl.statuses[id].Comment;
        };
        ctrl.getAdvStatusName = function (id) {
            var status = ctrl.advStatuses.find(function (item) {
                return item.value === id;
            });
            return status ? status.label : undefined;
        };
        function compare(a, b) {
            if (a.pecStatus < b.pecStatus) return -1;
            if (a.pecStatus > b.pecStatus) return 1;
            return 0;
        }
    };
    ShippingPecSyncStatusCtrl.$inject = ['$http', 'toaster'];
    ng.module('shippingMethod')
        .controller('ShippingPecSyncStatusCtrl', ShippingPecSyncStatusCtrl)
        .component('shippingPecSyncStatus', {
            templateUrl: shippingPecSyncStatusTemplate,
            controller: 'ShippingPecSyncStatusCtrl',
            bindings: {
                onInit: '&',
                methodId: '@',
                statusesReference: '@',
                statusesSource: '@',
            },
        });
})(window.angular);
