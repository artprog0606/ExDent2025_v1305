import boxberryTemplate from './boxberry.tpl.html';

(function (ng) {
    'use strict';

    ng.module('boxberry', ['zone', 'checkout', 'shipping', 'toaster'])
        .controller('BoxberryCtrl', [
            'zoneService',
            'checkoutService',
            'toaster',
            '$scope',
            '$timeout',
            'shippingService',
            function (zoneService, checkoutService, toaster, $scope, $timeout, shippingService) {
                var ctrl = this;

                ctrl.init = function () {
                    if (!window.isBoxberryLoaded) {
                        jQuery.getScript('https://points.boxberry.de/js/boxberry.js?test=1', function () {
                            window.isBoxberryLoaded = true;
                            $timeout(function () {
                                shippingService.fireTemplateReady($scope);
                            });
                        });
                    } else {
                        shippingService.fireTemplateReady($scope);
                    }
                };

                ctrl.boxberryCallback_function = function (points) {
                    if (typeof delivery === 'string') {
                        delivery = new Function('return ' + delivery)();
                    }

                    var additionalData = {
                        address: points.address,
                        code: points.id,
                        name: points.name,
                        deliveryPeriod: points.period,
                        phone: points.phone,
                        price: points.price,
                        workSchedule: points.workschedule,
                    };

                    if (!ctrl.boxberryIsAdmin && points.name !== ctrl.boxberryWidgetConfigData.city && ctrl.boxberryContact.ContactId) {
                        $timeout(function () {
                            toaster.error('Нельзя менять город через виджет');
                        }, 0);

                        return;
                    }

                    ctrl.boxberryDeliveryShipping.PickpointId = points.id;
                    ctrl.boxberryDeliveryShipping.PickpointAddress = points.address;
                    ctrl.boxberryDeliveryShipping.PickpointAdditionalData = JSON.stringify(additionalData);
                    //ctrl.boxberryDeliveryShipping.Rate = points.price;
                    //ctrl.boxberryDeliveryShipping.DeliveryTime = points.period == "" ? "" : points.period + "дн.";
                    ctrl.boxberryDeliveryCost = points.price;

                    if (!ctrl.boxberryIsAdmin && points.name !== ctrl.boxberryWidgetConfigData.city) {
                        ctrl.boxberryContact.City = points.name;
                        if (ctrl.boxberryContact.ContactId) {
                            //зарегенный пользователь
                            $timeout(function () {
                                toaster.error('Нельзя менять город через виджет');
                            }, 0);
                            return;

                            // var contact = checkoutService.getContactFromCache();
                            //
                            // if (contact !== null) {
                            //     //обновляем данные адреса клиента
                            //     contact.City = points.name;
                            //
                            //     checkoutService.saveContact(contact).then(function (response) {
                            //         ctrl.boxberryDeliveryCallback({
                            //             event: 'boxberryDelivery',
                            //             field: ctrl.boxberryDeliveryShipping.PickpointId || 0,
                            //             shipping: ctrl.boxberryDeliveryShipping,
                            //         });
                            //     });
                            // }
                        } else {
                            var beforeShipping = ng.copy(ctrl.boxberryDeliveryShipping);
                            var callBackFunction = function () {
                                ctrl.boxberryDeliveryShipping.PickpointId = beforeShipping.PickpointId;
                                ctrl.boxberryDeliveryShipping.PickpointAddress = beforeShipping.PickpointAddress;
                                ctrl.boxberryDeliveryShipping.PickpointAdditionalData = beforeShipping.PickpointAdditionalData;
                                ctrl.boxberryDeliveryShipping.Rate = beforeShipping.Rate;
                                ctrl.boxberryDeliveryShipping.DeliveryTime = beforeShipping.DeliveryTime;

                                ctrl.boxberryDeliveryCallback({
                                    event: 'boxberryDelivery',
                                    field: ctrl.boxberryDeliveryShipping.PickpointId || 0,
                                    shipping: ctrl.boxberryDeliveryShipping,
                                });
                                checkoutService.removeCallback('address', callBackFunction);
                            };

                            // после setCurrentZone сработает обновление списка доставок в checkout,
                            // по завершению чего будет вызван Callback 'address'
                            checkoutService.addCallback('address', callBackFunction);
                            zoneService.getCurrentZone().then(function (data) {
                                zoneService.setCurrentZone(points.name, null, data.CountryId, null, data.CountryName, null);
                            });
                        }
                    } else {
                        ctrl.boxberryDeliveryCallback({
                            event: 'boxberryDelivery',
                            field: ctrl.boxberryDeliveryShipping.PickpointId || 0,
                            shipping: ctrl.boxberryDeliveryShipping,
                        });
                    }

                    //$scope.$digest();
                };

                ctrl.boxberryOpenWidget = function () {
                    boxberry.open(
                        ctrl.boxberryCallback_function,
                        ctrl.boxberryWidgetConfigData.api_token,
                        ctrl.boxberryWidgetConfigData.custom_city,
                        ctrl.boxberryWidgetConfigData.targetstart,
                        ctrl.boxberryWidgetConfigData.ordersum,
                        ctrl.boxberryWidgetConfigData.weight,
                        ctrl.boxberryWidgetConfigData.paysum || 0,
                        ctrl.boxberryWidgetConfigData.height,
                        ctrl.boxberryWidgetConfigData.width,
                        ctrl.boxberryWidgetConfigData.depth,
                    );
                };
            },
        ])

        .directive('boxberry', [
            'urlHelper',
            function (urlHelper) {
                return {
                    scope: {
                        boxberryDeliveryShipping: '=',
                        boxberryDeliveryCallback: '&',
                        //boxberryOpenWidget: '&',
                        boxberryWidgetConfigData: '=',
                        boxberryIsSelected: '=',
                        boxberryContact: '=',
                        boxberryIsAdmin: '<?',

                        boxberryDeliveryAmount: '=',
                        boxberryDeliveryWeight: '=',
                        boxberryDeliveryCost: '=',
                        boxberryDeliveryDimensions: '=',
                    },
                    controller: 'BoxberryCtrl',
                    controllerAs: 'boxberry',
                    bindToController: true,
                    templateUrl: boxberryTemplate,
                    link: function (scope, element, attrs, ctrl) {
                        ctrl.init();
                    },
                };
            },
        ]);
})(window.angular);
