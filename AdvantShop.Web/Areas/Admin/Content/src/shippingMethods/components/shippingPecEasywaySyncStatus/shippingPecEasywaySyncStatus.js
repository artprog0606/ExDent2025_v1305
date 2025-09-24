import shippingPecEasywaySyncStatusTemplate from './templates/shippingPecEasywaySyncStatus.html';
(function (ng) {
    'use strict';

    var ShippingPecEasywaySyncStatusCtrl = function ($http, toaster) {
        var ctrl = this;
        ctrl.statuses = {
            '9f02eabc-6aa3-11e6-80e9-003048baa05f': {
                Name: 'Новый',
                Comment: '',
            },
            '9ed94bff-5d7b-11e7-80cf-00155d233d13': {
                Name: 'Возврат с перенаправкой',
                Comment: '',
            },
            '41552567-6b97-11e6-80e9-003048baa05f': {
                Name: 'Ожидание',
                Comment: '',
            },
            '57230d22-df1e-11e7-80d6-00155d8c101b': {
                Name: 'Забор выполнен',
                Comment: '',
            },
            '65b5ceda-6b97-11e6-80e9-003048baa05f': {
                Name: 'На складе',
                Comment: '',
            },
            '7c4b94cd-6f5c-11e6-80ea-003048baa05f': {
                Name: 'На складе (сортировка)',
                Comment: '',
            },
            'b122101b-6f61-11e6-80ea-003048baa05f': {
                Name: 'В пути',
                Comment: '',
            },
            'bfcc82dc-6f8e-11e6-80ea-003048baa05f': {
                Name: 'На складе ФС',
                Comment: '',
            },
            '8628571a-6b97-11e6-80e9-003048baa05f': {
                Name: 'На доставке',
                Comment: '',
            },
            'f510368a-8973-11e6-80c7-000d3a2542c4': {
                Name: 'Возврат в пути',
                Comment: '',
            },
            '9aaa55ed-6b97-11e6-80e9-003048baa05f': {
                Name: 'Возврат на складе',
                Comment: '',
            },
            '1094ba91-8ca3-11e6-80c7-000d3a2542c4': {
                Name: 'Возврат выдан',
                Comment: '',
            },
            'b3e0596a-6b97-11e6-80e9-003048baa05f': {
                Name: 'Выдан',
                Comment: '',
            },
            '675f4358-6f61-11e6-80ea-003048baa05f': {
                Name: 'На терминале (ПВЗ)',
                Comment: '',
            },
            '00a72c8b-7e4a-11e6-80c7-000d3a2542c4': {
                Name: 'Отменен',
                Comment: '',
            },
            'dccadb9c-75d1-11e7-80d0-00155d233d13': {
                Name: 'Перенос',
                Comment: '',
            },
            '3dfb3584-2f31-11e7-80cd-00155d233d13': {
                Name: 'Порча',
                Comment: '',
            },
            '52f7e108-5009-11e7-80cf-00155d233d13': {
                Name: 'Проблема',
                Comment: '',
            },
            '8a57e606-3585-11e7-80ce-00155d233d13': {
                Name: 'Утиль',
                Comment: '',
            },
            'd9dd07f6-2f27-11e7-80cd-00155d233d13': {
                Name: 'Утрата',
                Comment: '',
            },
        };
        ctrl.$onInit = function () {
            ctrl.listStatuses = [];
            for (var value in ctrl.statuses) {
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
    ShippingPecEasywaySyncStatusCtrl.$inject = ['$http', 'toaster'];
    ng.module('shippingMethod')
        .controller('ShippingPecEasywaySyncStatusCtrl', ShippingPecEasywaySyncStatusCtrl)
        .component('shippingPecEasywaySyncStatus', {
            templateUrl: shippingPecEasywaySyncStatusTemplate,
            controller: 'ShippingPecEasywaySyncStatusCtrl',
            bindings: {
                onInit: '&',
                methodId: '@',
                statusesReference: '@',
            },
        });
})(window.angular);
