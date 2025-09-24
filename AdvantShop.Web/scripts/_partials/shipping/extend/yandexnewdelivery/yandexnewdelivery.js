import yandexnewdeliveryTemplate from './yandexnewdelivery.tpl.html';
(function (ng) {
    'use strict';
    //https://yandex.ru/support/delivery-3/widgets/cart-widget.html
    ng.module('yandexnewdelivery', [])
        .controller('YandexNewDeliveryCtrl', [
            '$document',
            '$scope',
            'urlHelper',
            'shippingService',
            function ($document, $scope, urlHelper, shippingService) {
                var ctrl = this;

                ctrl.init = function () {
                    if (ctrl.widget == null) {
                        jQuery.getScript('https://widgets.delivery.yandex.ru/script/api', function () {
                            function start() {
                                ctrl.cart = {
                                    places: [
                                        {
                                            dimensions: {
                                                weight: ctrl.yandexNewDeliveryWeight,
                                                length: ctrl.yandexNewDeliveryLength,
                                                width: ctrl.yandexNewDeliveryWidth,
                                                height: ctrl.yandexNewDeliveryHeight,
                                            },
                                            items: [
                                                {
                                                    price: ctrl.yandexNewDeliveryCost,
                                                    assessedValue: ctrl.yandexNewDeliveryCost,
                                                    dimensions: {
                                                        weight: ctrl.yandexNewDeliveryWeight,
                                                        length: ctrl.yandexNewDeliveryLength,
                                                        width: ctrl.yandexNewDeliveryWidth,
                                                        height: ctrl.yandexNewDeliveryHeight,
                                                    },
                                                },
                                            ],
                                        },
                                    ],
                                    cost: {
                                        itemsSum: ctrl.yandexNewDeliveryCost,
                                        assessedValue: ctrl.yandexNewDeliveryCost,
                                    },
                                    shipment: {
                                        fromWarehouseId: ctrl.yandexNewDeliveryWarehouseId,
                                    },
                                    deliveryTypes: [ctrl.yandexNewDeliveryType],
                                };

                                window.YaDelivery.createWidget({
                                    containerId: 'ydwidget_' + ctrl.yandexNewDeliveryType, // Идентификатор HTML-элемента (контейнера),
                                    type: 'deliveryCart',
                                    params: {
                                        apiKey: ctrl.yandexNewDeliveryWidgetApiKey,
                                        senderId: ctrl.yandexNewDeliverySenderId,
                                        cart: ctrl.cart,
                                    },
                                })
                                    .then(successCallback)
                                    .catch(failureCallback);

                                function successCallback(widget) {
                                    shippingService.fireTemplateReady($scope);

                                    widget.on('submitDeliveryOption', (deliveryOption) => {
                                        var additionalData = {
                                            tariffId: deliveryOption.deliveryOption.tariffId,
                                            partnerId: deliveryOption.deliveryOption.partner,
                                            deliveryType: deliveryOption.deliveryType,
                                        };

                                        ctrl.yandexNewDeliveryCost = deliveryOption.deliveryOption.cost.deliveryForCustomer;
                                        ctrl.yandexNewDeliveryShipping.AdditionalData = JSON.stringify(additionalData);
                                        ctrl.yandexNewDeliveryShipping.TariffId = deliveryOption.deliveryOption.tariffId;

                                        if (deliveryOption.deliveryType == 'PICKUP') {
                                            let description = deliveryOption.pickupPoint.name != null ? deliveryOption.pickupPoint.name : '';
                                            if (
                                                deliveryOption.pickupPoint.address != null &&
                                                deliveryOption.pickupPoint.address.addressString != null
                                            ) {
                                                description += (description == '' ? '' : ', ') + deliveryOption.pickupPoint.address.addressString;
                                            }

                                            ctrl.yandexNewDeliveryShipping.IsAvailablePaymentCashOnDelivery =
                                                deliveryOption.pickupPoint.supportedFeatures.card == true ||
                                                deliveryOption.pickupPoint.supportedFeatures.cash == true;
                                            ctrl.yandexNewDeliveryShipping.PickpointId = deliveryOption.pickupPoint.id;
                                            ctrl.yandexNewDeliveryShipping.ShippingInfo = description;
                                        } else {
                                            let description =
                                                'Курьером ' +
                                                (deliveryOption.deliveryService.name != null ? deliveryOption.deliveryService.name : '');
                                            ctrl.yandexNewDeliveryShipping.ShippingInfo = description;

                                            ctrl.yandexNewDeliveryShipping.IsAvailablePaymentCashOnDelivery =
                                                deliveryOption.deliveryOption.services.filter(function (service) {
                                                    return service.code === 'CASH_SERVICE';
                                                }).length > 0;

                                            ctrl.yandexNewDeliveryShipping.PickpointId = null;
                                        }

                                        if (deliveryOption.deliveryOption.deliveryDays != null) {
                                            var daysMin = Math.abs(
                                                Math.round(
                                                    (Date.parse(deliveryOption.deliveryOption.deliveryDays.from) - Date.now()) /
                                                        1000 /
                                                        (60 * 60 * 24),
                                                ),
                                            );
                                            var daysMax = Math.abs(
                                                Math.round(
                                                    (Date.parse(deliveryOption.deliveryOption.deliveryDays.to) - Date.now()) / 1000 / (60 * 60 * 24),
                                                ),
                                            );

                                            if (daysMin != daysMax)
                                                ctrl.yandexNewDeliveryShipping.ShippingInfo += ', ' + daysMin + ' - ' + daysMax + ' дн';
                                            else ctrl.yandexNewDeliveryShipping.ShippingInfo += ', ' + daysMin + ' дн';
                                        }

                                        ctrl.yandexNewDeliveryCallback({
                                            event: 'yandexNewDelivery',
                                            field: ctrl.yandexNewDeliveryShipping.PickpointId || 0,
                                            shipping: ctrl.yandexNewDeliveryShipping,
                                        });

                                        ctrl.yandexNewDeliveryIsSelected = true;
                                    });
                                    ctrl.widget = widget;
                                }

                                function failureCallback(error) {
                                    //console.log(error);
                                }
                            }

                            window.YaDelivery ? start() : window.addEventListener('YaDeliveryLoad', start);
                        });
                    }
                };

                ctrl.openModal = function () {
                    if (ctrl.widget != null) {
                        if (ctrl.yandexNewDeliveryContact != null && ctrl.yandexNewDeliveryContact.City != null) {
                            ctrl.widget
                                .getRegionsByName(
                                    ctrl.yandexNewDeliveryContact.City +
                                        ', ' +
                                        (ctrl.yandexNewDeliveryContact.Region != null ? ctrl.yandexNewDeliveryContact.Region : ''),
                                )
                                .then((regions) => {
                                    ctrl.widget.setRegion({ id: regions[0].id });
                                });
                        }

                        ctrl.widget.showDeliveryOptions(ctrl.cart);
                    }
                };
            },
        ])
        .directive('yandexNewDelivery', [
            'urlHelper',
            function (urlHelper) {
                return {
                    scope: {
                        yandexNewDeliveryShipping: '=',
                        yandexNewDeliveryCallback: '&',
                        yandexNewDeliveryWidgetApiKey: '=',
                        yandexNewDeliverySenderId: '=',
                        yandexNewDeliveryWarehouseId: '=',
                        yandexNewDeliveryAmount: '=',
                        yandexNewDeliveryWeight: '=',
                        yandexNewDeliveryCost: '=',
                        yandexNewDeliveryHeight: '=',
                        yandexNewDeliveryWidth: '=',
                        yandexNewDeliveryLength: '=',
                        yandexNewDeliveryIsSelected: '=',
                        yandexNewDeliveryContact: '=',
                        yandexNewDeliveryType: '=',
                    },
                    controller: 'YandexNewDeliveryCtrl',
                    controllerAs: 'yandexNewDelivery',
                    bindToController: true,
                    templateUrl: yandexnewdeliveryTemplate,
                    link: function (scope, element, attrs, ctrl) {
                        ctrl.init();
                    },
                };
            },
        ]);
})(window.angular);
