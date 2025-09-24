import shippingOzonRocketSyncStatusTemplate from './templates/shippingOzonRocketSyncStatus.html';
(function (ng) {
    'use strict';

    var ShippingOzonRocketSyncStatusCtrl = function ($http, toaster) {
        var ctrl = this;
        ctrl.statuses = {
            5: {
                Name: 'Отправление зарегистрировано',
                Comment: 'Манифест успешно загружен от принципала. Информация о заказе передана службе доставки.',
            },
            10: {
                Name: 'Передано в службу доставки',
                Comment: 'Отправление принято и оприходовано на складе.',
            },
            1010: {
                Name: 'Отправление аннулировано',
                Comment: 'Отправление отменено.',
            },
            20: {
                Name: 'Отправлено в ваш город',
                Comment: 'Отправка перевозки со склада.',
            },
            40: {
                Name: 'Прибыло в ваш город',
                Comment: 'Перевозка принята на складе субагента.',
            },
            45: {
                Name: 'Готово к выдаче',
                Comment: 'Отправление принято и оприходовано на складе субагента.',
            },
            50: {
                Name: 'Отправление выдано',
                Comment: 'Отправление выдано в пункте выдачи заказов.',
            },
            60: {
                Name: 'Отправление выдано частично',
                Comment: 'Отправление частично выдано в пункте выдачи заказов или при доставке курьером.',
            },
            65: {
                Name: 'Частичный возврат после выдачи',
                Comment: 'Клиентский возврат. Возврат части экземпляров после выдачи в пункте выдачи заказов.',
            },
            70: {
                Name: 'Отказ клиента',
                Comment: 'Отказ клиента от заказа или отказ клиента от заказа при доставке курьером.',
            },
            80: {
                Name: 'Отправление не востребовано',
                Comment:
                    'Автовозврат, если отправление невостребовано. Автоматическая пометка отправления на возврат при истечении срока хранения в пункте выдачи согласно условиям хранения по договору.',
            },
            90: {
                Name: 'Передано курьеру',
                Comment: 'Отправление передано курьеру.',
            },
            91: {
                Name: 'Выехал к клиенту',
                Comment: 'Отправление доставляется курьером.',
            },
            92: {
                Name: 'Передаётся клиенту',
                Comment: 'Отправление передаётся клиенту.',
            },
            93: {
                Name: 'Выполненo',
                Comment: 'Заказ выдан (при доставке курьером).',
            },
            100: {
                Name: 'Возврат отправлен на склад',
                Comment: 'Перевозка с возвратным заказом отправлена на склад.',
            },
            110: {
                Name: 'Возврат прибыл на склад',
                Comment: 'Перевозка с возвратным заказом прибыла на склад.',
            },
            115: {
                Name: 'Возврат готов к передаче отправителю',
                Comment: 'Возврат готов к передаче принципалу. Прибывший возврат обработан на складе и помещён в перевозку.',
            },
            120: {
                Name: 'Возврат передан отправителю',
                Comment: 'Возврат передан принципалу.',
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
                                    ozonRocketStatus: arr[0],
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
                    return item.ozonRocketStatus === ctrl.Status;
                })
            ) {
                toaster.error('Данный статус уже указан. Чтобы обновить необходимо удалить.');
                return;
            }
            ctrl.syncStatuses.push({
                ozonRocketStatus: ctrl.Status,
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
                    return x.ozonRocketStatus + ',' + x.advStatus;
                })
                .join(';');
            ctrl.update = true;
        };
        ctrl.getStatusNameByObj = function (obj) {
            return ctrl.getStatusName(obj.ozonRocketStatus);
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
            if (a.ozonRocketStatus < b.ozonRocketStatus) return -1;
            if (a.ozonRocketStatus > b.ozonRocketStatus) return 1;
            return 0;
        }
    };
    ShippingOzonRocketSyncStatusCtrl.$inject = ['$http', 'toaster'];
    ng.module('shippingMethod')
        .controller('ShippingOzonRocketSyncStatusCtrl', ShippingOzonRocketSyncStatusCtrl)
        .component('shippingOzonRocketSyncStatus', {
            templateUrl: shippingOzonRocketSyncStatusTemplate,
            controller: 'ShippingOzonRocketSyncStatusCtrl',
            bindings: {
                onInit: '&',
                methodId: '@',
                statusesReference: '@',
            },
        });
})(window.angular);
