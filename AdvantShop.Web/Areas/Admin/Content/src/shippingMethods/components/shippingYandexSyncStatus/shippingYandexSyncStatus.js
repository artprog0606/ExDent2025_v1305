import shippingYandexSyncStatusTemplate from './templates/shippingYandexSyncStatus.html';
(function (ng) {
    'use strict';

    var ShippingYandexSyncStatus = function ($http, toaster) {
        var ctrl = this;
        ctrl.statuses = {
            DRAFT: {
                Name: 'Черновик',
                Comment: 'Заказ доступен для редактирования.',
            },
            VALIDATING: {
                Name: 'На проверке',
                Comment: 'Заказ находится на проверке.',
            },
            VALIDATING_ERROR: {
                Name: 'Не прошел проверку',
                Comment: 'Заказ не прошел проверку.',
            },
            CREATED: {
                Name: 'Проверен и отправлен в службу доставки',
                Comment: 'Заказ проверен и отправлен в службу доставки.',
            },
            DELIVERY_PROCESSING_STARTED: {
                Name: 'Создаётся в сортировочном центре',
                Comment: 'Заказ создаётся в сортировочном центре.',
            },
            DELIVERY_TRACK_RECEIVED: {
                Name: 'В службе доставки',
                Comment: 'Заказ создан в системе службы доставки.',
            },
            SENDER_WAIT_FULFILLMENT: {
                Name: 'Ожидается поступление на склад',
                Comment: 'Ожидается поступление на склад.',
            },
            SORTING_CENTER_LOADED: {
                Name: 'Заказ создан',
                Comment: 'Заказ был успешно создан в системе Яндекс.Доставки',
            },
            DELIVERY_LOADED: {
                Name: 'Подтвержден',
                Comment: 'Интервал доставки и её возможность были успешно подтверждены.',
            },
            DELIVERY_AT_START: {
                Name: 'В сортировочном центрe',
                Comment: 'Заказ поступил в сортировочный центр.',
            },
            DELIVERY_TRANSPORTATION: {
                Name: 'Доставляется',
                Comment: 'Заказ доставляется.',
            },
            DELIVERY_ARRIVED: {
                Name: 'В населенном пункте получателя',
                Comment: 'Заказ находится в населенном пункте получателя.',
            },
            DELIVERY_TRANSPORTATION_RECIPIENT: {
                Name: 'Доставляется по населенному пункту получателя',
                Comment: 'Заказ в пути к получателю.',
            },
            DELIVERY_STORAGE_PERIOD_EXTENDED: {
                Name: 'Срок хранения увеличен',
                Comment: 'Срок хранения заказа в службе доставки увеличен.',
            },
            DELIVERY_STORAGE_PERIOD_EXPIRED: {
                Name: 'Срок хранения истек',
                Comment: 'Срок хранения заказа в службе доставки истек.',
            },
            DELIVERY_UPDATED: {
                Name: 'Доставка перенесена по вине магазина',
                Comment: '',
            },
            DELIVERY_UPDATED_BY_RECIPIENT: {
                Name: 'Доставка перенесена по просьбе клиента',
                Comment: '',
            },
            DELIVERY_UPDATED_BY_DELIVERY: {
                Name: 'Доставка перенесена службой доставки',
                Comment: '',
            },
            DELIVERY_ARRIVED_PICKUP_POINT: {
                Name: 'В пункте самовывоза',
                Comment: 'Этот статус означает, что заказ прибыл на ПВЗ и ожидает вручения.',
            },
            DELIVERY_AT_START_SORT: {
                Name: 'Заказ прибыл в регион доставки',
                Comment: 'Заказ прибыл в регион доставки и ожидает выдачи курьеру.',
            },
            DELIVERY_TRANSMITTED_TO_RECIPIENT: {
                Name: 'Вручен клиенту',
                Comment: 'Заказ передан получателю',
            },
            DELIVERY_DELIVERED: {
                Name: 'Доставлен получателю',
                Comment: 'Курьер вернулся на сортировочный центр и подтвердил вручение',
            },
            DELIVERY_ATTEMPT_FAILED: {
                Name: 'Неудачная попытка вручения заказа',
                Comment: 'Не получилось передать посылку получателю. Попытаемся ещё дважды.',
            },
            DELIVERY_CAN_NOT_BE_COMPLETED: {
                Name: 'Не может быть доставлен',
                Comment: 'Заказ не может быть доставлен.',
            },
            RETURN_PREPARING: {
                Name: 'Готовится к возврату',
                Comment: 'Заказ готовится к возврату.',
            },
            SORTING_CENTER_AT_START: {
                Name: 'Поступил на склад сортировочного центра',
                Comment: 'Посылка в сортировочном центре.',
            },
            SORTING_CENTER_TRANSMITTED: {
                Name: 'Заказ отгружен сортировочным центром в службу доставки',
                Comment: 'Заказ отправился в другой регион или готов к отгрузке в том же.',
            },
            SORTING_CENTER_PREPARED: {
                Name: 'Заказ на складе сортировочного центра подготовлен к отправке в службу доставки',
                Comment: '',
            },
            SORTING_CENTER_RETURN_PREPARING: {
                Name: 'Сортировочный центр получил данные о планируемом возврате заказа',
                Comment: '',
            },
            SORTING_CENTER_RETURN_RFF_ARRIVED_FULFILLMENT: {
                Name: 'Возвратный заказ поступил на склад сортировочного центра',
                Comment: '',
            },
            SORTING_CENTER_RETURN_ARRIVED: {
                Name: 'Возвратный заказ на складе сортировочного центра',
                Comment: '',
            },
            SORTING_CENTER_RETURN_PREPARING_SENDER: {
                Name: 'Возвратный заказ готов для передачи магазину',
                Comment: '',
            },
            SORTING_CENTER_RETURN_TRANSFERRED: {
                Name: 'Возвратный заказ передан на доставку в магазин',
                Comment: '',
            },
            SORTING_CENTER_RETURN_RETURNED: {
                Name: 'Заказ возвращен',
                Comment: 'Заказ успешно возвращён на СЦ, он вернётся к вам при следующем заборе.',
            },
            SORTING_CENTER_CANCELED: {
                Name: 'Сортировочный центр: заказ отменен',
                Comment: '',
            },
            SORTING_CENTER_ERROR: {
                Name: 'Ошибка создания заказа в сортировочном центре',
                Comment: '',
            },
            CANCELLED: {
                Name: 'Заказ отменен',
                Comment: '',
            },
            CANCELED_IN_PLATFORM: {
                Name: 'Заказ отменен в логистической платформе',
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
                                    yandexStatus: arr[0],
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
                    return item.yandexStatus === ctrl.Status;
                })
            ) {
                toaster.error('Данный статус уже указан. Чтобы обновить необходимо удалить.');
                return;
            }
            ctrl.syncStatuses.push({
                yandexStatus: ctrl.Status,
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
                    return x.yandexStatus + ',' + x.advStatus;
                })
                .join(';');
            ctrl.update = true;
        };
        ctrl.getStatusNameByObj = function (obj) {
            return ctrl.getStatusName(obj.yandexStatus);
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
            if (a.yandexStatus < b.yandexStatus) return -1;
            if (a.yandexStatus > b.yandexStatus) return 1;
            return 0;
        }
    };
    ShippingYandexSyncStatus.$inject = ['$http', 'toaster'];
    ng.module('shippingMethod')
        .controller('ShippingYandexSyncStatus', ShippingYandexSyncStatus)
        .component('shippingYandexSyncStatus', {
            templateUrl: shippingYandexSyncStatusTemplate,
            controller: 'ShippingYandexSyncStatus',
            bindings: {
                onInit: '&',
                methodId: '@',
                statusesReference: '@',
            },
        });
})(window.angular);
